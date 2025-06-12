using Application.Extensions;
using Application.Interfaces;
using Application.Interfaces.Gateways;
using Application.UseCases.InvoiceRequest;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;

namespace Infrastructure.Email;

public class NotificationGateway : INotificationGateway
{
    private readonly string ownerCode;
    private readonly HttpClient httpClient;

    public NotificationGateway(IConfiguration config, IAppSettings appSettings)
    {
        ownerCode = appSettings.OwnerCode;
        var configSection = config.GetSection("TTNotifications");
        var clientId = configSection["ClientId"];
        var secret = configSection["Secret"];
        var url = configSection["Url"];

        if (clientId.IsEmpty() || secret.IsEmpty() || url.IsEmpty())
        {
            throw new Exception("TTNotifications configuration missing");
        }

        var authToken = Encoding.ASCII.GetBytes($"{clientId}:{secret}");
        httpClient = new() { BaseAddress = new(url) };
        httpClient.DefaultRequestHeaders.Accept.Clear();
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authToken));
    }

    public async Task EmailAboutInvoiceRequest(CustomerSupplierDto supplier, JobForSupplier job, List<ProductLineDto> lines, MemoryStream excel)
    {
        var url = EmailRepository.GetVmiWebUrl(ownerCode);

        var docketNo = job.SupplierRefNo;
        var type = job.JobNo.StartsWith("OUT") ? "delivery docket" : "stocktransfer";
        var deliveryDateText = job.JobNo.StartsWith("OUT") ? $" on {job.ETD:dd/MM/yyyy}" : string.Empty;

        var title = $"Commercial invoice request for {type} to Electrolux {supplier.FactoryName} - {supplier.CompanyName} - {docketNo}";
        var message = @$"
Dears,

<p>Please issue a commercial invoice, according to attached file, for parts to be shipped to Electrolux {supplier.FactoryName}{deliveryDateText}.</p>

<p>Details are reported in attached Excel file.</p>

<table style=""border-collapse: collapse; width: 90%"" cellpadding=""2"" border=""1"">
<tbody>
    <tr>
        <td style=""width: 10%; text-align: center;""><strong>Customer Code</strong></td>
        <td style=""width: 20%; text-align: center;""><strong>Company Name</strong></td>
        <td style=""width: 10%; text-align: center;""><strong>Supplier ID</strong></td>
        <td style=""width: 10%; text-align: center;""><strong>Job</strong></td>
        <td style=""width: 10%; text-align: center;""><strong>Docket Number</strong></td>
        <td style=""width: 15%; text-align: center;""><strong>ASN</strong></td>
        <td style=""width: 10%; text-align: center;""><strong>Product Code</strong></td>
        <td style=""width: 5%; text-align: center;""><strong>Qty</strong></td>
        <td style=""width: 10%; text-align: center;""><strong>Im4 No</strong></td>
    </tr>
    {string.Join("\n", lines.Select(a => GetProductTableRow(supplier, docketNo, a)))}
</tbody>
</table>

<p>Upload <a href=""{url}"">here</a></p>
{EmailRepository.Regards}
{EmailRepository.NoReplyInfo}";

        var uri = $"NotifySupplier/{supplier.FactoryId}/{supplier.SupplierId}/withTTPlanner";
        var fileName = $"{docketNo}.xlsx";
        await Send(uri, title, message, excel, fileName);
    }

    public async Task EmailAboutInvoiceBatchUploadCustomsFlow(string companyName, List<(string dd, string factory)> details)
    {
        var title = "New upload of invoices from the supplier";
        var message = @$"
<p>Szanowni Państwo,</p>
<p>Dostawca {companyName} przesłał nowe faktury. Szczegóły poniżej:</p>
{string.Join("<br>", details.Select(x => $"{x.dd} - {x.factory}"))}
<p>Z poważaniem</p>
<br>
<p>---------------</p>
<br>
<p>Dears,</p>
<p>The supplier {companyName} uploaded new invoices in the system. Please, see the details below:</p>
{string.Join("<br>", details.Select(x => $"{x.dd} - {x.factory}"))}
{EmailRepository.Regards}
{EmailRepository.NoReplyInfo}";

        var uri = "NotifyToyota/planner";
        await Send(uri, title, message);
    }

    public async Task EmailAboutBatchWithPriceValidationError(CustomerSupplierDto supplier, List<string> docketNumbers, string ttlogixPrice, string invoicePrice)
    {
        var title = "Invoices price mismatch";
        var message = @$"
<p>Dear planner,</p>
<p>The supplier {supplier.CompanyName} ({supplier.FactoryId}) uploaded new invoices for the following delivery dockets/stock transfers:</p>
{string.Join("<br>", docketNumbers)}
<p>The inserted total price ({invoicePrice}) is not matching with the internal total price ({ttlogixPrice}).</p>
{EmailRepository.Regards}
{EmailRepository.NoReplyInfo}";

        var uri = "NotifyToyota/planner";
        await Send(uri, title, message);
    }

    public async Task EmailAboutRejectedInvoiceBatch(CustomerSupplierDto supplier, List<string> docketNumbers)
    {
        var dockets = string.Join(", ", docketNumbers);
        var title = $"Commercial invoice(s) uploaded by VMI supplier REJECTED ({supplier.FactoryId}) for document(s): {dockets}";
        var message = @$"
<p>Dear supplier,</p>
<p>Your invoice upload was rejected due to some errors in the uploaded documents.</p>
<p>Please contact Toyota Tsusho Europe SA staff to have more information. You will be able to upload the invoices again in the portal.</p>
{EmailRepository.Regards}
{EmailRepository.NoReplyInfo}";

        var uri = $"Notify/factory/{supplier.FactoryId}/supplier/{supplier.SupplierId}?isHtml=true&withPlanner=false";
        await Send(uri, title, message);
    }

    public async Task EmailAboutApprovedInvoiceBatchStandardFlow(CustomerSupplierDto supplier, List<string> docketNumbers, List<NamedStream> invoices)
    {
        var dockets = string.Join(", ", docketNumbers);
        var title = $"New upload of commercial invoice(s) uploaded by VMI supplier ({supplier.FactoryId}, {supplier.CompanyName}) for document(s): {dockets}";

        var message = @$"
<p>Please find attached the new commercial invoice(s) uploaded in the VMI portal according to details in the email subject.</p>
{EmailRepository.Regards}
{EmailRepository.NoReplyInfo}";

        var uri = $"NotifyElux/Invoicing/{supplier.FactoryId}";
        await Send(uri, title, message, invoices);
    }

    public async Task EmailAboutApprovedInvoiceRequestCustomsFlow(CustomerSupplierDto supplier, JobForSupplier job, List<ProductLineDto> lines, MemoryStream excel, List<NamedStream> invoices, string depHour, string? comment)
    {
        var docketNo = job.SupplierRefNo;
        var type = job.JobNo.StartsWith("OUT") ? "delivery docket" : "stocktransfer";
        var deliveryDateText = job.JobNo.StartsWith("OUT") ? $" with delivery on {job.ETD:dd/MM/yyyy}" : string.Empty;

        var title = $"Commercial invoice request for {type} to Electrolux {supplier.FactoryName} - {supplier.CompanyName} - {docketNo}";
        var message = @$"
<p><b>{depHour}</b></p>
<p><b>{comment?.ToUpper()}</b></p>

<p>Please issue invoice request for goods to Electrolux {supplier.FactoryName} plant{deliveryDateText}.</p>

<p>Details are reported in attached Excel file.</p>

<table style=""border-collapse: collapse; width: 90%"" cellpadding=""2"" border=""1"">
<tbody>
    <tr>
        <td style=""width: 10%; text-align: center;""><strong>Customer Code</strong></td>
        <td style=""width: 20%; text-align: center;""><strong>Company Name</strong></td>
        <td style=""width: 10%; text-align: center;""><strong>Supplier ID</strong></td>
        <td style=""width: 10%; text-align: center;""><strong>Job</strong></td>
        <td style=""width: 10%; text-align: center;""><strong>Docket Number</strong></td>
        <td style=""width: 15%; text-align: center;""><strong>ASN</strong></td>
        <td style=""width: 10%; text-align: center;""><strong>Product Code</strong></td>
        <td style=""width: 5%; text-align: center;""><strong>Qty</strong></td>
        <td style=""width: 10%; text-align: center;""><strong>Im4 No</strong></td>
    </tr>
    {string.Join("\n", lines.Select(a => GetProductTableRow(supplier, docketNo, a)))}
</tbody>
</table>
{EmailRepository.Regards}
{EmailRepository.NoReplyInfo}";

        var uri = $"NotifyCustoms/{supplier.FactoryId}/{supplier.SupplierId}";
        var files = new List<NamedStream>(invoices)
        {
            new() { FileName = $"{docketNo}.xlsx", Content = excel }
        };

        await Send(uri, title, message, files);
    }

    private string GetProductTableRow(CustomerSupplierDto basic, string docketNo, ProductLineDto line)
    {
        return @$"<tr>
                    <td>{basic.FactoryId}</td>
                    <td>{basic.CompanyName}</td>
                    <td>{basic.SupplierId}</td>
                    <td>{line.JobNo}</td>
                    <td>{docketNo}</td>
                    <td>{line.AsnNo}</td>
                    <td>{line.ProductCode}</td>
                    <td>{line.Qty}</td>
                    <td>{line.Im4No}</td>
                </tr>";
    }

    private async Task Send(string uri, string title, string message)
    {
        var body = new { Title = title, Message = message };
        var response = await httpClient.PostAsJsonAsync(uri, body);
        response.EnsureSuccessStatusCode();
    }

    private async Task Send(string uri, string title, string message, Stream stream, string fileName)
    {
        using var content = new MultipartFormDataContent
        {
            { new StringContent(title), "title" },
            { new StringContent(message), "message" },
            { new StringContent($"{true}"), "isHtml" },
            { new StreamContent(stream), "files", fileName }
        };
        var response = await httpClient.PostAsync(uri, content);
        response.EnsureSuccessStatusCode();
    }

    private async Task Send(string uri, string title, string message, List<NamedStream> files)
    {
        using var content = new MultipartFormDataContent
        {
            { new StringContent(title), "title" },
            { new StringContent(message), "message" },
            { new StringContent($"{true}"), "isHtml" }
        };
        files.ForEach(f => content.Add(new StreamContent(f.Content), "files", f.FileName));
        var response = await httpClient.PostAsync(uri, content);
        response.EnsureSuccessStatusCode();
    }
}

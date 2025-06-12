using Application.Extensions;
using Application.Interfaces.Gateways;
using Application.UseCases.InvoiceRequest;
using Application.UseCases.InvoiceRequest.Queries;
using Application.UseCases.InvoiceRequest.Queries.GetBatches;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using static Infrastructure.Extensions.NPOIExtensions;

namespace Infrastructure;

public class ExcelWriter : IExcelWriter
{
    public ExcelWriter()
    {

    }

    public MemoryStream GetBatchesExcel(List<InvoiceBatchDto> data)
    {
        var workbook = new XSSFWorkbook(XSSFWorkbookType.XLSX);

        var headerFont = workbook.CreateFont("Arial", 10, true);
        var rowFont = workbook.CreateFont("Arial", 10, false);
        var headerStyle = CreateHeaderStyle(workbook, headerFont);
        var rowStyle = CreateRowStyle(workbook, rowFont);

        // Batches sheet
        var sheetBatch = workbook.CreateSheet("Batch");
        var headersBatch = new List<string>() { "Batch Number", "Supplier ID", "Total Value", "Created Date", "Created By", "Status" };
        CreateHeaderRow(sheetBatch, headersBatch, headerStyle);

        foreach (var b in data)
        {
            var values = new List<string>()
            {
                b.BatchNumber,
                b.SupplierId,
                Math.Round(b.Invoices.Sum(i => i.Value), 2).ToString(),
                b.UploadedOn.Date.ToString("d"),
                b.UploadedBy,
                b.Status
            };
            CreateDataRow(sheetBatch, values, rowStyle);
        }

        // Details sheet
        var sheetDetails = workbook.CreateSheet("Details");
        var headersDetails = new List<string>() { "Batch Number", "Delivery Docket", "Job No", "ASN No", "Product Code", "Qty", "PO/SA Number", "PO/SA Line No" };
        CreateHeaderRow(sheetDetails, headersDetails, headerStyle);

        foreach (var b in data)
        {
            foreach (var j in b.Jobs)
            {
                foreach (var d in j.Details)
                {
                    var values = new List<string?>()
                    {
                        b.BatchNumber,
                        j.DeliveryDocket,
                        j.JobNo,
                        d.AsnNo,
                        d.ProductCode,
                        Math.Round(d.Qty, 2).ToString(),
                        d.PONumber,
                        d.POLineNo
                    };
                    CreateDataRow(sheetDetails, values, rowStyle);
                }
            }
        }

        // Invoices sheet
        var sheetInvoices = workbook.CreateSheet("Invoice");
        var headers3 = new List<string>() { "Batch Number", "Invoice Number", "Value" };
        CreateHeaderRow(sheetInvoices, headers3, headerStyle);

        foreach (var b in data)
        {
            foreach (var i in b.Invoices)
            {
                var values = new List<string>()
                {
                    b.BatchNumber,
                    i.InvoiceNumber,
                    Math.Round(i.Value, 2).ToString()
                };
                CreateDataRow(sheetInvoices, values, rowStyle);
            }
        }

        var ms = new MemoryStream();
        workbook.Write(ms, true);
        ms.Position = 0;
        return ms;
    }

    public MemoryStream GetNotInvoicedExcel(List<JobDto> data)
    {
        var workbook = new XSSFWorkbook(XSSFWorkbookType.XLSX);

        var headerFont = workbook.CreateFont("Arial", 10, true);
        var rowFont = workbook.CreateFont("Arial", 10, false);
        var headerStyle = CreateHeaderStyle(workbook, headerFont);
        var rowStyle = CreateRowStyle(workbook, rowFont);

        var sheet = workbook.CreateSheet("NotInvoiced");
        var headers = new List<string>() { "DD/ST", "ASN No", "Part Number", "Qty", "PO/SA Number", "PO/SA Line No" };
        CreateHeaderRow(sheet, headers, headerStyle);

        foreach (var j in data)
        {
            foreach (var d in j.Details)
            {
                var values = new List<string?>()
                {
                    j.DeliveryDocket,
                    d.AsnNo,
                    d.ProductCode,
                    Math.Round(d.Qty, 2).ToString(),
                    d.PONumber,
                    d.POLineNo
                };
                CreateDataRow(sheet, values, rowStyle);
            }
        }

        var ms = new MemoryStream();
        workbook.Write(ms, true);
        ms.Position = 0;
        return ms;
    }

    public MemoryStream GetInvoiceRequestExcel(CustomerSupplierDto supplier, string jobNo, string supplierRefNo, List<ProductLineDto> lines)
    {
        var workbook = new XSSFWorkbook(XSSFWorkbookType.XLSX);

        var headerFont = workbook.CreateFont("Arial", 10, true);
        var rowFont = workbook.CreateFont("Arial", 10, false);
        var headerStyle = CreateHeaderStyle(workbook, headerFont);
        var rowStyle = CreateRowStyle(workbook, rowFont);

        var sheet = workbook.CreateSheet("InvoiceRequest");

        var headers = new List<string>() { "Customer Code", "Company Name", "Supplier ID", "Job", "Docket Number", "ASN", "Product Code", "Qty", "Im4 No" };
        CreateHeaderRow(sheet, headers, headerStyle);

        foreach (var line in lines)
        {
            var values = new List<string?>()
            {
                supplier.FactoryId,
                supplier.CompanyName,
                supplier.SupplierId,
                jobNo,
                supplierRefNo,
                line.AsnNo,
                line.ProductCode,
                line.Qty.ToString(),
                line.Im4No
            };
            CreateDataRow(sheet, values, rowStyle);
        }

        var ms = new MemoryStream();
        workbook.Write(ms, true);
        ms.Position = 0;
        return ms;
    }

    private static void CreateHeaderRow(ISheet sheet, List<string> headers, ICellStyle style)
    {
        var headerRow = sheet.CreateRow(0);
        headerRow.Height = 700;

        foreach (var (value, idx) in headers.WithIndex())
        {
            CreateCell(headerRow, idx, value, style);
            sheet.SetColumnWidth(idx, 6000);
        }
    }

    private static ICellStyle CreateHeaderStyle(XSSFWorkbook workbook, IFont font)
    {
        var style = workbook.CreateCellStyle();
        style.SetFont(font);
        style.SetBorders();
        style.SetBackgroundColor(HSSFColor.PaleBlue.Index);
        style.VerticalAlignment = VerticalAlignment.Center;
        return style;
    }

    private static ICellStyle CreateRowStyle(XSSFWorkbook workbook, IFont font)
    {
        var style = workbook.CreateCellStyle();
        style.SetFont(font);
        style.SetBorders();
        return style;
    }
}

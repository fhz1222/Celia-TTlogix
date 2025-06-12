using Application.Exceptions;
using Application.Interfaces.Gateways;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;

namespace Infrastructure;
public class ILogConnectGateway : IILogConnectGateway
{

    readonly HttpClient? httpClient = null;
    readonly ILogger<ILogConnectGateway> logger;

    public ILogConnectGateway(IConfiguration configuration, ILogger<ILogConnectGateway> logger)
    {
        this.logger = logger;
        var iLogConnectConfig = configuration.GetSection("iLogConnect");
        if (iLogConnectConfig == null) return;
        var clientId = iLogConnectConfig["ClientId"];
        var secret = iLogConnectConfig["Secret"];
        var url = iLogConnectConfig["Url"];
        if (clientId == null || secret == null || url == null) return;
        var authToken = Encoding.ASCII.GetBytes($"{clientId}:{secret}");
        httpClient = new()
        {
            BaseAddress = new(url)
        };
        httpClient.DefaultRequestHeaders.Accept.Clear();
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authToken));
    }

    public void PalletTransferRequestCreated(string jobNo)
    {
        if (httpClient != null)
        {
            Task.Run(async () => 
            {
                try
                {
                    var res = await httpClient.PostAsync("ttlogix/pallettransferrequest", JsonContent.Create(new { jobNo }));
                    res.EnsureSuccessStatusCode();
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Raising Create Pallet Transfer event failed for job {jobNo}.", jobNo);
                }
            }).ConfigureAwait(false);
        }
    }

    public void IntegrationStatusChanged()
    {
        if (httpClient == null) { return; }

        var action = async () =>
        {
            try
            {
                var response = await httpClient.PostAsync("configchanged", null);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Raising Config Changed event failed.");
            }
        };

        RunInNewThread(action);
    }

    public void AdjustmentCompleted(string jobNo)
    {
        if (httpClient == null) { return; }

        var action = async () =>
        {
            try
            {
                var response = await httpClient.PostAsync("ttlogix/adjustment", JsonContent.Create(new { jobNo }));
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Raising Adjustment event failed.");
            }
        };

        RunInNewThread(action);
    }

    public async Task<bool> IsPickingRequestStarted(string pickingRequestId, CancellationToken cancellationToken)
    {
        if (httpClient == null) { return false; }

        try
        {
            var url = $"api/isProcessingPickingRequest?pickingRequestNumber={pickingRequestId}";
            var response = await httpClient.GetStringAsync(url, cancellationToken);
            return bool.Parse(response);
        }
        catch
        {
            throw new ApplicationError("Unable to get picking request status from iLog.");
        }
    }

    public void PickingRequestCreated(string pickingRequestId, int revision)
    {
        if (httpClient == null) { return; }

        var action = async () =>
        {
            try
            {
                var response = await httpClient.PostAsync("ttlogix/pickingrequest", JsonContent.Create(new { pickingRequestId, revision }));
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Raising Picking Request event failed.");
            }
        };

        RunInNewThread(action);
    }

    private void RunInNewThread(Func<Task?> action)
    {
        Task.Run(action).ConfigureAwait(false);
    }
}

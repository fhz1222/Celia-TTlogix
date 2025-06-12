using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using TT.Services.Interfaces;

namespace TT.Services.Services.Utilities
{
    //todo: move somewhere...
    public class ILogConnect : IILogConnect
    {
        readonly HttpClient httpClient = null;
        private readonly ILogger<ILogConnect> logger;

        public ILogConnect(IConfiguration configuration, ILogger<ILogConnect> logger)
        {
            this.logger = logger;
            var iLogConnectConfig = configuration.GetSection("iLogConnect");
            if (iLogConnectConfig == null) return;
            var clientId = iLogConnectConfig["ClientId"];
            var secret = iLogConnectConfig["Secret"];
            var url = iLogConnectConfig["Url"];
            if (clientId == null ||  secret == null || url == null) return;
            var authToken = Encoding.ASCII.GetBytes($"{clientId}:{secret}");
            httpClient = new()
            {
                BaseAddress = new(url)
            };
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authToken));
        }

        public void InboundCompleted(string jobNo)
        {
            if (httpClient == null) { return; }

            var action = async () =>
            {
                try
                {
                    var response = await httpClient.PostAsync("ttlogix/inbound", JsonContent.Create(new { jobNo }));
                    response.EnsureSuccessStatusCode();
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Raising Inbound event failed.");
                }
            };

            RunInNewThread(action);
        }

        public void QuarantineJobCreated(string jobNo)
        {
            if (httpClient == null) { return; }

            var action = async () =>
            {
                try
                {
                    var response = await httpClient.PostAsync("ttlogix/quarantine", JsonContent.Create(new { jobNo }));
                    response.EnsureSuccessStatusCode();
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Raising Quarantine event failed.");
                }
            };

            RunInNewThread(action);
        }

        public void StockTransferCompleted(string jobNo)
        {
            if(httpClient == null) { return; }

            var action = async () =>
            {
                try
                {
                    var response = await httpClient.PostAsync("ttlogix/stockTransfer", JsonContent.Create(new { jobNo }));
                    response.EnsureSuccessStatusCode();
                }
                catch(Exception ex)
                {
                    logger.LogError(ex, "Raising Stock Transfer event failed.");
                }
            };

            RunInNewThread(action);
        }

        public async Task<bool> IsProcessingOutbound(string jobNo)
        {
            if(httpClient == null) { return false; }

            try
            {
                var isProcessing = await httpClient.GetFromJsonAsync<bool>($"api/isProcessingOutbound?jobNo={jobNo}");
                return isProcessing;
            }
            catch(Exception ex)
            {
                logger.LogError(ex, "IsProcessingPickingRequest call failed.");
            }
            return false; 
        }

        public void PidAddedToStockTransfer(IEnumerable<string> pids)
        {
            if (httpClient == null) { return; }

            var action = async () =>
            {
                try
                {
                    var response = await httpClient.PostAsync("ttlogix/addToStf", JsonContent.Create(new { pids }));
                    response.EnsureSuccessStatusCode();
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Raising Add To Stf event failed.");
                }
            };

            RunInNewThread(action);
        }

        public void PidRemovedFromStockTransfer(IEnumerable<string> pids)
        {
            if (httpClient == null) { return; }

            var action = async () =>
            {
                try
                {
                    var response = await httpClient.PostAsync("ttlogix/removeFromStf", JsonContent.Create(new { pids }));
                    response.EnsureSuccessStatusCode();
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Raising Remove From Stf event failed.");
                }
            };

            RunInNewThread(action);
        }

        private void RunInNewThread(Func<Task> action)
        {
            Task.Run(action).ConfigureAwait(false);
        }
    }
}

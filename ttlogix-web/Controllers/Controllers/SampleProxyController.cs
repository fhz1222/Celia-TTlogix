using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using TT.Controllers.Extensions;

namespace TT.Controllers.Controllers
{
    [Route("api/sampleproxy")]
    [ApiController]
    public class SampleProxyController : ControllerBase
    {
        private readonly IConfiguration configuration;

        public SampleProxyController(IConfiguration configuration) : base()
        {
            this.configuration = configuration;
        }
        /// <summary>
        /// Example how to call some method from external proxy configuration.
        /// It just calls the API that generates the report and sends result stream (file) as the response but in general the result can be processed before sending it back, 
        /// or it is possible to pass some additional data to the request
        /// </summary>
        /// <returns>report file</returns>
        [HttpGet]
        [Route("testreport")]
        public async Task<Stream> TestReport()
        {
            using (var client = new HttpClient())
            {
                var reportApiUrl = configuration.GetReportApiUrl();
                client.BaseAddress = new Uri(reportApiUrl);

                var request = new HttpRequestMessage(HttpMethod.Get, "/reporting/GenerateReport?reportName=SampleTableReport&id=22");
                var response = await client.SendAsync(request);
                return response.Content.ReadAsStream();
            }
        }
    }
}

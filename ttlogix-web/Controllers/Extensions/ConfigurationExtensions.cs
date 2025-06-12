using Microsoft.Extensions.Configuration;

namespace TT.Controllers.Extensions
{
    public static class ConfigurationExtensions
    {
        public static string GetReportApiUrl(this IConfiguration configuration)
        {
            return configuration.GetSection("ReverseProxy").GetSection("Clusters")
                .GetSection("reportcluster")
                .GetSection("Destinations")
                .GetSection("ReportApiUrl")
                .GetValue<string>("Address", string.Empty);
        }

        public static string GetNewApiUrl(this IConfiguration configuration)
        {
            return configuration.GetSection("ReverseProxy").GetSection("Clusters")
                .GetSection("newapicluster")
                .GetSection("Destinations")
                .GetSection("NewApiUrl")
                .GetValue<string>("Address", string.Empty);
        }
    }
}

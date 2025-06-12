using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using TT.Core.Entities;
using TT.Core.Interfaces;
using TT.Services.Interfaces;

namespace TT.Services.Services
{
    public class LoggerSerivce : ILoggerService
    {
        public LoggerSerivce(ILogger<LoggerSerivce> logger, ITTLogixRepository repository)
        {
            this.logger = logger;
            this.repository = repository;
        }

        public async Task LogError(string jobNo, string methodName, string message, bool notify)
        {
            try
            {
                logger.LogError($"Error, job no: {jobNo}, method: {methodName}, message: {message}");
                await repository.AddErrorLogAsync(new ErrorLog { JobNo = jobNo, Method = methodName, ErrorMessage = message, Notify = (byte)(notify ? 1 : 0) });
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error logging error.");
            }
        }

        public void LogInformation(string message)
        {
            logger.LogInformation(message);
        }

        private readonly ILogger<LoggerSerivce> logger;
        private readonly ITTLogixRepository repository;
    }
}

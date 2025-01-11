using System;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace CarteiraInvestimento.Functions
{
    public class FechamentoFunction
    {
        private readonly ILogger _logger;

        public FechamentoFunction(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<FechamentoFunction>();
        }

        [Function("FechamentoFunction")]
        public void Run([TimerTrigger("0 19 * * *")] TimerInfo myTimer)
        {
            _logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            if (myTimer.ScheduleStatus is not null)
            {
                _logger.LogInformation($"Next timer schedule at: {myTimer.ScheduleStatus.Next}");
            }
        }
    }
}

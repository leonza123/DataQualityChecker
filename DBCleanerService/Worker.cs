using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DBCleanerService.Constants;
using DBCleanerService.Data;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DBCleanerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                using (QualityProjectDbContext db = new QualityProjectDbContext())
                {
                    try 
                    {
                        db.RemoveOldData(DelayConstants.WorkerDelayInHours);
                    }
                    catch(Exception ex) 
                    {
                        _logger.LogError("Worker error at: {time}.\nError: " + ex.Message + "\nStackTrace: " + ex.StackTrace + "\nInnerException: " + ex.InnerException, DateTimeOffset.Now);
                    }
                }
                await Task.Delay(DelayConstants.WorkerDelayInMilliseconds, stoppingToken);
            }
        }
    }
}

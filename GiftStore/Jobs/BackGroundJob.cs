using GiftStore.Data;
using GiftStore.Services;

namespace GiftStore.Jobs
{
    public class BackGroundJob:BackgroundService
    {

        private readonly Db_API _dbContext;
        private readonly ILogger<BackGroundJob> _logger;
        public BackGroundJob(Db_API dbContext, ILogger<BackGroundJob> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                // Check the number of rows in the giftcardtable
                var rowCount = _dbContext.giftCards.Where(x => x.Status != "در دسترس").ToList().Count();

                if (rowCount < 5)
                {
                    // Log or send a notification
                    _logger.LogWarning($"فقط {rowCount} عدد گیفت کارت در دسترس باقی مانده");
                    // You can also call a notification service here (e.g., email, SMS).
                }

                // Wait for 6 hours before the next check
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }
}

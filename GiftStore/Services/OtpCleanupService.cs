using GiftStore.Data;

namespace GiftStore.Services
{
   
        public class OtpCleanupService : IHostedService, IDisposable
        {
            private Timer _timer;
            private readonly IServiceProvider _services;
        public Db_API db { get; set; }

        public OtpCleanupService(IServiceProvider services, Db_API db_)
            {
                _services = services;
            db = db_;
            }

            public Task StartAsync(CancellationToken cancellationToken)
            {
                // Run the cleanup task every 10 seconds (adjust as needed)
                _timer = new Timer(CleanupExpiredOtps, null, TimeSpan.Zero, TimeSpan.FromSeconds(10));
                return Task.CompletedTask;
            }

            private void CleanupExpiredOtps(object state)
            {
                using (var scope = _services.CreateScope())
                {
                  //  var dbContext = scope.ServiceProvider.GetRequiredService<db>();
                    var expiredOtps = db.smsBanks
                        .Where(x => x.CreatedAt < DateTime.UtcNow.AddSeconds(-30))
                        .ToList();

                    if (expiredOtps.Any())
                    {
                        db.smsBanks.RemoveRange(expiredOtps);
                        db.SaveChanges();
                    }
                }
            }

            public Task StopAsync(CancellationToken cancellationToken)
            {
                _timer?.Change(Timeout.Infinite, 0);
                return Task.CompletedTask;
            }

            public void Dispose()
            {
                _timer?.Dispose();
            }
        }
    
}

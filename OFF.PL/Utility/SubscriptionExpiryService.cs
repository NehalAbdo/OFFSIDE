using OFF.DAL.Context;
using OFF.DAL.Model;
using OFF.PL.Controllers;

namespace OFF.PL.Utility
{
    public class SubscriptionExpiryService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public SubscriptionExpiryService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<CompanyDbContext>();

                    var expiredSubscriptions = context.Subscriptions
                        .Where(s => s.EndDate < DateTime.UtcNow && s.SubscriptionStatus == subscriptionStatus.Active)
                        .ToList();

                    foreach (var subscription in expiredSubscriptions)
                    {
                        subscription.SubscriptionStatus = subscriptionStatus.Expired;

                        // Update VIP status of the associated agent
                        var agent = await context.Agents.FindAsync(subscription.AgentId);
                        if (agent != null)
                        {
                            var activeSubscription = agent.Subscriptions.FirstOrDefault(s => s.SubscriptionStatus == subscriptionStatus.Active && s.EndDate >= DateTime.UtcNow);
                            agent.VIP = activeSubscription != null;
                        }
                    }

                    await context.SaveChangesAsync();
                }

                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }
    }
}
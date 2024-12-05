using Microsoft.EntityFrameworkCore;
using OFF.BLL.Interfaces;
using OFF.DAL.Context;
using OFF.DAL.Model;


namespace OFF.BLL.Repository
{
    public class SubscriptionRepository : GenericRepository<Subscription>, ISubscriptionRepository
    {
        public SubscriptionRepository(CompanyDbContext context) : base(context)
        {
        }


        public async Task<List<Subscription>> GetActiveSubscriptionsAsync()
        {
            return await _context.Subscriptions
                .Where(s => s.EndDate >= DateTime.UtcNow)
                .ToListAsync();
        }

        public async Task<List<Subscription>> GetSubscriptionsByAgentIdAsync(string agentId)
        {
            return await _context.Subscriptions
                .Where(s => s.AgentId == agentId)
                .ToListAsync();
        }

        public async Task<List<Subscription>> UpdateAsync(Subscription subscription)
        {
            _context.Entry(subscription).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return await GetSubscriptionsByAgentIdAsync(subscription.AgentId);

        }
       
        public async Task<Subscription?> GetByIdAsync(int id) => await _context.Subscriptions.FindAsync(id);

        public async Task<Subscription> GetActiveSubscriptionAsync(string agentId)
        {
                  return await _context.Subscriptions.FirstOrDefaultAsync(s => s.AgentId == agentId && s.SubscriptionStatus == subscriptionStatus.Active);
        }

        public async Task<bool> HasActiveSubscriptionAsync(string agentId)
        {
            return await _context.Subscriptions
                .AnyAsync(s => s.AgentId == agentId && s.SubscriptionStatus == subscriptionStatus.Active);
        }
        public async Task RemoveRange(IEnumerable<Subscription> subscriptions)
        {
            _context.Subscriptions.RemoveRange(subscriptions);
            await _context.SaveChangesAsync();
        }
    }

} 
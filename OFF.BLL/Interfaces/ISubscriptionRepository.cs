using OFF.DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OFF.BLL.Interfaces
{
    public interface ISubscriptionRepository : IGenericRepository<Subscription>
    {
        

        Task<List<Subscription>> GetActiveSubscriptionsAsync();
        Task<List<Subscription>> GetSubscriptionsByAgentIdAsync(string agentId);
        Task<List<Subscription>> UpdateAsync(Subscription subscription);
        Task<Subscription?> GetByIdAsync(int id);
        Task<Subscription> GetActiveSubscriptionAsync(string agentId);
        Task<bool> HasActiveSubscriptionAsync(string agentId);
        Task RemoveRange(IEnumerable<Subscription> subscriptions);

    }
}

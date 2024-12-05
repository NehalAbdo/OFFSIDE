using Microsoft.EntityFrameworkCore;
using OFF.BLL.Interfaces;
using OFF.DAL.Context;
using OFF.DAL.Model;

namespace OFF.BLL.Repository
{
    public class AgentRepository :GenericRepository<Agent>,IAgentRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly CompanyDbContext _context;

        public AgentRepository(CompanyDbContext context, IUnitOfWork unitOfWork) : base(context)
        {
            _context = context;
            _unitOfWork = unitOfWork;
        }
        public async Task<Agent> GetAgentWithSubscriptionsAsync(string agentId)
        {
            return await _context.Agents
                                 .Include(a => a.Subscriptions)
                                 .FirstOrDefaultAsync(a => a.Id == agentId);
        }
        public async Task UpdateAgentVipStatus(string agentId)
        {
            var agent = await _unitOfWork.Agents.GetAgentWithSubscriptionsAsync(agentId);

            if (agent != null)
            {
                var activeSubscription = agent.Subscriptions.FirstOrDefault(s => s.SubscriptionStatus == subscriptionStatus.Active && s.EndDate >= DateTime.UtcNow);
                agent.VIP = activeSubscription != null;
                _unitOfWork.Agents.Update(agent);
                await _unitOfWork.completeAsync();
            }
        }
        public async Task<List<Subscription>> GetSubscriptionsByAgentIdAsync(string agentId)
        {
            return await _context.Subscriptions
                                 .Where(s => s.AgentId == agentId)
                                 .ToListAsync();
        }


    }
}

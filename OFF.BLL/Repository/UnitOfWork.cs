using Microsoft.EntityFrameworkCore;
using OFF.BLL.Interfaces;
using OFF.DAL.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OFF.BLL.Repository.UnitOfWork;

namespace OFF.BLL.Repository
{
        public class UnitOfWork : IUnitOfWork
        {
        private readonly IPlayerRepository players;
        private readonly IAgentRepository agents;
        private readonly IPostRepository posts;
        private readonly ISubscriptionRepository subscriptions;
        private readonly IPaymentRepository payments;
        private readonly CompanyDbContext _context;
        private readonly IContactRepository contacts;
        public UnitOfWork(CompanyDbContext context)
        {
            players = new PlayerRepository(context);
            agents = new AgentRepository(context,this);
            _context = context;
            posts = new PostRepository(context);
            subscriptions = new SubscriptionRepository(context);
            payments= new PaymentRepository(context);
            contacts= new ContactRepository(context);
        }
        public IAgentRepository Agents => agents;
        public IPlayerRepository Players => players;
        public IPostRepository Posts => posts;
        public IPaymentRepository Payments => payments;
        public ISubscriptionRepository Subscriptions => subscriptions;
        public IContactRepository Contacts => contacts;

        public async Task<int> completeAsync()=> await _context.SaveChangesAsync();

        public async ValueTask DisposeAsync() => await _context.DisposeAsync();
        }
    
}

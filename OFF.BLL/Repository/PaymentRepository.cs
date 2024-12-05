using Microsoft.EntityFrameworkCore;
using OFF.BLL.Interfaces;
using OFF.DAL.Context;
using OFF.DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OFF.BLL.Repository
{
    public class PaymentRepository : GenericRepository<Payment>, IPaymentRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        public PaymentRepository(CompanyDbContext context, IUnitOfWork unitOfWork = null) : base(context)
        {
            _unitOfWork = unitOfWork;
        }



        public void  Update(Payment payment)
        {
            _context.Payments.Update(payment);
        }
        public async Task<List<Payment>> GetAllAsync()
        {
            return await _context.Payments
                .Include(p => p.Subscription) 
                .ThenInclude(s => s.Agent) 
                .ToListAsync();
        }
        public async Task<List<Payment>> GetPaymentsByUserIdAsync(string userId)
        {
            return await _context.Payments
                .Where(p => p.Subscription.AgentId == userId) // Adjust this based on your data model
                .ToListAsync();
        }
        public async Task<List<Payment>> GetPaymentsBySubscriptionId(int subscriptionId)
        {
            return await _context.Payments
                .Where(p => p.SubscriptionId == subscriptionId)
                .ToListAsync();
        }

        public async Task<Payment> GetByPaymentIntentId(string paymentIntentId)
        {
            return await _context.Payments.FirstOrDefaultAsync(p => p.PaymentIntentId == paymentIntentId);
        }


    }
}

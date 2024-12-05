using OFF.DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OFF.BLL.Interfaces
{
    public interface IPaymentRepository : IGenericRepository<Payment>
    {
      
        void Update(Payment payment);
        Task<List<Payment>> GetPaymentsBySubscriptionId(int subscriptionId);
        Task<Payment> GetByPaymentIntentId(string paymentIntentId);

        Task<List<Payment>> GetPaymentsByUserIdAsync(string userId);
        Task<List<Payment>> GetAllAsync();

    }
}

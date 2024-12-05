using OFF.DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OFF.BLL.Interfaces
{
    public interface IAgentRepository:IGenericRepository<Agent>
    {
       public Task<Agent> GetAgentWithSubscriptionsAsync(string agentId);
      public Task UpdateAgentVipStatus(string agentId);


    }
}

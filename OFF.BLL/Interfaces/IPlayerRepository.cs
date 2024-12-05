using OFF.DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace OFF.BLL.Interfaces
{
    public interface IPlayerRepository:IGenericRepository<PLayer>
    {
        Task<IEnumerable<PLayer>> GetAllAsync(string Address);
        Task<IEnumerable<PLayer>> GetAllAsync(Expression<Func<PLayer, bool>> expression);
        IQueryable<PLayer> Include(params Expression<Func<PLayer, object>>[] includeProperties);
        Task<PLayer> FirstOrDefaultAsync(Expression<Func<PLayer, bool>> predicate);


    }
}

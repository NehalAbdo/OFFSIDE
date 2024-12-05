using Microsoft.EntityFrameworkCore;
using OFF.BLL.Interfaces;
using OFF.DAL.Context;
using OFF.DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace OFF.BLL.Repository
{
    public class PlayerRepository :GenericRepository<PLayer>,IPlayerRepository
    {
        public PlayerRepository(CompanyDbContext context) : base(context)
        {
        }
        public Task<IEnumerable<PLayer>> GetAllAsync(string Address)
        {
            throw new NotImplementedException();
        }
        public async Task<PLayer> FirstOrDefaultAsync(Expression<Func<PLayer, bool>> predicate)
        {
            return await _context.Players.FirstOrDefaultAsync(predicate);
        }
        public Task<IEnumerable<PLayer>> GetAllAsync(Expression<Func<PLayer, bool>> expression)
        {
            throw new NotImplementedException();
        }

        public IQueryable<PLayer> Include(params Expression<Func<PLayer, object>>[] includeProperties)
        {
            IQueryable<PLayer> query = _context.Players;
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query;
        }

      
    }
}


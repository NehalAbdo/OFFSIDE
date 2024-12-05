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
    public class GenericRepository<T> :IGenericRepository<T> where T : class
    {
        protected readonly CompanyDbContext _context;

        public GenericRepository(CompanyDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
        }

        public void Delete(T entity)
        {
            _context.Set<T>().Remove(entity);
        }
        public async Task<IEnumerable<T>> GetAllPostsAsync(Expression<Func<T, bool>> predicate)
        {
            return await _context.Set<T>().Where(predicate).ToListAsync();
        }
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _context.Set <T>().ToListAsync();
        }

        public async Task<T?> GetAsync(string id) => await _context.Set<T>().FindAsync(id);

        public void Update(T entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
        }

        public async Task<T?> GetBySubscriptionIdAsync(int id) => await _context.Set<T>().FindAsync(id);
        public async Task<T?> GetByIdAsync(int id) => await _context.Set<T>().FindAsync(id);


    }
}

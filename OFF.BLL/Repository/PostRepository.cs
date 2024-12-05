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
    public class PostRepository : GenericRepository<Post>, IPostRepository
    {
        public PostRepository(CompanyDbContext context) : base(context)
        {
        
        }
        public async Task<IEnumerable<Post>> GetPostsByUserIdAsync(string userId)
        {
            return await _context.Posts.Where(p => p.AppUserId == userId).ToListAsync();
        }
        public void RemoveRange(IEnumerable<Post> posts)
        {
            _context.Posts.RemoveRange(posts);
        }

    }
}

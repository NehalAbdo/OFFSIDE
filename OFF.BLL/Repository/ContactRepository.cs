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
    public class ContactRepository:GenericRepository<Contact>,IContactRepository
    {
        public ContactRepository(CompanyDbContext context) : base(context)
        {

        }
    }
}

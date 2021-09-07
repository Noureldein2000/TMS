using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TMS.Data;

namespace TMS.Services.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        void SaveChanges();
        Task SaveChangesAsync();
        ApplicationDbContext GetContext();
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using TMS.Data;

namespace TMS.Services.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        void SaveChanges();
        ApplicationDbContext GetContext();
    }
}

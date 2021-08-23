using System;
using System.Collections.Generic;
using System.Text;
using TMS.Data;

namespace TMS.Services.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private bool disposed = false;
        public UnitOfWork(ApplicationDbContext contex)
        {
            _context = contex;
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            this.disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public ApplicationDbContext GetContext()
        {
            return _context;
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }
    }
}

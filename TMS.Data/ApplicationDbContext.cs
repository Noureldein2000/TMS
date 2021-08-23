using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using TMS.Data.Entities;

namespace TMS.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public virtual DbSet<Fee> Fees { get; set; }
        public virtual DbSet<DenominationServiceProvider> DenominationServiceProviders { get; set; }
        public virtual DbSet<FeesType> FeesTypes { get; set; }
        public virtual DbSet<InquiryBill> InquiryBills { get; set; }
        public virtual DbSet<InquiryBillDetails> InquiryBillDetails { get; set; }
        public virtual DbSet<Parameter> Parameters { get; set; }
        public virtual DbSet<ProviderServiceConfigeration> ProviderServiceConfigerations { get; set; }
        public virtual DbSet<ProviderServiceRequest> ProviderServiceRequests { get; set; }
        public virtual DbSet<ProviderServiceRequestParams> ProviderServiceRequestParams { get; set; }
        public virtual DbSet<ProviderServiceRequestStatus> ProviderServiceRequestStatus { get; set; }
        public virtual DbSet<ProviderServiceResponse> ProviderServiceResponses { get; set; }
        public virtual DbSet<ProviderServiceResponseParam> ProviderServiceResponseParams { get; set; }
        public virtual DbSet<ReceiptBodyParam> ReceiptBodyParams { get; set; }
        public virtual DbSet<RequestType> RequestTypes { get; set; }
        public virtual DbSet<ServiceConfigeration> ServiceConfigerations { get; set; }
        public virtual DbSet<ServiceConfigParms> ServiceConfigParms { get; set; }
        public virtual DbSet<ServiceProvider> ServiceProviders { get; set; }
        public override int SaveChanges()
        {
            var entries = ChangeTracker
            .Entries()
            .Where(e => e.Entity is BaseEntity<int> && (
                    e.State == EntityState.Added
                    || e.State == EntityState.Modified));

            foreach (var entityEntry in entries)
            {
                switch (entityEntry.State)
                {
                    case EntityState.Modified:
                        ((BaseEntity<int>)entityEntry.Entity).UpdateDate = DateTime.Now;
                        break;
                    case EntityState.Added:
                        ((BaseEntity<int>)entityEntry.Entity).CreationDate = DateTime.Now;
                        break;
                    default:
                        break;
                }
                
            }

            return base.SaveChanges();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(modelBuilder);
        }
    }
}

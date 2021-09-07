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
        public virtual DbSet<ProviderServiceRequestParam> ProviderServiceRequestParams { get; set; }
        public virtual DbSet<ProviderServiceRequestStatus> ProviderServiceRequestStatus { get; set; }
        public virtual DbSet<ProviderServiceResponse> ProviderServiceResponses { get; set; }
        public virtual DbSet<ProviderServiceResponseParam> ProviderServiceResponseParams { get; set; }
        public virtual DbSet<ReceiptBodyParam> ReceiptBodyParams { get; set; }
        public virtual DbSet<RequestType> RequestTypes { get; set; }
        public virtual DbSet<ServiceConfigeration> ServiceConfigerations { get; set; }
        public virtual DbSet<ServiceConfigParms> ServiceConfigParms { get; set; }
        public virtual DbSet<ServiceProvider> ServiceProviders { get; set; }
        public virtual DbSet<Denomination> Denominations { get; set; }
        public virtual DbSet<PaymentMode> PaymentModes { get; set; }
        public virtual DbSet<ServiceCategory> ServiceCategories { get; set; }
        public virtual DbSet<BillPaymentMode> BillPaymentModes { get; set; }
        public virtual DbSet<ServiceType> ServiceTypes { get; set; }
        public virtual DbSet<ServiceEntity> ServiceEntities { get; set; }
        public virtual DbSet<Currency> Currencies { get; set; }
        public virtual DbSet<Service> Services { get; set; }
        public virtual DbSet<Log> Logs { get; set; }
        public virtual DbSet<MainStatusCode> MainStatusCodes { get; set; }
        public virtual DbSet<StatusCode> StatusCodes { get; set; }
        public virtual DbSet<ProviderStatusCode> ProviderStatusCodes { get; set; }
        public virtual DbSet<RequestStatus> RequestStatus { get; set; }
        public virtual DbSet<DenominationFee> DenominationFees { get; set; }
        public virtual DbSet<DenominationEntity> DenominationEntities { get; set; }
        public virtual DbSet<DenominationCommission> DenominationCommissions { get; set; }
        public virtual DbSet<Commission> Commissions { get; set; }
        public virtual DbSet<CommissionType> CommissionTypes { get; set; }
        public virtual DbSet<DenominationParam> DenominationParams { get; set; }
        public virtual DbSet<DenominationParameter> DenominationParameters { get; set; }
        public virtual DbSet<DenominationParamValueMode> DenominationParamValueModes { get; set; }
        public virtual DbSet<DenominationParamValueType> DenominationParamValueTypes { get; set; }
        public virtual DbSet<DenominationProviderConfiguration> DenominationProviderConfigerations { get; set; }
        public virtual DbSet<DenominationReceiptData> DenominationReceiptData { get; set; }
        public virtual DbSet<AccountFee> AccountFees { get; set; }
        public virtual DbSet<DenominationReceiptParam> DenominationReceiptParams { get; set; }
        public virtual DbSet<AccountCommission> AccountCommissions { get; set; }
        public virtual DbSet<AccountProfileDenomination> AccountProfileDenominations { get; set; }
        public virtual DbSet<AccountProfileCommission> AccountProfileCommissions { get; set; }
        public virtual DbSet<AccountProfileFee> AccountProfileFees { get; set; }
        public virtual DbSet<Request> Requests { get; set; }
        //public virtual DbSet<Invoice> Invoices { get; set; }
        //public virtual DbSet<InvoiceDetails> InvoiceDetails { get; set; }
        public virtual DbSet<ServicesField> ServicesFields { get; set; }
        public virtual DbSet<FieldType> FieldTypes { get; set; }
        public virtual DbSet<ServiceBalanceType> ServiceBalanceTypes { get; set; }
        public virtual DbSet<Transaction> Transactions { get; set; }
        public virtual DbSet<AccountTransactionCommission> AccountTransactionCommissions { get; set; }
        


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
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            //modelBuilder.ForSqlServerHasSequence<int>("DBSequence")
            //                  .StartsAt(1000).IncrementsBy(2);
            

        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using TMS.Data.Entities;

namespace TMS.Data.Configurations
{
    public class DenominationConfiguration : IEntityTypeConfiguration<Denomination>
    {
        public void Configure(EntityTypeBuilder<Denomination> builder)
        {
            builder.HasKey(s => s.ID);
            builder.Property(s => s.APIValue).HasColumnType("decimal(18, 3)");
            builder.Property(s => s.MinValue).HasColumnType("decimal(18, 3)");
            builder.Property(s => s.MaxValue).HasColumnType("decimal(18, 3)");
            builder.Property(s => s.Value).HasColumnType("decimal(18, 3)");
            builder.HasOne(s => s.BillPaymentMode).WithMany(s => s.Denominations).HasForeignKey(s => s.BillPaymentModeID)
                .OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(s => s.PaymentMode).WithMany(s => s.Denominations).HasForeignKey(s => s.PaymentModeID)
                .OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(s => s.ServiceCategory).WithMany(s => s.Denominations).HasForeignKey(s => s.ServiceCategoryID)
                .OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(s => s.Currency).WithMany(s => s.Denominations).HasForeignKey(s => s.CurrencyID)
                .OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(s => s.Service).WithMany(s => s.Denominations).HasForeignKey(s => s.ServiceID)
                .OnDelete(DeleteBehavior.NoAction);

        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using TMS.Data.Entities;

namespace TMS.Data.Configurations
{
    public class TaxConfiguration : IEntityTypeConfiguration<Tax>
    {
        public void Configure(EntityTypeBuilder<Tax> builder)
        {
            builder.HasKey(s => s.ID);
            builder.Property(s => s.AmountFrom).HasColumnType("decimal(18,3)");
            builder.Property(s => s.AmountTo).HasColumnType("decimal(18,3)");
            builder.Property(s => s.Value).HasColumnType("decimal(18,3)");

            builder.HasOne(s => s.TaxType).WithMany(s => s.Taxes).HasForeignKey(s => s.TaxTypeID)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(s => s.PaymentMode).WithMany(s => s.Taxes).HasForeignKey(s => s.PaymentModeID)
               .OnDelete(DeleteBehavior.NoAction);
        }
    }
}

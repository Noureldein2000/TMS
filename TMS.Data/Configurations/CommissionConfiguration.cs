using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using TMS.Data.Entities;

namespace TMS.Data.Configurations
{
    public class CommissionConfiguration : IEntityTypeConfiguration<Commission>
    {
        public void Configure(EntityTypeBuilder<Commission> builder)
        {
            
            builder.HasKey(s => s.ID);
            builder.Property(s => s.AmountFrom).HasColumnType("decimal(18, 3)");
            builder.Property(s => s.AmountTo).HasColumnType("decimal(18, 3)");
            builder.Property(s => s.Value).HasColumnType("decimal(18, 3)");

            builder.HasOne(s => s.PaymentMode).WithMany(s => s.Commissions)
                .HasForeignKey(s => s.PaymentModeID).OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(s => s.CommissionType).WithMany(s => s.Commissions)
               .HasForeignKey(s => s.CommissionTypeID).OnDelete(DeleteBehavior.NoAction);
        }
    }
}

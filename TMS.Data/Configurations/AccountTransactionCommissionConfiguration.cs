using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using TMS.Data.Entities;

namespace TMS.Data.Configurations
{
    public class AccountTransactionCommissionConfiguration : IEntityTypeConfiguration<AccountTransactionCommission>
    {
        public void Configure(EntityTypeBuilder<AccountTransactionCommission> builder)
        {
            builder.HasKey(s => s.ID);
            builder.Property(s => s.Commission).HasColumnType("decimal(18, 3)");
            builder.HasOne(s => s.Transaction).WithMany(s => s.AccountTransactionCommissions)
                .HasForeignKey(s => s.TransactionID).OnDelete(DeleteBehavior.NoAction);
        }
    }
}

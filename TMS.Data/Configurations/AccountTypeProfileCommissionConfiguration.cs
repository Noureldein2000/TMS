using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using TMS.Data.Entities;

namespace TMS.Data.Configurations
{
    class AccountTypeProfileCommissionConfiguration : IEntityTypeConfiguration<AccountTypeProfileCommission>
    {
        public void Configure(EntityTypeBuilder<AccountTypeProfileCommission> builder)
        {
            builder.HasKey(s => s.ID);
            builder.HasOne(s => s.AccountTypeProfileDenomination).WithMany(s => s.AccountTypeProfileCommissions)
                .HasForeignKey(s => s.AccountTypeProfileDenominationID).OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(s => s.Commission).WithMany(s => s.AccountTypeProfileCommissions)
                .HasForeignKey(s => s.CommissionID).OnDelete(DeleteBehavior.NoAction);
        }
    }
}

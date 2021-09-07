using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using TMS.Data.Entities;

namespace TMS.Data.Configurations
{
    public class AccountCommissionConfiguration : IEntityTypeConfiguration<AccountCommission>
    {
        public void Configure(EntityTypeBuilder<AccountCommission> builder)
        {
            builder.HasKey(s => s.ID);
            builder.HasOne(s => s.Commission).WithMany(s => s.AccountCommissions)
                .HasForeignKey(s => s.CommessionID).OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(s => s.Denomination).WithMany(s => s.AccountCommissions)
                .HasForeignKey(s => s.DenominationID).OnDelete(DeleteBehavior.NoAction);
        }
    }
}

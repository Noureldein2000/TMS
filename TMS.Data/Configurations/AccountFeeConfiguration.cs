using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using TMS.Data.Entities;

namespace TMS.Data.Configurations
{
    public class AccountFeeConfiguration : IEntityTypeConfiguration<AccountFee>
    {
        public void Configure(EntityTypeBuilder<AccountFee> builder)
        {
            builder.HasKey(s => s.ID);
            builder.HasOne(s => s.Fee).WithMany(s => s.AccountFees)
                .HasForeignKey(s => s.FeesID).OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(s => s.Denomination).WithMany(s => s.AccountFees)
                .HasForeignKey(s => s.DenominationID).OnDelete(DeleteBehavior.NoAction);
        }
    }
}

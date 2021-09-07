using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using TMS.Data.Entities;

namespace TMS.Data.Configurations
{
    public class AccountProfileFeeConfiguration : IEntityTypeConfiguration<AccountProfileFee>
    {
        public void Configure(EntityTypeBuilder<AccountProfileFee> builder)
        {
            builder.HasKey(s => s.ID);
            builder.HasOne(s => s.AccountProfileDenomination).WithMany(s => s.AccountProfileFees)
                .HasForeignKey(s => s.AccountProfileDenominationID).OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(s => s.Fee).WithMany(s => s.AccountProfileFees)
                .HasForeignKey(s => s.FeesID).OnDelete(DeleteBehavior.NoAction);
        }
    }
}

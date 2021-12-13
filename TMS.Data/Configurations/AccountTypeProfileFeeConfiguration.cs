using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using TMS.Data.Entities;

namespace TMS.Data.Configurations
{
    public class AccountTypeProfileFeeConfiguration : IEntityTypeConfiguration<AccountTypeProfileFee>
    {
        public void Configure(EntityTypeBuilder<AccountTypeProfileFee> builder)
        {
            builder.HasKey(s => s.ID);
            builder.HasOne(s => s.AccountTypeProfileDenomination).WithMany(s => s.AccountTypeProfileFees)
                .HasForeignKey(s => s.AccountTypeProfileDenominationID).OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(s => s.Fee).WithMany(s => s.AccountTypeProfileFees)
                .HasForeignKey(s => s.FeesID).OnDelete(DeleteBehavior.NoAction);
        }
    }
}

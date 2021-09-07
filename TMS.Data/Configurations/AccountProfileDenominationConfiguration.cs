using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using TMS.Data.Entities;

namespace TMS.Data.Configurations
{
    public class AccountProfileDenominationConfiguration : IEntityTypeConfiguration<AccountProfileDenomination>
    {
        public void Configure(EntityTypeBuilder<AccountProfileDenomination> builder)
        {
            builder.HasKey(s => s.ID);
            builder.HasOne(s => s.Denomination).WithMany(s => s.AccountProfileDenominations)
                .HasForeignKey(s => s.DenominationID).OnDelete(DeleteBehavior.NoAction);
        }
    }
}

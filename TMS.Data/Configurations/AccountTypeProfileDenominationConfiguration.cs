using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using TMS.Data.Entities;

namespace TMS.Data.Configurations
{
    public class AccountTypeProfileDenominationConfiguration : IEntityTypeConfiguration<AccountTypeProfileDenomination>
    {
        public void Configure(EntityTypeBuilder<AccountTypeProfileDenomination> builder)
        {
            builder.HasKey(s => s.ID);

            builder.HasOne(s => s.Denomination).WithMany(s => s.AccountTypeProfileDenominations)
                .HasForeignKey(s => s.DenominationID).OnDelete(DeleteBehavior.NoAction);

            builder.HasIndex(s => new { s.DenominationID, s.AccountTypeProfileID }).IsUnique();
        }
    }
}

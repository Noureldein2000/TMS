using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using TMS.Data.Entities;

namespace TMS.Data.Configurations
{
    public class DenominationCommissionConfiguration : IEntityTypeConfiguration<DenominationCommission>
    {
        public void Configure(EntityTypeBuilder<DenominationCommission> builder)
        {
            builder.HasKey(s => s.ID);
            builder.HasOne(s => s.Denomination).WithMany(s => s.DenominationCommissions)
                .HasForeignKey(s => s.DenominationID).OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(s => s.Commission).WithMany(s => s.DenominationCommissions)
                .HasForeignKey(s => s.CommissionID).OnDelete(DeleteBehavior.NoAction);
        }
    }
}

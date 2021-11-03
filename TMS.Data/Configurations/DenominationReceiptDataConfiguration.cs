using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using TMS.Data.Entities;

namespace TMS.Data.Configurations
{
    public class DenominationReceiptDataConfiguration : IEntityTypeConfiguration<DenominationReceiptData>
    {
        public void Configure(EntityTypeBuilder<DenominationReceiptData> builder)
        {
            builder.HasKey(s => s.ID);
            builder.HasOne(s => s.Denomination).WithOne(s => s.DenominationReceiptData).OnDelete(DeleteBehavior.NoAction);

            builder.Property(s => s.Title).HasMaxLength(2000);
            builder.Property(s => s.Disclaimer).HasMaxLength(2000);
            builder.Property(s => s.Footer).HasMaxLength(2000);

        }
    }
}

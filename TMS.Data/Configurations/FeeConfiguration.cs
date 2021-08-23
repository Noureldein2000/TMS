using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using TMS.Data.Entities;

namespace TMS.Data.Configurations
{
    public class FeeConfiguration : IEntityTypeConfiguration<Fee>
    {
        public void Configure(EntityTypeBuilder<Fee> builder)
        {
            builder.HasKey(s => s.ID);
            builder.Property(s => s.AmountFrom).HasColumnType("decimal(18,3)");
            builder.Property(s => s.AmountTo).HasColumnType("decimal(18,3)");
            builder.HasOne(s => s.FeesType).WithMany(s => s.Fees).HasForeignKey(s => s.FeesTypeID)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using TMS.Data.Entities;

namespace TMS.Data.Configurations
{
    public class DenominationParamConfiguration : IEntityTypeConfiguration<DenominationParam>
    {
        public void Configure(EntityTypeBuilder<DenominationParam> builder)
        {
            builder.HasKey(s => s.ID);
            builder.Property(s => s.Label).HasMaxLength(50);
            builder.Property(s => s.Title).HasMaxLength(200);
            builder.Property(s => s.ParamKey).HasMaxLength(200);

            builder.HasOne(s => s.DenominationParamValueType).WithMany(s => s.DenominationParams)
                .HasForeignKey(s => s.ValueTypeID).OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(s => s.DenominationParamValueMode).WithMany(s => s.DenominationParams)
                .HasForeignKey(s => s.ValueModeID).OnDelete(DeleteBehavior.NoAction);
        }
    }
}

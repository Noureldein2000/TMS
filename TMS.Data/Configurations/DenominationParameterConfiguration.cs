using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using TMS.Data.Entities;

namespace TMS.Data.Configurations
{
    public class DenominationParameterConfiguration : IEntityTypeConfiguration<DenominationParameter>
    {
        public void Configure(EntityTypeBuilder<DenominationParameter> builder)
        {
            builder.HasKey(s => s.ID);
            builder.Property(s => s.Value).HasMaxLength(100);
            builder.Property(s => s.ValueList).HasMaxLength(1000);
            builder.Property(s => s.ValidationMessage).HasMaxLength(1000);
            builder.Property(s => s.ValidationExpression).HasMaxLength(1000);
            builder.HasOne(s => s.DenominationParam).WithMany(s => s.DenominationParameters)
                .HasForeignKey(s => s.DenominationParamID).OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(s => s.Denomination).WithMany(s => s.DenominationParameters)
                .HasForeignKey(s => s.DenominationID).OnDelete(DeleteBehavior.NoAction);

        }
    }
}

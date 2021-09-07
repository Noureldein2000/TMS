using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using TMS.Data.Entities;

namespace TMS.Data.Configurations
{
    public class DenominationProviderConfigurationConfiguration : IEntityTypeConfiguration<DenominationProviderConfiguration>
    {
        public void Configure(EntityTypeBuilder<DenominationProviderConfiguration> builder)
        {
            builder.HasKey(s => s.ID);
            builder.Property(s => s.Name).HasMaxLength(50);
            builder.Property(s => s.Value).HasMaxLength(50);
            builder.HasOne(s => s.DenominationServiceProvider).WithMany(s => s.DenominationProviderConfigerations)
                .HasForeignKey(s => s.DenominationProviderID).OnDelete(DeleteBehavior.NoAction);
        }
    }
}

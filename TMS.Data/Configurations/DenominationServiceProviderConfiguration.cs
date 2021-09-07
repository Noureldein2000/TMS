using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using TMS.Data.Entities;

namespace TMS.Data.Configurations
{
    public class DenominationServiceProviderConfiguration : IEntityTypeConfiguration<DenominationServiceProvider>
    {
        public void Configure(EntityTypeBuilder<DenominationServiceProvider> builder)
        {
            builder.HasKey(s => s.ID);
            builder.Property(s => s.Balance).HasColumnType("decimal(18,3)");
            builder.Property(s => s.ProviderAmount).HasColumnType("decimal(18,3)");
            builder.HasOne(s => s.ServiceProvider).WithMany(s => s.DenominationServiceProviders)
                .HasForeignKey(s => s.ServiceProviderID)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(s => s.Denomination).WithMany(s => s.DenominationServiceProviders)
                .HasForeignKey(s => s.DenominationID)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}

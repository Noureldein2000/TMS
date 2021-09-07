using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using TMS.Data.Entities;

namespace TMS.Data.Configurations
{
    public class ProviderStatusCodeConfiguration : IEntityTypeConfiguration<ProviderStatusCode>
    {
        public void Configure(EntityTypeBuilder<ProviderStatusCode> builder)
        {
            builder.HasKey(s => s.ID);
            builder.Property(s => s.StatusCode).HasMaxLength(50);
            builder.Property(s => s.ProviderMessage).HasMaxLength(100);
            builder.HasOne(s => s.ServiceProvider).WithMany(s => s.ProviderStatusCodes)
                .HasForeignKey(s => s.ServiceProviderID).OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(s => s.StatusCodeModel).WithMany(s => s.ProviderStatusCodes)
                .HasForeignKey(s => s.StatusCodeID).OnDelete(DeleteBehavior.NoAction);
        }
    }
}

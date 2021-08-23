using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using TMS.Data.Entities;

namespace TMS.Data.Configurations
{
    public class ProviderServiceResponseConfiguration : IEntityTypeConfiguration<ProviderServiceResponse>
    {
        public void Configure(EntityTypeBuilder<ProviderServiceResponse> builder)
        {
            builder.HasKey(s => s.ID);
            builder.Property(s => s.TotalAmount).HasColumnType("decimal(18,3)");
            builder.HasMany(s => s.InquiryBills).WithOne(s => s.ProviderServiceResponse)
                .HasForeignKey(s => s.ProviderServiceResponseID)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(s => s.ProviderServiceResponseParams).WithOne(s => s.ProviderServiceResponse)
                .HasForeignKey(s => s.ProviderServiceResponseID)
                .OnDelete(DeleteBehavior.NoAction);

        }
    }
}

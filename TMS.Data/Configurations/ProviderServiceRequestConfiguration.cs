using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using TMS.Data.Entities;

namespace TMS.Data.Configurations
{
    public class ProviderServiceRequestConfiguration : IEntityTypeConfiguration<ProviderServiceRequest>
    {
        public void Configure(EntityTypeBuilder<ProviderServiceRequest> builder)
        {
            builder.HasKey(s => s.ID);
            builder.HasMany(s => s.ProviderServiceResponses).WithOne(s => s.ProviderServiceRequest)
                .HasForeignKey(s => s.ProviderServiceRequestID)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(s => s.ReceiptBodyParams).WithOne(s => s.ProviderServiceRequest)
                .HasForeignKey(s => s.ProviderServiceRequestID)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}

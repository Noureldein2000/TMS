using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using TMS.Data.Entities;

namespace TMS.Data.Configurations
{
    public class RequestTypeConfiguration : IEntityTypeConfiguration<RequestType>
    {
        public void Configure(EntityTypeBuilder<RequestType> builder)
        {
            builder.HasKey(s => s.ID);
            builder.HasMany(s => s.ProviderServiceRequests).WithOne(s => s.RequestType)
                .HasForeignKey(s => s.RequestTypeID)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}

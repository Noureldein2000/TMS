﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using TMS.Data.Entities;

namespace TMS.Data.Configurations
{
    public class ProviderServiceRequestStatusConfiguration : IEntityTypeConfiguration<ProviderServiceRequestStatus>
    {
        public void Configure(EntityTypeBuilder<ProviderServiceRequestStatus> builder)
        {
            builder.HasKey(s => s.ID);
            builder.HasMany(s => s.ProviderServiceRequests).WithOne(s => s.ProviderServiceRequestStatus)
                .HasForeignKey(s => s.ProviderServiceRequestStatusID)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}

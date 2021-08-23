using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using TMS.Data.Entities;

namespace TMS.Data.Configurations
{
    public class ServiceConfigerationConfiguration : IEntityTypeConfiguration<ServiceConfigeration>
    {
        public void Configure(EntityTypeBuilder<ServiceConfigeration> builder)
        {
            builder.HasKey(s => s.ID);
            builder.HasMany(s => s.ServiceConfigParms).WithOne(s => s.ServiceConfigeration)
                .HasForeignKey(s => s.ServiceConfigerationID)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}

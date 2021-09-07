using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using TMS.Data.Entities;

namespace TMS.Data.Configurations
{
    public class ServiceConfiguration : IEntityTypeConfiguration<Service>
    {
        public void Configure(EntityTypeBuilder<Service> builder)
        {
            builder.HasKey(s => s.ID);

            builder.HasOne(s => s.ServiceEntity).WithMany(s => s.Services)
               .HasForeignKey(s => s.ServiceEntityID).OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(s => s.ServiceType).WithMany(s => s.Services)
               .HasForeignKey(s => s.ServiceTypeID).OnDelete(DeleteBehavior.NoAction);
        }
    }
}

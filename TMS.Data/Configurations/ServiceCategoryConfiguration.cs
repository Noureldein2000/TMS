using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using TMS.Data.Entities;

namespace TMS.Data.Configurations
{
    public class ServiceCategoryConfiguration : IEntityTypeConfiguration<ServiceCategory>
    {
        public void Configure(EntityTypeBuilder<ServiceCategory> builder)
        {
            builder.HasKey(s => s.ID);
            builder.HasOne(s => s.Parent).WithMany(s => s.Children).HasForeignKey(s => s.ParentID)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}

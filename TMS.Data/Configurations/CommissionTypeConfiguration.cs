using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using TMS.Data.Entities;

namespace TMS.Data.Configurations
{
    public class CommissionTypeConfiguration : IEntityTypeConfiguration<CommissionType>
    {
        public void Configure(EntityTypeBuilder<CommissionType> builder)
        {
            builder.HasKey(s => s.ID);
            builder.Property(s => s.Name).HasMaxLength(50);
            builder.Property(s => s.ArName).HasMaxLength(50);
        }
    }
}

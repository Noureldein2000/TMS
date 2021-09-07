using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using TMS.Data.Entities;

namespace TMS.Data.Configurations
{
    public class MainStatusCodeConfigurtion : IEntityTypeConfiguration<MainStatusCode>
    {
        public void Configure(EntityTypeBuilder<MainStatusCode> builder)
        {

            builder.HasKey(s => s.ID);
            builder.Property(s => s.Code).HasMaxLength(10);
            builder.Property(s => s.Message).HasMaxLength(100);
            builder.Property(s => s.ArMessage).HasMaxLength(100);
        }
    }
}

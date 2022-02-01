using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using TMS.Data.Entities;

namespace TMS.Data.Configurations
{
    public class TaxesTypeConfiguration : IEntityTypeConfiguration<TaxType>
    {
        public void Configure(EntityTypeBuilder<TaxType> builder)
        {
            builder.HasKey(s => s.ID);
            builder.Property(s => s.Name).HasColumnType("nvarchar(50)");
            builder.Property(s => s.ArName).HasColumnType("nvarchar(50)");
        }
    }
}

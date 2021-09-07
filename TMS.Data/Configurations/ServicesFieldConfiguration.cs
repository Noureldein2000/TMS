using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using TMS.Data.Entities;

namespace TMS.Data.Configurations
{
    public class ServicesFieldConfiguration : IEntityTypeConfiguration<ServicesField>
    {
        public void Configure(EntityTypeBuilder<ServicesField> builder)
        {
            builder.HasKey(s => s.ID);
            builder.Property(s => s.FieldName).HasMaxLength(50);
            builder.Property(s => s.EnglishFieldName).HasMaxLength(50);

            builder.HasOne(s => s.FieldType).WithMany(s => s.ServicesFields)
                .HasForeignKey(s => s.FieldTypeID).OnDelete(DeleteBehavior.NoAction);
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using TMS.Data.Entities;

namespace TMS.Data.Configurations
{
    public class StatusCodeConfiguration : IEntityTypeConfiguration<StatusCode>
    {
        public void Configure(EntityTypeBuilder<StatusCode> builder)
        {
            builder.HasKey(s => s.ID);
            builder.Property(s => s.Message).HasMaxLength(150);
            builder.Property(s => s.ArMessage).HasMaxLength(150);
            builder.Property(s => s.Code).HasMaxLength(10);

            builder.HasOne(s => s.MainStatusCode).WithMany(s => s.StatusCodes)
                .HasForeignKey(s => s.MainStatusCodeID).OnDelete(DeleteBehavior.NoAction);
        }
    }
}

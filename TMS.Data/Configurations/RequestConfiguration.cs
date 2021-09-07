using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using TMS.Data.Entities;

namespace TMS.Data.Configurations
{
    public class RequestConfiguration : IEntityTypeConfiguration<Request>
    {
        public void Configure(EntityTypeBuilder<Request> builder)
        {
            builder.HasKey(s => s.ID);
            builder.Property(s => s.Amount).HasColumnType("decimal(18, 3)");
            builder.Property(s => s.UUID).HasMaxLength(50);
            builder.Property(s => s.RRN).HasMaxLength(150);
            builder.Property(s => s.BillingAccount).HasMaxLength(200);
            builder.Property(s => s.ChannelID).HasMaxLength(50);

            builder.HasOne(s => s.Denomination).WithMany(s => s.Requests)
                .HasForeignKey(s => s.ServiceDenominationID).OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(s => s.RequestStatus).WithMany(s => s.Requests)
                .HasForeignKey(s => s.StatusID).OnDelete(DeleteBehavior.NoAction);
        }
    }
}

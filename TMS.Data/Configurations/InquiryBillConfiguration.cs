using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using TMS.Data.Entities;

namespace TMS.Data.Configurations
{
    public class InquiryBillConfiguration : IEntityTypeConfiguration<InquiryBill>
    {
        public void Configure(EntityTypeBuilder<InquiryBill> builder)
        {
            builder.HasKey(s => s.ID);
            builder.Property(s => s.Amount).HasColumnType("decimal(18,3)");
            builder.HasMany(s => s.InquiryBillDetails).WithOne(s => s.InquiryBill)
                .HasForeignKey(s => s.InquiryBillID)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}

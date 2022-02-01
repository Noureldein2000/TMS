using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using TMS.Data.Entities;

namespace TMS.Data.Configurations
{
    public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> builder)
        {
            builder.HasKey(s => s.ID);
            builder.Property(s => s.OriginalAmount).HasColumnType("decimal(18, 3)");
            builder.Property(s => s.TotalAmount).HasColumnType("decimal(18, 3)");
            builder.Property(s => s.Fees).HasColumnType("decimal(18, 3)");
            builder.Property(s => s.Taxes).HasColumnType("decimal(18, 3)");
        }
    }
}

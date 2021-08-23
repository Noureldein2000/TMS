using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using TMS.Data.Entities;

namespace TMS.Data.Configurations
{
    public class ParameterConfiguration : IEntityTypeConfiguration<Parameter>
    {
        public void Configure(EntityTypeBuilder<Parameter> builder)
        {
            builder.HasKey(s => s.ID);
            builder.HasMany(s => s.ProviderServiceRequestParams).WithOne(s => s.Parameter)
                .HasForeignKey(s => s.ParameterID)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(s => s.ProviderServiceResponseParams).WithOne(s => s.Parameter)
                .HasForeignKey(s => s.ParameterID)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(s => s.ReceiptBodyParams).WithOne(s => s.Parameter)
                .HasForeignKey(s => s.ParameterID)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(s => s.InquiryBillDetails).WithOne(s => s.Parameter)
                .HasForeignKey(s => s.ParameterID)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}

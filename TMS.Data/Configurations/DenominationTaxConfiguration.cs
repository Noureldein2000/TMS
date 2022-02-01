using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using TMS.Data.Entities;

namespace TMS.Data.Configurations
{
    public class DenominationTaxConfiguration : IEntityTypeConfiguration<DenominationTax>
    {
        public void Configure(EntityTypeBuilder<DenominationTax> builder)
        {
            builder.HasKey(s => s.ID);

            builder.HasOne(s => s.Tax).WithMany(s => s.DenominationTax)
                .HasForeignKey(s => s.TaxID)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(s => s.Denomination).WithMany(s => s.DenominationTaxes)
                .HasForeignKey(s => s.DenominationID)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}

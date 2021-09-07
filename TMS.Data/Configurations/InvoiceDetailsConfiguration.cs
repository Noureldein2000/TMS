using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using TMS.Data.Entities;

namespace TMS.Data.Configurations
{
    //public class InvoiceDetailsConfiguration : IEntityTypeConfiguration<InvoiceDetails>
    //{
    //    public void Configure(EntityTypeBuilder<InvoiceDetails> builder)
    //    {
    //        builder.HasKey(s => s.ID);
    //        builder.Property(s => s.Value).HasMaxLength(1000);

    //        builder.HasOne(s => s.Invoice).WithMany(s => s.InvoiceDetails)
    //            .HasForeignKey(s => s.InvoiceId).OnDelete(DeleteBehavior.NoAction);

    //        builder.HasOne(s => s.ServicesField).WithMany(s => s.InvoiceDetails)
    //           .HasForeignKey(s => s.ServiceFieldId).OnDelete(DeleteBehavior.NoAction);
    //    }
    //}
}

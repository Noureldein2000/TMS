using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using TMS.Data.Entities;

namespace TMS.Data.Configurations
{
    //public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
    //{
    //    public void Configure(EntityTypeBuilder<Invoice> builder)
    //    {
    //        builder.HasKey(s => s.ID);
    //        builder.Property(s => s.InCome).HasColumnType("decimal(18, 3)");
    //        builder.Property(s => s.added_money).HasColumnType("decimal(18, 3)");
    //        builder.Property(s => s.faw_paid).HasColumnType("decimal(18, 3)");
    //        builder.Property(s => s.faw_fees).HasColumnType("decimal(18, 3)");
    //        builder.Property(s => s.Basci_val).HasColumnType("decimal(18, 3)");
    //        builder.Property(s => s.InCome_cash).HasColumnType("decimal(18, 3)");
    //        builder.Property(s => s.ClientName).HasMaxLength(80);
    //        builder.Property(s => s.ClientPhone).HasMaxLength(150);
    //        builder.Property(s => s.cause).HasMaxLength(500);
    //        builder.Property(s => s.faw_FPTN).HasMaxLength(150);
    //        builder.Property(s => s.faw_FCRN).HasMaxLength(150);
    //        builder.Property(s => s.faw_status_code).HasMaxLength(350);
    //        builder.Property(s => s.faw_status_message).HasMaxLength(350);
    //        builder.Property(s => s.Provider_Response).HasMaxLength(500);
    //        builder.Property(s => s.account_number).HasMaxLength(50);
    //        builder.Property(s => s.Code).HasMaxLength(50);
    //        builder.Property(s => s.Message).HasMaxLength(50);
    //        builder.Property(s => s.data).HasMaxLength(500);
    //        builder.Property(s => s.tel_code).HasMaxLength(50);
    //        builder.Property(s => s.tel_number).HasMaxLength(50);
    //        builder.Property(s => s.ProviderCardTransactionId).HasMaxLength(500);
    //        builder.Property(s => s.ECardmomknPaymentId).HasMaxLength(500);

    //    }
    //}
}

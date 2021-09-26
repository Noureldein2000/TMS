using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using TMS.Data.Entities;

namespace TMS.Data.Configurations
{
    class PendingPaymentCardConfiguration : IEntityTypeConfiguration<PendingPaymentCard>
    {
        public void Configure(EntityTypeBuilder<PendingPaymentCard> builder)
        {
            builder.HasKey(s => s.ID);

            builder.HasOne(s => s.Transaction).WithMany(s => s.PendingPaymentCards)
                .HasForeignKey(s => s.TransactionID).OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(s => s.CardType).WithMany(s => s.PendingPaymentCards)
               .HasForeignKey(s => s.CardTypeID).OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(s => s.PendingPaymentCardStatus).WithMany(s => s.PendingPaymentCards)
               .HasForeignKey(s => s.PengingPaymentCardStatusID).OnDelete(DeleteBehavior.NoAction);

        }
    }
}

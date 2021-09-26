using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using TMS.Data.Entities;

namespace TMS.Data.Configurations
{
    class PendingPaymentCardStatusConfiguration : IEntityTypeConfiguration<PendingPaymentCardStatus>
    {
        public void Configure(EntityTypeBuilder<PendingPaymentCardStatus> builder)
        {
            builder.HasKey(s => s.ID);

            builder.HasData(
               new PendingPaymentCardStatus
               {
                   ID = 1,
                   Name = "Initiated",
                   NameAr = "بدأت",
                   CreationDate = DateTime.Now
               },
                new PendingPaymentCardStatus
                {
                    ID = 2,
                    Name = "Canceled",
                    NameAr = "ألغيت",
                    CreationDate = DateTime.Now
                },
                new PendingPaymentCardStatus
                {
                    ID = 3,
                    Name = "Confirmed",
                    NameAr = "مؤكد",
                    CreationDate = DateTime.Now
                },
                new PendingPaymentCardStatus
                {
                    ID = 4,
                    Name = "AutoCanceled",
                    NameAr = "مُلغى تلقائيًا",
                    CreationDate = DateTime.Now
                });
        }

    }
}

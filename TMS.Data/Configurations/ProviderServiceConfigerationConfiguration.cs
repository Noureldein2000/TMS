using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using TMS.Data.Entities;

namespace TMS.Data.Configurations
{
    public class ProviderServiceConfigerationConfiguration : IEntityTypeConfiguration<ProviderServiceConfigeration>
    {
        public void Configure(EntityTypeBuilder<ProviderServiceConfigeration> builder)
        {
            builder.HasKey(s => s.ID);
            builder.HasOne(s => s.ServiceConfigeration).WithMany(s => s.ProviderServiceConfigerations)
                .HasForeignKey(s => s.ServiceConfigerationID)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(s => s.DenominationServiceProvider).WithOne(s => s.ProviderServiceConfigerations)
               .OnDelete(DeleteBehavior.NoAction);
        }
    }
}

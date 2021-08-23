using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace TMS.Data.Configurations
{
    //public class BalanceTypeConfiguration : IEntityTypeConfiguration<BalanceType>
    //{
    //    public void Configure(EntityTypeBuilder<BalanceType> builder)
    //    {
    //        builder.HasKey(s => s.ID);
    //        builder.Property(s => s.Name).IsRequired();
    //        builder.Property(s => s.ArName).IsRequired();
    //        builder.HasData(
    //            new BalanceType
    //            {
    //                ID = 1,
    //                ArName = "رصيد ممكن",
    //                Name = "Momkm Balance"
    //            },
    //            new BalanceType
    //            {
    //                ID = 2,
    //                ArName = "رصيد كاش",
    //                Name = "Cash Balance"
    //            });
    //    }
    //}
}

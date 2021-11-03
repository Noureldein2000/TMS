using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using TMS.Data.Entities;

namespace TMS.Data.Configurations
{
    public class DenominationParamValueTypeConfiguration : IEntityTypeConfiguration<DenominationParamValueType>
    {
        public void Configure(EntityTypeBuilder<DenominationParamValueType> builder)
        {
            builder.HasKey(s => s.ID);
            builder.HasData(
                new DenominationParamValueType
                {
                    ID = 1,
                    Name = "Number",
                    CreationDate = DateTime.Now
                },
                new DenominationParamValueType
                {
                    ID = 2,
                    Name = "String",
                    CreationDate = DateTime.Now
                },
                new DenominationParamValueType
                {
                    ID = 3,
                    Name = "List",
                    CreationDate = DateTime.Now
                },
                new DenominationParamValueType
                {
                    ID = 4,
                    Name = "Date",
                    CreationDate = DateTime.Now
                });
        }
    }
}

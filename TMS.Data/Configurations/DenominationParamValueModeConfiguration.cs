using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using TMS.Data.Entities;

namespace TMS.Data.Configurations
{
    public class DenominationParamValueModeConfiguration : IEntityTypeConfiguration<DenominationParamValueMode>
    {
        public void Configure(EntityTypeBuilder<DenominationParamValueMode> builder)
        {
            builder.HasKey(s => s.ID);
            builder.HasData(
                new DenominationParamValueMode
                {
                    ID = 1,
                    Name = "FIXED",
                    CreationDate = DateTime.Now
                },
                new DenominationParamValueType
                {
                    ID = 2,
                    Name = "DYNAMIC",
                    CreationDate = DateTime.Now
                });
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace TMS.Data.Entities
{
    public class DenominationProviderConfiguration : BaseEntity<int>
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public int DenominationProviderID { get; set; }
        public virtual DenominationServiceProvider DenominationServiceProvider { get; set; }
    }
}

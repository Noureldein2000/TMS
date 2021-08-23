using System;
using System.Collections.Generic;
using System.Text;

namespace TMS.Data.Entities
{
    public class ServiceProvider : BaseEntity<int>
    {
        public string Name { get; set; }
        public virtual ICollection<DenominationServiceProvider> DenominationServiceProviders { get; set; }
    }
}

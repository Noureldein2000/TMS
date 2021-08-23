using System;
using System.Collections.Generic;
using System.Text;

namespace TMS.Data.Entities
{
    public class ProviderServiceConfigeration : BaseEntity<int>
    {
        public int DenominationServiceProviderID { get; set; }
        public virtual DenominationServiceProvider DenominationServiceProvider { get; set; }
        public int ServiceConfigerationID { get; set; }
        public virtual ServiceConfigeration ServiceConfigeration { get; set; }
    }
}

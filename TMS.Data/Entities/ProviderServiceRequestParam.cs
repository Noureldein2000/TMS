using System;
using System.Collections.Generic;
using System.Text;

namespace TMS.Data.Entities
{
    public class ProviderServiceRequestParam : BaseEntity<int>
    {
        public int ProviderServiceRequestID { get; set; }
        public virtual ProviderServiceRequest ProviderServiceRequest { get; set; }
        public int ParameterID { get; set; }
        public virtual Parameter Parameter { get; set; }
        public string Value { get; set; }
    }
}

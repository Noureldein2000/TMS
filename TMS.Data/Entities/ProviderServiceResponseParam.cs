using System;
using System.Collections.Generic;
using System.Text;

namespace TMS.Data.Entities
{
    public class ProviderServiceResponseParam : BaseEntity<int>
    {
        public int ProviderServiceResponseID { get; set; }
        public virtual ProviderServiceResponse ProviderServiceResponse { get; set; }
        public int ParameterID { get; set; }
        public virtual Parameter Parameter { get; set; }
    }
}

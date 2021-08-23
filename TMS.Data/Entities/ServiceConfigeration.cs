using System;
using System.Collections.Generic;
using System.Text;

namespace TMS.Data.Entities
{
    public class ServiceConfigeration : BaseEntity<int>
    {
        public string URL { get; set; }
        public int TimeOut { get; set; }
        public string UserName { get; set; }
        public string UserPassword { get; set; }
        public virtual ICollection<ServiceConfigParms> ServiceConfigParms { get; set; }
        public virtual ICollection<ProviderServiceConfigeration> ProviderServiceConfigerations { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace TMS.Data.Entities
{
    public class ServiceType : BaseEntity<int>
    {
        public string Name { get; set; }
        public virtual ICollection<Service> Services { get; set; }
    }
}

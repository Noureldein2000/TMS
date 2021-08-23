using System;
using System.Collections.Generic;
using System.Text;

namespace TMS.Data.Entities
{
    public class ServiceConfigParms : BaseEntity<int>
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public int ServiceConfigerationID { get; set; }
        public virtual ServiceConfigeration ServiceConfigeration { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using TMS.Infrastructure;

namespace TMS.Data.Entities
{
    public class Service : BaseEntity<int>
    {
        public string Name { get; set; }
        public string ArName { get; set; }
        public bool Status { get; set; }
        public int ServiceTypeID { get; set; }
        public virtual ServiceType ServiceType { get; set; }
        public string Code { get; set; }
        public int ServiceEntityID { get; set; }
        public virtual ServiceEntity ServiceEntity { get; set; }
        public int? ServiceCategoryID { get; set; }
        public virtual ServiceCategory ServiceCategory { get; set; }
        public string PathClass { get; set; }
        public ServiceClassType ClassType { get; set; }
        public virtual ICollection<Denomination> Denominations { get; set; }
        public virtual ICollection<ServiceBalanceType> ServiceBalanceTypes { get; set; }
    }
}

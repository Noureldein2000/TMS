using System;
using System.Collections.Generic;
using System.Text;

namespace TMS.Data.Entities
{
    public class ServiceCategory : BaseEntity<int>
    {
        public string Name { get; set; }
        public string ArName { get; set; }
        public bool LastNode { get; set; }
        public int ServiceIndex { get; set; }
        public int ServiceLevel { get; set; }
        public string ServiceSubCategory { get; set; }
        public string Title { get; set; }
        public int? ParentID { get; set; }
        public virtual ServiceCategory Parent { get; set; }
        public virtual ICollection<ServiceCategory> Children { get; set; }
        public virtual ICollection<Denomination> Denominations { get; set; }
    }
}

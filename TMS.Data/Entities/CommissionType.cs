using System;
using System.Collections.Generic;
using System.Text;

namespace TMS.Data.Entities
{
    public class CommissionType : BaseEntity<int>, ILookupType
    {
        public string Name { get; set; }
        public string ArName { get; set; }
        public virtual ICollection<Commission> Commissions { get; set; }
    }
}

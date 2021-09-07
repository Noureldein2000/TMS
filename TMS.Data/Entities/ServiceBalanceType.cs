using System;
using System.Collections.Generic;
using System.Text;

namespace TMS.Data.Entities
{
    public class ServiceBalanceType : BaseEntity<int>
    {
        public int ServiceID { get; set; }
        public virtual Service Service { get; set; }
        public int BalanceTypeID { get; set; }
    }
}

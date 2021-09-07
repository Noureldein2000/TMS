using System;
using System.Collections.Generic;
using System.Text;

namespace TMS.Data.Entities
{
    public class BillPaymentMode : BaseEntity<int>
    {
        public string Name { get; set; }
        public string ArName { get; set; }
        public virtual ICollection<Denomination> Denominations { get; set; }
    }
}

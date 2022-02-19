using System;
using System.Collections.Generic;
using System.Text;

namespace TMS.Data.Entities
{
    public class PaymentMode : BaseEntity<int>
    {
        public string Name { get; set; }
        public string ArName { get; set; }
        public virtual ICollection<Denomination> Denominations { get; set; }
        public virtual ICollection<Commission> Commissions { get; set; }
        public virtual ICollection<Fee> Fees { get; set; }
        public virtual ICollection<Tax> Taxes { get; set; }
    }
}

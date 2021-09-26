using System;
using System.Collections.Generic;
using System.Text;

namespace TMS.Data.Entities
{
    public class PendingPaymentCardStatus : BaseEntity<int>
    {
        public string Name { get; set; }
        public string NameAr { get; set; }
        public virtual ICollection<PendingPaymentCard> PendingPaymentCards { get; set; }
    }
}

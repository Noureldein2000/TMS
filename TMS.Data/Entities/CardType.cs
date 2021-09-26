using System;
using System.Collections.Generic;
using System.Text;

namespace TMS.Data.Entities
{
    public class CardType : BaseEntity<int>
    {
        public string Name { get; set; }
        public string NameAr { get; set; }
        public string Abbreviation { get; set; }
        public ICollection<PendingPaymentCard> PendingPaymentCards { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace TMS.Data.Entities
{
    public class AccountFee : BaseEntity<int>
    {
        public int AccountID { get; set; }
        public int FeesID { get; set; }
        public virtual Fee Fee { get; set; }
        public int DenominationID { get; set; }
        public virtual Denomination Denomination { get; set; }
    }
}

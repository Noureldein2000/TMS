using System;
using System.Collections.Generic;
using System.Text;

namespace TMS.Data.Entities
{
    public class AccountProfileFee : BaseEntity<int>
    {
        public int FeesID { get; set; }
        public virtual Fee Fee { get; set; }
        public int AccountProfileDenominationID { get; set; }
        public virtual AccountProfileDenomination AccountProfileDenomination { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace TMS.Data.Entities
{
    public class AccountTypeProfileFee : BaseEntity<int>
    {
        public int FeesID { get; set; }
        public virtual Fee Fee { get; set; }
        public int AccountTypeProfileDenominationID { get; set; }
        public virtual AccountTypeProfileDenomination AccountTypeProfileDenomination { get; set; }
    }
}

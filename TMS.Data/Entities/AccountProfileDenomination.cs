using System;
using System.Collections.Generic;
using System.Text;

namespace TMS.Data.Entities
{
    public class AccountProfileDenomination : BaseEntity<int>
    {
        public int AccountProfileID { get; set; }
        public int DenominationID { get; set; }
        public virtual Denomination Denomination { get; set; }
        public bool Status { get; set; }
        public virtual ICollection<AccountProfileFee> AccountProfileFees { get; set; }
        public virtual ICollection<AccountProfileCommission> AccountProfileCommissions { get; set; }
    }
}

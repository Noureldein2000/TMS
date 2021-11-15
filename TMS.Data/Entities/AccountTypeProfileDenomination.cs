using System;
using System.Collections.Generic;
using System.Text;

namespace TMS.Data.Entities
{
    public class AccountTypeProfileDenomination : BaseEntity<int>
    {
        public int AccountTypeProfileID { get; set; }
        public int DenominationID { get; set; }
        public virtual Denomination Denomination { get; set; }
        public bool Status { get; set; }
        public virtual ICollection<AccountTypeProfileFee> AccountTypeProfileFees { get; set; }
        public virtual ICollection<AccountTypeProfileCommission> AccountTypeProfileCommissions { get; set; }
    }
}

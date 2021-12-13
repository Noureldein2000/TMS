using System;
using System.Collections.Generic;
using System.Text;

namespace TMS.Data.Entities
{
    public class AccountTypeProfileCommission : BaseEntity<int>
    {
        public int CommissionID { get; set; }
        public virtual Commission Commission { get; set; }
        public int AccountTypeProfileDenominationID { get; set; }
        public virtual AccountTypeProfileDenomination AccountTypeProfileDenomination { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace TMS.Data.Entities
{
    public class AccountProfileCommission : BaseEntity<int>
    {
        public int CommissionID { get; set; }
        public virtual Commission Commission { get; set; }
        public int AccountProfileDenominationID { get; set; }
        public virtual AccountProfileDenomination AccountProfileDenomination { get; set; }
    }
}

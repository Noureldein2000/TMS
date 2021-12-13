using System;
using System.Collections.Generic;
using System.Text;

namespace TMS.Data.Entities
{
    public class Commission : BaseEntity<int>
    {
        public decimal AmountFrom { get; set; }
        public decimal AmountTo { get; set; }
        public int PaymentModeID { get; set; }
        public virtual PaymentMode PaymentMode { get; set; }
        public decimal Value { get; set; }
        public bool Status { get; set; }
        public int CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int CommissionTypeID { get; set; }
        public virtual CommissionType CommissionType { get; set; }
        public virtual ICollection<DenominationCommission> DenominationCommissions { get; set; }
        public virtual ICollection<AccountCommission> AccountCommissions { get; set; }
        public virtual ICollection<AccountTypeProfileCommission> AccountTypeProfileCommissions { get; set; }
    }
}

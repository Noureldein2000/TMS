using System;
using System.Collections.Generic;
using System.Text;

namespace TMS.Data.Entities
{
    public class Fee : BaseEntity<int>
    {
        public decimal AmountFrom { get; set; }
        public decimal AmountTo { get; set; }
        public int PaymentModeID { get; set; }
        public decimal Value { get; set; }
        public bool Status { get; set; }
        public int CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int FeesTypeID { get; set; }
        public virtual FeesType FeesType { get; set; }
        public virtual ICollection<DenominationFee> DenominationFees { get; set; }
        public virtual ICollection<AccountFee> AccountFees { get; set; }
        public virtual ICollection<AccountProfileFee> AccountProfileFees { get; set; }
    }
}

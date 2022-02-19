using System;
using System.Collections.Generic;
using System.Text;

namespace TMS.Data.Entities
{
    public class Tax : BaseEntity<int>
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
        public int TaxTypeID { get; set; }
        public virtual TaxType TaxType { get; set; }
        public virtual PaymentMode PaymentMode { get; set; }
        public virtual ICollection<DenominationTax> DenominationTax { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using TMS.Infrastructure;

namespace TMS.Data.Entities
{
    public class Denomination : BaseEntity<int>
    {
        public string Name { get; set; }
        public decimal Value { get; set; }
        public int ServiceID { get; set; }
        public virtual Service Service { get; set; }
        public string OldDenominationID { get; set; }
        public int? PaymentModeID { get; set; }
        public virtual PaymentMode PaymentMode { get; set; }
        public bool Status { get; set; }
        public int? CurrencyID { get; set; }
        public virtual Currency Currency { get; set; }
        public decimal APIValue { get; set; }
        public decimal MinValue { get; set; }
        public decimal MaxValue { get; set; }
        public int Interval { get; set; }
        public int? ServiceCategoryID { get; set; }
        public virtual ServiceCategory ServiceCategory { get; set; }
        public bool Inquirable { get; set; }
        public int? BillPaymentModeID { get; set; }
        public virtual BillPaymentMode BillPaymentMode { get; set; }
        public virtual ICollection<DenominationServiceProvider> DenominationServiceProviders { get; set; }
        public string PathClass { get; set; }
        public DenominationClassType ClassType { get; set; }
        public virtual ICollection<DenominationFee> DenominationFees { get; set; }
        public virtual ICollection<AccountFee> AccountFees { get; set; }
        public virtual ICollection<DenominationEntity> DenominationEntities { get; set; }
        public virtual ICollection<DenominationCommission> DenominationCommissions { get; set; }
        public virtual ICollection<DenominationParameter> DenominationParameters { get; set; }
        public virtual DenominationReceiptData DenominationReceiptData { get; set; }
        public virtual ICollection<AccountCommission> AccountCommissions { get; set; }
        public virtual ICollection<AccountTypeProfileDenomination> AccountTypeProfileDenominations { get; set; }
        public virtual ICollection<Request> Requests { get; set; }
        public virtual ICollection<DenominationReceiptParam> DenominationReceiptParams { get; set; }
    }
}

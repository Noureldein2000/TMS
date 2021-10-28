using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMS.Infrastructure;

namespace TMS.API.Models
{
    public class DenominationModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Value { get; set; }
        public int ServiceID { get; set; }
        public string OldDenominationID { get; set; }
        public bool Status { get; set; }
        public int CurrencyID { get; set; }
        public decimal APIValue { get; set; }
        public decimal MinValue { get; set; }
        public decimal MaxValue { get; set; }
        public int Interval { get; set; }
        public int ServiceCategoryID { get; set; }
        public DenominationClassType ClassType { get; set; }
        public int ServiceProviderId { get; set; }
        public string ServiceEntity { get; set; }
        public string PathClass { get; set; }
        public bool Inquirable { get; set; }
        public int? BillPaymentModeID { get; set; }
        public string BillPaymentModeName { get; set; }
        public int? PaymentModeID { get; set; }
        public string PaymentModeName { get; set; }
    }
}

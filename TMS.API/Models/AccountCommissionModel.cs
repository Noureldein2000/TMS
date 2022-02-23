using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TMS.API.Models
{
    public class AccountCommissionModel
    {
        public int Id { get; set; }
        public int CommissionId { get; set; }
        public int AccountId { get; set; }
        public decimal AmountFrom { get; set; }
        public decimal AmountTo { get; set; }
        public decimal CommissionValue { get; set; }
        public int PaymentModeId { get; set; }
        public string PaymentMode { get; set; }
        public int CommissionTypeId { get; set; }
        public string CommissionTypeName { get; set; }
        public int DenomiinationId { get; set; }
        public string DenominationFullName { get; set; }
        public int ServiceId { get; set; }
        public string ServiceName { get; set; }
        public DateTime CreationDate { get; set; }
    }
}

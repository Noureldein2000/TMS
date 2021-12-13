using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TMS.API.Models
{
    public class AccountTypeProfileFeesModel
    {
        public int Id { get; set; }
        public int FeesID { get; set; }
        public int AccountTypeProfileDenominationID { get; set; }
        public string ServiceName { get; set; }
        public string DenomintionName { get; set; }
        public string AccountTypeName { get; set; }
        public string ProfileName { get; set; }
        public decimal FeesValue { get; set; }
        public decimal AmountFrom { get; set; }
        public decimal AmountTo { get; set; }
        public string PaymentModeName { get; set; }
        public string FeesTypeName { get; set; }
    }
}

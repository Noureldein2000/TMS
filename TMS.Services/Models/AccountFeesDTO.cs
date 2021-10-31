using System;
using System.Collections.Generic;
using System.Text;

namespace TMS.Services.Models
{
    public class AccountFeesDTO
    {
        public int Id { get; set; }
        public int FeesId { get; set; }
        public int AccountId { get; set; }
        public decimal FeesValue { get; set; }
        public int PaymentModeId { get; set; }
        public string PaymentMode { get; set; }
        public int FeesTypeId { get; set; }
        public string FeesTypeName { get; set; }
        public int DenomiinationId { get; set; }
        public string DenominationFullName { get; set; }
        public int ServiceId { get; set; }
        public string ServiceName { get; set; }
        public DateTime CreationDate { get; set; }
    }
}

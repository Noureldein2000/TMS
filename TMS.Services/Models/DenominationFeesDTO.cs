using System;
using System.Collections.Generic;
using System.Text;

namespace TMS.Services.Models
{
    public class DenominationFeesDTO
    {
        public int Id { get; set; }
        public int FeesId { get; set; }
        public decimal FeesValue { get; set; }
        public int PaymentModeId { get; set; }
        public string PaymentMode { get; set; }
        public int FeesTypeId { get; set; }
        public string FeesTypeName { get; set; }
        public int DenominationId { get; set; }
        public decimal AmountFrom { get; set; }
        public decimal AmountTo { get; set; }
        public DateTime CreationDate { get; set; }
    }

}

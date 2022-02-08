using System;
using System.Collections.Generic;
using System.Text;

namespace TMS.Services.Models
{
    public class DenominationCommissionDTO
    {
        public int Id { get; set; }
        public int CommissionId { get; set; }
        public decimal CommissionValue { get; set; }
        public int PaymentModeId { get; set; }
        public string PaymentMode { get; set; }
        public int CommissionTypeId { get; set; }
        public string CommissionTypeName { get; set; }
        public int DenominationId { get; set; }
        public string Range { get; set; }
        public DateTime CreationDate { get; set; }
    }

}

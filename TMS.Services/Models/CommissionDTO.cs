using System;
using System.Collections.Generic;
using System.Text;

namespace TMS.Services.Models
{
    public class CommissionDTO
    {
        public int ID { get; set; }
        public int CommissionTypeID { get; set; }
        public string CommissionTypeName { get; set; }
        public string CommissionRange { get; set; }
        public decimal Commission { get; set; }
        public decimal AmountFrom { get; set; }
        public decimal AmountTo { get; set; }
        public int PaymentModeID { get; set; }
        public decimal Value { get; set; }
        public bool Status { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace TMS.Services.Models
{
    public class FeesDTO
    {
        public int ID { get; set; }
        public int FeesTypeID { get; set; }
        public string FeesTypeName { get; set; }
        public string FeeRange { get; set; }
        public decimal Fees { get; set; }
        public decimal AmountFrom { get; set; }
        public decimal AmountTo { get; set; }
        public int PaymentModeID { get; set; }
        public int CreatedBy { get; set; }
        public string PaymentModeName { get; set; }
        public decimal Value { get; set; }
        public bool Status { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TMS.API.Models
{
    public class TaxModel
    {
        public int ID { get; set; }
        public int TaxesTypeID { get; set; }
        public string TaxesTypeName { get; set; }
        public string TaxRange { get; set; }
        public decimal Taxes { get; set; }
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

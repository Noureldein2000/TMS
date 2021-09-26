using System;
using System.Collections.Generic;
using System.Text;

namespace TMS.Services.Models
{
    public class InquiryBillDetailDTO
    {
        public int InquiryBillID { get; set; }
        public string ProviderName { get; set; }
        public string ParameterName { get; set; }
        public string Value { get; set; }
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public int ParameterId { get; set; }

    }
}

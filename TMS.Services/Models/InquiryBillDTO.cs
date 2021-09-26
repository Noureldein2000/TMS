using System;
using System.Collections.Generic;
using System.Text;

namespace TMS.Services.Models
{
    public class InquiryBillDTO
    {
        public int ProviderServiceResponseID { get; set; }
        public int Sequence { get; set; }
        public decimal Amount { get; set; }
        public int Id { get; set; }
    }

    public class InquiryBillDetailDTO : InquiryBillDTO
    {
        public int InquiryBillID { get; set; }
        public string ParameterName { get; set; }
        public int ParameterID { get; set; }
        public string Value { get; set; }
    }
}

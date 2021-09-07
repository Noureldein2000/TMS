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
}

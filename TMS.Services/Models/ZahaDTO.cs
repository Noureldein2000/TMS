using System;
using System.Collections.Generic;
using System.Text;

namespace TMS.Services.Models
{
    class ZahaDTO
    {
    }
    public class InquiryZaha
    {
        public string BillingAccount { get; set; }
        public int RequestID { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public InquiryZaha()
        {
            Data = new List<SwitchData>();
        }

        public List<SwitchData> Data { get; set; }
    }

    public class PaymentZaha : InquiryZaha
    {
        public decimal Amount { get; set; }
    }
}

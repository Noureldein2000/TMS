using System;
using System.Collections.Generic;
using System.Text;

namespace TMS.Services.Models
{
    class WEInternetDTO
    {
    }
    public class InquiryWeDTO
    {
        public string TelephoneNumber { get; set; }
        public int TransactionId { get; set; }
    }

    public class PaymentWeDTO
    {
        public string TelephoneNumber { get; set; }
        public int TransactionId { get; set; }
        public string InvoiceAmount { get; set; }
        public bool ExtraQuota { get; set; }
    }
}

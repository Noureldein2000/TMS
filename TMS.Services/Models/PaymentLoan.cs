using System;
using System.Collections.Generic;
using System.Text;

namespace TMS.Services.Models
{
    public class PaymentLoan
    {
        public string BillingAccount { get; set; }
        public decimal Amount { get; set; }
        public string RequestID { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string AccountNumber { get; set; }
    }
}

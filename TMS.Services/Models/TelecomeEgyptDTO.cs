using System;
using System.Collections.Generic;
using System.Text;

namespace TMS.Services.Models
{
    public class TelecomeEgyptDTO
    {
    }

    public class PaymentTelecomeEgypt
    {
        public string RequestID { get; set; }
        public string TelephoneCode { get; set; }
        public string Phone { get; set; }
        public string Amount { get; set; }
        public string Fees { get; set; }
        public string Password { get; set; }
        public string UserName { get; set; }

    }
}

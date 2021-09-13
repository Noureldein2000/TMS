using System;
using System.Collections.Generic;
using System.Text;

namespace TMS.Services.Models
{
    public class CashUTopUpDTO
    {

    }

    public class InquiryCashUTopUP
    {
        public string AccountId { get; set; }
        public string TransactionId { get; set; }
        public string Amount { get; set; }
    }
}

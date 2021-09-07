using System;
using System.Collections.Generic;
using System.Text;

namespace TMS.Services.Models
{
    public class SwitchRequestBodyDTO
    {
        public int TransactionId { get; set; }
        public string UserId { get; private set; } = "momkn";
        public string Password { get; private set; } = "hZ3BGBayXUCXsTnr";
        public int BillsCount { get; set; }
        public string PaymentCode { get; set; }
    }

    public class SwitchPaymentRequestBodyDTO : SwitchRequestBodyDTO
    {
        public string BillRefNumber { get; set; }
        public decimal Amount { get; set; }
        public string Fees { get; set; }
        public string AsyncRqUID { get; set; }
        public string ExtraBillInfo { get; set; }
    }
}

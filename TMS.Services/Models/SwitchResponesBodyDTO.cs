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
    }


    public class InquiryResponseCashIn : SwitchRequestBodyDTO
    {
        public string BillReference { get; set; }
    }

   public class PaymentCashIn : InquiryResponseCashIn
    {
        public decimal Amount { get; set; }
        public string InquiryReference { get; set; }
    }

    public class CheckPaymentStatusCashIn : SwitchRequestBodyDTO
    {
        public string PaymentTransactionId { get; set; }
    }

    public class PaymentCashU : SwitchRequestBodyDTO
    {
        public string CenterId { get; set; }
        public int DenominationId { get; set; }
        public int Quantity { get; set; }
    }
}

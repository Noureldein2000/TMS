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

    public class SwitchPaymentRequestBodyDTO : SwitchRequestBodyDTO
    {
        public string BillRefNumber { get; set; }
        public decimal Amount { get; set; }
        public string Fees { get; set; }
        public string AsyncRqUID { get; set; }
        public string ExtraBillInfo { get; set; }
        public string PaymentCode { get; set; }
        public int BillsCount { get; set; }
    }
    public class SwitchPaymentRequestEducationBodyDTO : SwitchRequestBodyDTO
    {
        public string BillRefNumber { get; set; }
        public decimal Amount { get; set; }
        public string Fees { get; set; }
        public string AsyncRqUID { get; set; }
        public string ExtraBillInfo { get; set; }
        public string Ssn { get; set; }
        public string SfId { get; set; }
        public string ServiceId { get; set; }
    }
    //public class InquiryBTech : SwitchRequestBodyDTO
    //{
    //    public string PaymentCode { get; set; }
    //    public int BillsCount { get; set; }
    //}

    public class InquiryCashIn : SwitchRequestBodyDTO
    {
        public string BillReference { get; set; }
    }

    class PaymentCashIn : InquiryCashIn
    {
        public decimal Amount { get; set; }
        public string InquiryReference { get; set; }
    }

    public class InquiryEducationServiceDTO //: SwitchRequestBodyDTO
    {
        public string UserId { get; private set; } = "momkn";
        public string Password { get; private set; } = "hZ3BGBayXUCXsTnr";
        public int TransactionId { get; set; }
        public string Ssn { get; set; }
        public string SfId { get; set; }
        public string ServiceId { get; set; }

        //public string userId = "momkn";
        //public string password = "hZ3BGBayXUCXsTnr";
        //public string transactionId { get; set; }
        //public string ssn { get; set; }
        //public string sfId { get; set; }
        //public string serviceId { get; set; }
    }
}

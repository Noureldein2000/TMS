using System;
using System.Collections.Generic;
using System.Text;

namespace TMS.Services.Models
{
    public class SwitchResponseBodyDTO
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

    public class CashUCoupon
    {
        public string CardNumber { get; set; }
        private string creationDate;
        public string CreationDate
        {
            get { return creationDate; }
            set { creationDate = value; }
        }
        public string DenominationValue { get; set; }
        private string expirationDate;
        public string ExpirationDate
        {
            get { return expirationDate; }
            set { expirationDate = value; }
        }
        public string Serial { get; set; }
    }
    public class CashUCoupon2
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
        public string CardNumber { get; set; }
        private string creationDate;
        public string CreationDate
        {
            get { return creationDate; }
            set { creationDate = value; }
        }
        public string DenominationValue { get; set; }
        private string expirationDate;
        public string ExpirationDate
        {
            get { return expirationDate; }
            set { expirationDate = value; }
        }
        public string Serial { get; set; }

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



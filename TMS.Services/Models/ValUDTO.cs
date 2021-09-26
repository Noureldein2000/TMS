using System;
using System.Collections.Generic;
using System.Text;

namespace TMS.Services.Models
{
    public class ValUDTO
    {

    }

    class InquiryValU
    {
        public string MobileNumber = "";
        public string PassportNnumber = "";
        public string CustomerCode = "";
        public string NationalId = "";
        public string TransactionId { get; set; }

    }
    class PaymentValU : InquiryValU
    {
        public decimal Amount { get; set; }
    }

    public class Purchase
    {
        public string AmountPayable;
        public string PaymentDueDate;
        public string PurchaseCode;
        public string PurchaseId;

    }

}

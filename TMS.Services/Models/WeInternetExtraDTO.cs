using System;
using System.Collections.Generic;
using System.Text;

namespace TMS.Services.Models
{
    public class WeInternetExtraDTO
    {

    }

    public class InquiryWeInternetExtraDTO
    {
        public string UserId { get; private set; } = "momkn";
        public string Password { get; private set; } = "hZ3BGBayXUCXsTnr";
        public int TransactionId { get; set; }
        public string TelephoneNumber { get; set; }
    }

    public class ProductOfferDTO
    {
        public string Name { get; set; }
        public string ChargeAmount { get; set; }
        public string ChargeAmountWithTaxes { get; set; }

    }
    public class PaymentQuotaDTO
    {
        public string UserId { get; private set; } = "momkn";
        public string Password { get; private set; } = "hZ3BGBayXUCXsTnr";
        public string InvoiceAmount { get; set; }
        public bool ExtraQuota { get; set; }
        public int TransactionId { get; set; }
        public string TelephoneNumber { get; set; }
    }
    public class CheckTransactionDTO
    {
        public string PaymentTransactionId { get; set; }
        public string TransactionId { get; set; }
    }
}

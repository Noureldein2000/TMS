using System;
using System.Collections.Generic;
using System.Text;

namespace TMS.Services.Models
{
    public class OneCardDTO
    {
        public string ProductCode { get; set; }
    }

    class PaymentOneCard : OneCardDTO
    {
        public string TerminalId = "123456";
        public string TransactionId { get; set; }
    }

    public class Product
    {
        public string Available { get; set; }
        public string MerchantNameAr { get; set; }
        public string MerchantNameEn { get; set; }
        public string NameAr { get; set; }
        public string NameEn { get; set; }
        public string PosCurrency { get; set; }
        public string PosPrice { get; set; }
        public string ProductCode { get; set; }
        public string ProductCurrency { get; set; }
        public string ProductPrice { get; set; }
        public string AddedMoney { get; set; }
        public string TotalWithAddedMoney { get; set; }
    }
}

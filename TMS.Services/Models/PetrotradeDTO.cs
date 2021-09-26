using System;
using System.Collections.Generic;
using System.Text;

namespace TMS.Services.Models
{
    class PetrotradeDTO
    {
        public string UserId = "momkn";
        public string Password = "hZ3BGBayXUCXsTnr";
        public string TransactionId { get; set; }
        public string MobileNo { get; set; }
        public string RegisterNo { get; set; }
    }

    class PaymentPetrotrade : PetrotradeDTO
    {
        public string BillRefNumber { get; set; }
        public decimal Amount { get; set; }
        public string Fees { get; set; }
        public string AsyncRqUID { get; set; }
        public string ExtraBillInfo { get; set; }

    }
    public class BillInfo
    {
        public string item;
        public string value;
    }
}

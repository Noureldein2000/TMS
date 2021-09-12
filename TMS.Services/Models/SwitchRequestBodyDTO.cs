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
}

   

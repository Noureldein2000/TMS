using System;
using System.Collections.Generic;
using System.Text;

namespace TMS.Data.Entities
{
    public class PendingPaymentCard : BaseEntity<int>
    {
        public string PaymentRefInfo { get; set; }
        public int TransactionID { get; set; }
        public int CardTypeID { get; set; }
        public string HostTransactionID { get; set; }
        public int PengingPaymentCardStatusID { get; set; }
        public int? Brn { get; set; }
        public virtual Transaction Transaction { get; set; }
        public virtual CardType CardType { get; set; }
        public virtual PendingPaymentCardStatus PendingPaymentCardStatus { get; set; }
    }
}

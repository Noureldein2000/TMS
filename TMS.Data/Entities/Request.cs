using System;
using System.Collections.Generic;
using System.Text;

namespace TMS.Data.Entities
{
    public class Request : BaseEntity<int>
    {
        public string UUID { get; set; }
        public int  AccountID { get; set; }
        public int ServiceDenominationID { get; set; }
        public virtual Denomination Denomination { get; set; }
        public int? StatusID { get; set; }
        public virtual RequestStatus RequestStatus { get; set; }
        public int? TransactionID { get; set; }
        public int UserID { get; set; }
        public string RRN { get; set; }
        public DateTime ResponseDate { get; set; }
        public decimal Amount { get; set; }
        public string BillingAccount { get; set; }
        public string ChannelID { get; set; }
        public int? ProviderServiceRequestID { get; set; }
    }
}

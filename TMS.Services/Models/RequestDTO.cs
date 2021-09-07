using System;
using System.Collections.Generic;
using System.Text;

namespace TMS.Services.Models
{
    public class RequestDTO
    {
        public string HostTransactionId { get; set; }
        public int AccountId { get; set; }
        public int DenominationId { get; set; }
        public decimal Amount { get; set; }
        public string BillingAccount { get; set; }
        public string ChannelID { get; set; }
    }
}

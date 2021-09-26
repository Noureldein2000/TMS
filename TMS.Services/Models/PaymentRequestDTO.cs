using System;
using System.Collections.Generic;
using System.Text;

namespace TMS.Services.Models
{
    public class PaymentRequestDTO
    {
        public string Version { get; set; }
        public string ServiceListVersion { get; set; }
        public int Brn { get; set; }
        public string BillingAccount { get; set; }
        public decimal Amount { get; set; }
        public string HostTransactionID { get; set; } = "0";
        public DateTime LocalDate { get; set; }
        public List<DataDTO> Data { get; set; } = new List<DataDTO>();
        public int AccountId { get; set; }
        public int AccountProfileId { get; set; }
        public string MomknPaymentId { get; set; }
        public string ChannelId { get; set; }
        public string ChannelIdentifier { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace TMS.Services.Models
{
    public class CancelDTO
    {
        public string BillingAcccount { get; set; }
        public string CardType { get; set; }
        public string PaymentRefInfo { get; set; }
        public string BillNumber { get; set; }
        public string MomknPaymentId { get; set; }
        public string BillerId { get; set; }
        public string BillRecId { get; set; }
        public List<FeesAmounts> FeesAmounts { get; set; }
        public List<PaymentCardAmounts> PaymentAmounts { get; set; }
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
        public string ChannelId { get; set; }
        public string ChannelIdentifier { get; set; }
        public int TransactionId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    public class CancelModel
    {
        public string BillingAcccount { get; set; }
        public string TransactionId { get; set; }
        public string CardType { get; set; }
        public string PaymentRefInfo { get; set; }
        public string BillNumber { get; set; }
        public string MomknPaymentId { get; set; }
        public string BillerId { get; set; }
        public string BillRecId { get; set; }
        public List<FeesAmounts> FeesAmounts { get; set; }
        public List<PaymentCardAmounts> PaymentAmounts { get; set; }
    }

    class CancelITunes
    {
        public string ProductCode { get; set; }
        public string TransactionId { get; set; }
        public string ReferenceTransactionId { get; set; }
    }

    public class CancellProviderDTO
    {
        public int ServiceID { get; set; }
        public int DenomationId { get; set; }
        public int ProviderServiceRequestId { get; set; }
        public string BillingAccount { get; set; }
        public string ProviderCode { get; set; }
        public int Brn { get; set; }
        public string MomknPaymentId { get; set; }
        public string OldDenominationID { get; set; }
        public List<FeesAmounts> FeesAmounts { get; set; }
        public List<PaymentCardAmounts> PayCardAmounts { get; set; }
    }
}

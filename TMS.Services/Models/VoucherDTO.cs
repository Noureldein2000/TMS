using System;
using System.Collections.Generic;
using System.Text;

namespace TMS.Services.Models
{
    class BulkVoucher
    {
        public string TransactionId { get; set; }
        public string ServiceId { get; set; }
        public string Amount { get; set; }
        public string Count { get; set; }
    }
    class VoucherDTO
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string ServiceID { get; set; }
        public string RequestID { get; set; }
        public string Amount { get; set; }
        public string ProviderID { get; set; }
        public string OperatorID { get; set; }
        public string TerminalID { get; set; }

    }

    public class VoucherData
    {
        public string pin { get; set; }
        public string serial { get; set; }
        public string transactionId { get; set; }

    }
}

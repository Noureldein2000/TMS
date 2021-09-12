using System;
using System.Collections.Generic;
using System.Text;

namespace TMS.Data.Entities
{
    public class Transaction : BaseEntity<int>
    {
        public int? AccountIDFrom { get; set; }
        public int? AccountIDTo { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal OriginalAmount { get; set; }
        public decimal Fees { get; set; }
        public int TransactionType { get; set; }
        public bool IsReversed { get; set; }
        public string OriginalTrx { get; set; }
        public int? InvoiceID { get; set; }
        public int? RequestID { get; set; }
        public virtual Request Request { get; set; }
        public string TransactionID { get; set; }
        public virtual ICollection<AccountTransactionCommission> AccountTransactionCommissions { get; set; }
        public virtual ICollection<TransactionReceipt> TransactionReceipts { get; set; }
        public virtual ICollection<ReceiptBodyParam> ReceiptBodyParams { get; set; }
    }
}

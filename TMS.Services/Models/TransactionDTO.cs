using System;
using System.Collections.Generic;
using System.Text;

namespace TMS.Services.Models
{
    public class TransactionDTO
    {
        public int Id { get; set; }
        public int? InvoiceId { get; set; }
        public decimal TotalAmount { get; set; }
        public int? RequestId{ get; set; }
    }
}

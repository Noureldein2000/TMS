using System;
using System.Collections.Generic;
using System.Text;

namespace TMS.Data.Entities
{
    public class AccountTransactionCommission : BaseEntity<int>
    {
        public int TransactionID { get; set; }
        public virtual Transaction Transaction { get; set; }
        public decimal Commission { get; set; }
    }
}

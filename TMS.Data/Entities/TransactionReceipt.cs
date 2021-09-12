using System;
using System.Collections.Generic;
using System.Text;

namespace TMS.Data.Entities
{
    public class TransactionReceipt : BaseEntity<int>
    {
        public int TransactionID { get; set; }
        public virtual Transaction Transaction { get; set; }
        public string Receipt { get; set; }
    }
}

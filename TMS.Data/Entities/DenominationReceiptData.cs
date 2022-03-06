using System;
using System.Collections.Generic;
using System.Text;

namespace TMS.Data.Entities
{
    public class DenominationReceiptData : BaseEntity<int>
    {
        public int DenominationID { get; set; }
        public virtual Denomination Denomination { get; set; }
        public string Title { get; set; }
        public string Disclaimer { get; set; }
        public string Footer { get; set; }
        public virtual ICollection<DenominationReceiptParam> DenominationReceiptParams { get; set; }
    }
}

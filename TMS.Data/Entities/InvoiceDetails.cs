using System;
using System.Collections.Generic;
using System.Text;

namespace TMS.Data.Entities
{
    public class InvoiceDetails : BaseEntity<int>
    {
        public int InvoiceId { get; set; }
        public virtual Invoice Invoice { get; set; }
        public int ServiceFieldId { get; set; }
        public virtual ServicesField ServicesField { get; set; }
        public string Value { get; set; }
    }
}

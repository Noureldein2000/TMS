using System;
using System.Collections.Generic;
using System.Text;

namespace TMS.Data.Entities
{
    public class ServicesField : BaseEntity<int>
    {
        public int ServId { get; set; }
        public string FieldName { get; set; }
        public int FieldTypeID { get; set; }
        public virtual FieldType FieldType { get; set; }
        public bool Req { get; set; }
        public int fRank { get; set; }
        public bool Printed { get; set; }
        public bool Display { get; set; }
        public int printed_rank { get; set; }
        public string EnglishFieldName { get; set; }
        //public virtual ICollection<InvoiceDetails> InvoiceDetails { get; set; }
    }
}

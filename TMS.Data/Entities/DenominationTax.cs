using System;
using System.Collections.Generic;
using System.Text;

namespace TMS.Data.Entities
{
    public class DenominationTax : BaseEntity<int>
    {
        public int DenominationID { get; set; }
        public virtual Denomination Denomination { get; set; }
        public int TaxID { get; set; }
        public virtual Tax Tax { get; set; }

    }
}

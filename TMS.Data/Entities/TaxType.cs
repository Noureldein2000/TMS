using System;
using System.Collections.Generic;
using System.Text;

namespace TMS.Data.Entities
{
    public class TaxType : BaseEntity<int>, ILookupType
    {
        public string Name { get; set; }
        public string ArName { get; set; }
        public virtual ICollection<Tax> Taxes { get; set; }
    }
}

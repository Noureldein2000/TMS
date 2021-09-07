using System;
using System.Collections.Generic;
using System.Text;

namespace TMS.Data.Entities
{
    public class Currency : BaseEntity<int>
    {
        public string Name { get; set; }
        public string ArName { get; set; }
        public decimal Value { get; set; }
        public virtual ICollection<Denomination> Denominations { get; set; }
    }
}

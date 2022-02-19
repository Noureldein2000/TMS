using System;
using System.Collections.Generic;
using System.Text;

namespace TMS.Data.Entities
{
    public class FeesType : BaseEntity<int>, ILookupType
    {
        public string Name { get; set; }
        public string ArName { get; set; }
        public virtual ICollection<Fee> Fees { get; set; }
    }
}

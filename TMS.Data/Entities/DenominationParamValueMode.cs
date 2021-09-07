using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TMS.Data.Entities
{
    public class DenominationParamValueMode : BaseEntity<int>
    {
        [MaxLength(50)]
        public string Name { get; set; }
        public virtual ICollection<DenominationParam> DenominationParams { get; set; }
    }
}

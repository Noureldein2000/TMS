using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TMS.Data.Entities
{
    public class FieldType : BaseEntity<int>
    {
        [MaxLength(250)]
        public string Name { get; set; }
        [MaxLength(250)]
        public string ValExp { get; set; }
        [MaxLength(250)]
        public string message { get; set; }
        public virtual ICollection<ServicesField> ServicesFields { get; set; }
    }
}

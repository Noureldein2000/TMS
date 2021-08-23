using System;
using System.Collections.Generic;
using System.Text;

namespace TMS.Data.Entities
{
    public class BaseEntity<T>
    {
        public T ID { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? UpdateDate { get; set; }
    }
}

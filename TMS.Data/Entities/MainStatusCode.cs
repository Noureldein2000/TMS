using System;
using System.Collections.Generic;
using System.Text;

namespace TMS.Data.Entities
{
    public class MainStatusCode : BaseEntity<int>
    {
        public string Code { get; set; }
        public string Message { get; set; }
        public string ArMessage { get; set; }
        public virtual ICollection<StatusCode> StatusCodes { get; set; }
    }
}

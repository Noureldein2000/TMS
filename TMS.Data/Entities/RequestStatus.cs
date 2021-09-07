using System;
using System.Collections.Generic;
using System.Text;

namespace TMS.Data.Entities
{
    public class RequestStatus : BaseEntity<int>
    {
        public string Description { get; set; }
        public string ResponseCode { get; set; }
        public virtual ICollection<StatusCode> StatusCodes { get; set; }
        public virtual ICollection<Request> Requests { get; set; }
    }
}

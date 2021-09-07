using System;
using System.Collections.Generic;
using System.Text;

namespace TMS.Data.Entities
{
    public class StatusCode : BaseEntity<int>
    {
        public string Code { get; set; }
        public string Message { get; set; }
        public string ArMessage { get; set; }
        public int RequestStatusID { get; set; }
        public virtual RequestStatus RequestStatus { get; set; }
        public int MainStatusCodeID { get; set; }
        public virtual MainStatusCode MainStatusCode { get; set; }
        public virtual ICollection<ProviderStatusCode> ProviderStatusCodes { get; set; }
    }
}

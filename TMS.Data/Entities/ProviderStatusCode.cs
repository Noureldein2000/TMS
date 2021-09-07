using System;
using System.Collections.Generic;
using System.Text;

namespace TMS.Data.Entities
{
    public class ProviderStatusCode : BaseEntity<int>
    {
        public int ServiceProviderID { get; set; }
        public virtual ServiceProvider ServiceProvider { get; set; }
        public string StatusCode { get; set; }
        public string ProviderMessage { get; set; }
        public int StatusCodeID { get; set; }
        public virtual StatusCode StatusCodeModel { get; set; }
    }
}

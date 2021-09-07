using System;
using System.Collections.Generic;
using System.Text;
using TMS.Infrastructure;

namespace TMS.Data.Entities
{
    public class Log : BaseEntity<int>
    {
        public string LogText { get; set; }
        public int ProviderServiceRequestID { get; set; }
        public LoggingType LogTypeID { get; set; }
    }
}

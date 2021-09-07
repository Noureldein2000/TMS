using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TMS.API.Models
{
    public class FeesRequestModel
    {
        public decimal Amount { get; set; }
        public string Version { get; set; }
        public string ServiceListVersion { get; set; }
        public List<DataModel> Data { get; set; } = new List<DataModel>();
        public int Brn { get; set; }
        public int AccountId { get; set; }
        public int AccountProfileId { get; set; }
    }
}

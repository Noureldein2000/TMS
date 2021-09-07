using System;
using System.Collections.Generic;
using System.Text;

namespace TMS.Services.Models
{
    public class FeesRequestDTO
    {
        public decimal Amount { get; set; }
        public string Version { get; set; }
        public string ServiceListVersion { get; set; }
        public List<DataDTO> Data { get; set; } = new List<DataDTO>();
        public int Brn { get; set; }
        public int AccountId { get; set; }
        public int AccountProfileId { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace TMS.Services.Models
{
    public class FeesResponseDTO
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public List<DataDTO> Data { get; set; } = new List<DataDTO>();
        public decimal Amount { get; set; }
        public decimal Fees { get; set; }
        public decimal Taxes { get; set; }
        public decimal TotalAmount { get; set; }
        public int Brn { get; set; }
    }
}

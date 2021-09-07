using System;
using System.Collections.Generic;
using System.Text;

namespace TMS.Services.Models
{
    public class InvoiceDTO
    {
        public InvoiceDTO()
        {
            Data = new List<DataDTO>();
        }
        public List<DataDTO> Data { get; set; }
        public decimal Amount { get; set; }
        public int Sequence { get; set; }
    }
}

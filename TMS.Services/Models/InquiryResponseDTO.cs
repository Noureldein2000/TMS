using System;
using System.Collections.Generic;
using System.Text;

namespace TMS.Services.Models
{
    public class InquiryResponseDTO
    {
        public InquiryResponseDTO()
        {
            Data = new List<DataDTO>();
            Invoices = new List<InvoiceDTO>();
        }
        public string Code { get; set; }
        public string Message { get; set; }
        public decimal TotalAmount { get; set; }
        public int Brn { get; set; }
        public List<DataDTO> Data { get; set; }
        public List<InvoiceDTO> Invoices { get; set; }

    }
}

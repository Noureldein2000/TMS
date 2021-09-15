using System;
using System.Collections.Generic;
using System.Text;

namespace TMS.Services.Models
{
    public class PaymentResponseDTO
    {
        public string Code { get; set; }
        public string Message { get; set; }
        public decimal AvailableBalance { get; set; }
        public int TransactionId { get; set; }
        public int InvoiceId { get; set; }
        public string LocalDate { get; set; }
        public string ServerDate { get; set; }
        //public List<Root> Recept { get; set; }
        public List<DataListDTO> DataList { get; set; } = new List<DataListDTO>();
        public List<Root> Receipt { get; set; } = new List<Root>();
    }
}

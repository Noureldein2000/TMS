using System;
using System.Collections.Generic;
using System.Text;

namespace TMS.Services.Models
{
    public class DenominationReceiptDataDTO
    {
        public int Id { get; set; }
        public int DenominationID { get; set; }
        public string Title { get; set; }
        public string Disclaimer { get; set; }
        public string Footer { get; set; }
    }
}

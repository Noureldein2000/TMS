using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TMS.API.Models
{
    public class DenominationReceiptDataModel
    {
        public int Id { get; set; }
        public int DenominationID { get; set; }
        public string Title { get; set; }
        public string Disclaimer { get; set; }
        public string Footer { get; set; }
    }
}

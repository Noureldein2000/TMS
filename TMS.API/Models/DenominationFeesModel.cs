using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TMS.API.Models
{
    public class DenominationFeesModel
    {
        public int Id { get; set; }
        public int FeesId { get; set; }
        public decimal FeesValue { get; set; }
        public int PaymentModeId { get; set; }
        public string PaymentMode { get; set; }
        public int FeesTypeId { get; set; }
        public string FeesTypeName { get; set; }
        public int DenominationId { get; set; }
        public string DenominationFullName { get; set; }
        public string Range { get; set; }
        public DateTime CreationDate { get; set; }
    }
   
}

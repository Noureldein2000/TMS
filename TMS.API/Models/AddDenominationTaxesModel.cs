using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TMS.API.Models
{
    public class AddDenominationTaxesModel
    {
        public int Id { get; set; }
        public int TaxId { get; set; }
        public int DenominationId { get; set; }
    }
}

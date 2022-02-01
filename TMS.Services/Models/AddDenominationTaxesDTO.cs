using System;
using System.Collections.Generic;
using System.Text;

namespace TMS.Services.Models
{
    public class AddDenominationTaxesDTO
    {
        public int Id { get; set; }
        public int TaxId { get; set; }
        public int DenominationId { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace TMS.Services.Models
{
    public class AddDenominationCommissionDTO
    {
        public int Id { get; set; }
        public int CommissionId { get; set; }
        public int DenominationId { get; set; }
    }
}

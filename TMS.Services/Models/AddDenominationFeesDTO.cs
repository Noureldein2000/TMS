using System;
using System.Collections.Generic;
using System.Text;

namespace TMS.Services.Models
{
    public class AddDenominationFeesDTO
    {
        public int Id { get; set; }
        public int FeesId { get; set; }
        public int DenominationId { get; set; }
    }
}

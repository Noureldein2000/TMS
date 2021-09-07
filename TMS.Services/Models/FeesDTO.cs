using System;
using System.Collections.Generic;
using System.Text;

namespace TMS.Services.Models
{
    public class FeesDTO
    {
        public int FeesTypeID { get; set; }
        public string FeesTypeName { get; set; }
        public decimal Fees { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace TMS.Services.Models
{
   public class ProviderResponseDTO
    {
        public int StatusCode { get; set; }
        public int Code { get; set; }
        public string Message { get; set; }
    }
}

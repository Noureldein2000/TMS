using System;
using System.Collections.Generic;
using System.Text;

namespace TMS.Services.Models
{
    public class StatusCodeDTO
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public string Code { get; set; }
        public int MainStatusId { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace TMS.Services.Models
{
  public  class TopUpDTO
    {
        public string RequestID { get; set; }
        public string RequestDate { get; set; }
        public string Phone { get; set; }
        public string Amount { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string ProviderID { get; set; }
        public string TerminalID { get; set; }
        public string ServiceID { get; set; }
    }
}

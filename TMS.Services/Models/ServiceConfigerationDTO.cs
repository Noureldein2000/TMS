using System;
using System.Collections.Generic;
using System.Text;

namespace TMS.Services.Models
{
    public class ServiceConfigerationDTO
    {
        public int Id { get; set; }
        public string URL { get; set; }
        public int TimeOut { get; set; }
        public string UserName { get; set; }
        public string UserPassword { get; set; }
        public List<ServiceConfigParmsDTO> ServiceConfigParms { get; set; }
    }
}

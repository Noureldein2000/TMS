using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TMS.API.Models
{
    public class ServiceConfigerationModel
    {
        public int Id { get; set; }
        public string URL { get; set; }
        public int TimeOut { get; set; }
        public string UserName { get; set; }
        public string UserPassword { get; set; }
        public List<ServiceConfigParmsModel> ServiceConfigParmsModel { get; set; }
    }
}

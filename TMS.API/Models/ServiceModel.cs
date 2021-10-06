using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TMS.API.Models
{
    public class ServiceModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ArName { get; set; }
        public int ServiceTypeID { get; set; }
        public string Code { get; set; }
        public int ServiceEntityID { get; set; }
        public string PathClass { get; set; }
    }
}

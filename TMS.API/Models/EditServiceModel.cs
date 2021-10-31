using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMS.Infrastructure;

namespace TMS.API.Models
{
    public class EditServiceModel
    {
        public int Id{ get; set; }
        public string Name { get; set; }
        public string ArName { get; set; }
        public bool Status { get; set; }
        public int ServiceTypeID { get; set; }
        public string Code { get; set; }
        public int ServiceEntityID { get; set; }
        public int? ServiceCategoryID { get; set; }
        public string PathClass { get; set; }
        public ServiceClassType ClassType { get; set; }
    }
}

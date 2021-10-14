using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TMS.API.Models
{
    public class DenominationModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Value { get; set; }
        public int ServiceID { get; set; }
        public string OldDenominationID { get; set; }
        public bool Status { get; set; }
        public int ServiceCategoryID { get; set; }
        public int ServiceProviderId { get; set; }
        public string ServiceEntity { get; set; }
        public int? PaymentModeID { get; set; }
    }
}

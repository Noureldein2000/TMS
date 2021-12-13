using System;
using System.Collections.Generic;
using System.Text;

namespace TMS.Data.Entities
{
    public class Restaurant : BaseEntity<int>
    {
        public string RestaurantCode { get; set; }
        public string RestaurantName { get; set; }
        public string RestaurantNameAr { get; set; }
    }
}

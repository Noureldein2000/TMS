using System;
using System.Collections.Generic;
using System.Text;

namespace TMS.Services.Models
{
    public class RestaurantDTO
    {
        public int ID{ get; set; }
        public DateTime CreationDate{ get; set; }
        public string RestaurantCode { get; set; }
        public string RestaurantName { get; set; }
    }
}

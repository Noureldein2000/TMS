using System;
using System.Collections.Generic;
using System.Text;

namespace TMS.Services.Models
{
    public class ServiceCategoryDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ArName { get; set; }
        public bool LastNode { get; set; }
        public int ServiceIndex { get; set; }
        public int ServiceLevel { get; set; }
        public string ServiceSubCategory { get; set; }
        public string Title { get; set; }
        public int? ParentID { get; set; }
        public DateTime CreationDate { get; set; }
    }
}

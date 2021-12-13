using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TMS.API.Models
{
    public class AccountTypeProfileDenominationModel
    {
        public int Id { get; set; }
        public int AccountTypeProfileID { get; set; }
        public int DenominationID { get; set; }
        public string DenominationName { get; set; }
        public string AccountTypeName { get; set; }
        public string ProfileName { get; set; }
        public bool Status { get; set; }
    }
}

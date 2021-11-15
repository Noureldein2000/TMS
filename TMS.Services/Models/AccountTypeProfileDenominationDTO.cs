using System;
using System.Collections.Generic;
using System.Text;

namespace TMS.Services.Models
{
    public class AccountTypeProfileDenominationDTO
    {
        public int Id { get; set; }
        public int AccountTypeProfileID { get; set; }
        public int DenominationID { get; set; }
        public string DenominationName { get; set; }
        public string AccountTypeName{ get; set; }
        public string ProfileName { get; set; }
        public bool Status { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TMS.API.Models
{
    public class CancelRequestModel
    {
        public int Brn { get; set; }
        public int TransactionId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public int AccountId { get; set; }
    }
}

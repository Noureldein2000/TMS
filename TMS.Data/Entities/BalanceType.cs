using System;
using System.Collections.Generic;
using System.Text;

namespace TMS.Data.Entities
{
    public class BalanceType : BaseEntity<int>
    {
        public string Name { get; set; }
        public string ArName { get; set; }

    }
}

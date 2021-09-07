﻿using System;
using System.Collections.Generic;
using System.Text;

namespace TMS.Data.Entities
{
    public class DenominationCommission : BaseEntity<int>
    {
        public int DenominationID { get; set; }
        public virtual Denomination Denomination { get; set; }
        public int CommissionID { get; set; }
        public virtual Commission Commission { get; set; }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Text;

namespace TMS.Data.Entities
{
    public class DenominationReceiptParam : BaseEntity<int>
    {
        public int DenominationID { get; set; }
        public virtual Denomination Denomination { get; set; }
        public int ParameterID { get; set; }
        public virtual Parameter Parameter { get; set; }
        public bool Bold { get; set; }
        public int Alignment { get; set; }
    }
}

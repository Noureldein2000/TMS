﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TMS.API.Models
{
    public class AddAccountFeeModel
    {
        public int AccountId { get; set; }
        public int DenominationId { get; set; }
        public int FeeId { get; set; }
    }
}

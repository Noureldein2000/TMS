﻿using System;
using System.Collections.Generic;
using System.Text;
using TMS.Infrastructure;

namespace TMS.Services.Models
{
    public class DenominationDTO
    {
        public string Name { get; set; }
        public decimal Value { get; set; }
        public int ServiceID { get; set; }
        public bool Status { get; set; }
        public int CurrencyID { get; set; }
        public decimal APIValue { get; set; }
        public decimal MinValue { get; set; }
        public decimal MaxValue { get; set; }
        public int Interval { get; set; }
        public int ServiceCategoryID { get; set; }
        public DenominationClassType ClassType { get; set; }
        public int ServiceProviderId { get; set; }
        public bool Inquirable { get; set; }
        public int? BillPaymentModeID { get; set; }
        public int? PaymentModeID { get; set; }
    }
}
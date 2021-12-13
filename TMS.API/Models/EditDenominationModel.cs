using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TMS.API.Models
{
    public class EditDenominationModel
    {
        public DenominationModel Denomination { get; set; }
        public List<DenominationServiceProvidersModel> DenominationServiceProviders { get; set; }
        public List<DenominationParameterModel> DenominationParameters { get; set; }
        public DenominationReceiptModel DenominationReceipt { get; set; }
    }
}

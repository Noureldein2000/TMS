using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TMS.Data.Entities;
using TMS.Infrastructure;
using TMS.Infrastructure.Helpers;
using TMS.Services.Models;

namespace TMS.Services.BusinessLayer
{
    public abstract class BaseProvider
    {
        public virtual async Task<InquiryResponseDTO> Inquiry(InquiryRequestDTO inquiryModel, int userId, int id)
        {
            return Validate(inquiryModel);
        }
        public abstract FeesResponseDTO Fees(FeesRequestDTO feesModel, int userId, int id);
        public abstract Task<PaymentResponseDTO> Pay(PaymentRequestDTO payModel, int userId, int id);
        private InquiryResponseDTO Validate(InquiryRequestDTO request)
        {
            if (string.IsNullOrEmpty(request.BillingAccount) || string.IsNullOrEmpty(request.Version) || string.IsNullOrEmpty(request.ServiceListVersion))
                throw new TMSException("MissingData", "15");
            return new InquiryResponseDTO();
        }
    }
}

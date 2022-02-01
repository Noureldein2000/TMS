using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TMS.Services.Models;

namespace TMS.Services.BusinessLayer
{
    public interface IBaseProvider
    {
        Task<InquiryResponseDTO> Inquiry(InquiryRequestDTO inquiryModel, int userId, int id, int serviceProviderId);
        FeesResponseDTO Fees(FeesRequestDTO feesModel, int userId, int id);
        Task<PaymentResponseDTO> Pay(PaymentRequestDTO payModel, int userId, int id, decimal totalAmount, decimal fees, int serviceProviderId, decimal taxes = 0.0M);
    }
}

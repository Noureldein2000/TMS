using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TMS.Services.Models;

namespace TMS.Services.BusinessLayer
{
    public interface IBaseProvider
    {
        Task<InquiryResponseDTO> Inquiry(InquiryRequestDTO inquiry, int userId, int id, int serviceProviderId);
        FeesResponseDTO Fees(FeesRequestDTO inquiry, int userId, int id);
        Task<PaymentResponseDTO> Pay(PaymentRequestDTO inquiry, int userId, int id, decimal totalAmount, decimal fees, int serviceProviderId);
    }
}

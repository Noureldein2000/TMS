using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TMS.Services.Models;

namespace TMS.Services.Services
{
    public interface IDynamicService
    {
        Task<InquiryResponseDTO> Inquiry(InquiryRequestDTO inquiry, int userId, int id); 
        Task<FeesResponseDTO> Fees(FeesRequestDTO fees, int userId, int id);
        Task<PaymentResponseDTO> Pay(PaymentRequestDTO fees, int userId, int id);
        Task<PaymentResponseDTO> Cancel(int transactionId,int accountId,int userId, int id);
    }
}

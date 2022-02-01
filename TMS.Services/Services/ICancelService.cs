using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TMS.Services.Models;

namespace TMS.Services.Services
{
    public interface ICancelService
    {
        Task<PaymentResponseDTO> Cancel(CancelDTO model, int userId, int id, decimal fees, int serviceProviderId,decimal taxes);
        ProviderResponseDTO CallCancellProvider(CancellProviderDTO model, out bool isCancelled);
    }
}

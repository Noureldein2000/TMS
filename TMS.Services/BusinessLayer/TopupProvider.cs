using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TMS.Infrastructure;
using TMS.Services.Models;

namespace TMS.Services.BusinessLayer
{
    public class TopupProvider : BaseProvider
    {
        public TopupProvider()
        {

        }
        public override FeesResponseDTO Fees(FeesRequestDTO inquiry, int userId, int id)
        {
            throw new NotImplementedException();
        }

        public override async Task<InquiryResponseDTO> Inquiry(InquiryRequestDTO inquiry, int userId, int id)
        {
            await base.Inquiry(inquiry, userId, id);
            throw new NotImplementedException();
        }

        public override Task<PaymentResponseDTO> Pay(PaymentRequestDTO inquiry, int userId, int id)
        {
            throw new NotImplementedException();
        }
    }
}

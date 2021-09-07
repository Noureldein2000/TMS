using System;
using System.Collections.Generic;
using System.Text;
using TMS.Services.Models;

namespace TMS.Services.Services
{
    public interface IInquiryBillService
    {
        int AddInquiryBill(InquiryBillDTO model);
        void AddReceiptBodyParam(params ReceiptBodyParamDTO[] model);
        IEnumerable<InquiryBillDTO> GetInquiryBillSequence(int providerServiceRequestId);
        void UpdateReceiptBodyParam(int providerServiceRequestId, int transactionId);
    }
}

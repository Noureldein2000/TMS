using System;
using System.Collections.Generic;
using System.Text;
using TMS.Services.Models;

namespace TMS.Services.Services
{
    public interface IInquiryBillService
    {
        int AddInquiryBill(InquiryBillDTO model);
        void AddInquiryBillDetail(params InquiryBillDetailDTO[] model);
        void AddReceiptBodyParam(params ReceiptBodyParamDTO[] model);
        List<InquiryBillDTO> GetInquiryBillSequence(int providerServiceRequestId);
        IEnumerable<ReceiptBodyParamDTO> GetReceiptListByTransacationId(int id);
        void UpdateReceiptBodyParam(int providerServiceRequestId, int transactionId);
        bool CheckBillAmountExist(int brn, decimal amount);
        List<InquiryBillDetailDTO> GetInquiryBillDetails(int providerServiceRequestID, int sequence, string language = "ar");
    }
}

using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMS.Data.Entities;
using TMS.Services.Models;
using TMS.Services.Repositories;

namespace TMS.Services.Services
{
    public class InquiryBillService : IInquiryBillService
    {
        private readonly IBaseRepository<InquiryBill, int> _inquiryBill;
        private readonly IBaseRepository<InquiryBillDetails, int> _inquiryBillDetail;
        private readonly IBaseRepository<ReceiptBodyParam, int> _receiptBodyParam;
        private readonly IBaseRepository<Parameter, int> _parameters;
        private readonly IUnitOfWork _unitOfWork;
        public InquiryBillService(
            IBaseRepository<InquiryBill, int> inquiryBill,
            IBaseRepository<InquiryBillDetails, int> inquiryBillDetails,
            IBaseRepository<ReceiptBodyParam, int> receiptBodyParam,
             IBaseRepository<Parameter, int> parameters,
            IUnitOfWork unitOfWork
            )
        {
            _inquiryBill = inquiryBill;
            _inquiryBillDetail = inquiryBillDetails;
            _receiptBodyParam = receiptBodyParam;
            _parameters = parameters;
            _unitOfWork = unitOfWork;
        }
        public int AddInquiryBill(InquiryBillDTO model)
        {
            var obj = _inquiryBill.Add(new InquiryBill
            {
                Amount = model.Amount,
                ProviderServiceResponseID = model.ProviderServiceResponseID,
                Sequence = model.Sequence
            });
            _unitOfWork.SaveChanges();
            return obj.ID;
        }

        public void AddInquiryBillDetail(params InquiryBillDetailDTO[] model)
        {
            var paramNames = model.Select(s => s.ParameterName).ToList();
            var parameters = _parameters.Getwhere(s => paramNames.Contains(s.ProviderName)).ToList();
            foreach (var param in parameters)
            {
                var bodyParam = model.Where(s => s.ParameterName == param.ProviderName).FirstOrDefault();
                _inquiryBillDetail.Add(new InquiryBillDetails
                {
                    InquiryBillID = bodyParam.InquiryBillID,
                    ParameterID = param.ID,
                    Value = bodyParam.Value
                });
            }
            _unitOfWork.SaveChanges();
        }

        public void AddReceiptBodyParam(params ReceiptBodyParamDTO[] model)
        {
            var paramNames = model.Select(s => s.ParameterName).ToList();
            var parameters = _parameters.Getwhere(s => paramNames.Contains(s.ProviderName)).ToList();
            foreach (var param in parameters)
            {
                var bodyParam = model.Where(s => s.ParameterName == param.ProviderName).FirstOrDefault();
                _receiptBodyParam.Add(new ReceiptBodyParam
                {
                    ParameterID = param.ID,
                    ProviderServiceRequestID = bodyParam.ProviderServiceRequestID,
                    TransactionID = bodyParam.TransactionID,
                    Value = bodyParam.Value
                });
            }
            _unitOfWork.SaveChanges();
        }

        public IEnumerable<InquiryBillDetailDTO> GetInquiryBillDetails(int providerServiceRequestId, int sequence)
        {
            return _inquiryBill.Getwhere(s => s.ProviderServiceResponse.ProviderServiceRequestID == providerServiceRequestId && s.Sequence == sequence).Include(x => x.InquiryBillDetails)
                .Select(s => new InquiryBillDetailDTO
                {
                    Amount = s.Amount,
                    Id = s.ID,
                    ProviderServiceResponseID = s.ProviderServiceResponseID,
                    Sequence = s.Sequence,
                    Value = s.InquiryBillDetails.Select(x => x.Value).FirstOrDefault(),
                    ParameterID = s.InquiryBillDetails.Select(x => x.ParameterID).FirstOrDefault()
                }).ToList();
        }

        public IEnumerable<InquiryBillDTO> GetInquiryBillSequence(int providerServiceRequestId)
        {
            return _inquiryBill.Getwhere(s => s.ProviderServiceResponse.ProviderServiceRequestID == providerServiceRequestId)
                .Select(s => new InquiryBillDTO
                {
                    Amount = s.Amount,
                    Id = s.ID,
                    ProviderServiceResponseID = s.ProviderServiceResponseID,
                    Sequence = s.Sequence
                }).ToList();
        }

        public IEnumerable<ReceiptBodyParamDTO> GetReceiptListByTransacationId(int id)
        {
            return _receiptBodyParam.Getwhere(rbp => rbp.TransactionID == id).Select(rbp => new ReceiptBodyParamDTO()
            {
                ParameterName = rbp.Parameter.ArName,
                ProviderServiceRequestID = rbp.ProviderServiceRequestID,
                TransactionID = rbp.TransactionID,
                Value = rbp.Value
            }).ToList();
        }

        public void UpdateReceiptBodyParam(int providerServiceRequestId, int transactionId)
        {
            var receiptBodyParam = _receiptBodyParam.Getwhere(p => p.ProviderServiceRequestID == providerServiceRequestId).ToList();
            foreach (var bodyParams in receiptBodyParam)
            {
                bodyParams.TransactionID = transactionId;
            }
            _unitOfWork.SaveChanges();
        }
    }
}

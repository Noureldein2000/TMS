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
    public class ProviderService : IProviderService
    {
        private readonly IBaseRepository<ProviderServiceRequest, int> _providerServiceRequests;
        private readonly IBaseRepository<ProviderServiceRequestParam, int> _providerServiceRequestParams;
        private readonly IBaseRepository<ProviderServiceResponse, int> _providerServiceResponses;
        private readonly IBaseRepository<ProviderServiceResponseParam, int> _providerServiceResponseParams;
        private readonly IBaseRepository<Parameter, int> _parameters;
        private readonly IUnitOfWork _unitOfWork;
        public ProviderService(
            IBaseRepository<ProviderServiceRequest, int> providerServiceRequests,
            IBaseRepository<ProviderServiceRequestParam, int> providerServiceRequestParams,
            IBaseRepository<ProviderServiceResponse, int> providerServiceResponses,
            IBaseRepository<ProviderServiceResponseParam, int> providerServiceResponseParams,
            IBaseRepository<Parameter, int> parameters,
            IUnitOfWork unitOfWork)
        {
            _providerServiceRequests = providerServiceRequests;
            _providerServiceRequestParams = providerServiceRequestParams;
            _providerServiceResponses = providerServiceResponses;
            _providerServiceResponseParams = providerServiceResponseParams;
            _parameters = parameters;
            _unitOfWork = unitOfWork;
        }
        public int AddProviderServiceRequest(ProviderServiceRequestDTO model)
        {
            var obj = _providerServiceRequests.Add(new ProviderServiceRequest
            {
                BillingAccount = model.BillingAccount,
                Brn = model.Brn,
                CreatedBy = model.CreatedBy,
                DenominationID = model.DenominationID,
                ProviderServiceRequestStatusID = model.ProviderServiceRequestStatusID,
                RequestTypeID = model.RequestTypeID,

            });
            _unitOfWork.SaveChanges();
            return obj.ID;
        }

        public void AddProviderServiceRequestParam(ProviderServiceRequestParamDTO model)
        {
            var parameter = _parameters.Getwhere(s => s.ProviderName == model.ParameterName).FirstOrDefault();
            _providerServiceRequestParams.Add(new ProviderServiceRequestParam
            {
                ParameterID = parameter.ID,
                ProviderServiceRequestID = model.ProviderServiceRequestID,
                Value = model.Value
            });
            _unitOfWork.SaveChanges();
        }

        public int AddProviderServiceResponse(ProviderServiceResponseDTO model)
        {
            var obj = _providerServiceResponses.Add(new ProviderServiceResponse
            {
                TotalAmount = model.TotalAmount,
                ProviderServiceRequestID = model.ProviderServiceRequestID
            });
            _unitOfWork.SaveChanges();
            return obj.ID;
        }

        public void AddProviderServiceResponseParam(params ProviderServiceResponseParamDTO[] model)
        {
            var paramNames = model.Select(s => s.ParameterName).ToList();
            var parameters = _parameters.Getwhere(s => paramNames.Contains(s.ProviderName)).ToList();
            foreach (var param in parameters)
            {
                var bodyParam = model.Where(s => s.ParameterName == param.ProviderName).FirstOrDefault();
                _providerServiceResponseParams.Add(new ProviderServiceResponseParam
                {
                    ParameterID = param.ID,
                    ProviderServiceResponseID = bodyParam.ServiceRequestID,
                    Value = bodyParam.Value
                });
            }
            _unitOfWork.SaveChanges();
        }

        public int GetMaxProviderServiceRequest(int brn, int requestTypeId)
        {
            return _providerServiceRequests.Getwhere(s => s.Brn == brn && s.ProviderServiceRequestStatusID == 2
            && s.RequestTypeID == requestTypeId).Max(s => s.ID);
        }

        public string GetProviderServiceRequestBillingAccount(int brn, int userId, int denominationId)
        {
            return _providerServiceRequests.Getwhere(s => s.DenominationID == denominationId
            && s.ID == brn && s.CreatedBy == userId).Select(s => s.BillingAccount).FirstOrDefault();
        }

        public bool IsProviderServiceRequestExsist(int requestTypeId, int brn, int providerServiceRequestStatusId, int denominationId, int userId)
        {
            return _providerServiceRequests.Any(s => s.DenominationID == denominationId
             && s.CreatedBy == userId
             && s.ProviderServiceRequestStatusID == providerServiceRequestStatusId
             && (s.ID == brn || s.Brn == brn)
             && s.RequestTypeID == requestTypeId
             && EF.Functions.DateDiffSecond(s.CreationDate, DateTime.Now) < 300);

        }

        public void UpdateProviderServiceRequestStatus(int id, int providerServiceRequestStatusId, int updatedBy)
        {
            var request = _providerServiceRequests.GetById(id);
            request.ProviderServiceRequestStatusID = providerServiceRequestStatusId;
            request.UpdatedBy = updatedBy;
            _unitOfWork.SaveChanges();
        }
    }
}

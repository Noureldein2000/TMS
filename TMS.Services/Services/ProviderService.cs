using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMS.Data.Entities;
using TMS.Infrastructure;
using TMS.Services.Models;
using TMS.Services.Repositories;
using RequestType = TMS.Infrastructure.RequestType;

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
                ProviderServiceRequestStatusID = (int)model.ProviderServiceRequestStatusID,
                RequestTypeID = (int)model.RequestTypeID,

            });
            _unitOfWork.SaveChanges();
            return obj.ID;
        }

        public void AddProviderServiceRequestParam(ProviderServiceRequestParamDTO model)
        {
            var parameter = _parameters.Getwhere(s => s.ProviderName == model.ParameterName).FirstOrDefault();
            if (parameter != null)
            {

                _providerServiceRequestParams.Add(new ProviderServiceRequestParam
                {
                    ParameterID = parameter.ID,
                    ProviderServiceRequestID = model.ProviderServiceRequestID,
                    Value = model.Value
                });
                _unitOfWork.SaveChanges();
            }
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

        public int GetMaxProviderServiceRequest(int brn, RequestType requestTypeId)
        {
            return _providerServiceRequests.Getwhere(s => s.Brn == brn && s.ProviderServiceRequestStatusID == 2
            && s.RequestTypeID == (int)requestTypeId).Max(s => s.ID);
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

        public void UpdateProviderServiceRequestStatus(int id, ProviderServiceRequestStatusType providerServiceRequestStatusId, int updatedBy)
        {
            var request = _providerServiceRequests.GetById(id);
            request.ProviderServiceRequestStatusID = (int)providerServiceRequestStatusId;
            request.UpdatedBy = updatedBy;
            _unitOfWork.SaveChanges();
        }
        public Dictionary<string, string> GetProviderServiceRequestParams(int providerServiceRequestId, string language = "ar", params string[] parameterNames)
        {
            return _providerServiceRequestParams.Getwhere(s => parameterNames.Contains(s.Parameter.ProviderName)
           && s.ProviderServiceRequestID == providerServiceRequestId).Select(s => new Dictionary<string, string>
           {
                { language == "ar" ? s.Parameter.ArName : s.Parameter.Name, s.Value }
           }).FirstOrDefault();
        }
        //public Dictionary<string, decimal> GetProviderServiceRequestParam(int providerServiceRequestId, string parameterName, string language = "ar")
        //{
        //    return _providerServiceRequestParams.Getwhere(s => s.Parameter.ProviderName == parameterName
        //    && s.ProviderServiceRequestID == providerServiceRequestId).Select(s => new Dictionary<string, decimal>
        //    {
        //        { language == "ar" ? s.Parameter.ArName : s.Parameter.Name, decimal.Parse(s.Value) }
        //    }).FirstOrDefault();

        //}

        //public Dictionary<string, string> GetProviderServiceResponseParam(int providerServiceRequestId, string parameterName, string language = "ar")
        //{
        //    var resp = _providerServiceResponseParams.Getwhere(s => s.Parameter.ProviderName == parameterName
        //   && s.ProviderServiceResponse.ProviderServiceRequestID == providerServiceRequestId).Include(s => s.Parameter).FirstOrDefault();

        //    return new Dictionary<string, string>
        //    {
        //        { language == "ar" ? resp.Parameter.ArName : resp.Parameter.Name, resp.Value }
        //    };
        //}

        public IEnumerable<ProviderServiceResponseParamDTO> GetProviderServiceResponseParams(int providerServiceRequestId, string language = "ar", params string[] parameterNames)
        {
            return _providerServiceResponseParams.Getwhere(s => parameterNames.Contains(s.Parameter.ProviderName)
           && s.ProviderServiceResponse.ProviderServiceRequestID == providerServiceRequestId).Include(s => s.Parameter)
           .Select(s => new ProviderServiceResponseParamDTO
           {
               ParameterName = language == "ar" ? s.Parameter.ArName : s.Parameter.Name,
               ProviderName = s.Parameter.ProviderName,
               Value = s.Value
           }).ToList();

            //return new Dictionary<string, string>
            //{
            //    { language == "ar" ? resp.Parameter.ArName : resp.Parameter.Name, resp.Value }
            //};
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using TMS.Infrastructure;
using TMS.Services.Models;

namespace TMS.Services.Services
{
    public interface IProviderService
    {
        int AddProviderServiceRequest(ProviderServiceRequestDTO model);
        bool IsProviderServiceRequestExsist(int requestTypeId, int brn, int providerServiceRequestStatusId, int denominationId, int userId);
        int AddProviderServiceResponse(ProviderServiceResponseDTO model);
        void AddProviderServiceRequestParam(ProviderServiceRequestParamDTO model);
        void AddProviderServiceResponseParam(params ProviderServiceResponseParamDTO[] model);
        //GetProviderServiceResponseParamDTO GetProviderServiceResponseParam();
        void UpdateProviderServiceRequestStatus(int ID, ProviderServiceRequestStatusType ProviderServiceRequestStatusID, int UpdatedBy);
        int GetMaxProviderServiceRequest(int brn, int requestTypeId);
        string GetProviderServiceRequestBillingAccount(int brn, int userId, int denominationId);
        //Dictionary<string, string> GetProviderServiceResponseParam(int providerServiceRequestId, string parameterName, string language = "ar");
        IEnumerable<ProviderServiceResponseParamDTO> GetProviderServiceResponseParams(int providerServiceRequestId, string language = "ar", params string[] parameterNames);
        Dictionary<string, string> GetProviderServiceRequestParams(int providerServiceRequestId, string language = "ar", params string[] parameterNames);
        //Dictionary<string, decimal> GetProviderServiceRequestParam(int providerServiceRequestId, string parameterName, string language = "ar");
    }
}

using System;
using System.Collections.Generic;
using System.Text;
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
        void UpdateProviderServiceRequestStatus(int ID, int ProviderServiceRequestStatusID, int UpdatedBy);
        int GetMaxProviderServiceRequest(int brn, int requestTypeId);
        string GetProviderServiceRequestBillingAccount(int brn, int userId, int denominationId);
    }
}

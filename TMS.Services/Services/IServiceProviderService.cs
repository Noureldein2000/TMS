using System;
using System.Collections.Generic;
using System.Text;
using TMS.Services.Models;

namespace TMS.Services.Services
{
    public interface IServiceProviderService
    {
        PagedResult<ServiceProviderDTO> GetServiceProviders(int page, int pageSize);
        ServiceProviderDTO GetServiceProviderById(int id);
        void AddServiceProviders(ServiceProviderDTO model);
        void EditServiceProviders(ServiceProviderDTO model);
        void DeleteServiceProviders(int id);
    }
}

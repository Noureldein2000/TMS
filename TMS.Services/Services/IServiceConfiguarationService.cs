using System;
using System.Collections.Generic;
using System.Text;
using TMS.Services.Models;

namespace TMS.Services.Services
{
    public interface IServiceConfiguarationService
    {
        PagedResult<ServiceConfigerationDTO> GetServiceConfiguartions(int page, int size);
        ServiceConfigerationDTO AddServiceConfiguartions(ServiceConfigerationDTO model);
        ServiceConfigerationDTO GetServiceConfiguartionById(int id);
        void EditServiceConfiguartions(ServiceConfigerationDTO model);
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using TMS.Data.Entities;
using TMS.Services.Models;

namespace TMS.Services.Services
{
    public interface IAdminService
    {
        PagedResult<AdminServiceDTO> GetServices(int pageNumber, int pageSize, string language = "ar");
        PagedResult<AdminServiceDTO> SearchServices(int? dropDownFilter, string searchKey, int pageNumber, int pageSize, string language = "ar");
        void AddService(AdminServiceDTO service);
        AdminServiceDTO GetServiceById(int id);
        void EditService(AdminServiceDTO service);
        void ChangeStatus(int id);
        IEnumerable<ServiceEntityDTO> GetServiceEntities();
        IEnumerable<ServiceTypesDTO> GetServiceTypes();
        IEnumerable<ServiceCategoryDTO> GetServiceCategories();

    }
}

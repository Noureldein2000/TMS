using System;
using System.Collections.Generic;
using System.Text;
using TMS.Services.Models;
using TMS.Services.Models.SwaggerModels;

namespace TMS.Services.Services
{
    public interface ICommissionService
    {
        IEnumerable<CommissionDTO> GetAccountCommission(int denominationId, decimal originalAmount, int accountId, out decimal sum, string language = "ar");
        IEnumerable<CommissionDTO> GetAccountProfileCommission(int denominationId, decimal originalAmount, int accountProfileId, out decimal sum, string language = "ar");
        IEnumerable<CommissionDTO> GetDenominationCommission(int denominationId, decimal originalAmount, out decimal sum, string language = "ar");
        IEnumerable<CommissionDTO> GetCommission(int denominationId, decimal originalAmount, int accountId, int accountProfileId, out decimal sum, string language = "ar");
        PagedResult<CommissionDTO> GetCommission(int page, int pageSize, string language);
        CommissionDTO GetCommissionById(int id);
        void ChangeStatus(int id);
        void DeleteCommission(int id);
        void AddCommission(CommissionDTO commission);
        void EditCommission(CommissionDTO commission);
    }
}

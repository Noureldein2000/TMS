using System;
using System.Collections.Generic;
using System.Text;
using TMS.Services.Models;

namespace TMS.Services.Services
{
    public interface IFeesService
    {
        IEnumerable<FeesDTO> GetAccountFees(int denominationId, decimal originalAmount, int accountId, out decimal sum, string language = "ar");
        IEnumerable<FeesDTO> GetAccountProfileFees(int denominationId, decimal originalAmount, int accountProfileId, out decimal sum, string language = "ar");
        IEnumerable<FeesDTO> GetDenominationFees(int denominationId, decimal originalAmount, out decimal sum, string language = "ar");
        IEnumerable<FeesDTO> GetFees(int denominationId, decimal originalAmount, int accountId, int accountProfileId, out decimal sum, string language = "ar");
        PagedResult<FeesDTO> GetFees(int page, int pageSize, string language);
        FeesDTO GetFeeById(int id);
        void ChangeStatus(int id);
        void DeleteFee(int id);
        void AddFee(FeesDTO fee);
        void EditFee(FeesDTO fee);
    }
}

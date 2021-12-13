using System;
using System.Collections.Generic;
using System.Text;
using TMS.Services.Models;

namespace TMS.Services.Services
{
    public interface IAccountTypeProfileDenominationService
    {
        void Add(AccountTypeProfileDenominationDTO model);
        void ChangeStatus(int id);
        void Delete(int id);
        PagedResult<AccountTypeProfileDenominationDTO> GetAccountTypeProfileDenominations(int page, int pageSize);
    }
}

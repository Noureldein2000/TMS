using System;
using System.Collections.Generic;
using System.Text;
using TMS.Services.Models;

namespace TMS.Services.Services
{
   public interface IAccountTypeProfileCommissionService
    {
        void Add(AccountTypeProfileCommissionDTO model);
        void Delete(int id);
        PagedResult<AccountTypeProfileCommissionDTO> GetAccountTypeProfileCommissions(int id, int page, int pageSize, string language);
    }
}

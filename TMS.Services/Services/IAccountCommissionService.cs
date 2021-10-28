using System;
using System.Collections.Generic;
using System.Text;
using TMS.Services.Models;

namespace TMS.Services.Services
{
    public interface IAccountCommissionService
    {
        void AddAccountCommission(AccountCommissionDTO model);
        void DeleteAccountCommission(int id);
        PagedResult<AccountCommissionDTO> GetAccountCommissionByAccountId(int accountId, int pagenumber, int pageSize, string language = "ar");
    }
}

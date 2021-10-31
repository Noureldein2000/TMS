using System;
using System.Collections.Generic;
using System.Text;
using TMS.Services.Models;

namespace TMS.Services.Services
{
   public interface IAccountFeesService
    {
        void DeleteAccountFees(int id);
        void AddAccountFees(AccountFeesDTO model);
        PagedResult<AccountFeesDTO> GetAccountFeesByAccountId(int accountId, int pagenumber, int pageSize, string language = "ar");
    }
}

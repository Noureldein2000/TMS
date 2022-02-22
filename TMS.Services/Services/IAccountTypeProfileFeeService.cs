using System;
using System.Collections.Generic;
using System.Text;
using TMS.Services.Models;

namespace TMS.Services.Services
{
    public interface IAccountTypeProfileFeeService
    {
        AccountTypeProfileFeesDTO Add(AccountTypeProfileFeesDTO model);
        void Delete(int id);
        PagedResult<AccountTypeProfileFeesDTO> GetAccountTypeProfileFees(int id,int page, int pageSize,string language);
    }
}

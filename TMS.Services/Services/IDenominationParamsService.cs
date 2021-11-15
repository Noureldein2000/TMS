using System;
using System.Collections.Generic;
using System.Text;
using TMS.Services.Models;

namespace TMS.Services.Services
{
    public interface IDenominationParamService
    {
        PagedResult<DenominationParamDTO> GetParams(int page, int pageSize, string language);
        DenominationParamDTO GetParamById(int id);
        DenominationParamDTO AddParam(DenominationParamDTO model);
        void EditParam(DenominationParamDTO model);
        void DeleteParam(int id);
    }
}

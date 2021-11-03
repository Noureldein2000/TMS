using System;
using System.Collections.Generic;
using System.Text;
using TMS.Services.Models;

namespace TMS.Services.Services
{
    public interface IParameterService
    {
        PagedResult<ParameterDTO> GetParamters(int page, int pageSize, string language);
        ParameterDTO GetParamterById(int id);
        void AddParameter(ParameterDTO model);
        void EditParameter(ParameterDTO model);
        void DeleteParameter(int id);
    }
}

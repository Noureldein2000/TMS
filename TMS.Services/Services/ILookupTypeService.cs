using System;
using System.Collections.Generic;
using System.Text;
using TMS.Infrastructure;
using TMS.Services.Models;

namespace TMS.Services.Services
{
    public interface ILookupTypeService
    {
        IEnumerable<LookupTypeDTO> GetAllLookups(string language);
        LookupTypeDTO GetLookupTypeById(int id, LookupType identifier);
        void DeleteLookupType(int id, LookupType identifier );
        LookupTypeDTO AddLookupType(LookupTypeDTO dto);
        void EditLookupType(LookupTypeDTO tax);
    }
}

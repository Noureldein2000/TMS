using System;
using System.Collections.Generic;
using System.Text;
using TMS.Services.Models;

namespace TMS.Services.Services
{
    public interface ILookupTypeService
    {
        IEnumerable<LookupTypeDTO> GetAllLookups(string language);
        //TaxesDTO GetTaxById(int id);
        //void ChangeStatus(int id);
        //void DeleteTax(int id);
        LookupTypeDTO AddLookupType(LookupTypeDTO dto);
        //void EditTax(TaxesDTO tax);
    }
}

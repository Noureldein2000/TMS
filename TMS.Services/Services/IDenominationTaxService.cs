using System;
using System.Collections.Generic;
using System.Text;
using TMS.Services.Models;

namespace TMS.Services.Services
{
    public interface IDenominationTaxService
    {
        void AddDenominationTaxes(AddDenominationTaxesDTO model);
        void DeleteDenominationTaxes(int id);
        IEnumerable<DenominationTaxesDTO> GetDeniminationTaxesByDenominationId(int denominationId, string language);
    }
}

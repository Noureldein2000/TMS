using System;
using System.Collections.Generic;
using System.Text;
using TMS.Services.Models;

namespace TMS.Services.Services
{
    public interface IDenominationFeesService
    {
        DenominationFeesDTO AddDenominationFees(AddDenominationFeesDTO model);
        void DeleteDenominationFees(int id);
        IEnumerable<DenominationFeesDTO> GetDeniminationFeesByDenominationId(int denominationId, string language);
    }
}

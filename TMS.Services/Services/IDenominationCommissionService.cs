using System;
using System.Collections.Generic;
using System.Text;
using TMS.Services.Models;

namespace TMS.Services.Services
{
    public interface IDenominationCommissionService
    {
        void AddDenominationCommission(AddDenominationCommissionDTO model);
        void DeleteDenominationCommission(int id);
        IEnumerable<DenominationCommissionDTO> GetDeniminationCommissionsByDenominationId(int denominationId, string language);
    }
}

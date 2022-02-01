using System;
using System.Collections.Generic;
using System.Text;
using TMS.Services.Models;

namespace TMS.Services.Services
{
   public interface ITaxService
    {
        IEnumerable<TaxesDTO> GetDenominationTaxes(int denominationId, decimal originalAmount, out decimal sum, string language = "ar");
        IEnumerable<TaxesDTO> GetTaxes(int denominationId, decimal originalAmount, int accountId, int accountProfileId, out decimal sum, string language = "ar");
        PagedResult<TaxesDTO> GetTaxes(int page, int pageSize, string language);
        TaxesDTO GetTaxById(int id);
        void ChangeStatus(int id);
        void DeleteTax(int id);
        void AddTax(TaxesDTO tax);
        void EditTax(TaxesDTO tax);
    }
}

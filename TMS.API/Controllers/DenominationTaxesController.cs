using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMS.API.Models;
using TMS.Infrastructure.Helpers;
using TMS.Services.Models;
using TMS.Services.Services;

namespace TMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DenominationTaxesController : BaseController
    {
        private readonly IDenominationTaxService _denominationTaxService;
        private readonly IStringLocalizer<LanguageResource> _localizer;
        public DenominationTaxesController(IDenominationTaxService denominationTaxService,
            IStringLocalizer<LanguageResource> localizer)
        {
            _denominationTaxService = denominationTaxService;
            _localizer = localizer;
        }

        [HttpGet]
        [Route("GetdenominationTaxesByDenominationId/{denominationId}")]
        [ProducesResponseType(typeof(List<DenominationTaxesModel>), StatusCodes.Status200OK)]
        public IActionResult GetdenominationTaxesByDenominationId(int denominationId, string language = "ar")
        {
            try
            {
                var result = _denominationTaxService.GetDeniminationTaxesByDenominationId(denominationId, language).Select(x => Map(x)).ToList();
                return Ok(result);
            }
            catch (TMSException ex)
            {
                return BadRequest(_localizer[ex.Message].Value, ex.ErrorCode);
            }
            catch (Exception ex)
            {
                return BadRequest(_localizer["GeneralError"].Value, "-1");
            }
        }
        [HttpPost]
        [Route("AddDenominationTaxes")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        public IActionResult AddDenominationTaxes([FromBody] AddDenominationTaxesModel model)
        {
            try
            {
                _denominationTaxService.AddDenominationTaxes(new AddDenominationTaxesDTO
                {
                    TaxId = model.TaxId,
                    DenominationId = model.DenominationId
                });

                return Ok();
            }
            catch (TMSException ex)
            {
                return BadRequest(_localizer[ex.Message].Value, ex.ErrorCode);
            }
            catch (Exception ex)
            {
                return BadRequest(_localizer["GeneralError"].Value, "-1");
            }
        }
        [HttpDelete]
        [Route("DeleteDenominationTax/{id}")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        public IActionResult DeleteDenominationTax(int id)
        {
            try
            {
                _denominationTaxService.DeleteDenominationTaxes(id);
                return Ok();
            }
            catch (TMSException ex)
            {
                return BadRequest(_localizer[ex.Message].Value, ex.ErrorCode);
            }
            catch (Exception ex)
            {
                return BadRequest(_localizer["GeneralError"].Value, "-1");
            }
        }

        private DenominationTaxesModel Map(DenominationTaxesDTO model)
        {
            return new DenominationTaxesModel
            {
                Id = model.Id,
                TaxId = model.TaxId,
                TaxTypeId = model.TaxTypeId,
                TaxTypeName = model.TaxTypeName,
                TaxValue = model.TaxValue,
                PaymentModeId = model.PaymentModeId,
                PaymentMode = model.PaymentMode,
                DenominationId = model.DenominationId,
                DenominationFullName = model.DenominationFullName
            };
        }
    }
}

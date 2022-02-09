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
    public class DenominationFeesController : BaseController
    {
        private readonly IDenominationFeesService _denominationFeeService;
        private readonly IStringLocalizer<LanguageResource> _localizer;
        public DenominationFeesController(IDenominationFeesService denominationFeeService,
            IStringLocalizer<LanguageResource> localizer)
        {
            _denominationFeeService = denominationFeeService;
            _localizer = localizer;
        }

        [HttpGet]
        [Route("GetdenominationFeesByDenominationId/{denominationId}")]
        [ProducesResponseType(typeof(List<DenominationFeesModel>), StatusCodes.Status200OK)]
        public IActionResult GetdenominationFeesByDenominationId(int denominationId, string language = "ar")
        {
            try
            {
                var result = _denominationFeeService.GetDeniminationFeesByDenominationId(denominationId, language).Select(x => Map(x)).ToList();
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
        [Route("AddDenominationFees")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        public IActionResult AddDenominationFees([FromBody] AddDenominationFeesModel model)
        {
            try
            {
                _denominationFeeService.AddDenominationFees(new AddDenominationFeesDTO
                {
                    FeesId = model.FeesId,
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
        [Route("DeleteDenominationFee/{id}")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        public IActionResult DeleteDenominationFee(int id)
        {
            try
            {
                _denominationFeeService.DeleteDenominationFees(id);
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

        private DenominationFeesModel Map(DenominationFeesDTO model)
        {
            return new DenominationFeesModel
            {
                Id = model.Id,
                FeesId = model.FeesId,
                FeesTypeId = model.FeesTypeId,
                FeesTypeName = model.FeesTypeName,
                FeesValue = model.FeesValue,
                PaymentModeId = model.PaymentModeId,
                PaymentMode = model.PaymentMode,
                DenominationId = model.DenominationId,
                Range = model.Range
            };
        }

    }
}

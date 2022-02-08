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
    public class DenominationCommissionController : BaseController
    {
        private readonly IDenominationCommissionService _denominationCommissionService;
        private readonly IStringLocalizer<LanguageResource> _localizer;
        public DenominationCommissionController(IDenominationCommissionService denominationCommissionService,
            IStringLocalizer<LanguageResource> localizer)
        {
            _denominationCommissionService = denominationCommissionService;
            _localizer = localizer;
        }

        [HttpGet]
        [Route("GetdenominationCommissionByDenominationId/{denominationId}")]
        [ProducesResponseType(typeof(List<DenominationCommissionModel>), StatusCodes.Status200OK)]
        public IActionResult GetAccountCommissionByDenominationId(int denominationId, string language = "ar")
        {
            try
            {
                var result = _denominationCommissionService.GetDeniminationCommissionsByDenominationId(denominationId, language).Select(x => Map(x)).ToList();
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
        [Route("AddDenominationCommission")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        public IActionResult AddDenominationCommission([FromBody] AddDenominationCommissionModel model)
        {
            try
            {
                _denominationCommissionService.AddDenominationCommission(new AddDenominationCommissionDTO
                {
                    CommissionId = model.CommissionId,
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
        [Route("DeleteDenominationCommission/{id}")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        public IActionResult DeleteDenominationCommission(int id)
        {
            try
            {
                _denominationCommissionService.DeleteDenominationCommission(id);
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

        private DenominationCommissionModel Map(DenominationCommissionDTO model)
        {
            return new DenominationCommissionModel
            {
                Id = model.Id,
                CommissionId = model.CommissionId,
                CommissionTypeId = model.CommissionTypeId,
                CommissionTypeName = model.CommissionTypeName,
                CommissionValue = model.CommissionValue,
                PaymentModeId = model.PaymentModeId,
                PaymentMode = model.PaymentMode,
                DenominationId = model.DenominationId,
                DenominationFullName = model.DenominationFullName,
                Range = model.Range
            };
        }

    }
}

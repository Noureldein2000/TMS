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
    public class FeesController : BaseController
    {
        private readonly IFeesService _feeService;
        private readonly IStringLocalizer<LanguageResource> _localizer;
        public FeesController(IFeesService feeService,
            IStringLocalizer<LanguageResource> localizer)
        {
            _feeService = feeService;
            _localizer = localizer;
        }

        [HttpGet]
        [Route("GetFees")]
        [ProducesResponseType(typeof(List<FeesModel>), StatusCodes.Status200OK)]
        public IActionResult GetFees()
        {
            try
            {
                var response = _feeService.GetFees().Select(x => Map(x)).ToList();
                //response.Message = _localizer["Success"].Value;
                return Ok(response);
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
       
        private FeesModel Map(FeesDTO fee)
        {
            return new FeesModel
            {
                ID = fee.ID,
                FeesTypeID = fee.FeesTypeID,
                FeesTypeName = fee.FeesTypeName,
                Value = fee.Value,
                FeeRange = fee.FeeRange,
                PaymentModeID = fee.PaymentModeID,
                Status = fee.Status,
                AmountFrom = fee.AmountFrom,
                AmountTo = fee.AmountTo
            };
        }

    }
}

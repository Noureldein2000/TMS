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
using TMS.Services.Models.SwaggerModels;
using TMS.Services.Services;

namespace TMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommissionController : BaseController
    {
        private readonly ICommissionService _commissionService;
        private readonly IStringLocalizer<LanguageResource> _localizer;
        public CommissionController(ICommissionService commissionService,
            IStringLocalizer<LanguageResource> localizer)
        {
            _commissionService = commissionService;
            _localizer = localizer;
        }

        [HttpGet]
        [Route("GetCommissions")]
        [ProducesResponseType(typeof(List<CommissionModel>), StatusCodes.Status200OK)]
        public IActionResult GetCommissions()
        {
            try
            {
                var response = _commissionService.GetCommission().Select(x => Map(x)).ToList();
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
        private CommissionModel Map(CommissionDTO fee)
        {
            return new CommissionModel
            {
                ID = fee.ID,
                CommissionTypeID = fee.CommissionTypeID,
                CommissionTypeName = fee.CommissionTypeName,
                Value = fee.Value,
                CommissionRange = fee.CommissionRange,
                PaymentModeID = fee.PaymentModeID,
                Status = fee.Status,
                AmountFrom = fee.AmountFrom,
                AmountTo = fee.AmountTo
            };
        }
    }
}

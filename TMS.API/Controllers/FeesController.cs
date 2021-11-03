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
        [ProducesResponseType(typeof(PagedResult<FeesModel>), StatusCodes.Status200OK)]
        public IActionResult GetFees(int pageNumber = 1, int pageSize = 10, string language = "ar")
        {
            try
            {
                var response = _feeService.GetFees(pageNumber, pageSize, language);
                return Ok(new PagedResult<FeesModel>
                {
                    Results = response.Results.Select(ard => MapToModel(ard)).ToList(),
                    PageCount = response.PageCount
                });
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

        [HttpGet]
        [Route("GetFeeById/{id}")]
        [ProducesResponseType(typeof(FeesModel), StatusCodes.Status200OK)]
        public IActionResult GetFeeById(int id)
        {
            try
            {
                var response = _feeService.GetFeeById(id);
                return Ok(MapToModel(response));
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

        [HttpPut]
        [Route("ChangeStatus/{id}")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        public IActionResult ChangeStatus(int id)
        {
            try
            {
                _feeService.ChangeStatus(id);
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
        [Route("DeleteFee/{id}")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        public IActionResult DeleteFee(int id)
        {
            try
            {
                _feeService.DeleteFee(id);
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

        [HttpPost]
        [Route("AddFee")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        public IActionResult AddFee(FeesModel fee)
        {
            try
            {
                _feeService.AddFee(new FeesDTO
                {
                    FeesTypeID = fee.FeesTypeID,
                    Value = fee.Value,
                    PaymentModeID = fee.PaymentModeID,
                    Status = fee.Status,
                    AmountFrom = fee.AmountFrom,
                    AmountTo = fee.AmountTo,
                    StartDate = fee.StartDate,
                    EndDate = fee.EndDate,
                    CreatedBy = UserIdentityId
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

        [HttpPut]
        [Route("EditFee")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        public IActionResult EditFee(FeesModel fee)
        {
            try
            {
                _feeService.EditFee(new FeesDTO
                {
                    ID = fee.ID,
                    FeesTypeID = fee.FeesTypeID,
                    Value = fee.Value,
                    PaymentModeID = fee.PaymentModeID,
                    Status = fee.Status,
                    AmountFrom = fee.AmountFrom,
                    AmountTo = fee.AmountTo,
                    StartDate = fee.StartDate,
                    EndDate = fee.EndDate,
                    CreatedBy = UserIdentityId
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

        private FeesModel MapToModel(FeesDTO fee)
        {
            return new FeesModel
            {
                ID = fee.ID,
                FeesTypeID = fee.FeesTypeID,
                FeesTypeName = fee.FeesTypeName,
                Value = fee.Value,
                FeeRange = fee.FeeRange,
                PaymentModeID = fee.PaymentModeID,
                PaymentModeName = fee.PaymentModeName,
                Status = fee.Status,
                CreatedBy = fee.CreatedBy,
                AmountFrom = fee.AmountFrom,
                AmountTo = fee.AmountTo,
                StartDate = fee.StartDate,
                EndDate = fee.EndDate
            };
        }

    }
}

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
        [ProducesResponseType(typeof(PagedResult<CommissionModel>), StatusCodes.Status200OK)]
        public IActionResult GetCommissions(int pageNumber = 1, int pageSize = 10, string language = "ar")
        {
            try
            {
                var response = _commissionService.GetCommission(pageNumber, pageSize, language);
                return Ok(new PagedResult<CommissionModel>
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
                return BadRequest(ex.Message, "-1");
            }
        }
        [HttpGet]
        [Route("GetCommissionById/{id}")]
        [ProducesResponseType(typeof(CommissionModel), StatusCodes.Status200OK)]
        public IActionResult GetCommissionById(int id)
        {
            try
            {
                var response = _commissionService.GetCommissionById(id);
                return Ok(MapToModel(response));
            }
            catch (TMSException ex)
            {
                return BadRequest(_localizer[ex.Message].Value, ex.ErrorCode);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message, "-1");
            }
        }
        [HttpPut]
        [Route("ChangeStatus/{id}")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        public IActionResult ChangeStatus(int id)
        {
            try
            {
                _commissionService.ChangeStatus(id);
                return Ok();
            }
            catch (TMSException ex)
            {
                return BadRequest(_localizer[ex.Message].Value, ex.ErrorCode);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message, "-1");
            }
        }
        [HttpDelete]
        [Route("DeleteCommission/{id}")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        public IActionResult DeleteCommission(int id)
        {
            try
            {
                _commissionService.DeleteCommission(id);
                return Ok();
            }
            catch (TMSException ex)
            {
                return BadRequest(_localizer[ex.Message].Value, ex.ErrorCode);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message, "-1");
            }
        }
        [HttpPost]
        [Route("AddCommission")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        public IActionResult AddCommission(CommissionModel model)
        {
            try
            {
                _commissionService.AddCommission(new CommissionDTO
                {
                    CommissionTypeID = model.CommissionTypeID,
                    Value = model.Value,
                    PaymentModeID = model.PaymentModeID,
                    Status = model.Status,
                    AmountFrom = model.AmountFrom,
                    AmountTo = model.AmountTo,
                    StartDate = model.StartDate,
                    EndDate = model.EndDate,
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
                return BadRequest(ex.Message, "-1");
            }
        }
        [HttpPut]
        [Route("EditCommission")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        public IActionResult EditCommission(CommissionModel model)
        {
            try
            {
                _commissionService.EditCommission(new CommissionDTO
                {
                    ID=model.ID,
                    CommissionTypeID = model.CommissionTypeID,
                    Value = model.Value,
                    PaymentModeID = model.PaymentModeID,
                    Status = model.Status,
                    AmountFrom = model.AmountFrom,
                    AmountTo = model.AmountTo,
                    StartDate = model.StartDate,
                    EndDate = model.EndDate,
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
                return BadRequest(ex.Message, "-1");
            }
        }

        private CommissionModel MapToModel(CommissionDTO model)
        {
            return new CommissionModel
            {
                ID = model.ID,
                CommissionTypeID = model.CommissionTypeID,
                CommissionTypeName = model.CommissionTypeName,
                Value = model.Value,
                PaymentModeID = model.PaymentModeID,
                Status = model.Status,
                AmountFrom = model.AmountFrom,
                AmountTo = model.AmountTo,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                PaymentModeName = model.PaymentModeName,
                CreatedBy = model.CreatedBy
            };
        }
    }
}

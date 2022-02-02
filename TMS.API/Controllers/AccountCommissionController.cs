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
    public class AccountCommissionController : BaseController
    {
        private readonly IAccountCommissionService _accountCommissionService;
        private readonly IStringLocalizer<LanguageResource> _localizer;
        public AccountCommissionController(IAccountCommissionService accountCommissionService,
            IStringLocalizer<LanguageResource> localizer)
        {
            _accountCommissionService = accountCommissionService;
            _localizer = localizer;
        }

        [HttpGet]
        [Route("GetAccountCommissionByAccountId/{accountId}")]
        [ProducesResponseType(typeof(PagedResult<AccountCommissionModel>), StatusCodes.Status200OK)]
        public IActionResult GetAccountCommissionByAccountId(int accountId, int pageNumber = 1, int pageSize = 10, string language = "ar")
        {
            try
            {
                var result = _accountCommissionService.GetAccountCommissionByAccountId(accountId, pageNumber, pageSize, language);
                return Ok(new PagedResult<AccountCommissionModel>
                {
                    Results = result.Results.Select(ard => Map(ard)).ToList(),
                    PageCount = result.PageCount
                });
            }
            catch (TMSException ex)
            {
                return BadRequest(_localizer[ex.Message].Value, ex.ErrorCode);
            }
            catch (Exception  )
            {
                return BadRequest(_localizer["GeneralError"].Value, "-1");
            }
        }
        [HttpPost]
        [Route("AddAccountCommission")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        public IActionResult AddAccountCommission([FromBody] AddAccountCommissionModel model)
        {
            try
            {
                _accountCommissionService.AddAccountCommission(new AccountCommissionDTO
                {
                    CommissionId = model.CommissionId,
                    AccountId = model.AccountId,
                    DenomiinationId = model.DenominationId
                });

                return Ok();
            }
            catch (TMSException ex)
            {
                return BadRequest(_localizer[ex.Message].Value, ex.ErrorCode);
            }
            catch (Exception  )
            {
                return BadRequest(_localizer["GeneralError"].Value, "-1");
            }
        }

        [HttpDelete]
        [Route("DeleteAccountCommission/{id}")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        public IActionResult DeleteAccountCommission(int id)
        {
            try
            {
                _accountCommissionService.DeleteAccountCommission(id);
                return Ok();
            }
            catch (TMSException ex)
            {
                return BadRequest(_localizer[ex.Message].Value, ex.ErrorCode);
            }
            catch (Exception  )
            {
                return BadRequest(_localizer["GeneralError"].Value, "-1");
            }
        }

        private AccountCommissionModel Map(AccountCommissionDTO model)
        {
            return new AccountCommissionModel
            {
                Id = model.Id,
                CommissionId = model.CommissionId,
                CommissionTypeId = model.CommissionTypeId,
                CommissionTypeName = model.CommissionTypeName,
                CommissionValue = model.CommissionValue,
                PaymentModeId = model.PaymentModeId,
                PaymentMode = model.PaymentMode,
                DenomiinationId = model.DenomiinationId,
                DenominationFullName = model.DenominationFullName
            };
        }
    }
}

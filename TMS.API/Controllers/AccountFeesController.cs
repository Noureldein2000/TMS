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
    public class AccountFeesController : BaseController
    {
        private readonly IAccountFeesService _accountFeesService;
        private readonly IStringLocalizer<LanguageResource> _localizer;
        public AccountFeesController(IAccountFeesService accountFeesService,
            IStringLocalizer<LanguageResource> localizer)
        {
            _accountFeesService = accountFeesService;
            _localizer = localizer;
        }

        [HttpGet]
        [Route("GetAccountFeesByAccountId/{accountId}")]
        [ProducesResponseType(typeof(PagedResult<AccountFeesModel>), StatusCodes.Status200OK)]
        public IActionResult GetAccountFeesByAccountId(int accountId, int pageNumber = 1, int pageSize = 10, string language = "ar")
        {
            try
            {
                var result = _accountFeesService.GetAccountFeesByAccountId(accountId, pageNumber, pageSize, language);
                return Ok(new PagedResult<AccountFeesModel>
                {
                    Results = result.Results.Select(ard => Map(ard)).ToList(),
                    PageCount = result.PageCount
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

        [HttpPost]
        [Route("AddAccountFees")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        public IActionResult AddAccountFees([FromBody] AddAccountFeeModel model)
        {
            try
            {
                _accountFeesService.AddAccountFees(new AccountFeesDTO
                {
                    FeesId = model.FeeId,
                    AccountId = model.AccountId,
                    DenomiinationId = model.DenominationId
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
        [Route("DeleteAccountFee/{id}")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        public IActionResult DeleteAccountFee(int id)
        {
            try
            {
                _accountFeesService.DeleteAccountFees(id);
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

        private AccountFeesModel Map(AccountFeesDTO model)
        {
            return new AccountFeesModel
            {
                Id = model.Id,
                FeesId = model.FeesId,
                FeesTypeId = model.FeesTypeId,
                FeesTypeName = model.FeesTypeName,
                FeesValue = model.FeesValue,
                PaymentModeId = model.PaymentModeId,
                PaymentMode = model.PaymentMode,
                DenomiinationId = model.DenomiinationId,
                DenominationFullName = model.DenominationFullName
            };
        }
    }
}

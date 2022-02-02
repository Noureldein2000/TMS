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
    public class AccountTypeProfileFeeController : BaseController
    {
        private readonly IAccountTypeProfileFeeService _accountTypeProfileFeeService;
        private readonly IStringLocalizer<LanguageResource> _localizer;
        public AccountTypeProfileFeeController(IAccountTypeProfileFeeService accountTypeProfileFeeService,
            IStringLocalizer<LanguageResource> localizer)
        {
            _accountTypeProfileFeeService = accountTypeProfileFeeService;
            _localizer = localizer;
        }

        [HttpGet]
        [Route("GetAccountTypeProfileFees/{id}")]
        [ProducesResponseType(typeof(PagedResult<AccountTypeProfileFeesModel>), StatusCodes.Status200OK)]
        public IActionResult GetAccountTypeProfileFees(int id, int pageNumber = 1, int pageSize = 10, string language = "ar")
        {
            try
            {
                var result = _accountTypeProfileFeeService.GetAccountTypeProfileFees(id, pageNumber, pageSize, language);
                return Ok(new PagedResult<AccountTypeProfileFeesModel>
                {
                    Results = result.Results.Select(ard => MapToModel(ard)).ToList(),
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
        [Route("AddAccountTypeProfileFee")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        public IActionResult AddAccountTypeProfileFee(AccountTypeProfileFeesModel model)
        {
            try
            {
                _accountTypeProfileFeeService.Add(MapToDTO(model));
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
        [Route("Delete/{id}")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        public IActionResult Delete(int id)
        {
            try
            {
                _accountTypeProfileFeeService.Delete(id);
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

        private AccountTypeProfileFeesModel MapToModel(AccountTypeProfileFeesDTO atpf)
        {
            return new AccountTypeProfileFeesModel
            {
                Id = atpf.Id,
                FeesValue = atpf.FeesValue,
                FeesTypeName = atpf.FeesTypeName,
                PaymentModeName = atpf.PaymentModeName,
                AmountFrom = atpf.AmountFrom,
                AmountTo = atpf.AmountTo,
                DenomintionName = atpf.DenomintionName,
                ServiceName = atpf.ServiceName,
                FeesID = atpf.FeesID,
                AccountTypeProfileDenominationID = atpf.AccountTypeProfileDenominationID
            };
        }
        private AccountTypeProfileFeesDTO MapToDTO(AccountTypeProfileFeesModel atpf)
        {
            return new AccountTypeProfileFeesDTO
            {
                Id = atpf.Id,
                FeesValue = atpf.FeesValue,
                FeesTypeName = atpf.FeesTypeName,
                PaymentModeName = atpf.PaymentModeName,
                AmountFrom = atpf.AmountFrom,
                AmountTo = atpf.AmountTo,
                DenomintionName = atpf.DenomintionName,
                ServiceName = atpf.ServiceName,
                FeesID = atpf.FeesID,
                AccountTypeProfileDenominationID = atpf.AccountTypeProfileDenominationID
            };
        }
    }
}

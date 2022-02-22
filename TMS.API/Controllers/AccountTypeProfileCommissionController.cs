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
    public class AccountTypeProfileCommissionController : BaseController
    {
        private readonly IAccountTypeProfileCommissionService _accountTypeProfileCommissionService;
        private readonly IStringLocalizer<LanguageResource> _localizer;
        public AccountTypeProfileCommissionController(IAccountTypeProfileCommissionService accountTypeProfileCommissionService,
            IStringLocalizer<LanguageResource> localizer)
        {
            _accountTypeProfileCommissionService = accountTypeProfileCommissionService;
            _localizer = localizer;
        }

        [HttpGet]
        [Route("GetAccountTypeProfileCommissions/{id}")]
        [ProducesResponseType(typeof(PagedResult<AccountTypeProfileCommissionModel>), StatusCodes.Status200OK)]
        public IActionResult GetAccountTypeProfileCommissions(int id, int pageNumber = 1, int pageSize = 10, string language = "ar")
        {
            try
            {
                var result = _accountTypeProfileCommissionService.GetAccountTypeProfileCommissions(id, pageNumber, pageSize, language);
                return Ok(new PagedResult<AccountTypeProfileCommissionModel>
                {
                    Results = result.Results.Select(ard => MapToModel(ard)).ToList(),
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
        [Route("AddAccountTypeProfileCommission")]
        [ProducesResponseType(typeof(AccountTypeProfileCommissionModel), StatusCodes.Status200OK)]
        public IActionResult AddAccountTypeProfileCommission(AccountTypeProfileCommissionModel model)
        {
            try
            {
                var result = _accountTypeProfileCommissionService.Add(MapToDTO(model));
                return Ok(MapToModel(result));
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
        [Route("Delete/{id}")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        public IActionResult Delete(int id)
        {
            try
            {
                _accountTypeProfileCommissionService.Delete(id);
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

        private AccountTypeProfileCommissionModel MapToModel(AccountTypeProfileCommissionDTO atpf)
        {
            return new AccountTypeProfileCommissionModel
            {
                Id = atpf.Id,
                CommissionValue = atpf.CommissionValue,
                CommissionTypeName = atpf.CommissionTypeName,
                PaymentModeName = atpf.PaymentModeName,
                AmountFrom = atpf.AmountFrom,
                AmountTo = atpf.AmountTo,
                DenomintionName = atpf.DenomintionName,
                ServiceName = atpf.ServiceName,
                CommissionID = atpf.CommissionID,
                AccountTypeProfileDenominationID = atpf.AccountTypeProfileDenominationID
            };
        }
        private AccountTypeProfileCommissionDTO MapToDTO(AccountTypeProfileCommissionModel atpf)
        {
            return new AccountTypeProfileCommissionDTO
            {
                Id = atpf.Id,
                CommissionValue = atpf.CommissionValue,
                CommissionTypeName = atpf.CommissionTypeName,
                PaymentModeName = atpf.PaymentModeName,
                AmountFrom = atpf.AmountFrom,
                AmountTo = atpf.AmountTo,
                DenomintionName = atpf.DenomintionName,
                ServiceName = atpf.ServiceName,
                CommissionID = atpf.CommissionID,
                AccountTypeProfileDenominationID = atpf.AccountTypeProfileDenominationID
            };
        }
    }
}

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
    public class AccountTypeProfileDenominationController : BaseController
    {
        private readonly IDenominationService _denominationService;
        private readonly IAccountTypeProfileDenominationService _accountTypeProfileDenominationService;
        private readonly IStringLocalizer<LanguageResource> _localizer;
        public AccountTypeProfileDenominationController(IDenominationService denominatioService,
            IAccountTypeProfileDenominationService accountTypeProfileDenominationService,
            IStringLocalizer<LanguageResource> localizer)
        {
            _denominationService = denominatioService;
            _accountTypeProfileDenominationService = accountTypeProfileDenominationService;
            _localizer = localizer;
        }

        [HttpGet]
        [Route("GetAccountTypeProfileDenominations")]
        [ProducesResponseType(typeof(PagedResult<AccountTypeProfileDenominationModel>), StatusCodes.Status200OK)]
        public IActionResult GetAccountTypeProfileDenominations(int pageNumber = 1, int pageSize = 10, string language = "ar")
        {
            try
            {
                var result = _accountTypeProfileDenominationService.GetAccountTypeProfileDenominations(pageNumber, pageSize);
                return Ok(new PagedResult<AccountTypeProfileDenominationModel>
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
        [Route("AddAccountTypeProfileDenominations")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        public IActionResult AddAccountTypeProfileDenominations(AccountTypeProfileDenominationModel model)
        {
            try
            {
                _accountTypeProfileDenominationService.Add(MapToDTO(model));
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
        [Route("ChangeAccountTypeProfileDenominationStatus/{id}")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        public IActionResult ChnageStatus(int id)
        {
            try
            {
                _accountTypeProfileDenominationService.ChangeStatus(id);
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
        [Route("DeleteAccountTypeProfileDenomination/{id}")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        public IActionResult Delete(int id)
        {
            try
            {
                _accountTypeProfileDenominationService.Delete(id);
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

        private AccountTypeProfileDenominationModel MapToModel(AccountTypeProfileDenominationDTO model)
        {
            return new AccountTypeProfileDenominationModel
            {
                Id = model.Id,
                DenominationName = model.DenominationName,
                DenominationID = model.DenominationID,
                AccountTypeProfileID = model.AccountTypeProfileID,
                Status = model.Status
            };
        }
        private AccountTypeProfileDenominationDTO MapToDTO(AccountTypeProfileDenominationModel model)
        {
            return new AccountTypeProfileDenominationDTO
            {
                Id = model.Id,
                DenominationName = model.DenominationName,
                DenominationID = model.DenominationID,
                AccountTypeProfileID = model.AccountTypeProfileID,
                Status = model.Status
            };
        }
    }
}

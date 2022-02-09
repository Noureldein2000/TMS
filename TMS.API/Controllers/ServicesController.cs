using Microsoft.AspNetCore.Authorization;
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
    public class ServicesController : BaseController
    {
        private readonly IDynamicService _service;
        private readonly IStringLocalizer<LanguageResource> _localizer;
        public ServicesController(IDynamicService service,
            IStringLocalizer<LanguageResource> localizer)
        {
            _service = service;
            _localizer = localizer;
        }
        [HttpPost]
        [Route("{serviceId}/inquiry")]
        [ProducesResponseType(typeof(InquiryResponseModel), StatusCodes.Status200OK)]
        public async Task<IActionResult> Inquiry([FromBody] InquiryRequestModel model, int serviceId)
        {
            try
            {
                var response = await _service.Inquiry(new InquiryRequestDTO
                {
                    BillingAccount = model.BillingAccount,
                    ServiceListVersion = model.ServiceListVersion,
                    Version = model.Version,
                    Data = model.Data.Select(d => new DataDTO
                    {
                        Key = d.Key,
                        Value = d.Value
                    }).ToList()
                }, UserIdentityId, serviceId);
                response.Message = _localizer[response.Message].Value;
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
        [HttpPost]
        [Route("{serviceId}/fees")]
        [ProducesResponseType(typeof(FeesResponseDTO), StatusCodes.Status200OK)]
        public async Task<IActionResult> Fees([FromBody] FeesRequestModel model, int serviceId)
        {
            try
            {
                var response = await _service.Fees(new FeesRequestDTO
                {
                    ServiceListVersion = model.ServiceListVersion,
                    Version = model.Version,
                    Amount = model.Amount,
                    Brn = model.Brn,
                    AccountId = model.AccountId,
                    AccountProfileId = model.AccountProfileId,
                    Data = model.Data.Select(d => new DataDTO
                    {
                        Key = d.Key,
                        Value = d.Value
                    }).ToList()
                }, UserIdentityId, serviceId);
                response.Message = _localizer[response.Message].Value;
                return Ok(response);
            }
            catch (TMSException ex)
            {
                return BadRequest(ex.Message, ex.ErrorCode);
            }
            catch (Exception ex)
            {
                return BadRequest("General Error", "-1");
            }
        }
        [HttpPost]
        [Route("{serviceId}/payment")]
        [ProducesResponseType(typeof(PaymentResponseDTO), StatusCodes.Status200OK)]
        public async Task<IActionResult> Payment([FromBody] PaymentRequestModel model, int serviceId)
        {
            try
            {
                var response = await _service.Pay(new PaymentRequestDTO
                {
                    ServiceListVersion = model.ServiceListVersion,
                    Version = model.Version,
                    Amount = model.Amount,
                    Brn = model.Brn,
                    AccountId = 6,
                    AccountProfileId = 7,
                    BillingAccount = model.BillingAccount
                }, UserIdentityId, serviceId);
                response.Message = _localizer[response.Message].Value;
                return Ok(response);
            }
            catch (TMSException ex)
            {
                return BadRequest(ex.Message, ex.ErrorCode);
            }
            catch (Exception ex)
            {
                return BadRequest("General Error", "-1");
            }
        }
        [HttpDelete]
        [Route("{serviceId}/payment")]
        [ProducesResponseType(typeof(PaymentResponseDTO), StatusCodes.Status200OK)]
        public async Task<IActionResult> Cancel([FromBody] CancelRequestModel model, int serviceId)
        {
            try
            {
                var response = await _service.Cancel(model.TransactionId, model.AccountId, UserIdentityId, serviceId);
                return Ok(response);
            }
            catch (TMSException ex)
            {
                return BadRequest(ex.Message, ex.ErrorCode);
            }
            catch (Exception ex)
            {
                return BadRequest("General Error", "-1");
            }
        }
    }
}

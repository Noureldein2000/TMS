using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        public ServicesController(IDynamicService service)
        {
            _service = service;
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
        [Route("{serviceId}/{transactionId}/{accountId}/payment")]
        [ProducesResponseType(typeof(PaymentResponseDTO), StatusCodes.Status200OK)]
        public async Task<IActionResult> Cancel(int serviceId, int transactionId, int accountId)
        {
            try
            {
                var response = await _service.Cancel(transactionId, accountId, UserIdentityId, serviceId);
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

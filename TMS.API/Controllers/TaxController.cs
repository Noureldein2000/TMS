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
    public class TaxController : BaseController
    {
        private readonly ITaxService _taxService;
        private readonly IStringLocalizer<LanguageResource> _localizer;
        public TaxController(ITaxService taxService,
            IStringLocalizer<LanguageResource> localizer)
        {
            _taxService = taxService;
            _localizer = localizer;
        }

        [HttpGet]
        [Route("GetTaxes")]
        [ProducesResponseType(typeof(PagedResult<TaxModel>), StatusCodes.Status200OK)]
        public IActionResult GetTaxes(int pageNumber = 1, int pageSize = 10, string language = "ar")
        {
            try
            {
                var response = _taxService.GetTaxes(pageNumber, pageSize, language);
                return Ok(new PagedResult<TaxModel>
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
        [Route("GetTaxById/{id}")]
        [ProducesResponseType(typeof(TaxModel), StatusCodes.Status200OK)]
        public IActionResult GetTaxById(int id)
        {
            try
            {
                var response = _taxService.GetTaxById(id);
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
                _taxService.ChangeStatus(id);
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
        [Route("DeleteTax/{id}")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        public IActionResult DeleteTax(int id)
        {
            try
            {
                _taxService.DeleteTax(id);
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
        [Route("AddTax")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        public IActionResult AddTax(TaxModel tax)
        {
            try
            {
                _taxService.AddTax(new TaxesDTO
                {
                    TaxesTypeID = tax.TaxesTypeID,
                    Value = tax.Value,
                    PaymentModeID = tax.PaymentModeID,
                    Status = tax.Status,
                    AmountFrom = tax.AmountFrom,
                    AmountTo = tax.AmountTo,
                    StartDate = tax.StartDate,
                    EndDate = tax.EndDate,
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
        [Route("EditTax")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        public IActionResult EditTax(TaxModel tax)
        {
            try
            {
                _taxService.EditTax(new TaxesDTO
                {
                    ID = tax.ID,
                    TaxesTypeID = tax.TaxesTypeID,
                    Value = tax.Value,
                    PaymentModeID = tax.PaymentModeID,
                    Status = tax.Status,
                    AmountFrom = tax.AmountFrom,
                    AmountTo = tax.AmountTo,
                    StartDate = tax.StartDate,
                    EndDate = tax.EndDate,
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

        private TaxModel MapToModel(TaxesDTO tax)
        {
            return new TaxModel
            {
                ID = tax.ID,
                TaxesTypeID = tax.TaxesTypeID,
                TaxesTypeName = tax.TaxesTypeName,
                Value = tax.Value,
                PaymentModeID = tax.PaymentModeID,
                PaymentModeName = tax.PaymentModeName,
                Status = tax.Status,
                CreatedBy = tax.CreatedBy,
                AmountFrom = tax.AmountFrom,
                AmountTo = tax.AmountTo,
                StartDate = tax.StartDate,
                EndDate = tax.EndDate
            };
        }
    }
}

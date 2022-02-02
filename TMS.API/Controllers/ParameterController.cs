using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
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
    public class ParameterController : BaseController
    {
        private readonly IParameterService _parameterService;
        private readonly IStringLocalizer<LanguageResource> _localizer;
        public ParameterController(IParameterService parameterService,
            IStringLocalizer<LanguageResource> localizer)
        {
            _parameterService = parameterService;
            _localizer = localizer;
        }

        [HttpGet]
        [Route("GetParamters")]
        [ProducesResponseType(typeof(PagedResult<ParametersModel>), StatusCodes.Status200OK)]
        public IActionResult GetParamters(int pageNumber = 1, int pageSize = 10, string language = "ar")
        {
            try
            {
                var result = _parameterService.GetParamters(pageNumber, pageSize, language);
                return Ok(new PagedResult<ParametersModel>
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

        [HttpGet]
        [Route("GetParamterById/{id}")]
        [ProducesResponseType(typeof(ParametersModel), StatusCodes.Status200OK)]
        public IActionResult GetParamterById(int id)
        {
            try
            {
                var result = _parameterService.GetParamterById(id);
                return Ok(MapToModel(result));
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
        [Route("AddParameter")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        public IActionResult AddParameter(ParametersModel model)
        {
            try
            {
                _parameterService.AddParameter(new ParameterDTO
                {
                    Name = model.Name,
                    NameAr = model.NameAr,
                    ProviderName = model.ProviderName
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
        [HttpPut]
        [Route("EditParameter")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        public IActionResult EditParameter(ParametersModel model)
        {
            try
            {
                _parameterService.EditParameter(new ParameterDTO
                {
                    Id = model.Id,
                    Name = model.Name,
                    NameAr = model.NameAr,
                    ProviderName = model.ProviderName
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
        [Route("DeleteParameter/{id}")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        public IActionResult DeleteParameter(int id)
        {
            try
            {
                _parameterService.DeleteParameter(id);
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

        private ParametersModel MapToModel(ParameterDTO model)
        {
            return new ParametersModel
            {
                Id = model.Id,
                Name = model.Name,
                NameAr = model.NameAr,
                ProviderName = model.ProviderName
            };
        }

    }
}

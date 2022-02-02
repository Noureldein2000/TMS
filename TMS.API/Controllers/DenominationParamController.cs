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
    public class DenominationParamController : BaseController
    {
        private readonly IDenominationParamService _denominationParamsService;
        private readonly IStringLocalizer<LanguageResource> _localizer;
        public DenominationParamController(IDenominationParamService denominationParamsService,
            IStringLocalizer<LanguageResource> localizer)
        {
            _denominationParamsService = denominationParamsService;
            _localizer = localizer;
        }

        [HttpGet]
        [Route("GetParams")]
        [ProducesResponseType(typeof(PagedResult<DenominationParamModel>), StatusCodes.Status200OK)]
        public IActionResult GetParams(int pageNumber = 1, int pageSize = 10, string language = "ar")
        {
            try
            {
                var response = _denominationParamsService.GetParams(pageNumber, pageSize, language);
                return Ok(new PagedResult<DenominationParamModel>
                {
                    Results = response.Results.Select(ard => MapToModel(ard)).ToList(),
                    PageCount = response.PageCount
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
        [Route("GetParamById/{id}")]
        [ProducesResponseType(typeof(DenominationParamModel), StatusCodes.Status200OK)]
        public IActionResult GetParamById(int id)
        {
            try
            {
                var response = _denominationParamsService.GetParamById(id);
                return Ok(MapToModel(response));
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
        [Route("AddParam")]
        [ProducesResponseType(typeof(DenominationParamModel), StatusCodes.Status200OK)]
        public IActionResult AddParam(DenominationParamModel model)
        {
            try
            {
                var result = _denominationParamsService.AddParam(new DenominationParamDTO()
                {
                    Label = model.Label,
                    Title = model.Title,
                    ParamKey = model.ParamKey,
                    ValueModeID = model.ValueModeID,
                    ValueModeName = model.ValueModeName,
                    ValueTypeID = model.ValueTypeID,
                    ValueTypeName = model.ValueTypeName
                });

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
        [HttpPut]
        [Route("EditParam")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        public IActionResult EditParam(DenominationParamModel model)
        {
            try
            {
                _denominationParamsService.EditParam(new DenominationParamDTO()
                {
                    Id = model.Id,
                    Label = model.Label,
                    Title = model.Title,
                    ParamKey = model.ParamKey,
                    ValueModeID = model.ValueModeID,
                    ValueModeName = model.ValueModeName,
                    ValueTypeID = model.ValueTypeID,
                    ValueTypeName = model.ValueTypeName
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
        [Route("DeleteParam/{id}")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        public IActionResult DeleteParam(int id)
        {
            try
            {
                _denominationParamsService.DeleteParam(id);
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

        //Helper Methods
        private DenominationParamModel MapToModel(DenominationParamDTO model)
        {
            return new DenominationParamModel
            {
                Id = model.Id,
                Label = model.Label,
                Title = model.Title,
                ParamKey = model.ParamKey,
                ValueModeID = model.ValueModeID,
                ValueModeName = model.ValueModeName,
                ValueTypeID = model.ValueTypeID,
                ValueTypeName = model.ValueTypeName,
            };
        }
    }
}

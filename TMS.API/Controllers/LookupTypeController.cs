﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMS.API.Models;
using TMS.Infrastructure;
using TMS.Infrastructure.Helpers;
using TMS.Services.Models;
using TMS.Services.Services;

namespace TMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LookupTypeController : BaseController
    {
        private readonly ILookupTypeService _lookupTypeService;
        private readonly IStringLocalizer<LanguageResource> _localizer;
        public LookupTypeController(ILookupTypeService lookupTypeService,
            IStringLocalizer<LanguageResource> localizer)
        {
            _lookupTypeService = lookupTypeService;
            _localizer = localizer;
        }

        [HttpGet]
        [Route("GetAllLookupTypes")]
        [ProducesResponseType(typeof(LookupTypeModel), StatusCodes.Status200OK)]
        public IActionResult GetAllLookupTypes(string language = "ar")
        {
            try
            {
                var response = _lookupTypeService.GetAllLookups(language);
                return Ok(new LookupTypeModel()
                {
                    GeneralLookups = response.Select(x => MapToModel(x)).ToList()
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

        [HttpPost]
        [Route("AddLookupType")]
        [ProducesResponseType(typeof(GeneralLookupTypeModel), StatusCodes.Status200OK)]
        public IActionResult AddLookupType(GeneralLookupTypeModel model)
        {
            try
            {
                var response = _lookupTypeService.AddLookupType(MapToDto(model));
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

        [HttpDelete]
        [Route("DeleteLookupType/{id}")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        public IActionResult DeleteLookupType(int id, LookupType lookup)
        {
            try
            {
                _lookupTypeService.DeleteLookupType(id, lookup);
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
        [Route("EditLookupType")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        public IActionResult EditLookupType(GeneralLookupTypeModel model)
        {
            try
            {
                _lookupTypeService.EditLookupType(MapToDto(model));
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
        [HttpGet]
        [Route("GetLookupTypeById/{id}")]
        [ProducesResponseType(typeof(GeneralLookupTypeModel), StatusCodes.Status200OK)]
        public IActionResult GetLookupTypeById(int id, LookupType lookup)
        {
            try
            {
                var response = _lookupTypeService.GetLookupTypeById(id, lookup);
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

        private GeneralLookupTypeModel MapToModel(LookupTypeDTO dto)
        {
            return new GeneralLookupTypeModel
            {
                Id = dto.Id,
                Name = dto.Name,
                NameAr = dto.NameAr,
                IdentifierType = dto.IdentifierType
            };
        }

        private LookupTypeDTO MapToDto(GeneralLookupTypeModel model)
        {
            return new LookupTypeDTO
            {
                Id = model.Id,
                Name = model.Name,
                NameAr = model.NameAr,
                IdentifierType = model.IdentifierType
            };
        }

    }
}

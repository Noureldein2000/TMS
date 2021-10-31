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
    public class ServiceConfigurationController : BaseController
    {
        private readonly IServiceConfiguarationService _serviceConfiguaration;
        private readonly IStringLocalizer<LanguageResource> _localizer;
        public ServiceConfigurationController(IServiceConfiguarationService serviceConfiguaration,
            IStringLocalizer<LanguageResource> localizer)
        {
            _serviceConfiguaration = serviceConfiguaration;
            _localizer = localizer;
        }

        [HttpGet]
        [Route("GetServiceConfiguartions")]
        [ProducesResponseType(typeof(PagedResult<ServiceConfigerationModel>), StatusCodes.Status200OK)]
        public IActionResult GetServiceConfiguartions(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var result = _serviceConfiguaration.GetServiceConfiguartions(pageNumber, pageSize);
                return Ok(new PagedResult<ServiceConfigerationModel>
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

        [HttpGet]
        [Route("GetServiceConfiguartionsById/{id}")]
        [ProducesResponseType(typeof(ServiceConfigerationModel), StatusCodes.Status200OK)]
        public IActionResult GetServiceConfiguartionsById(int id)
        {
            try
            {
                var result = _serviceConfiguaration.GetServiceConfiguartionById(id);
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

        [HttpPost]
        [Route("AddServiceConfiguartions")]
        [ProducesResponseType(typeof(ServiceConfigerationModel), StatusCodes.Status200OK)]
        public IActionResult AddServiceConfiguartions(ServiceConfigerationModel model)
        {
            try
            {
                var result = _serviceConfiguaration.AddServiceConfiguartions(MapToDTO(model));
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

        [HttpPut]
        [Route("EditServiceConfiguartions")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        public IActionResult EditServiceConfiguartions(ServiceConfigerationModel model)
        {
            try
            {
                _serviceConfiguaration.EditServiceConfiguartions(MapToDTO(model));
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

        //Helper Methods
        private ServiceConfigerationModel MapToModel(ServiceConfigerationDTO model)
        {
            return new ServiceConfigerationModel
            {
                Id = model.Id,
                URL = model.URL,
                TimeOut = model.TimeOut,
                UserName = model.UserName,
                UserPassword = model.UserPassword,
                ServiceConfigParmsModel = model.ServiceConfigParms?.Select(x => MapToModel(x)).ToList()
            };
        }
        private ServiceConfigerationDTO MapToDTO(ServiceConfigerationModel model)
        {
            return new ServiceConfigerationDTO
            {
                Id = model.Id,
                URL = model.URL,
                TimeOut = model.TimeOut,
                UserName = model.UserName,
                UserPassword = model.UserPassword,
                ServiceConfigParms = model.ServiceConfigParmsModel?.Select(x => MapToDTO(x)).ToList()
            };
        }
        private ServiceConfigParmsDTO MapToDTO(ServiceConfigParmsModel model)
        {
            return new ServiceConfigParmsDTO
            {
                Id = model.Id,
                Name = model.Name,
                Value = model.Value
            };
        }
        private ServiceConfigParmsModel MapToModel(ServiceConfigParmsDTO model)
        {
            return new ServiceConfigParmsModel
            {
                Id = model.Id,
                Name = model.Name,
                Value = model.Value
            };
        }
    }
}

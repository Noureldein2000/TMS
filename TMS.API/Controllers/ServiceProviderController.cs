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
    public class ServiceProviderController : BaseController
    {
        private readonly IServiceProviderService _serviceProvider;
        private readonly IStringLocalizer<LanguageResource> _localizer;
        public ServiceProviderController(IServiceProviderService serviceProvider,
            IStringLocalizer<LanguageResource> localizer)
        {
            _serviceProvider = serviceProvider;
            _localizer = localizer;
        }


        [HttpGet]
        [Route("GetServiceProvider")]
        [ProducesResponseType(typeof(PagedResult<ServiceProviderModel>), StatusCodes.Status200OK)]
        public IActionResult GetServiceProvider(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var result = _serviceProvider.GetServiceProviders(pageNumber, pageSize);
                return Ok(new PagedResult<ServiceProviderModel>
                {
                    Results = result.Results.Select(ard => Map(ard)).ToList(),
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
        [Route("GetServiceProviderById/{id}")]
        [ProducesResponseType(typeof(ServiceProviderModel), StatusCodes.Status200OK)]
        public IActionResult GetServiceProviderById(int id)
        {
            try
            {
                var result = _serviceProvider.GetServiceProviderById(id);
                return Ok(Map(result));
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
        [Route("AddServiceProvider")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        public IActionResult AddServiceProvider(ServiceProviderModel model)
        {
            try
            {
                _serviceProvider.AddServiceProviders(new ServiceProviderDTO { Name = model.Name });
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
        [Route("EditServiceProvider")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        public IActionResult EditServiceProvider(ServiceProviderModel model)
        {
            try
            {
                _serviceProvider.EditServiceProviders(new ServiceProviderDTO { Id = model.Id, Name = model.Name });
                return Ok();
            }
            catch (TMSException ex)
            {
                return BadRequest(_localizer[ex.Message].Value, ex.ErrorCode);
            }
            catch (Exception )
            {
                return BadRequest(_localizer["GeneralError"].Value, "-1");
            }
        }

        [HttpDelete]
        [Route("DeleteServiceProvider/{id}")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        public IActionResult DeleteServiceProvider(int id)
        {
            try
            {
                _serviceProvider.DeleteServiceProviders(id);
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

        private ServiceProviderModel Map(ServiceProviderDTO model)
        {
            return new ServiceProviderModel
            {
                Id = model.Id,
                Name = model.Name,
            };
        }
    }
}

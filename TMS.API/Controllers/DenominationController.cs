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
    public class DenominationController : BaseController
    {
        private readonly IDenominationService _denominationService;
        private readonly IStringLocalizer<LanguageResource> _localizer;
        public DenominationController(IDenominationService denominatioService,
            IStringLocalizer<LanguageResource> localizer)
        {
            _denominationService = denominatioService;
            _localizer = localizer;
        }

        [HttpGet]
        [Route("GetServices")]
        [ProducesResponseType(typeof(List<ServiceModel>), StatusCodes.Status200OK)]
        public IActionResult GetServices()
        {
            try
            {
                var result = _denominationService.GetServices().Select(r => Map(r)).ToList();
                return Ok(result);
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
        [Route("GetDenominationsByServiceId/{serviceId}")]
        [ProducesResponseType(typeof(List<DenominationModel>), StatusCodes.Status200OK)]
        public IActionResult GetDenominationsByServiceId(int serviceId)
        {
            try
            {
                var result = _denominationService.GetDenominationsByServiceId(serviceId).Select(r => Map(r)).ToList();
                return Ok(result);
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



        private ServiceModel Map(ServiceDTO service)
        {
            return new ServiceModel
            {
                Id = service.Id,
                Name = service.Name,
                ArName = service.ArName,
                Code = service.Code,
                PathClass = service.PathClass,
                ServiceEntityID = service.ServiceEntityID,
                ServiceTypeID = service.ServiceTypeID
            };
        }

        private DenominationModel Map(DenominationDTO denomination)
        {
            return new DenominationModel
            {
                Id = denomination.Id,
                Name = denomination.Name,
                ServiceID = denomination.ServiceID,
                Status = denomination.Status,
                ServiceProviderId = denomination.ServiceProviderId,
                PaymentModeID = denomination.PaymentModeID,
                OldDenominationID = denomination.OldDenominationID
            };
        }
    }
}

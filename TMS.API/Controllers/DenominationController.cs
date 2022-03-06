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
        [Route("GetDenominationsByServiceId/{serviceId}")]
        [ProducesResponseType(typeof(PagedResult<DenominationModel>), StatusCodes.Status200OK)]
        public IActionResult GetDenominationsByServiceId(int serviceId, int pageNumber = 1, int pageSize = 10, string language = "ar")
        {
            try
            {
                var result = _denominationService.GetDenominationsByServiceId(serviceId, pageNumber, pageSize, language);
                return Ok(new PagedResult<DenominationModel>
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
        [Route("AddDenomination")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        public IActionResult AddDenomination(AddDenominationModel model)
        {
            try
            {
                _denominationService.AddDenomination(MapToDTO(model));
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

        [HttpGet]
        [Route("GetDenominationById/{id}")]
        [ProducesResponseType(typeof(EditDenominationModel), StatusCodes.Status200OK)]
        public IActionResult GetDenominationById(int id)
        {
            try
            {
                var model = _denominationService.GetDenominationById(id);
                return Ok(new EditDenominationModel
                {
                    Denomination = MapToModel(model.Denomination),
                    DenominationServiceProviders = model.DenominationServiceProvidersDto.Select(x => MapToModel(x)).ToList(),
                    DenominationParameters = model.DenominationParameterDTOs.Select(x => MapToModel(x)).ToList(),
                    DenominationReceipt = new DenominationReceiptModel
                    {
                        DenominationReceiptData = MapToModel(model.DenominationRecepitDTO.DenominationReceiptDataDTO),
                        DenominationReceiptParams = model.DenominationRecepitDTO.DenominationReceiptParamDTOs.Select(x => MapToModel(x)).ToList()
                    }
                });
            }
            catch (TMSException ex)
            {
                return BadRequest(_localizer[ex.Message].Value, ex.ErrorCode);
            }
            catch (Exception)
            {
                return BadRequest(_localizer["GeneralError"].Value, "-1");
            }
        }
        [HttpPut]
        [Route("EditDenomination")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        public IActionResult EditDenomination(DenominationModel model)
        {
            try
            {
                _denominationService.EditDenomination(MapToDTO(model));

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
        [Route("ChangeStatus")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        public IActionResult ChangeStatus(int id)
        {
            try
            {
                _denominationService.ChangeStatus(id);
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
        [Route("ChangeDenominationServiceProviderStatus")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        public IActionResult ChangeDenominationServiceProviderStatus(int id)
        {
            try
            {
                _denominationService.ChangeDenominationServiceProviderStatus(id);
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
        [HttpGet]
        [Route("GetDenominationServiceProviderByDenominationId/{id}")]
        [ProducesResponseType(typeof(DenominationServiceProvidersModel), StatusCodes.Status200OK)]
        public IActionResult GetDenominationServiceProvider(int id)
        {
            try
            {
                var result = _denominationService.GetDenominationServiceProviderById(id);
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
        [Route("EditDenominationServiceProvider")]
        [ProducesResponseType(typeof(DenominationServiceProvidersModel), StatusCodes.Status200OK)]
        public IActionResult EditDenominationServiceProvider(DenominationServiceProvidersModel model)
        {
            try
            {
                var result = _denominationService.EditDenominationServiceProvdier(MapToDTO(model));
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
        [Route("AddDenominationServiceProvdier")]
        [ProducesResponseType(typeof(DenominationServiceProvidersModel), StatusCodes.Status200OK)]
        public IActionResult AddDenominationServiceProvdier(DenominationServiceProvidersModel model)
        {
            try
            {
                var result = _denominationService.AddDenominationServiceProvdier(MapToDTO(model));
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
        [HttpGet]
        [Route("GetDenominationParameterById/{id}")]
        [ProducesResponseType(typeof(DenominationParameterModel), StatusCodes.Status200OK)]
        public IActionResult GetDenominationParameterById(int id)
        {
            try
            {
                var result = _denominationService.GetDenominationParameterById(id);
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
        [Route("EditDenominationParameter")]
        [ProducesResponseType(typeof(DenominationParameterModel), StatusCodes.Status200OK)]
        public IActionResult EditDenominationParameter(DenominationParameterModel model)
        {
            try
            {
                var result = _denominationService.EditDenominationParameter(MapToDTO(model));
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
        [Route("AddDenominationParameter")]
        [ProducesResponseType(typeof(DenominationParameterModel), StatusCodes.Status200OK)]
        public IActionResult AddDenominationParameter(DenominationParameterModel model)
        {
            try
            {
                var result = _denominationService.AddDenominationParameter(MapToDTO(model));
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
        [HttpDelete]
        [Route("DeleteDenominationParameter/{id}")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        public IActionResult DeleteDenominationParameter(int id)
        {
            try
            {
                _denominationService.DeleteDenominationParameter(id);
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
        [Route("EditDenominationRecepitData")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        public IActionResult EditDenominationRecepitData(DenominationReceiptDataModel model)
        {
            try
            {
                _denominationService.EditDenominationReceiptData(MapToDTO(model));
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
        [Route("EditDenominationReceiptParam")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        public IActionResult EditDenominationReceiptParam(DenominationReceiptParamModel model)
        {
            try
            {
                _denominationService.EditDenominationReceiptParam(MapToDTO(model));
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
        [Route("ChangeDenominationReceiptParamStatus")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        public IActionResult ChangeDenominationReceiptParamStatus(int id)
        {
            try
            {
                _denominationService.ChangeDenominationReceiptParamStatus(id);
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
        [HttpGet]
        [Route("GetDenominationReceiptParamById/{id}")]
        [ProducesResponseType(typeof(DenominationReceiptParamModel), StatusCodes.Status200OK)]
        public IActionResult GetDenominationReceiptParamById(int id)
        {
            try
            {
                var model = _denominationService.GetDenominationReceiptParamById(id);
                return Ok(MapToModel(model));
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
        [Route("DeleteDenominationReceiptParam/{id}")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        public IActionResult DeleteDenominationReceiptParam(int id)
        {
            try
            {
                _denominationService.DeleteDenominationReceiptParam(id);
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
        [Route("EditDenominationReceipt")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        public IActionResult EditDenominationReceipt(DenominationReceiptModel model)
        {
            try
            {
                _denominationService.EditDenominationReceipt(new DenominationReceiptDTO
                {
                    DenominationReceiptDataDTO = MapToDTO(model.DenominationReceiptData),
                    DenominationReceiptParamDTOs = model.DenominationReceiptParams.Select(x => MapToDTO(x)).ToList(),
                });
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
        [HttpGet]
        [Route("SearchDenominations")]
        [ProducesResponseType(typeof(PagedResult<DenominationModel>), StatusCodes.Status200OK)]
        public IActionResult SearchDenominations(string serviceName, string serviceCode, string denomninationName, string denomniationCode, int pageNumber = 1, int pageSize = 10, string language = "ar")
        {
            try
            {
                var result = _denominationService.SearchDenominations(serviceName, serviceCode, denomninationName, denomniationCode, pageNumber, pageSize, language);
                return Ok(new PagedResult<DenominationModel>
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
        [Route("GetDenominationServiceProvidersByDenominationId/{denominationId}")]
        [ProducesResponseType(typeof(List<DenominationServiceProvidersModel>), StatusCodes.Status200OK)]
        public IActionResult GetDenominationServiceProvidersByDenominationId(int denominationId)
        {
            try
            {
                var result = _denominationService.GetDenominationServiceProvidersByDenominationId(denominationId).Select(ard => MapToModel(ard));
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

        //Helper Methods
        private AddDenominationDTO MapToDTO(AddDenominationModel model)
        {
            return new AddDenominationDTO
            {
                Denomination = MapToDTO(model.Denomination),
                DenominationServiceProvidersDto = MapToDTO(model.DenominationServiceProviders),
                ServiceConfigerationDto = MapToDTO(model.ServiceConfigeration),
                DenominationParameter = MapToDTO(model.DenominationParameter),
                DenominationReceiptData = MapToDTO(model.DenominationReceiptData),
                DenominationReceiptParams = model.DenominationReceiptParams.Select(x => MapToDTO(x)).ToList()
            };
        }
        private DenominationModel MapToModel(DenominationDTO denomination)
        {
            return new DenominationModel
            {
                Id = denomination.Id,
                Name = denomination.Name,
                ServiceID = denomination.ServiceID,
                ServiceName = denomination.ServiceName,
                Status = denomination.Status,
                ServiceProviderId = denomination.ServiceProviderId,
                PaymentModeID = denomination.PaymentModeID,
                PaymentModeName = denomination.PaymentModeName,
                OldDenominationID = denomination.OldDenominationID,
                ServiceCategoryID = denomination.ServiceCategoryID,
                ServiceEntity = denomination.ServiceEntity,
                Value = denomination.Value,
                APIValue = denomination.APIValue,
                BillPaymentModeID = denomination.BillPaymentModeID,
                BillPaymentModeName = denomination.BillPaymentModeName,
                ClassType = denomination.ClassType,
                CurrencyID = denomination.CurrencyID,
                Inquirable = denomination.Inquirable,
                Interval = denomination.Interval,
                MinValue = denomination.MinValue,
                MaxValue = denomination.MaxValue,
                PathClass = denomination.PathClass
            };
        }
        private ServiceConfigerationModel MapToModel(ServiceConfigerationDTO model)
        {
            return new ServiceConfigerationModel
            {
                Id = model.Id,
                URL = model.URL,
                TimeOut = model.TimeOut,
                UserName = model.UserName,
                UserPassword = model.UserPassword
            };
        }
        private DenominationServiceProvidersModel MapToModel(DenominationServiceProviderDTO model)
        {
            return new DenominationServiceProvidersModel
            {
                Id = model.Id,
                Balance = model.Balance,
                ProviderAmount = model.ProviderAmount,
                ProviderCode = model.ProviderCode,
                ProviderHasFees = model.ProviderHasFees,
                OldServiceId = (int)model.OldServiceId,
                ServiceProviderId = model.ServiceProviderId,
                ServiceProviderName = model.ServiceProviderName,
                ServiceConfigerationId = model.ServiceConfigerationId,
                Status = model.Status,
                DenominationId = model.DenominationId,
                DenominationProviderConfigurationModel = model.DenominationProviderConfigurations?.Select(x => MapToModel(x)).ToList()
            };
        }
        private DenominationDTO MapToDTO(DenominationModel model)
        {
            return new DenominationDTO
            {
                Id = model.Id,
                Name = model.Name,
                APIValue = model.APIValue,
                CurrencyID = (int)model.CurrencyID,
                ServiceID = model.ServiceID,
                Status = model.Status,
                ClassType = model.ClassType,
                Interval = model.Interval,
                MaxValue = model.MaxValue,
                MinValue = model.MinValue,
                Inquirable = model.Inquirable,
                Value = model.Value,
                BillPaymentModeID = model.BillPaymentModeID,
                PaymentModeID = model.PaymentModeID,
                OldDenominationID = model.OldDenominationID,
                PathClass = model.PathClass,
                ServiceCategoryID = model.ServiceCategoryID
            };
        }
        private DenominationServiceProviderDTO MapToDTO(DenominationServiceProvidersModel model)
        {
            return new DenominationServiceProviderDTO
            {
                Id = model.Id,
                DenominationId = model.DenominationId,
                Balance = model.Balance,
                ProviderAmount = model.ProviderAmount,
                ProviderCode = model.ProviderCode,
                ProviderHasFees = model.ProviderHasFees,
                OldServiceId = (int)model.OldServiceId,
                ServiceProviderId = model.ServiceProviderId,
                ServiceProviderName = model.ServiceProviderName,
                Status = model.Status,
                ServiceConfigerationId = model.ServiceConfigerationId,
                DenominationProviderConfigurations = model.DenominationProviderConfigurationModel?.Select(x => MapToDTO(x)).ToList()
            };
        }
        private DenominationProviderConfigurationDTO MapToDTO(DenominationProviderConfigerationModel model)
        {
            return new DenominationProviderConfigurationDTO
            {
                ID = model.ID,
                Name = model.Name,
                Value = model.Value,
                DenominationProviderID = model.DenominationProviderID
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
        private DenominationProviderConfigerationModel MapToModel(DenominationProviderConfigurationDTO model)
        {
            return new DenominationProviderConfigerationModel
            {
                ID = model.ID,
                Name = model.Name,
                Value = model.Value,
                DenominationProviderID = model.DenominationProviderID
            };
        }
        private DenominationParameterDTO MapToDTO(DenominationParameterModel model)
        {
            return new DenominationParameterDTO
            {
                Id = model.Id,
                DenominationID = model.DenominationID,
                Optional = model.Optional,
                Sequence = model.Sequence,
                ValidationExpression = model.ValidationExpression,
                ValidationMessage = model.ValidationMessage,
                DenominationParamID = model.DenominationParamID,
                Value = model.Value,
                ValueList = model.ValueList
            };
        }
        private DenominationParameterModel MapToModel(DenominationParameterDTO model)
        {
            return new DenominationParameterModel
            {
                Id = model.Id,
                DenominationID = model.DenominationID,
                Optional = model.Optional,
                Sequence = model.Sequence,
                ValidationExpression = model.ValidationExpression,
                ValidationMessage = model.ValidationMessage,
                ValidationMessageAr = model.ValidationMessageAr,
                DenominationParamID = model.DenominationParamID,
                Value = model.Value,
                ValueList = model.ValueList
            };
        }
        private DenominationReceiptDataDTO MapToDTO(DenominationReceiptDataModel model)
        {
            return new DenominationReceiptDataDTO
            {
                Id = model.Id,
                DenominationID = model.DenominationID,
                Title = model.Title,
                Disclaimer = model.Disclaimer,
                Footer = model.Footer
            };
        }
        private DenominationReceiptDataModel MapToModel(DenominationReceiptDataDTO model)
        {
            return new DenominationReceiptDataModel
            {
                Id = model.Id,
                DenominationID = model.DenominationID,
                Title = model.Title,
                Disclaimer = model.Disclaimer,
                Footer = model.Footer
            };
        }
        private DenominationReceiptParamDTO MapToDTO(DenominationReceiptParamModel model)
        {
            return new DenominationReceiptParamDTO
            {
                Id = model.Id,
                DenominationID = model.DenominationID,
                ParameterID = model.ParameterID,
                Bold = model.Bold,
                Alignment = model.Alignment,
                Status = model.Status,
                FontSize = model.FontSize
            };
        }
        private DenominationReceiptParamModel MapToModel(DenominationReceiptParamDTO model)
        {
            return new DenominationReceiptParamModel
            {
                Id = model.Id,
                DenominationID = model.DenominationID,
                ParameterID = model.ParameterID,
                ParameterName = model.ParameterName,
                Bold = model.Bold,
                Alignment = model.Alignment,
                Status = model.Status,
                FontSize = model.FontSize,
                DenominationReceiptDataID = model.DenominationReceiptDataID
            };
        }
    }
}

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
    public class AdminServiceController : BaseController
    {
        private readonly IAdminService _adminService;
        private readonly IStringLocalizer<LanguageResource> _localizer;
        public AdminServiceController(IAdminService adminService,
            IStringLocalizer<LanguageResource> localizer)
        {
            _adminService = adminService;
            _localizer = localizer;
        }

        [HttpGet]
        [Route("GetServices")]
        [ProducesResponseType(typeof(PagedResult<AdminServiceModel>), StatusCodes.Status200OK)]
        public IActionResult GetServices(int pageNumber = 1, int pageSize = 10, string language = "ar")
        {
            try
            {
                var result = _adminService.GetServices(pageNumber, pageSize, language);
                return Ok(new PagedResult<AdminServiceModel>
                {
                    Results = result.Results.Select(ard => Map(ard)).ToList(),
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
        [Route("SearchServices")]
        [ProducesResponseType(typeof(PagedResult<AdminServiceModel>), StatusCodes.Status200OK)]
        public IActionResult SearchServices(int? dropDownFilter, string searchKey, int pageNumber = 1, int pageSize = 10, string language = "ar")
        {
            try
            {
                var result = _adminService.SearchServices(dropDownFilter, searchKey, pageNumber, pageSize, language);
                return Ok(new PagedResult<AdminServiceModel>
                {
                    Results = result.Results.Select(ard => Map(ard)).ToList(),
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
        [Route("AddService")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        public IActionResult AddService(AddServiceModel model)
        {
            try
            {
                _adminService.AddService(new AdminServiceDTO
                {
                    Name = model.Name,
                    ArName = model.ArName,
                    ServiceTypeID = model.ServiceTypeID,
                    ServiceCategoryID = model.ServiceCategoryID,
                    Status = model.Status,
                    Code = model.Code,
                    ServiceEntityID = model.ServiceEntityID,
                    PathClass = model.PathClass,
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
        [Route("GetServiceById/{id}")]
        [ProducesResponseType(typeof(AdminServiceModel), StatusCodes.Status200OK)]
        public IActionResult GetServiceById(int id)
        {
            try
            {
                var result = _adminService.GetServiceById(id);
                return Ok(Map(result));
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
        [Route("EditService")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        public IActionResult EditService(EditServiceModel model)
        {
            try
            {
                _adminService.EditService(new AdminServiceDTO
                {
                    Id = model.Id,
                    Name = model.Name,
                    ArName = model.ArName,
                    ServiceTypeID = model.ServiceTypeID,
                    ServiceCategoryID = model.ServiceCategoryID,
                    Status = model.Status,
                    Code = model.Code,
                    ServiceEntityID = model.ServiceEntityID,
                    ClassType = model.ClassType,
                    PathClass = model.PathClass,
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

        [HttpPut]
        [Route("ChangeStatus")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        public IActionResult ChangeStatus(int id)
        {
            try
            {
                _adminService.ChangeStatus(id);
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
        [Route("GetServicesCategories")]
        [ProducesResponseType(typeof(List<ServiceCategoryModel>), StatusCodes.Status200OK)]
        public IActionResult GetServicesCategories()
        {
            try
            {
                var result = _adminService.GetServiceCategories().Select(r => Map(r)).ToList();
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
        [Route("GetServiceEntities")]
        [ProducesResponseType(typeof(List<ServiceEntityModel>), StatusCodes.Status200OK)]
        public IActionResult GetServiceEntities()
        {
            try
            {
                var result = _adminService.GetServiceEntities().Select(r => Map(r)).ToList();
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
        [Route("GetServiceTypes")]
        [ProducesResponseType(typeof(List<ServiceTypesModel>), StatusCodes.Status200OK)]
        public IActionResult GetServiceTypes()
        {
            try
            {
                var result = _adminService.GetServiceTypes().Select(r => Map(r)).ToList();
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


        private AdminServiceModel Map(AdminServiceDTO model)
        {
            return new AdminServiceModel
            {
                Id = model.Id,
                Name = model.Name,
                ArName = model.ArName,
                ServiceTypeID = model.ServiceTypeID,
                ServiceTypeName = model.ServiceTypeName,
                ServiceCategoryID = model.ServiceCategoryID,
                ServiceCategoryName = model.ServiceCategoryName,
                Status = model.Status,
                Code = model.Code,
                ServiceEntityID = model.ServiceEntityID,
                ServiceEntityName = model.ServiceEntityName,
                PathClass = model.PathClass,
                ClassType = model.ClassType,
                CreationDate = model.CreationDate
            };
        }

        private ServiceCategoryModel Map(ServiceCategoryDTO model)
        {
            return new ServiceCategoryModel
            {
                Id = model.Id,
                Name = model.Name,
                ArName = model.ArName
            };
        }

        private ServiceEntityModel Map(ServiceEntityDTO model)
        {
            return new ServiceEntityModel
            {
                Id = model.Id,
                Name = model.Name,
                ArName = model.ArName
            };
        }

        private ServiceTypesModel Map(ServiceTypesDTO model)
        {
            return new ServiceTypesModel
            {
                Id = model.Id,
                Name = model.Name,
            };
        }
    }
}

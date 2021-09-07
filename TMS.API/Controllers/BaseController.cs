using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMS.API.Models;

namespace TMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        [NonAction]
        public IActionResult BadRequest(string errorMessage, string errorCode)
        {
            var response = new TMSErrorMessages
            {
                Code = errorCode,
                Message = errorMessage
            };
            return BadRequest(response);
        }
        [NonAction]
        public IActionResult Ok(string errorCode, string errorMessage)
        {
            var response = new TMSErrorMessages
            {
                Code = errorCode,
                Message = errorMessage
            };
            return Ok(response);
        }
        public int UserIdentityId
        {
            get
            {
                return int.Parse(User.Identity.Name ?? "1");
            }
        }
        public int AccountId
        {
            get
            {
                return int.Parse(User.Claims.Where(s => s.Type == "AccountId").Select(s => s.Value).FirstOrDefault() ?? "6");
            }
        }
        public int AccountProfileId
        {
            get
            {
                return int.Parse(User.Claims.Where(s => s.Type == "AccountProfileId").Select(s => s.Value).FirstOrDefault() ?? "6");
            }
        }
    }
}

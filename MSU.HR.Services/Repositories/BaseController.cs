using Microsoft.AspNetCore.Mvc;
using MSU.HR.Models.Others;

namespace MSU.HR.Services.Repositories
{
    public class BaseController : ControllerBase
    {
        protected ActionResult BadRequest(string message)
        {
            ErrorModel errorModel = new ErrorModel
            {
                Message = message
            };
            return new BadRequestObjectResult(errorModel);
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MSU.HR.Models.Entities;
using MSU.HR.Models.Others;
using MSU.HR.Models.Requests;
using MSU.HR.Models.Responses;
using MSU.HR.Services.Interfaces;

namespace MSU.HR.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RoleController : ControllerBase
    {
        private readonly ILogger<RoleController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IRole _role;
        public RoleController(ILogger<RoleController> logger, IConfiguration configuration, IRole role)
        {
            _role = role;
            _logger = logger;
            _configuration = configuration;
            //userIdentity = new UserIdentityModel(User.Identity as ClaimsIdentity);
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            _logger.LogInformation("Get Data");
            var response = await _role.GetRolesAsync();

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest(id);

            var response = await _role.GetRoleAsync(id);

            if (response == null)
                return BadRequest("Not found : " + id);

            return Ok(response);
        }

        [HttpGet("Dropdown")]
        public async Task<IActionResult> GetDropdown()
        {
            _logger.LogInformation("Get Data");
            var response = await _role.GetDropdownModelAsync();

            return Ok(response);
        }

        [HttpGet("Pagination")]
        public async Task<IActionResult> GetPagination(int pageNumber, int pageSize, string? search)
        {
            search = search ?? string.Empty;

            var response = await _role.GetRolesAsync(search, new PaginationModel()
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            });

            return Ok(response);
        }

        //[HttpPost]
        //public async Task<ActionResult<PostResponse>> Post([FromBody] RoleRequest request)
        //{
        //    if (!ModelState.IsValid)
        //        return BadRequest(ModelState);

        //    var codeExists = await _role.CheckCodeExistsAsync(request.Code);
        //    if (codeExists)
        //    {
        //        ModelState.AddModelError("Code", "Code already exists");
        //        return BadRequest(ModelState);
        //    }

        //    Role Role = new()
        //    {
        //        Name = request.Name,
        //        Code = request.Code,
        //        Description = request.Description
        //    };

        //    var task = await _role.CreateAsync(Role);
        //    if (task == 0)
        //    {
        //        ModelState.AddModelError("Error", "Invalid Request");
        //        return BadRequest(ModelState);
        //    }
        //    else
        //        return Ok(new PostResponse
        //        {
        //            Data = request,
        //            IsSuccess = task > 0,
        //            Message = ""
        //        });
        //}


        //[HttpPut("{id}")]
        //public async Task<ActionResult<PostResponse>> Put(Guid id, [FromBody] RoleRequest request)
        //{
        //    if (!ModelState.IsValid && id == Guid.Empty)
        //        return BadRequest(ModelState);

        //    Role Role = new()
        //    {
        //        Name = request.Name,
        //        Code = request.Code,
        //        Description= request.Description
        //    };

        //    var task = await _role.UpdateAsync(id, Role);
        //    if (task == 0)
        //    {
        //        ModelState.AddModelError("Id", "Data not found");
        //        return BadRequest(ModelState);
        //    }
        //    else
        //        return Ok(new PostResponse
        //        {
        //            Data = request,
        //            IsSuccess = task > 0,
        //            Message = ""
        //        });
        //}

        //[HttpDelete("{id}")]
        //public async Task<IActionResult> Delete(Guid id)
        //{
        //    if (id == Guid.Empty)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    _logger.LogInformation("Get Data");
        //    var task = await _role.DeleteAsync(id);

        //    if (task == 0)
        //    {
        //        ModelState.AddModelError("Id", "Data not found");
        //        return BadRequest(ModelState);
        //    }
        //    else
        //    {
        //        return Ok(new PostResponse
        //        {
        //            Data = task,
        //            IsSuccess = task > 0,
        //            Message = ""
        //        });
        //    }
        //}
    }
}

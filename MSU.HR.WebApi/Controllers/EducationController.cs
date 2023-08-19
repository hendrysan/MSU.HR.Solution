using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MSU.HR.Models.Others;
using MSU.HR.Services.Interfaces;

namespace MSU.HR.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EducationController : ControllerBase
    {
        private readonly ILogger<EducationController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IEducation _education;
        public EducationController(ILogger<EducationController> logger, IConfiguration configuration, IEducation education)
        {
            _education = education;
            _logger = logger;
            _configuration = configuration;
            //userIdentity = new UserIdentityModel(User.Identity as ClaimsIdentity);
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            _logger.LogInformation("Get Data");
            var response = await _education.GetEducationsAsync();

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest(id);

            var response = await _education.GetEducationAsync(id);

            if (response == null)
                return BadRequest("Not found : " + id);

            return Ok(response);
        }

        [HttpGet("Dropdown")]
        public async Task<IActionResult> GetDropdown()
        {
            _logger.LogInformation("Get Data");
            var response = await _education.GetDropdownModelAsync();

            return Ok(response);
        }

        [HttpGet("Pagination")]
        public async Task<IActionResult> GetPagination(int pageNumber, int pageSize, string? search)
        {
            search = search ?? string.Empty;

            var response = await _education.GetEducationsAsync(search, new PaginationModel()
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            });

            return Ok(response);
        }

        /*
        [HttpPost]
        public async Task<ActionResult<PostResponse>> Post([FromBody] EducationRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var codeExists = await _education.CheckCodeExistsAsync(request.Code);
            if (codeExists)
            {
                ModelState.AddModelError("Code", "Code already exists");
                return BadRequest(ModelState);
            }

            Education Education = new()
            {
                Name = request.Name,
                Code = request.Code
            };

            var task = await _education.CreateAsync(Education);
            if (task == 0)
            {
                ModelState.AddModelError("Error", "Invalid Request");
                return BadRequest(ModelState);
            }
            else
                return Ok(new PostResponse
                {
                    Data = request,
                    IsSuccess = task > 0,
                    Message = ""
                });
        }


        [HttpPut("{id}")]
        public async Task<ActionResult<PostResponse>> Put(Guid id, [FromBody] EducationRequest request)
        {
            if (!ModelState.IsValid && id == Guid.Empty)
                return BadRequest(ModelState);

            Education Education = new()
            {
                Name = request.Name,
                Code = request.Code
            };

            var task = await _education.UpdateAsync(id, Education);
            if (task == 0)
            {
                ModelState.AddModelError("Id", "Data not found");
                return BadRequest(ModelState);
            }
            else
                return Ok(new PostResponse
                {
                    Data = request,
                    IsSuccess = task > 0,
                    Message = ""
                });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Get Data");
            var task = await _education.DeleteAsync(id);

            if (task == 0)
            {
                ModelState.AddModelError("Id", "Data not found");
                return BadRequest(ModelState);
            }
            else
            {
                return Ok(new PostResponse
                {
                    Data = task,
                    IsSuccess = task > 0,
                    Message = ""
                });
            }
        }
   
    */
    }
}

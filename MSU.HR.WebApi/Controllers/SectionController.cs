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
    public class SectionController : ControllerBase
    {
        private readonly ILogger<SectionController> _logger;
        private readonly IConfiguration _configuration;
        private readonly ISection _section;
        public SectionController(ILogger<SectionController> logger, IConfiguration configuration, ISection section)
        {
            _section = section;
            _logger = logger;
            _configuration = configuration;
            //userIdentity = new UserIdentityModel(User.Identity as ClaimsIdentity);
        }

        [HttpGet("{departmentId}")]
        public async Task<IActionResult> Get(Guid departmentId)
        {
            _logger.LogInformation("Get Data");
            var response = await _section.GetSectionsAsync(departmentId);

            return Ok(response);
        }

        [HttpGet("{departmentId}/{id}")]
        public async Task<IActionResult> Find(Guid departmentId, Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest(id);

            var response = await _section.GetSectionAsync(id);

            if (response == null)
                return BadRequest("Not found : " + id);

            return Ok(response);
        }

        [HttpGet("Dropdown/{departmentId}")]
        public async Task<IActionResult> GetDropdown(Guid departmentId)
        {
            _logger.LogInformation("Get Data");
            var response = await _section.GetDropdownModelAsync(departmentId);

            return Ok(response);
        }

        [HttpGet("Pagination/{departmentId}")]
        public async Task<IActionResult> GetPagination(Guid departmentId, int pageNumber, int pageSize, string? search)
        {
            search = search ?? string.Empty;

            var response = await _section.GetSectionsAsync(departmentId, search, new PaginationModel()
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            });

            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult<PostResponse>> Post([FromBody] SectionRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var codeExists = await _section.CheckCodeExistsAsync(request.Code);
            if (codeExists)
            {
                ModelState.AddModelError("Code", "Code already exists");
                return BadRequest(ModelState);
            }

            var task = await _section.CreateAsync(request);
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
        public async Task<ActionResult<PostResponse>> Put(Guid id, [FromBody] SectionRequest request)
        {
            if (!ModelState.IsValid && id == Guid.Empty)
                return BadRequest(ModelState);

            var task = await _section.UpdateAsync(id, request);
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
            var task = await _section.DeleteAsync(id);

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
    }
}

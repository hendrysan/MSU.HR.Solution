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

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            _logger.LogInformation("Get Data");
            var response = await _section.GetSectionsAsync();

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest(id);

            var response = await _section.GetSectionAsync(id);

            if (response == null)
                return BadRequest("Not found : " + id);

            return Ok(response);
        }

        [HttpGet("Dropdown")]
        public async Task<IActionResult> GetDropdown()
        {
            _logger.LogInformation("Get Data");
            var response = await _section.GetDropdownModelAsync();

            return Ok(response);
        }

        [HttpGet("Pagination")]
        public async Task<IActionResult> GetPagination(int pageNumber, int pageSize, string? search)
        {
            search = search ?? string.Empty;

            var response = await _section.GetSectionsAsync(search, new PaginationModel()
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

            Section Section = new()
            {
                Name = request.Name,
                Code = request.Code
            };

            var task = await _section.CreateAsync(Section);
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

            Section Section = new()
            {
                Name = request.Name,
                Code = request.Code
            };

            var task = await _section.UpdateAsync(id, Section);
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

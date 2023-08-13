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
    public class PTKPController : ControllerBase
    {
        private readonly ILogger<PTKPController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IPTKP _PTKP;
        public PTKPController(ILogger<PTKPController> logger, IConfiguration configuration, IPTKP PTKP)
        {
            _PTKP = PTKP;
            _logger = logger;
            _configuration = configuration;
            //userIdentity = new UserIdentityModel(User.Identity as ClaimsIdentity);
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            _logger.LogInformation("Get Data");
            var response = await _PTKP.GetPTKPsAsync();

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest(id);

            var response = await _PTKP.GetPTKPAsync(id);

            if (response == null)
                return BadRequest("Not found : " + id);

            return Ok(response);
        }

        [HttpGet("Dropdown")]
        public async Task<IActionResult> GetDropdown()
        {
            _logger.LogInformation("Get Data");
            var response = await _PTKP.GetDropdownModelAsync();

            return Ok(response);
        }

        [HttpGet("Pagination")]
        public async Task<IActionResult> GetPagination(int pageNumber, int pageSize, string? search)
        {
            search = search ?? string.Empty;

            var response = await _PTKP.GetPTKPsAsync(search, new PaginationModel()
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            });

            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult<PostResponse>> Post([FromBody] PTKPRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var codeExists = await _PTKP.CheckCodeExistsAsync(request.Code);
            if (codeExists)
            {
                ModelState.AddModelError("Code", "Code already exists");
                return BadRequest(ModelState);
            }

            PTKP PTKP = new()
            {
                Name = request.Name,
                Code = request.Code,
                Description = request.Description,
                Amount = request.Amount,
            };

            var task = await _PTKP.CreateAsync(PTKP);
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
        public async Task<ActionResult<PostResponse>> Put(Guid id, [FromBody] PTKPRequest request)
        {
            if (!ModelState.IsValid && id == Guid.Empty)
                return BadRequest(ModelState);

            PTKP PTKP = new()
            {
                Name = request.Name,
                Code = request.Code,
                Description = request.Description,
                Amount = request.Amount,
            };

            var task = await _PTKP.UpdateAsync(id, PTKP);
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
            var task = await _PTKP.DeleteAsync(id);

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

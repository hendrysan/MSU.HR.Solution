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
    public class JobController : ControllerBase
    {
        private readonly ILogger<JobController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IJob _job;
        public JobController(ILogger<JobController> logger, IConfiguration configuration, IJob job)
        {
            _job = job;
            _logger = logger;
            _configuration = configuration;
            //userIdentity = new UserIdentityModel(User.Identity as ClaimsIdentity);
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            _logger.LogInformation("Get Data");
            var response = await _job.GetJobsAsync();

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest(id);

            var response = await _job.GetJobAsync(id);

            if (response == null)
                return BadRequest("Not found : " + id);

            return Ok(response);
        }

        [HttpGet("Dropdown")]
        public async Task<IActionResult> GetDropdown()
        {
            _logger.LogInformation("Get Data");
            var response = await _job.GetDropdownModelAsync();

            return Ok(response);
        }

        [HttpGet("Pagination")]
        public async Task<IActionResult> GetPagination(int pageNumber, int pageSize, string? search)
        {
            search = search ?? string.Empty;

            var response = await _job.GetJobsAsync(search, new PaginationModel()
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            });

            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult<PostResponse>> Post([FromBody] JobRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var codeExists = await _job.CheckCodeExistsAsync(request.Code);
            if (codeExists)
            {
                ModelState.AddModelError("Code", "Code already exists");
                return BadRequest(ModelState);
            }

            Job Job = new()
            {
                Name = request.Name,
                Code = request.Code
            };

            var task = await _job.CreateAsync(Job);
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
        public async Task<ActionResult<PostResponse>> Put(Guid id, [FromBody] JobRequest request)
        {
            if (!ModelState.IsValid && id == Guid.Empty)
                return BadRequest(ModelState);

            Job Job = new()
            {
                Name = request.Name,
                Code = request.Code
            };

            var task = await _job.UpdateAsync(id, Job);
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
            var task = await _job.DeleteAsync(id);

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

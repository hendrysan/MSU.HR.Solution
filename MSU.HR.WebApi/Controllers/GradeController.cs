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
    public class GradeController : ControllerBase
    {
        private readonly ILogger<GradeController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IGrade _grade;
        public GradeController(ILogger<GradeController> logger, IConfiguration configuration, IGrade grade)
        {
            _grade = grade;
            _logger = logger;
            _configuration = configuration;
            //userIdentity = new UserIdentityModel(User.Identity as ClaimsIdentity);
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            _logger.LogInformation("Get Data");
            var response = await _grade.GetGradesAsync();

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest(id);

            var response = await _grade.GetGradeAsync(id);

            if (response == null)
                return BadRequest("Not found : " + id);

            return Ok(response);
        }

        [HttpGet("Dropdown")]
        public async Task<IActionResult> GetDropdown()
        {
            _logger.LogInformation("Get Data");
            var response = await _grade.GetDropdownModelAsync();

            return Ok(response);
        }

        [HttpGet("Pagination")]
        public async Task<IActionResult> GetPagination(int pageNumber, int pageSize, string? search)
        {
            search = search ?? string.Empty;

            var response = await _grade.GetGradesAsync(search, new PaginationModel()
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            });

            return Ok(response);
        }

        //[HttpPost]
        //public async Task<ActionResult<PostResponse>> Post([FromBody] GradeRequest request)
        //{
        //    if (!ModelState.IsValid)
        //        return BadRequest(ModelState);

        //    var codeExists = await _grade.CheckCodeExistsAsync(request.Code);
        //    if (codeExists)
        //    {
        //        ModelState.AddModelError("Code", "Code already exists");
        //        return BadRequest(ModelState);
        //    }

        //    Grade Grade = new()
        //    {
        //        Name = request.Name,
        //        Code = request.Code
        //    };

        //    var task = await _grade.CreateAsync(Grade);
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
        //public async Task<ActionResult<PostResponse>> Put(Guid id, [FromBody] GradeRequest request)
        //{
        //    if (!ModelState.IsValid && id == Guid.Empty)
        //        return BadRequest(ModelState);

        //    Grade Grade = new()
        //    {
        //        Name = request.Name,
        //        Code = request.Code
        //    };

        //    var task = await _grade.UpdateAsync(id, Grade);
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
        //    var task = await _grade.DeleteAsync(id);

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

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
    public class CorporateController : ControllerBase
    {
        private readonly ILogger<CorporateController> _logger;
        private readonly IConfiguration _configuration;
        private readonly ICorporate _corporate;
        public CorporateController(ILogger<CorporateController> logger, IConfiguration configuration, ICorporate corporate)
        {
            _corporate = corporate;
            _logger = logger;
            _configuration = configuration;
            //userIdentity = new UserIdentityModel(User.Identity as ClaimsIdentity);
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            _logger.LogInformation("Get Data");
            var response = await _corporate.GetCorporatesAsync();

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest(id);

            var response = await _corporate.GetCorporateAsync(id);

            if (response == null)
                return BadRequest("Not found : " + id);

            return Ok(response);
        }

        [HttpGet("Dropdown")]
        public async Task<IActionResult> GetDropdown()
        {
            _logger.LogInformation("Get Data");
            var response = await _corporate.GetDropdownModelAsync();

            return Ok(response);
        }

        [HttpGet("Pagination")]
        public async Task<IActionResult> GetPagination(int pageNumber, int pageSize, string? search)
        {
            search = search ?? string.Empty;

            var response = await _corporate.GetCorporatesAsync(search, new PaginationModel()
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            });

            return Ok(response);
        }
        /*
        [HttpPost]
        public async Task<ActionResult<PostResponse>> Post([FromBody] CorporateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var codeExists = await _corporate.CheckCodeExistsAsync(request.CorporateCode);
            if (codeExists)
            {
                ModelState.AddModelError("Code", "Code already exists");
                return BadRequest(ModelState);
            }

            Corporate corporate = new()
            {
                Name = request.Name,
                Address1 = request.Address1,
                Address2 = request.Address2,
                Address3 = request.Address3,
                City = request.City,
                CorporateCode = request.CorporateCode,
                Country = request.Country,
                Description = request.Description,
                Email1 = request.Email1,
                Email2 = request.Email2,
                IsActive = true,
                Logo = request.Logo,
                Phone1 = request.Phone1,
                Phone2 = request.Phone2,
                State = request.State,
                Website = request.Website,
                ZipCode = request.ZipCode
            };

            var task = await _corporate.CreateAsync(corporate);
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
        public async Task<ActionResult<PostResponse>> Put(Guid id, [FromBody] CorporateRequest request)
        {
            if (!ModelState.IsValid && id == Guid.Empty)
                return BadRequest(ModelState);

            Corporate corporate = new()
            {
                Name = request.Name,
                Address1 = request.Address1,
                Address2 = request.Address2,
                Address3 = request.Address3,
                City = request.City,
                CorporateCode = request.CorporateCode,
                Country = request.Country,
                Description = request.Description,
                Email1 = request.Email1,
                Email2 = request.Email2,
                IsActive = true,
                Logo = request.Logo,
                Phone1 = request.Phone1,
                Phone2 = request.Phone2,
                State = request.State,
                Website = request.Website,
                ZipCode = request.ZipCode
            };

            var task = await _corporate.UpdateAsync(id, corporate);
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
            var task = await _corporate.DeleteAsync(id);

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

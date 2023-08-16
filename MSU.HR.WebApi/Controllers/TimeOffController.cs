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
    public class TimeOffController : ControllerBase
    {
        private readonly ILogger<SectionController> _logger;
        private readonly IConfiguration _configuration;
        private readonly ITimeOff _timeOff;
        public TimeOffController(ILogger<SectionController> logger, IConfiguration configuration, ITimeOff timeOff)
        {
            _timeOff = timeOff;
            _logger = logger;
            _configuration = configuration;
            //userIdentity = new UserIdentityModel(User.Identity as ClaimsIdentity);
        }

        [HttpPost("Request")]
        public async Task<ActionResult<PostResponse>> Request([FromBody] TimeOffRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var task = await _timeOff.SubmitAsync(request);

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

        [HttpPost("Approve")]
        public async Task<ActionResult<PostResponse>> Approve([FromBody] TimeOffActionApproveRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var task = await _timeOff.ApproveAsync(request.TimeOffId, request.Remarks.ToString());

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

        [HttpPost("Reject")]
        public async Task<ActionResult<PostResponse>> Reject([FromBody] TimeOffActionRejectRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var task = await _timeOff.RejectAsync(request.TimeOffId, request.Remarks.ToString());

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

        [HttpPost("Finish")]
        public async Task<ActionResult<PostResponse>> Finish([FromBody] TimeOffActionFinishRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var task = await _timeOff.FinishAsync(request.TimeOffId);

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

        [HttpGet("{userId}")]
        public async Task<IActionResult> Get(Guid userId)
        {
            _logger.LogInformation("Get Data");
            if (userId == Guid.Empty)
                return BadRequest(userId);

            var response = await _timeOff.GetTimeOffsAsync(userId);

            return Ok(response);
        }

        [HttpGet("CountLeaveAllowance/{userId}")]
        public async Task<IActionResult> CountLeaveAllowance(Guid userId)
        {
            _logger.LogInformation("Get Data");
            if (userId == Guid.Empty)
                return BadRequest(userId);

            var response = await _timeOff.GetCountLeaveAllowanceAsync(userId);

            return Ok(response);
        }

        [HttpGet("Detail/{userId}/{timeOffId}")]
        public async Task<IActionResult> Detail(Guid userId, Guid timeOffId)
        {
            if (timeOffId == Guid.Empty)
                return BadRequest(timeOffId);

            if (userId == Guid.Empty)
                return BadRequest(userId);

            var response = new TimeOffDetailResponse();

            var timeOff = await _timeOff.GetTimeOffDetailAsync(userId, timeOffId);
            var histories = await _timeOff.GetTimeOffHistoriesAsync(timeOffId);
            response.TimeOff = timeOff;
            response.TimeOffHistories = histories;


            return Ok(response);
        }

        [HttpGet("PendingApproval/{userId}")]
        public async Task<IActionResult> PendingApproval(Guid userId)
        {
            if (userId == Guid.Empty)
                return BadRequest(userId);

            var timeOffPending = await _timeOff.GetPendingApprovalTimeOffsAsync(userId);
            return Ok(timeOffPending);
        }

        [HttpGet("PendingFinish")]
        public async Task<IActionResult> PendingFinish()
        {
            var timeOffPending = await _timeOff.GetPendingFinishTimeOffsAsync();
            return Ok(timeOffPending);
        }

        //[HttpGet("{id}")]
        //public async Task<IActionResult> Get(Guid id)
        //{
        //    if (id == Guid.Empty)
        //        return BadRequest(id);

        //    var response = await _section.GetSectionAsync(id);

        //    if (response == null)
        //        return BadRequest("Not found : " + id);

        //    return Ok(response);
        //}

        //[HttpGet("Dropdown")]
        //public async Task<IActionResult> GetDropdown()
        //{
        //    _logger.LogInformation("Get Data");
        //    var response = await _section.GetDropdownModelAsync();

        //    return Ok(response);
        //}

        //[HttpGet("Pagination")]
        //public async Task<IActionResult> GetPagination(int pageNumber, int pageSize, string? search)
        //{
        //    search = search ?? string.Empty;

        //    var response = await _section.GetSectionsAsync(search, new PaginationModel()
        //    {
        //        PageNumber = pageNumber,
        //        PageSize = pageSize
        //    });

        //    return Ok(response);
        //}

        //[HttpPost]
        //public async Task<ActionResult<PostResponse>> Post([FromBody] SectionRequest request)
        //{
        //    if (!ModelState.IsValid)
        //        return BadRequest(ModelState);

        //    Section Section = new()
        //    {
        //        Name = request.Name,
        //        Code = request.Code
        //    };

        //    var task = await _section.CreateAsync(Section);
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
        //public async Task<ActionResult<PostResponse>> Put(Guid id, [FromBody] SectionRequest request)
        //{
        //    if (!ModelState.IsValid && id == Guid.Empty)
        //        return BadRequest(ModelState);

        //    Section Section = new()
        //    {
        //        Name = request.Name,
        //        Code = request.Code
        //    };

        //    var task = await _section.UpdateAsync(id, Section);
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
        //    var task = await _section.DeleteAsync(id);

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

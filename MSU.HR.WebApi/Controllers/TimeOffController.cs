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

        [HttpGet("Detail/{timeOffId}")]
        public async Task<IActionResult> Detail(Guid timeOffId)
        {
            if (timeOffId == Guid.Empty)
                return BadRequest(timeOffId);

            var response = new TimeOffDetailResponse();

            var timeOff = await _timeOff.GetTimeOffDetailAsync(timeOffId);
            var histories = await _timeOff.GetTimeOffHistoriesAsync(timeOffId);
            response.TimeOff = timeOff;
            response.TimeOffHistories = histories;


            return Ok(response);
        }

        [HttpGet("PendingApproval/{userCode}")]
        public async Task<IActionResult> PendingApproval(string userCode)
        {
            if (string.IsNullOrEmpty(userCode))
                return BadRequest(userCode);

            var timeOffPending = await _timeOff.GetPendingApprovalTimeOffsAsync(userCode);
            return Ok(timeOffPending);
        }

        [HttpGet("PendingFinish")]
        public async Task<IActionResult> PendingFinish()
        {
            var timeOffPending = await _timeOff.GetPendingFinishTimeOffsAsync();
            return Ok(timeOffPending);
        }


        [HttpGet("Summary/{year}")]
        public async Task<IActionResult> Summary(int year)
        {
            

            return Ok();
        }
    }
}

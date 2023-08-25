using Microsoft.AspNetCore.Http;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using MSU.HR.Commons.Enums;
using MSU.HR.Contexts;
using MSU.HR.Models.Entities;
using MSU.HR.Models.Others;
using MSU.HR.Models.Requests;
using MSU.HR.Models.Responses;
using MSU.HR.Services.Interfaces;
using System.Security.Claims;
using System.Text.Json;

namespace MSU.HR.Services.Repositories
{
    public class TimeOffRepository : ITimeOff
    {
        private readonly DatabaseContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserIdentityModel userIdentity;
        private readonly ILogError _logError;
        public TimeOffRepository(DatabaseContext context, IHttpContextAccessor httpContextAccessor, ILogError logError)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            userIdentity = new UserIdentityModel(_httpContextAccessor.HttpContext.User.Identity as ClaimsIdentity);
            _logError = logError;
        }

        public static List<string> ListStatusTimeOffAllowed = new()
        {
            StatusTimeOffEnum.REQUESTED.ToString(),
            StatusTimeOffEnum.APPROVED.ToString(),
            StatusTimeOffEnum.EXPIRED.ToString(),
            StatusTimeOffEnum.FINISHED.ToString()
        };

        private async Task<int> SaveHistory(Guid timeOffId, string remarks, StatusTimeOffEnum status)
        {
            try
            {
                TimeOffHistory timeOffHistory = new TimeOffHistory();
                timeOffHistory.Id = Guid.NewGuid();
                timeOffHistory.TimeOffId = timeOffId;
                timeOffHistory.UserId = userIdentity.Id;
                timeOffHistory.UserFullName = userIdentity.FullName;
                timeOffHistory.StatusId = status.ToString();
                timeOffHistory.Remarks = remarks;
                timeOffHistory.CreatedDate = DateTime.Now;
                _context.TimeOffHistories.Add(timeOffHistory);
                return await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                await _logError.SaveAsync(ex, JsonSerializer.Serialize(timeOffId));
                throw new Exception("Time Off SubmitAsync Error : " + ex.Message);
            }
        }

        public async Task<int> ApproveAsync(Guid timeOffId, string remarks)
        {
            try
            {
                var statusBefore = StatusTimeOffEnum.REQUESTED;
                var trx = await _context.TimeOffs.Where(i => i.Id == timeOffId).FirstOrDefaultAsync();
                if (trx == null)
                    throw new Exception("badrequest Data Transaction Not found");

                if (trx.StatusId != statusBefore.ToString())
                    throw new Exception("badrequest StatusId Invalid");

                var user = await _context.Employees.Where(i => i.Code == userIdentity.Code).FirstOrDefaultAsync();
                if (trx.ApprovedBy != user.Code)
                    throw new Exception("badrequest User Invalid");

                var status = StatusTimeOffEnum.APPROVED;
                trx.ApprovedDate = DateTime.Now;
                //trx.ApprovedBy = userIdentity.Code;
                trx.ApprovedRemarks = remarks;
                trx.StatusId = status.ToString();
                var result = await _context.SaveChangesAsync();
                await SaveHistory(timeOffId, remarks, status);
                return result;
            }
            catch (Exception ex)
            {
                await _logError.SaveAsync(ex, JsonSerializer.Serialize(timeOffId));
                throw new Exception("Time Off ApproveAsync Error : " + ex.Message);
            }
        }

        public async Task<CountLeaveAllowanceResponse> GetCountLeaveAllowanceAsync(Guid userId)
        {
            try
            {
                CountLeaveAllowanceResponse result = new CountLeaveAllowanceResponse();
                result.Total = 0;
                result.UserId = userId;
                var user = await _context.Users.Where(i => i.Id == userId.ToString() && i.IsConnected).Include(e => e.Employee).FirstOrDefaultAsync();
                if (user == null)
                    return result;

                var trxApproved = await _context.TimeOffs.Where(i => i.UserId == userId && i.StatusId == StatusTimeOffEnum.APPROVED.ToString()).ToListAsync();
                var trxExpired = await _context.TimeOffs.Where(i => i.UserId == userId && i.StatusId == StatusTimeOffEnum.EXPIRED.ToString()).ToListAsync();
                var trxFinish = await _context.TimeOffs.Where(i => i.UserId == userId && i.StatusId == StatusTimeOffEnum.FINISHED.ToString()).ToListAsync();

                var date1 = DateTime.Now.Date;
                var date2 = user.Employee.JoinDate.Date;

                var countJoinDate = ((date1.Year - date2.Year) * 12) + date1.Month - date2.Month;//(DateTime.Now - user.Employee.JoinDate).TotalDays;

                if (countJoinDate < 12)
                    return result;


                var count = countJoinDate - trxApproved.Sum(i => i.Taken) - trxExpired.Sum(i => i.Taken) - trxFinish.Sum(i => i.Taken);

                result.Total = count;

                return result;
            }
            catch (Exception ex)
            {
                await _logError.SaveAsync(ex, JsonSerializer.Serialize(userId));
                throw new Exception("Time Off GetCountLeaveAllowanceAsync Error : " + ex.Message);
            }
        }

        public async Task<CountLeaveAllowanceResponse> GetCountLeaveAllowanceAsync()
        {
            try
            {
                var userId = userIdentity.Id;
                var task = await GetCountLeaveAllowanceAsync(userId);
                return task;
            }
            catch (Exception ex)
            {
                await _logError.SaveAsync(ex, "");
                throw new Exception("Time Off GetCountLeaveAllowanceAsync Error : " + ex.Message);
            }
        }

        public async Task<IEnumerable<TimeOffHistory>?> GetTimeOffHistoriesAsync(Guid timeOffId)
        {
            try
            {
                var histories = await _context.TimeOffHistories.Where(i => i.TimeOffId == timeOffId).ToListAsync();
                return histories.OrderByDescending(i => i.CreatedDate).ToList();
            }
            catch (Exception ex)
            {
                await _logError.SaveAsync(ex, JsonSerializer.Serialize(timeOffId));
                throw new Exception("Time Off GetTimeOffHistoriesAsync Error : " + ex.Message);
            }
        }

        public async Task<IEnumerable<TimeOff>?> GetTimeOffsAsync(Guid userId)
        {
            try
            {
                var trx = await _context.TimeOffs.Where(i => i.UserId == userId).Include(r => r.Reason).ToListAsync();
                return trx.OrderByDescending(i => i.CreatedDate).ToList();
            }
            catch (Exception ex)
            {
                await _logError.SaveAsync(ex, JsonSerializer.Serialize(userId));
                throw new Exception("Time Off GetTimeOffsAsync Error : " + ex.Message);
            }
        }

        public async Task<IEnumerable<TimeOff>?> GetTimeOffsAsync()
        {
            try
            {
                Guid userId = userIdentity.Id;
                var trx = await _context.TimeOffs.Where(i => i.UserId == userId).Include(r => r.Reason).ToListAsync();
                return trx.OrderByDescending(i => i.CreatedDate).ToList();
            }
            catch (Exception ex)
            {
                await _logError.SaveAsync(ex, "");
                throw new Exception("Time Off GetTimeOffsAsync Error : " + ex.Message);
            }
        }

        public async Task<Employee?> GetUserSuperiorityAsync(string code)
        {
            var employee = await _context.Employees.Where(i => i.Code == code)
                .Include(i => i.Department)
                .Include(i => i.Section)
                .Include(i => i.Grade)
                .FirstOrDefaultAsync();

            if (employee == null)
                return null;

            var department = employee.Department;
            var section = employee.Section;
            var grade = employee.Grade;

            var superiors = await _context.Employees
                .Include(i => i.Department)
                .Include(i => i.Section)
                .Include(i => i.Grade)
                .Where(i => i.Department == department && i.Section == section && i.IsActive && i.Grade != null)
                .ToListAsync();

            if (superiors.Count == 0)
                return null;

            var superior = superiors.Where(i => Convert.ToInt32(i.Grade?.Code) < Convert.ToInt32(grade?.Code)).OrderDescending().FirstOrDefault();
            return superior;
        }

        public async Task<int> RejectAsync(Guid timeOffId, string remarks)
        {
            try
            {
                var statusBefore = StatusTimeOffEnum.REJECTED;
                //var trxExists = await _context.TimeOffs.Where(i => i.UserId == userIdentity.Id && i.StatusId == statusBefore.ToString()).ToListAsync();
                //if (trxExists.Count > 0)
                //    throw new Exception("badrequest Data Transaction Invalid");

                var trx = await _context.TimeOffs.Where(i => i.Id == timeOffId).FirstOrDefaultAsync();
                if (trx == null)
                    throw new Exception("badrequest Data Not Found");

                if (trx.StatusId != StatusTimeOffEnum.REQUESTED.ToString())
                    throw new Exception("badrequest Status Transaction Invalid");

                if (trx.ApprovedBy != userIdentity.Code)
                    throw new Exception("badrequest User Code Invalid");

                var status = StatusTimeOffEnum.REJECTED;
                trx.ApprovedDate = DateTime.Now;
                //trx.ApprovedBy = userIdentity.Code;
                trx.ApprovedRemarks = remarks;
                trx.StatusId = status.ToString();
                var result = await _context.SaveChangesAsync();
                await SaveHistory(timeOffId, remarks, status);
                return result;
            }
            catch (Exception ex)
            {
                await _logError.SaveAsync(ex, JsonSerializer.Serialize(timeOffId));
                throw new Exception("Time Off ApproveAsync Error : " + ex.Message);
            }
        }

        public async Task<int> SubmitAsync(TimeOffRequest request)
        {
            try
            {
                var status = StatusTimeOffEnum.REQUESTED;
                var trxExists = await _context.TimeOffs.Where(i => i.UserId == userIdentity.Id && i.StatusId == status.ToString()).ToListAsync();
                if (trxExists.Count > 0)
                    throw new Exception("badrequest Submited Invalid, already exists Requested");

                var reason = await _context.Reasons.Where(i => i.Id == request.ReasonId).FirstOrDefaultAsync();
                var approvedBy = await GetUserSuperiorityAsync(userIdentity.Code);

                TimeOff trx = new TimeOff();
                trx.Id = Guid.NewGuid();
                trx.UserId = userIdentity.Id;
                trx.UserFullName = userIdentity.FullName;
                trx.UserCode = userIdentity.Code;
                trx.Reason = reason;
                trx.TemporaryAnnualLeaveAllowance = request.TemporaryAnnualLeaveAllowance;
                trx.StartDate = request.StartDate;
                trx.EndDate = request.EndDate;
                trx.Notes = request.Notes;
                trx.Taken = request.Taken;
                trx.StatusId = status.ToString();
                trx.ApprovedBy = approvedBy == null ? string.Empty : approvedBy.Code;
                trx.CreatedDate = DateTime.Now;
                _context.TimeOffs.Add(trx);
                var result = await _context.SaveChangesAsync();
                await SaveHistory(trx.Id, request.Notes ?? string.Empty, status);
                return result;
            }
            catch (Exception ex)
            {
                await _logError.SaveAsync(ex, JsonSerializer.Serialize(request));
                throw new Exception("Time Off SubmitAsync Error : " + ex.Message);
            }
        }

        public async Task<int> ExpiredAsync(Guid userId)
        {
            try
            {
                var status = StatusTimeOffEnum.EXPIRED.ToString();
                var user = await _context.Users.Where(i => i.Id == userId.ToString()).Include(i => i.Employee).FirstOrDefaultAsync();
                var taken = await GetCountLeaveAllowanceAsync(userId);
                TimeOff trx = new TimeOff();

                trx.Id = Guid.NewGuid();
                trx.UserId = userId;
                trx.UserCode = user.Employee?.Code ?? string.Empty;
                trx.UserFullName = user.Employee?.Name ?? string.Empty;
                trx.Reason = null;
                trx.TemporaryAnnualLeaveAllowance = taken.Total;
                trx.StartDate = DateTime.Now;
                trx.EndDate = DateTime.Now;
                trx.Notes = "Expired Submit by System";
                trx.Taken = taken.Total;
                trx.StatusId = status;
                trx.ApprovedBy = string.Empty;
                trx.CreatedDate = DateTime.Now;
                _context.TimeOffs.Add(trx);
                var result = await _context.SaveChangesAsync();


                TimeOffHistory timeOffHistory = new TimeOffHistory();
                timeOffHistory.Id = Guid.NewGuid();
                timeOffHistory.TimeOffId = trx.Id;
                timeOffHistory.UserId = Guid.Empty;
                timeOffHistory.UserFullName = "System";
                timeOffHistory.StatusId = status.ToString();
                timeOffHistory.Remarks = trx.Notes;
                timeOffHistory.CreatedDate = DateTime.Now;
                await _context.SaveChangesAsync();

                return result;
            }
            catch (Exception ex)
            {
                await _logError.SaveAsync(ex, JsonSerializer.Serialize(userId));
                throw new Exception("Time Off ExpiredAsync Error : " + ex.Message);
            }
        }

        public async Task<TimeOff> GetTimeOffDetailAsync(Guid timeOffId)
        {
            try
            {
                var trx = await _context.TimeOffs.Where(i => i.Id == timeOffId).Include(r => r.Reason).FirstOrDefaultAsync();
                return trx;
            }
            catch (Exception ex)
            {
                await _logError.SaveAsync(ex, JsonSerializer.Serialize(timeOffId));
                throw new Exception("Time Off GetTimeOffDetailAsync Error : " + ex.Message);
            }
        }

        public async Task<int> FinishAsync(Guid timeOffId)
        {
            try
            {
                var statusBefore = StatusTimeOffEnum.APPROVED;

                var trx = await _context.TimeOffs.Where(i => i.Id == timeOffId).FirstOrDefaultAsync();
                if (trx == null)
                    throw new Exception("badrequest Transaction Not Found");

                if (trx.StatusId != statusBefore.ToString())
                    throw new Exception("badrequest Status Transaction Invalid");

                var user = await _context.Employees.Where(i => i.Code == userIdentity.Code).Include(i => i.Department).FirstOrDefaultAsync();
                if (user == null)
                    throw new Exception("badrequest User Not Found");

                if (user.Department == null)
                    throw new Exception("badrequest User Department Not Found");

                if (user.Department.Code != "HRD")
                    throw new Exception("badrequest User Department Not HRD");

                var status = StatusTimeOffEnum.FINISHED;
                trx.StatusId = status.ToString();
                var result = await _context.SaveChangesAsync();
                await SaveHistory(timeOffId, string.Empty, status);
                return result;
            }
            catch (Exception ex)
            {
                await _logError.SaveAsync(ex, JsonSerializer.Serialize(timeOffId));
                throw new Exception("Time Off FinishAsync Error : " + ex.Message);
            }
        }

        public async Task<IEnumerable<TimeOff>?> GetPendingApprovalTimeOffsAsync(string code)
        {
            try
            {
                var status = StatusTimeOffEnum.REQUESTED.ToString();
                var trx = await _context.TimeOffs.Where(i => i.StatusId == status && i.ApprovedBy == code).Include(r => r.Reason).ToListAsync();
                return trx.OrderByDescending(i => i.CreatedDate).ToList();
            }
            catch (Exception ex)
            {
                await _logError.SaveAsync(ex, JsonSerializer.Serialize(code));
                throw new Exception("Time Off GetPendingApprovalTimeOffsAsync Error : " + ex.Message);
            }
        }

        public async Task<IEnumerable<TimeOff>?> GetPendingFinishTimeOffsAsync()
        {
            try
            {
                var employee = await _context.Employees.Where(i => i.Code == userIdentity.Code).Include(i => i.Department).FirstOrDefaultAsync();
                if (employee == null)
                    return null;

                if (employee.Department == null)
                    return null;

                if (employee.Department.Code != "HRD")
                    return null;


                var status = StatusTimeOffEnum.APPROVED.ToString();
                var trx = await _context.TimeOffs.Where(i => i.StatusId == status).Include(r => r.Reason).ToListAsync();
                return trx.OrderByDescending(i => i.CreatedDate).ToList();
            }
            catch (Exception ex)
            {
                await _logError.SaveAsync(ex, JsonSerializer.Serialize(string.Empty));
                throw new Exception("Time Off GetPendingFinishTimeOffsAsync Error : " + ex.Message);
            }
        }

        public async Task<TimeOffSummaryModel?> GetTimeOffSummaryAsync(string code, int year)
        {
            try
            {
                var result = new TimeOffSummaryModel();
                var timeOffs = await _context.TimeOffs.Where(i => i.UserCode == code && i.StartDate.Year == year).Include(i => i.Reason).ToListAsync();
                result.Year = year;
                result.ReminingBalance = (await this.GetCountLeaveAllowanceAsync(timeOffs.First().UserId)).Total;
                result.UserCode = code;

                if (timeOffs.Count > 0)
                {
                    result.TotalTaken = timeOffs.Where(i => ListStatusTimeOffAllowed.Contains(i.StatusId)).Sum(i => i.Taken);
                    result.Status = timeOffs.Select(i => i.StatusId).Distinct().OrderBy(i => i).ToList();
                    result.Details = timeOffs.Select(i => new TimeOffSummaryDetailModel()
                    {
                        Id = i.Id,
                        EndDate = i.EndDate,
                        StartDate = i.StartDate,
                        ReasonName = i.Reason?.Name ?? string.Empty,
                        Taken = i.Taken,
                        Status = Enum.GetName(enumType: typeof(StatusTimeOffEnum), i.StatusId) ?? string.Empty,
                    }).OrderBy(i => new { i.Status, i.StartDate }).ToList();
                }

                return result;
            }
            catch (Exception ex)
            {
                await _logError.SaveAsync(ex, JsonSerializer.Serialize(code));
                throw new Exception("Time Off GetTimeOffSummaryAsync Error : " + ex.Message);
            }
        }
    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
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
                var statusBefore = StatusTimeOffEnum.REQUEST;
                var trxExists = await _context.TimeOffs.Where(i => i.UserId == userIdentity.Id && i.StatusId == statusBefore.ToString()).ToListAsync();
                if (trxExists.Count > 0)
                    return 0;

                var trx = await _context.TimeOffs.Where(i => i.Id == timeOffId).FirstOrDefaultAsync();
                if (trx == null)
                    return 0;

                var status = StatusTimeOffEnum.APPROVED;
                trx.ApprovedDate = DateTime.Now;
                trx.ApprovedBy = userIdentity.Id;
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
                var trxFinish = await _context.TimeOffs.Where(i => i.UserId == userId && i.StatusId == StatusTimeOffEnum.FINISH.ToString()).ToListAsync();

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

        public async Task<IEnumerable<TimeOffHistory>> GetTimeOffHistoriesAsync(Guid timeOffId)
        {
            try
            {
                var histories = await _context.TimeOffHistories.Where(i => i.TimeOffId == timeOffId).ToListAsync();
                return histories.OrderBy(i => i.CreatedDate).ToList();
            }
            catch (Exception ex)
            {
                await _logError.SaveAsync(ex, JsonSerializer.Serialize(timeOffId));
                throw new Exception("Time Off GetTimeOffHistoriesAsync Error : " + ex.Message);
            }
        }

        public async Task<IEnumerable<TimeOff>> GetTimeOffsAsync(Guid userId)
        {
            try
            {
                var trx = await _context.TimeOffs.Where(i => i.UserId == userId).Include(r => r.Reason).ToListAsync();
                return trx.OrderByDescending(i=>i.CreatedDate).ToList();
            }
            catch (Exception ex)
            {
                await _logError.SaveAsync(ex, JsonSerializer.Serialize(userId));
                throw new Exception("Time Off GetTimeOffsAsync Error : " + ex.Message);
            }
        }

        public Task<Guid> GetUserSuperiorityAsync(Guid userId)
        {
            throw new NotImplementedException();
        }

        public async Task<int> RejectAsync(Guid timeOffId, string remarks)
        {
            try
            {
                var statusBefore = StatusTimeOffEnum.REJECT;
                var trxExists = await _context.TimeOffs.Where(i => i.UserId == userIdentity.Id && i.StatusId == statusBefore.ToString()).ToListAsync();
                if (trxExists.Count > 0)
                    return 0;

                var trx = await _context.TimeOffs.Where(i => i.Id == timeOffId).FirstOrDefaultAsync();
                if (trx == null)
                    return 0;

                var status = StatusTimeOffEnum.REJECT;
                trx.ApprovedDate = DateTime.Now;
                trx.ApprovedBy = userIdentity.Id;
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
                var status = StatusTimeOffEnum.REQUEST;
                var trxExists = await _context.TimeOffs.Where(i => i.UserId == userIdentity.Id && i.StatusId == status.ToString()).ToListAsync();
                if (trxExists.Count > 0)
                    return 0;

                var reason = await _context.Reasons.Where(i => i.Id == request.ReasonId).FirstOrDefaultAsync();

                TimeOff trx = new TimeOff();
                trx.Id = Guid.NewGuid();
                trx.UserId = userIdentity.Id;
                trx.Reason = reason;
                trx.TemporaryAnnualLeaveAllowance = request.TemporaryAnnualLeaveAllowance;
                trx.StartDate = request.StartDate;
                trx.EndDate = request.EndDate;
                trx.Notes = request.Notes;
                trx.Taken = request.Taken;
                trx.StatusId = status.ToString();
                trx.ApprovedBy = Guid.Empty;
                trx.CreatedDate = DateTime.Now;
                _context.TimeOffs.Add(trx);
                var result = await _context.SaveChangesAsync();
                await SaveHistory(trx.Id, request.Notes, status);
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

                var taken = await GetCountLeaveAllowanceAsync(userId);
                TimeOff trx = new TimeOff();
                var status = StatusTimeOffEnum.EXPIRED;
                trx.Id = Guid.NewGuid();
                trx.UserId = userId;
                trx.Reason = null;
                trx.TemporaryAnnualLeaveAllowance = 0;
                trx.StartDate = DateTime.Now;
                trx.EndDate = DateTime.Now;
                trx.Notes = "Expired Submit by System ";
                trx.Taken = taken.Total;
                trx.StatusId = status.ToString();
                trx.ApprovedBy = Guid.Empty;
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

        public async Task<TimeOff> GetTimeOffDetailAsync(Guid userId, Guid timeOffId)
        {
            try
            {
                var trx = await _context.TimeOffs.Where(i => i.UserId == userId && i.Id == timeOffId).Include(r => r.Reason).FirstOrDefaultAsync();
                return trx;
            }
            catch (Exception ex)
            {
                await _logError.SaveAsync(ex, JsonSerializer.Serialize(userId));
                throw new Exception("Time Off GetTimeOffDetailAsync Error : " + ex.Message);
            }
        }

        public async Task<int> FinishAsync(Guid timeOffId)
        {
            try
            {
                var statusBefore = StatusTimeOffEnum.APPROVED;
                var trxExists = await _context.TimeOffs.Where(i => i.UserId == userIdentity.Id && i.StatusId == statusBefore.ToString()).ToListAsync();
                if (trxExists.Count > 0)
                    return 0;

                var trx = await _context.TimeOffs.Where(i => i.Id == timeOffId).FirstOrDefaultAsync();
                if (trx == null)
                    return 0;

                var status = StatusTimeOffEnum.FINISH;
                trx.ApprovedDate = DateTime.Now;
                trx.ApprovedBy = userIdentity.Id;
                trx.ApprovedRemarks = string.Empty;
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
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.HttpSys;
using MSU.HR.Commons.Enums;
using MSU.HR.Commons.Extensions;
using MSU.HR.Models.Entities;
using MSU.HR.Models.Others;
using MSU.HR.Models.Requests;
using MSU.HR.Models.ViewModels;
using MSU.HR.Services.Interfaces;
using MySqlX.XDevAPI;
using System.Security.Claims;
using System.Text.Json;

namespace MSU.HR.WebClient.Controllers
{
    public class PaidLeaveController : BaseContoller
    {
        private readonly ITimeOff _timeOff;
        private readonly IReason _reason;
        private readonly IEmployee _employee;
        private readonly ILogError _logError;

        public PaidLeaveController(ITimeOff timeOff, IReason reason, IEmployee employee, ILogError logError)
        {
            _timeOff = timeOff;
            _reason = reason;
            _employee = employee;
            _logError = logError;
        }

        public async Task<IActionResult> List()
        {
            try
            {
                ViewBag.PageTitle = "List Of Paid Leave";
                ViewBag.BreadcrumbItems = new List<BreadcrumbItemModel>
                {
                    new BreadcrumbItemModel
                    {
                        IsActive = true,
                        Title = "List History",
                        Url = "#",
                        Sequence = 1
                    },
                };
                GetAlert();

                var data = await _timeOff.GetTimeOffsAsync();

                PaidLeaveIndexModel model = new PaidLeaveIndexModel();
                model.ReminingAllowance = (await _timeOff.GetCountLeaveAllowanceAsync()).Total;
                model.TimeOffs = data;

                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IActionResult> Detail(Guid id)
        {
            try
            {
                if (string.IsNullOrEmpty(id.ToString()))
                    throw new Exception("Id is null or empty");


                ViewBag.PageTitle = "Detail Of Paid Leave";
                ViewBag.BreadcrumbItems = new List<BreadcrumbItemModel>
                {
                    new BreadcrumbItemModel
                    {
                        IsActive = false,
                        Title = "List History",
                        Url = Url.Action("List","PaidLeave"),
                        Sequence = 1
                    },
                    new BreadcrumbItemModel
                    {
                        IsActive = true,
                        Title = "Detail",
                        Url = "#",
                        Sequence = 2
                    },
                };

                var detail = await _timeOff.GetTimeOffDetailAsync(id);
                if (detail == null)
                    throw new Exception("Data not found");

                var histories = await _timeOff.GetTimeOffHistoriesAsync(id);

                PaidLeaveDetailModel model = new PaidLeaveDetailModel();
                model.TimeOff = detail;
                model.TimeOffHistories = histories;

                model.CurrentUser = await _employee.GetEmployeeAsync();

                model.UserRequest = await _employee.GetEmployeeAsync(detail.UserCode);
                model.UserSuperior = await _employee.GetEmployeeAsync(detail.ApprovedBy);
                model.Id = id;

                return View(model);
            }
            catch (Exception ex)
            {
                await _logError.SaveAsync(ex, "");
                throw ex;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PostDetail(PaidLeaveDetailModel request)
        {
            try
            {
                SetAlert("Your action has been submitted", AlertType.Success);
                
                if (request.ActionSubmit == StatusTimeOffEnum.APPROVED.ToString())
                {
                    await _timeOff.ApproveAsync(request.Id, request.Remarks);
                    return RedirectToAction(actionName: "PendingApproval");
                }
                else if (request.ActionSubmit == StatusTimeOffEnum.REJECTED.ToString())
                {
                    await _timeOff.RejectAsync(request.Id, request.Remarks);
                    return RedirectToAction(actionName: "PendingApproval");
                }
                else if (request.ActionSubmit == StatusTimeOffEnum.FINISHED.ToString())
                {
                    await _timeOff.FinishAsync(request.Id);
                    return RedirectToAction(actionName: "PendingFinish");
                }

                SetAlert("Your action cannot registered", AlertType.Warning);

                return RedirectToAction(actionName: "List");
            }
            catch (Exception ex)
            {
                await _logError.SaveAsync(ex, "");
                throw ex;
            }
        }

        public async Task<IActionResult> Create()
        {
            try
            {
                ViewBag.PageTitle = "List Of Paid Leave";
                ViewBag.BreadcrumbItems = new List<BreadcrumbItemModel>
                {
                    new BreadcrumbItemModel
                    {
                        IsActive = false,
                        Title = "List History",
                        Url = Url.Action("List","PaidLeave"),
                        Sequence = 1
                    },
                    new BreadcrumbItemModel
                    {
                        IsActive = true,
                        Title = "Create",
                        Url = "#",
                        Sequence = 2
                    },
                };

                PaidLeaveCreateModel model = new PaidLeaveCreateModel();
                model.EmployeeInfo = await _employee.GetEmployeeAsync();

                model.RemainingAllowance = (await _timeOff.GetCountLeaveAllowanceAsync()).Total;
                model.ReasonDropdown = await _reason.GetDropdownModelAsync();

                var superior = await _timeOff.GetUserSuperiorityAsync(model.EmployeeInfo.Code);
                model.SuperiorityInfo = $"{superior.Code} - {superior.Name}";

                return View(model);
            }
            catch (Exception ex)
            {
                await _logError.SaveAsync(ex, "");
                throw ex;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PostCreate(PaidLeaveCreateModel request)
        {
            try
            {
                var model = new TimeOffRequest();
                model.StartDate = request.StartDate;
                model.EndDate = request.EndDate;
                model.TemporaryAnnualLeaveAllowance = request.RemainingAllowance;
                model.Notes = request.Notes;
                model.ReasonId = request.ReasonId;
                model.Taken = request.Taken; //Convert.ToInt32((request.EndDate.Date - request.StartDate.Date).TotalDays);
                await _timeOff.SubmitAsync(model);

                SetAlert("Your request has been submitted", AlertType.Success);

                return RedirectToAction(actionName: "List");
            }
            catch (Exception ex)
            {
                await _logError.SaveAsync(ex, "");
                throw ex;
            }
        }

        public async Task<IActionResult> PendingApproval()
        {
            try
            {
                ViewBag.PageTitle = "List Of Paid Leave Pending Approval";
                ViewBag.BreadcrumbItems = new List<BreadcrumbItemModel>
                {
                    new BreadcrumbItemModel
                    {
                        IsActive = true,
                        Title = "List Pending Approval",
                        Url = Url.Action("List","PaidLeave"),
                        Sequence = 1
                    }
                };
                GetAlert();
                var data = await _timeOff.GetPendingApprovalTimeOffsAsync(GetUserCode());

                PaidLeaveIndexModel model = new PaidLeaveIndexModel();
                model.ReminingAllowance = 0;
                model.TimeOffs = data;

                return View(model);
            }
            catch (Exception ex)
            {
                await _logError.SaveAsync(ex, "");
                throw ex;
            }
        }

        public async Task<IActionResult> PendingFinish()
        {
            try
            {
                ViewBag.PageTitle = "List Of Paid Leave Pending Finish";
                ViewBag.BreadcrumbItems = new List<BreadcrumbItemModel>
                {
                    new BreadcrumbItemModel
                    {
                        IsActive = true,
                        Title = "List Pending Finish",
                        Url = Url.Action("List","PaidLeave"),
                        Sequence = 1
                    }
                };
                GetAlert();
                var data = await _timeOff.GetPendingFinishTimeOffsAsync();

                PaidLeaveIndexModel model = new PaidLeaveIndexModel();
                model.ReminingAllowance = 0;
                model.TimeOffs = data;

                return View(model);
            }
            catch (Exception ex)
            {
                await _logError.SaveAsync(ex, "");
                throw ex;
            }
        }

        public async Task<IActionResult> Summary()
        {
            try
            {
                ViewBag.PageTitle = "List Of Summary Paid Leave ";
                ViewBag.BreadcrumbItems = new List<BreadcrumbItemModel>
                {
                    new BreadcrumbItemModel
                    {
                        IsActive = true,
                        Title = "Summaary",
                        Url = Url.Action("List","PaidLeave"),
                        Sequence = 1
                    }
                };
                GetAlert();
                var data = await _timeOff.GetTimeOffSummaryAsync(GetUserCode(), DateTime.Now.Year);

                return View(data);
            }
            catch (Exception ex)
            {
                await _logError.SaveAsync(ex, "");
                throw ex;
            }
        }
    }
}

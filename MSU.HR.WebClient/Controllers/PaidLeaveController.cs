using Microsoft.AspNetCore.Mvc;
using MSU.HR.Commons.Extensions;
using MSU.HR.Models.Entities;
using MSU.HR.Models.Others;
using MSU.HR.Models.Requests;
using MSU.HR.Models.ViewModels;
using MSU.HR.Services.Interfaces;
using System.Security.Claims;

namespace MSU.HR.WebClient.Controllers
{
    public class PaidLeaveController : Controller
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

        public async Task<IActionResult> Index()
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
                        Url = Url.Action("Index","PaidLeave"),
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
                await _timeOff.SubmitAsync(model);

                return
            }
            catch (Exception ex)
            {
                await _logError.SaveAsync(ex, "");
                throw ex;
            }
        }
    }
}

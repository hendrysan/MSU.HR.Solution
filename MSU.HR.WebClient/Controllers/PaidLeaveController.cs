using Microsoft.AspNetCore.Mvc;
using MSU.HR.Commons.Extensions;
using MSU.HR.Models.Others;
using MSU.HR.Models.ViewModels;
using MSU.HR.Services.Interfaces;
using System.Security.Claims;

namespace MSU.HR.WebClient.Controllers
{
    public class PaidLeaveController : Controller
    {
        private readonly ITimeOff _timeOff;

        public PaidLeaveController(ITimeOff timeOff)
        {
            _timeOff = timeOff;
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
    }
}

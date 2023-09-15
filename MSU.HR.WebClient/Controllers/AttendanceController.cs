using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using MSU.HR.Models.Entities;
using MSU.HR.Models.Others;
using MSU.HR.Models.Requests;
using MSU.HR.Models.ViewModels;
using MSU.HR.Services.Interfaces;
using System.Text.Json;
using ZstdSharp.Unsafe;

namespace MSU.HR.WebClient.Controllers
{
    public class AttendanceController : BaseContoller
    {
        private readonly ILogError _logError;
        private readonly IAttendance _attendance;
        private readonly IEmployee _employee;
        private readonly IUser _user;

        public AttendanceController(ILogError logError, IAttendance attendance, IEmployee employee, IUser user)
        {
            _logError = logError;
            _attendance = attendance;
            _employee = employee;
            _user = user;
        }

        public async Task<IActionResult> List()
        {
            return View();
        }

        public async Task<IActionResult> Upload()
        {
            ViewBag.PageTitle = "Upload Attendance";
            ViewBag.BreadcrumbItems = new List<BreadcrumbItemModel>
                {
                    new BreadcrumbItemModel
                    {
                        IsActive = true,
                        Title = "Upload Attendance",
                        Url = "#",
                        Sequence = 1
                    },
                };

            GetAlert();

            return View();
        }

        public async Task<IActionResult> DocumentDetails(Guid id)
        {
            try
            {
                if (string.IsNullOrEmpty(id.ToString()))
                    throw new Exception("Id is null or empty");


                ViewBag.PageTitle = "Detail Of Document Upload";
                ViewBag.BreadcrumbItems = new List<BreadcrumbItemModel>
                {
                    new BreadcrumbItemModel
                    {
                        IsActive = false,
                        Title = "Upload",
                        Url = Url.Action("Upload","Attendance"),
                        Sequence = 1
                    },
                    new BreadcrumbItemModel
                    {
                        IsActive = true,
                        Title = "Document Detail",
                        Url = "#",
                        Sequence = 2
                    },
                };


                var document = await _attendance.GetDocumentAttendance(id);

                if (document == null)
                    throw new Exception("Document not found");

                var model = new AttendanceDocumentDetailViewModel();
                model.Id = id;
                model.DocumentAttendance = document;
                model.User = await _user.GetProfile(document.CreatedBy);
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
        public async Task<IActionResult> PostDocumentDetail(Guid Id, string Action)
        {
            if (string.IsNullOrEmpty(Action))
                return BadRequest("No file selected for upload...");

            var task = await _attendance.ActionDocumentUploadAsync(Id, Action);

            SetAlert("Your document has been submitted", AlertType.Success);

            return RedirectToAction("Upload");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PostUpload(IFormFile postedFile, DateTime DocumentDate)
        {
            if (postedFile == null || postedFile.Length == 0)
                return BadRequest("No file selected for upload...");

            var task = await _attendance.UploadAsync(postedFile, DocumentDate);

            SetAlert("Your document has been submitted", AlertType.Success);

            return RedirectToAction("Upload");
        }

        [HttpPost]
        public async Task<JsonResult> GetDocumentUpload()
        {
            var draw = Request.Form["draw"].FirstOrDefault();
            var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
            var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
            var searchValue = Request.Form["search[value]"].FirstOrDefault();
            int pageSize = Convert.ToInt32(Request.Form["length"].FirstOrDefault() ?? "0");
            int skip = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0");

            var request = new DataTableRequest
            {
                draw = draw,
                sortColumn = sortColumn,
                sortColumnDirection = sortColumnDirection,
                searchValue = searchValue,
                pageSize = pageSize,
                skip = skip
            };

            var dataTable = await _attendance.GetDataTableDocumentAsync(request);

            var returnObj = Json(dataTable);
            return returnObj;
        }

        [HttpPost]
        public async Task<JsonResult> GetDetailDocumentUpload(Guid id)
        {
            var draw = Request.Form["draw"].FirstOrDefault();
            var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
            var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
            var searchValue = Request.Form["search[value]"].FirstOrDefault();
            int pageSize = Convert.ToInt32(Request.Form["length"].FirstOrDefault() ?? "0");
            int skip = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0");

            var request = new DataTableRequest
            {
                draw = draw,
                sortColumn = sortColumn,
                sortColumnDirection = sortColumnDirection,
                searchValue = searchValue,
                pageSize = pageSize,
                skip = skip
            };

            var dataTable = await _attendance.GetDataTableDocumentDetailAsync(request, id);

            var returnObj = Json(dataTable);
            return returnObj;
        }
    }
}

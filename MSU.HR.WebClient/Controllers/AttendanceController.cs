using Microsoft.AspNetCore.Mvc;
using MSU.HR.Models.Others;
using MSU.HR.Models.ViewModels;
using MSU.HR.Services.Interfaces;

namespace MSU.HR.WebClient.Controllers
{
    public class AttendanceController : BaseContoller
    {
        private readonly ILogError _logError;
        private readonly IAttendance _attendance;

        public AttendanceController(ILogError logError, IAttendance attendance)
        {
            _logError = logError;
            _attendance = attendance;
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PostUpload(IFormFile postedFile)
        {
            if (postedFile == null || postedFile.Length == 0)
                return BadRequest("No file selected for upload...");

            var task = await _attendance.UploadAsync(postedFile);

            //string fileName = Path.GetFileName(postedFile.FileName);
            //string contentType = postedFile.ContentType;

            SetAlert("Your document has been submitted", AlertType.Success);

            return RedirectToAction("Upload");
        }
    }
}

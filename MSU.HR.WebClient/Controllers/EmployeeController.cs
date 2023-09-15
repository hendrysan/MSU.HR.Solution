using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using MSU.HR.Models.Entities;
using MSU.HR.Models.Others;
using MSU.HR.Models.Requests;
using MSU.HR.Models.ViewModels;
using MSU.HR.Services.Interfaces;

namespace MSU.HR.WebClient.Controllers
{
    public class EmployeeController : BaseContoller
    {
        private readonly ILogError _logError;
        private readonly IEmployee _employee;
        private readonly IUser _user;
        private readonly IBank _bank;
        private readonly IDepartment _department;
        private readonly IEducation _education;
        private readonly IGrade _grade;
        private readonly IJob _job;
        private readonly ISection _section;
        private readonly ITypeEmployee _typeEmployee;
        private readonly IPTKP _ptkp;

        public EmployeeController(ILogError logError,
            IEmployee employee,
            IUser user,
            IBank bank,
            IDepartment department,
            IEducation education,
            IGrade grade,
            IJob job,
            ISection section,
            ITypeEmployee typeEmployee,
            IPTKP ptkp)
        {
            _logError = logError;
            _employee = employee;
            _user = user;
            _bank = bank;
            _department = department;
            _education = education;
            _grade = grade;
            _job = job;
            _section = section;
            _typeEmployee = typeEmployee;
            _ptkp = ptkp;
        }

        public async Task<IActionResult> List()
        {
            try
            {
                ViewBag.PageTitle = "List Of Employee";
                ViewBag.BreadcrumbItems = new List<BreadcrumbItemModel>
                {
                    new BreadcrumbItemModel
                    {
                        IsActive = true,
                        Title = "List Employee",
                        Url = "#",
                        Sequence = 1
                    },
                };
                GetAlert();

                return View();
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
                if (string.IsNullOrEmpty(id.ToString()) || id == Guid.Empty)
                    throw new Exception("Id is null or empty");

                ViewBag.PageTitle = "Detail Of Employee";
                ViewBag.BreadcrumbItems = new List<BreadcrumbItemModel>
                {
                    new BreadcrumbItemModel
                    {
                        IsActive = false,
                        Title = "List Employee",
                        Url = "#",
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

                var employee = await _employee.GetEmployeeAsync(id);
                if (employee == null)
                    throw new Exception("Employee not found");

                var model = new EmployeeDetailViewModel();

                var user = await _user.GetProfile(code: employee.Code);
                model.Id = id;
                model.Employee = employee;
                model.User = user;

                model.BankDropdown = (await _bank.GetDropdownModelAsync()).ToList();
                model.DepartmentDropdown = (await _department.GetDropdownModelAsync()).ToList();
                model.EducationDropdown = (await _education.GetDropdownModelAsync()).ToList();
                model.GradeDropdown = (await _grade.GetDropdownModelAsync()).ToList();
                model.JobDropdown = (await _job.GetDropdownModelAsync()).ToList();
                model.SectionDropdown = (await _section.GetDropdownModelAsync(model.Employee.Department.Id)).ToList();
                model.TypeEmployeeDropdown = (await _typeEmployee.GetDropdownModelAsync()).ToList();
                model.PtkpDropdown = (await _ptkp.GetDropdownModelAsync()).ToList();


                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public async Task<JsonResult> GetDataTableEmployee()
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

            var dataTable = await _employee.GetDataTableEmployeeAsync(request);

            var returnObj = Json(dataTable);
            return returnObj;
        }
    }
}

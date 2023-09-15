using MSU.HR.Models.Entities;
using MSU.HR.Models.Others;

namespace MSU.HR.Models.ViewModels
{
    public class EmployeeDetailViewModel
    {
        public Guid Id { get; set; }
        public Employee Employee { get; set; }
        public AspNetUser User { get; set; }

        public List<DropdownModel> BankDropdown { get; set; }
        public List<DropdownModel> DepartmentDropdown { get; set; }
        public List<DropdownModel> EducationDropdown { get; set; }
        public List<DropdownModel> ReligionDropdown { get; set; }
        public List<DropdownModel> GradeDropdown { get; set; }
        public List<DropdownModel> JobDropdown { get; set; }
        public List<DropdownModel> PtkpDropdown { get; set; }        
        public List<DropdownModel> SectionDropdown { get; set; }
        public List<DropdownModel> TypeEmployeeDropdown { get; set; }
    }
}

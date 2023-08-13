using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace MSU.HR.Commons.Attributes
{
    public class BeforeEndDateAttribute : ValidationAttribute
    {
        public string EndDatePropertyName { get; set; }
        //public string? StartDate { get; set; }


        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            PropertyInfo endDateProperty = validationContext.ObjectType.GetProperty(EndDatePropertyName);
            PropertyInfo startDateProperty = validationContext.ObjectType.GetProperty("StartDate");

            DateTime endDate = (DateTime)endDateProperty.GetValue(validationContext.ObjectInstance, null);
            DateTime startDate = (DateTime)startDateProperty.GetValue(validationContext.ObjectInstance, null);

            //var startDate = DateTime.Parse(StartDate);


            if (startDate <= endDate)
                return ValidationResult.Success;

            return new ValidationResult("Error Start and End Date");

            // Do comparison
            // return ValidationResult.Success; // if success
            /* return new ValidationResult("Error");*/ // if fail
        }

    }
}

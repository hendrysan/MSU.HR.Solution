using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace System.Web.Mvc

{
    public static class ViewExtensions
    {
        public static string LocalValidationSummary(this HtmlHelper html, string validationMessage)
        {
            if (!html.ViewData.ModelState.IsValid)
            {
                return "<div class=\"validation-summary\">" + html.ValidationSummary(validationMessage) + "</div>";
            }

            return "";
        }
    }
}

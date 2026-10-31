using Cirs.Web.Validators;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Cirs.Web.Extensions;

public static class ModelStateExtensions
{
    // Copies validator output onto ModelState so the shared views can display it.
    public static void AddValidationResult(this ModelStateDictionary modelState, FormValidationResult result)
    {
        foreach (var (field, messages) in result.Errors)
        {
            foreach (var message in messages)
            {
                modelState.AddModelError(field, message);
            }
        }
    }
}

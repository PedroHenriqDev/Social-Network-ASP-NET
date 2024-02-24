using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SocialWeave.Helpers;

namespace SocialWeave.Attributes
{
    public class LoggedInFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            var controller = context.Controller as Controller;

            if (controller != null && !controller.User.Identity.IsAuthenticated && controller.GetType().Name != "UserController")
            {
                context.Result = new RedirectToActionResult("Login", "User", null);
            }
            else
            {
                base.OnActionExecuted(context);
            }
        }
    }
}

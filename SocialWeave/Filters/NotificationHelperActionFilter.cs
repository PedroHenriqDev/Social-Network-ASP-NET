using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SocialWeave.Helpers;

public class NotificationHelperActionFilter : IAsyncActionFilter
{
    private readonly NotificationHelper _notificationHelper;

    public NotificationHelperActionFilter(NotificationHelper notificationHelper)
    {
        _notificationHelper = notificationHelper;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        try
        {
            context.HttpContext.Items["HasNotifications"] = _notificationHelper;
            await _notificationHelper.SetHasNotificationAsync();
            await next();
        }
        catch (Exception ex)
        {
            if (!context.HttpContext.User.Identity.IsAuthenticated)
            {
                var url = "/User/Login";
                context.Result = new RedirectResult(url);
            }
            else
            {   
                context.Result = new RedirectToActionResult("Error", "Home", null);
                throw;
            }
        }
    }

}
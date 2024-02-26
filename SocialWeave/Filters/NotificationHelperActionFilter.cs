using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SocialWeave.Helpers;

/// <summary>
/// Action filter responsible for handling notifications before and after the execution of an action.
/// </summary>
public class NotificationHelperActionFilter : IAsyncActionFilter
{
    private readonly NotificationHelper _notificationHelper;

    /// <summary>
    /// Initializes a new instance of the <see cref="NotificationHelperActionFilter"/> class.
    /// </summary>
    /// <param name="notificationHelper">An instance of <see cref="NotificationHelper"/>.</param>
    public NotificationHelperActionFilter(NotificationHelper notificationHelper)
    {
        _notificationHelper = notificationHelper;
    }

    /// <summary>
    /// Called asynchronously before the action method is invoked.
    /// </summary>
    /// <param name="context">The <see cref="ActionExecutingContext"/>.</param>
    /// <param name="next">The delegate to execute the next action filter or the action itself.</param>
    /// <returns>A task that represents the asynchronous on action execution operation.</returns>
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        try
        {
            // Set notification helper instance in HttpContext items
            context.HttpContext.Items["HasNotifications"] = _notificationHelper;

            // Set has notification asynchronously
            await _notificationHelper.SetHasNotificationAsync();

            // Execute the next action filter or the action itself
            await next();
        }
        catch (Exception ex)
        {
            if (!context.HttpContext.User.Identity.IsAuthenticated)
            {
                // Redirect to login page if user is not authenticated
                var url = "/User/Login";
                context.Result = new RedirectResult(url);
            }
            else
            {
                // Redirect to error page and rethrow exception
                context.Result = new RedirectToActionResult("Error", "Home", null);
                throw;
            }
        }
    }
}

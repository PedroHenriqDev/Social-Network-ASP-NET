using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;

namespace SocialWeave.Models.Services
{
    /// <summary>
    /// Factory class for creating URL helpers.
    /// </summary>
    public class UrlHelperFactory
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IActionContextAccessor _actionContextAccessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="UrlHelperFactory"/> class.
        /// </summary>
        /// <param name="httpContextAccessor">The HTTP context accessor.</param>
        /// <param name="actionContextAccessor">The action context accessor.</param>
        public UrlHelperFactory(IHttpContextAccessor httpContextAccessor, IActionContextAccessor actionContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _actionContextAccessor = actionContextAccessor;
        }

        /// <summary>
        /// Gets an instance of the URL helper for the specified HTTP request.
        /// </summary>
        /// <param name="request">The HTTP request.</param>
        /// <returns>An instance of the URL helper.</returns>
        public IUrlHelper GetUrlHelper(HttpRequest request)
        {
            var actionContext = _actionContextAccessor.ActionContext ?? new ActionContext(_httpContextAccessor.HttpContext, _httpContextAccessor.HttpContext.GetRouteData(), new ActionDescriptor());
            return new UrlHelper(actionContext);
        }
    }
}

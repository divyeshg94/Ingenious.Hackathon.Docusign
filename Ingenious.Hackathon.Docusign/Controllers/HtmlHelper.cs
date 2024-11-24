using Microsoft.AspNetCore.Mvc.Rendering;

namespace Ingenious.Hackathon.Docusign.Controllers
{
    public static class HtmlHelperExtensions
    {
        public static string ActiveClass(this IHtmlHelper htmlHelper, string route)
        {
            var routeData = htmlHelper.ViewContext.RouteData;
            var controller = routeData.Values["controller"]?.ToString();
            var action = routeData.Values["action"]?.ToString();
            var pageRoute = $"/{controller}/{action}";
            return route == pageRoute ? "active" : "";
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EmpowerU.Controllers
{
    public class BaseController : Controller
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var userId = context.HttpContext.Session.GetString("UserId");
            var controllerName = context.RouteData.Values["controller"].ToString();
            var actionName = context.RouteData.Values["action"].ToString();

            // Allow guests to access the Home page and Business profiles without login
            if (controllerName == "Home" || controllerName == "Business" || actionName == "Details")
            {
                // No redirection needed for Home or Business profile views
                base.OnActionExecuting(context);
                return;
            }

            // If the user is not logged in, redirect them to the login page
            if (string.IsNullOrEmpty(userId))
            {
                // Redirect to login page if the user is not logged in and tries to access restricted actions
                context.Result = RedirectToAction("Login", "Account");
            }
            else
            {
                base.OnActionExecuting(context);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace FIT.Filters
{
    public class LoginFilter : FilterAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            if (!SessionHelpers.IsAutenticado())
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Acceso", action = "Index" }));
            }
        }
    }

    public class NoLoginAttribute : FilterAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            if (SessionHelpers.IsAutenticado())
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Home", action = "Index" }));
            }
        }
    }
}
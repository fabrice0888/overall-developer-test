using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApi.Controllers
{
    public class BaseController : Controller
    {

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (Request.Cookies["username"] ==null || Request.Cookies["username"].Value==null)
            {              

                filterContext.Result = new RedirectResult(Url.Action("Login", "Account"));
            }

        }

      
    }
}
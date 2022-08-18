using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ConsultorioMVC.Filter
{
    public class Seguridad : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var usuario = HttpContext.Current.Session["idUsuario"];
            if (usuario == null)
            {
                filterContext.Result = new RedirectResult("~/Main");
            }
            base.OnActionExecuting(filterContext);
        }
    }
}
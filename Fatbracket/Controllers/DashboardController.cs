using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Fatbracket.Controllers
{
    public class DashboardController : Controller
    {
        // GET: Dashboard
        public ActionResult Home()
        {
            HttpCookie UserEmail = Request.Cookies["UserEmail"];

            ViewBag.UserEmail = UserEmail.Value;

            return View();
        }
    }
}
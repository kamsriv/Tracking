using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FinTracker.Models;

namespace FinTracker.Controllers.Home
{
    public class ActivityController : Controller
    {
        //
        // GET: /Activity/

        public ActionResult Index()
        {
            return View();
        }

        public JsonResult List(Int32 IsActive=0)
        {
            repository rep = new repository();
            var result = Json(rep.getActivities(IsActive), JsonRequestBehavior.AllowGet);
            return result;
        }

    }
}

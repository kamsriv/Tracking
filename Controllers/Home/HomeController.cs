using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FinTracker.Models;

namespace FinTracker.Controllers.Home
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            return View();
        }

     /*   public ActionResult CRUD() 
        {
            return PartialView();
        }

        public ActionResult logout()
        {
            return PartialView();
        }

        public JsonResult save(Employee data)
        {
            repository save = new repository();
            save.save(data);
            return null;
        }

        public JsonResult getList()
        {
            var emp = new List<Employee>();
            repository getdata = new repository();
            emp = getdata.getList();
            return Json(emp,JsonRequestBehavior.AllowGet);
        }

        public JsonResult delete(Employee emp)
        {
            repository del = new repository();
            del.delete(emp.id);
            return null;
        }

        public JsonResult update(Employee emp)
        {
            repository up = new repository();
            up.update(emp);
            return null;
        }
        */
    }
}

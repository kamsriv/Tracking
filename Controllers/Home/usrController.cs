using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FinTracker.Models;
using FinTracker.Models.ViewModel;

namespace FinTracker.Controllers.Home
{
    public class usrController : Controller
    {
        //
        // GET: /usr/

        public ActionResult Index()
        {
            return View();
        }
       
        public ActionResult list()
        {
            if (!Common.GetUser.IsActive || !Common.GetUser.IsAdmin)
                return View("UnAutherized");//Need to create an unautherized view
            return PartialView();
        }
        public JsonResult getList()
        {
            UserViewModel wms = new UserViewModel();
            repository getdata = new repository();
            wms = getdata.GetUsers();
            return Json(wms, JsonRequestBehavior.AllowGet);
        }
        public JsonResult save(User data)
        {
            repository objRep = new repository();
            return Json(objRep.save(data), JsonRequestBehavior.AllowGet);
        }
        public JsonResult update(User data)
        {
            repository objRep = new repository();
            return Json(objRep.update(data), JsonRequestBehavior.AllowGet);
        }
        public JsonResult delete(String data)
        {
            repository objRep = new repository();
            return Json(objRep.DeleteUser(data), JsonRequestBehavior.AllowGet);
        }
    }
}

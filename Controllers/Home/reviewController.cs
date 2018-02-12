using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FinTracker.Models;
using FinTracker.Models.ViewModel;
using System.Collections.Specialized;
namespace FinTracker.Controllers.Home
{
    public class reviewController : Controller
    {
        //
        // GET: /wm/

        public ActionResult list()
        {
            if (!Common.GetUser.IsActive || !Common.GetUser.IsReviewer)
                return View("UnAutherized");//Need to create an unautherized view
            return PartialView();
        }
        public JsonResult save(WorkMaint data)
        {
            repository save = new repository();
            return Json(save.insert_reviewerdata(data), JsonRequestBehavior.AllowGet);
        }


        public JsonResult getPending(FilterAndPagerInfo objFilterInfo)
        {
            WMViewModel wms = new WMViewModel("", false);
            repository getdata = new repository();

            wms = getdata.getPendingList(objFilterInfo);
            return Json(wms, JsonRequestBehavior.AllowGet);
        }
        /*
        public JsonResult getPending()
        {
            WMViewModel wms = new WMViewModel("",false);
            repository getdata = new repository();
            wms = getdata.getPendingList();
            return Json(wms, JsonRequestBehavior.AllowGet);
        }*/
        public JsonResult delete(WorkMaint wm)
        {
            repository del = new repository();
            del.delete(wm.WorkItemId);
            return null;
        }
        public JsonResult insertWMDetails(WorkMaint wm)
        {
            repository objRep = new repository();
            return Json(objRep.insert_reviewerdata(wm), JsonRequestBehavior.AllowGet);
        }
        public JsonResult update(WorkMaint wm)
        {
            repository objRep = new repository();
            return Json(objRep.update_reviewerdata(wm), JsonRequestBehavior.AllowGet);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FinTracker.Models;
using FinTracker.Models.ViewModel;
namespace FinTracker.Controllers.Home
{
    public class wmController : Controller
    {
        //
        // GET: /wm/

        public ActionResult list()
        {
            if (!Common.GetUser.IsActive)
                return View("UnAutherized");//Need to create an unautherized view
            //Based on the recent 12/15/2016 discussion with shashank there should be AP and expense teams tracker
            String team = Common.GetUser.AssignedTeam;
            if (team.Contains("|"))
            {
                //need to ask the user to choose from the available teams and direct to that view.
                //otherwise send them to their respective view.
                return RedirectToAction("TeamSelection");
            }
            switch(team)
            {
                case "ap":
                    return RedirectToAction("APTeam");
                case "ex":
                    return RedirectToAction("ExpTeam");
                case "tr":
                    return RedirectToAction("TrTeam");
                default:
                    return RedirectToAction("ArTeam");
                    
            }
        }

        public ActionResult TrTeam()
        {
            return PartialView("TrFPlist");
        }
        public ActionResult FPTeam()
        {
            return PartialView("FPlist");
        }
        public ActionResult ArTeam()
        {
            return PartialView("list");
        }

        public ActionResult APTeam()
        {
            return PartialView("APlist");
        }
        public ActionResult ExpTeam()
        {
            return PartialView("Explist");
        }

        public PartialViewResult TeamSelection()
        {
            return PartialView();
        }

        public JsonResult save(WorkMaint data)
        {
            repository save = new repository();
            return Json(save.save(data), JsonRequestBehavior.AllowGet);
        }
       
        public JsonResult getList(String team="")
        {
            WMViewModel wms = new WMViewModel("",false);
            repository getdata = new repository();
            getdata.Team = team;
            wms = getdata.getList();
            return Json(wms, JsonRequestBehavior.AllowGet);
        }
        public JsonResult delete(WorkMaint wm)
        {
            repository del = new repository();
            del.delete(wm.WorkItemId);
            return null;
        }
        public JsonResult update(WorkMaint wm)
        {
            repository objRep = new repository();
            return Json(objRep.update(wm), JsonRequestBehavior.AllowGet);
        }


        public PartialViewResult viewEntries()
        {
            return PartialView();
        }

        public JsonResult loadAllData()
        {
            repository objRep = new repository();
            return Json(objRep.GetWorkMaintData(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult loadItemByStatus(String mStatus)
        {
            repository objRep = new repository();
            return Json(objRep.GetWorkMaintData(mStatus), JsonRequestBehavior.AllowGet);
        }
    }
}

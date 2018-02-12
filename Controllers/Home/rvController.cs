using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FinTracker.Models;
using FinTracker.Models.ViewModel;

namespace FinTracker.Controllers.Home
{
    public class rvController:Controller 
    {
      public ActionResult list()
        {
            return PartialView();
        }
      public JsonResult GetReviewers()
      {
          repository getdata = new repository();
          ReviewerMgmt rmgmt = new ReviewerMgmt();
          UserViewModel uModel = new UserViewModel();
          uModel = getdata.GetUsers();
          rmgmt = getdata.GetReviewers();
          //remove the users who already have the reviewer role
          foreach(var p in rmgmt.lst){
              if (uModel.lst.Exists(s => s.UserId == p.ReviewerId))
              {
                  uModel.lst.Remove(uModel.lst.Single(s => s.UserId == p.ReviewerId));
              }
          }
          rmgmt.AvailableUsers = uModel.lst;
          return Json(rmgmt, JsonRequestBehavior.AllowGet);
      }
      public JsonResult delete(Reviewer rv)
      {
          repository objRep = new repository();
          return Json(objRep.delete(rv), JsonRequestBehavior.AllowGet);
      }
      public JsonResult save(Reviewer data)
      {
          repository objRep = new repository();
          return Json(objRep.save(data), JsonRequestBehavior.AllowGet);
      }
      
    }
}
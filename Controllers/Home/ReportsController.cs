using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FinTracker.Models;
using FinTracker.Models.ViewModel;
using System.IO;
using FinTracker.Utilities;
namespace FinTracker.Controllers.Home
{
    //for downloading the files used jquery plugin from http://johnculviner.com/jquery-file-download-plugin-for-ajax-like-feature-rich-file-downloads/
    public class reportsController : Controller
    {
        public ActionResult list(string generated="")
        {
            ViewBag.ReportGenerated = generated;
            return PartialView();
        }
        public JsonResult getErrorsReport()
        {
            repository objRep = new repository();
            objRep.GetErrorsReport();
            return null;
        }
        public ActionResult generateFile(WMReportInputs input)
        {
            try
            {
                Response.Cookies.Remove("fileDownload");
                repository objRep = new repository();
                WMRPTErrViewModel rep = objRep.GetHistory(input);
                XLExport objExp = new XLExport(rep);
                string flName = objExp.ExcelExport();
                return Json(new { success = true, fileName = flName }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public FileResult getHistory(string fileName)
        {
            try
            {
                Response.SetCookie(new HttpCookie("fileDownload", "true") { Path = "/" });
                return File(fileName, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", Path.GetFileName(fileName));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}

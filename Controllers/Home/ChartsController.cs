using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FinTracker.Models;
using System.Web.Script.Serialization;
using System.Text;
using FinTracker.Utilities;
using System.IO;

namespace FinTracker.Controllers.Home
{
    public class ChartsController : Controller
    {
        //
        // GET: /Charts/

        class procActivityData
        {
           public  Dictionary<String, String> actList;
           public Decimal totTime;
        }
        public ActionResult getChartsData(FilterAndPagerInfo filterInfo)
        {
            repository objRep = new repository();
            //FilterAndPagerInfo filterInfo = new FilterAndPagerInfo();
            List<FinTracker.Models.Filter> f = filterInfo.Filters != null ? filterInfo.Filters.ToList() : new List<FinTracker.Models.Filter>();
            User loginUser = Common.GetUser;


            Dictionary<String, procActivityData> procActivities = new Dictionary<String, procActivityData>();

            //For testing f.Add(new Models.Filter() { Key = "startdate", Value = "2017-12-29" });
            if (!Common.GetUser.IsAdmin)
            {
                f.Clear();//he should get all the users of that team.
                f.Add(new Models.Filter() { Key = "rcid", Value = loginUser.UserId });
                filterInfo.Filters = f.ToArray();
            }

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            WMDashboardInfoList data = objRep.GetChartData(filterInfo);
            List<String> activities = new List<String>();
            int cnt = 0;

            
            //build a string to pass on to the UI layer.           
            StringBuilder sb = new StringBuilder();
            foreach (WMDashboardInfo info in data.LstByProc)
            {
                if (cnt > 0) sb.Append(",");
                sb.AppendFormat("['{0}', {1}, {2}]", info.User, info.Target.ToString(), info.TotalTime.ToString());
                cnt++;
            }
            sb.Append("|");
            //Activites only
            cnt = 0;
            if (data.LstByProcAct.Count > 0)
            {
                sb.Append("['Name',");
                sb.Append("'Target Hrs',");

                //from db if we get pivot table also we can't guarantee that columns will always be same.
                //So writing a code that seggregates the values based on the activities
                foreach (WMDashboardInfo info in data.LstByProcAct)
                {
                    if (!activities.Exists(p => p == info.Activity))
                    {
                        if (cnt > 0) sb.Append(",");
                        activities.Add(info.Activity);
                        sb.AppendFormat("'{0}'", info.Activity);
                    }
                    cnt++;
                }
                sb.Append("]");
            }
            
            //We have the activities that are not duplicated so for every user initialize the activities.
            foreach (WMDashboardInfo info in data.LstByProcAct)
            {
                Dictionary<String, String> objActs = new Dictionary<String, String>();
                foreach (String act in activities)
                {
                    objActs.Add(act, "0.0");
                }
                if (!procActivities.ContainsKey(info.User))
                {
                    procActivities.Add(info.User,new procActivityData(){ actList = objActs, totTime=info.Target });
                }
            }

            

            sb.Append("|");
            //Processor by Activity
            
            cnt = 0;

            foreach (String act in activities)  //This need to be changed to accommodate user grouping and summation
            {
                //For all the available activities we should have an entry
                foreach (WMDashboardInfo info in data.LstByProcAct) // check this activity exists for this processor then assign the total time otherwise 0
                {
                    if (act == info.Activity)
                    {
                        procActivities[info.User].actList[act] = info.TotalTime.ToString();
                    }
                }
            }



            foreach (KeyValuePair<String, procActivityData> info in procActivities) //This need to be changed to accommodate user grouping and summation
            {
                if (cnt > 0) sb.Append(",");
                sb.AppendFormat("['{0}', {1}", info.Key, info.Value.totTime);
                info.Value.actList.ToList().ForEach(p => sb.AppendFormat(",{0}", p.Value));
                sb.Append("]");
                cnt++;
            }

            sb.Append("|");
            //Review information
            cnt = 0;
            foreach (WMDashboardInfo info in data.LstByRev)
            {
                if (cnt > 0) sb.Append(",");
                sb.AppendFormat("['{0}', {1}, {2}]", info.User, info.Target.ToString(), info.TotalTime.ToString());
                cnt++;
            }
            sb.Append("|");
            //Reviewer Activites only
            activities.Clear();
            procActivities.Clear();
            if (data.LstByRevAct.Count > 0)
            {
                sb.Append("['Name',");
                sb.Append("'Target Hrs',"); cnt = 0;
                foreach (WMDashboardInfo info in data.LstByRevAct)
                {
                    if (!activities.Exists(p => p == info.Activity))
                    {
                        if (cnt > 0) sb.Append(",");
                        activities.Add(info.Activity);
                        sb.AppendFormat("'{0}'", info.Activity);
                    }
                    cnt++;
                }
                sb.Append("]");
            }

            foreach (WMDashboardInfo info in data.LstByRevAct)
            {
                Dictionary<String, String> objActs = new Dictionary<String, String>();
                foreach (String act in activities)
                {
                    objActs.Add(act, "0.0");
                }
                if (!procActivities.ContainsKey(info.User))
                {
                    procActivities.Add(info.User, new procActivityData() { actList = objActs, totTime = info.Target });
                }
            }

            sb.Append("|");
            //Reviewer by Activity
            cnt = 0;
            foreach (WMDashboardInfo info in data.LstByRevAct) //This need to be changed to accommodate user grouping and summation
            {
                //For all the available activities we should have an entry
                foreach (String act in activities) // check this activity exists for this processor then assign the total time otherwise 0
                {
                    if (act == info.Activity)
                    {
                        procActivities[info.User].actList[act] = info.TotalTime.ToString();
                    }
                }
            }
            foreach (KeyValuePair<String, procActivityData> info in procActivities) //This need to be changed to accommodate user grouping and summation
            {
                if (cnt > 0) sb.Append(",");
                sb.AppendFormat("['{0}', {1}", info.Key, info.Value.totTime);
                info.Value.actList.ToList().ForEach(p => sb.AppendFormat(",{0}", p.Value));
                sb.Append("]");
                cnt++;
            }
     
            TempData["Processor"] = sb.ToString(); //Wanted to use this information in another action method when exporting to excel sheet.

            return Json(TempData["Processor"], JsonRequestBehavior.AllowGet);
        }

        public FileResult exportChartData()
        {
           XLExport objExp = new XLExport(Convert.ToString(TempData["processor"]));
           string flName = objExp.ExcelExportChartData();
           return File(flName, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", Path.GetFileName(flName));
        }

        public ActionResult showdailyCharts(FilterAndPagerInfo filterInfo)
        {
            repository objRep = new repository();
            List<FinTracker.Models.Filter> f = filterInfo.Filters != null ? filterInfo.Filters.ToList() : new List<FinTracker.Models.Filter>();
            User loginUser = Common.GetUser;

            f.Add(new Models.Filter() { Key = "rcid", Value = loginUser.UserId });
            if (Common.GetUser.IsAdmin)
            {
                // f.Clear();//he should get all the users of that team.
                //f.Add(new Models.Filter() { Key = "team", Value = loginUser.AssignedTeam });
                filterInfo.Filters = f.ToArray();
            }

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            WMDashboardInfoList data = objRep.GetChartData(filterInfo);
            //build a string to pass on to the UI layer.
            StringBuilder sb = new StringBuilder();
            foreach (WMDashboardInfo info in data.LstByProc)
            {
                sb.AppendFormat("['{0}', {1}, {2}]", info.User, info.Target.ToString(), info.TotalTime.ToString());
            }
            ViewBag.Processor = sb.ToString();

            ViewBag.ProcessorByAct = serializer.Serialize(data.LstByProcAct);
            ViewBag.Reviewer = serializer.Serialize(data.LstByRev);
            ViewBag.ReviewerByAct = serializer.Serialize(data.LstByRevAct);
            return PartialView();
        }
    }
}

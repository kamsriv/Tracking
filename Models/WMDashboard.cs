using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinTracker.Models
{
    public class WMDashboardInfo
    {
        public String User { get; set; }
        public String Activity { get; set; }
        public Decimal TotalTime { get; set; }
        public Int32 Target { get; set; }
    }

    public class WMDashboardInfoList:Info
    {
        public WMDashboardInfoList(String msg, Boolean isSuccess) : base(msg, isSuccess) { }
        public List<WMDashboardInfo> LstByProcAct { get; set; } //List by processor and activity
        public List<WMDashboardInfo> LstByProc { get; set; } //List by processor
        public List<WMDashboardInfo> LstByRevAct { get; set; } //List by Reviewer and activity
        public List<WMDashboardInfo> LstByRev { get; set; } //List by Reviewer
    }
}
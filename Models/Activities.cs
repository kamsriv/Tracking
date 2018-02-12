using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinTracker.Models
{
    public class Activities
    {
        public Int32 activityid { get; set; }
        public String description { get; set; }
        public Boolean isActive { get; set; }
    }
    public class ActivityList
    {
        public List<Activities> lst { get; set; }
    }
}
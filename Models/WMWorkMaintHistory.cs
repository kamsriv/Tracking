using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinTracker.Models
{
    public class WMWorkMaintHistory
    {
        public Int32 HistoryId { get; set; }
        public Int32 WorkitemId { get; set; }
        public Int32 ActivityId { get; set; }
        public String ActivityName { get; set; }
        public String ItemType { get; set; }
        public String Comments { get; set; }
        public String UserId { get; set; }
        public DateTime StartedDate { get; set; }
        public Int32 ReviewerId { get; set; }
        public String ReviewComments{get;set;}
        public String Status { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinTracker.Models
{
    public class Reviewer
    {
        public String[] RevieweeId { get; set; }
        public String ReviewerName { get; set; }
        public String ReviewerId { get; set; }
        public String RevieweeName { get; set; }
        public Boolean IsPrimary { get; set; }
    }
}
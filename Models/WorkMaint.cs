using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinTracker.Models
{
    public class WorkMaint
    {
        public Int32 WorkItemId { get; set; }
        public Int32 ActivityId { get; set; }
        public String ActivityName { get; set; }
        public String InvoiceNumber { get; set; }
        public String RequestNumber { get; set; }
        public String Comments { get; set; }
        public String UserId { get; set; }
        public DateTime StartedDate { get; set; }
        public String ReviewerComments { get; set; }
        public String Status { get; set; }
        public Boolean ErrFound { get; set; }
        public String ReviewerName { get; set; }
        public String Team { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinTracker.Models
{
    public class WMDetails
    {
        public WorkMaint WorkMaint { get; set; }
        public Int32 WorkItemId { get; set; }
        public Int32 ReviewerId { get; set; }
        public String ReviewComments { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
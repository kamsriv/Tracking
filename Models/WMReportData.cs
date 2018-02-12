using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinTracker.Models
{
    //We are planning to give three different reports for the team one is with all the data and other one is for time taken each item, next is reviewer time taken.
    public class WMReportData_AR : WMReportData
    {
        public String HistoryStatus { get; set; }
        public String HistoryUpdatedBy { get; set; }
        public DateTime HistoryUpdatedAt { get; set; }

        public String PresentStatus { get; set; }
        public String WatingingFor { get; set; }
        public DateTime LastUpdatedAt { get; set; }
        public String LastUpdatedBy { get; set; }

        public Int32 TimeTaken { get; set; }
        public String ReviewerComments { get; set; }
        public String ReviewedBy { get; set; }
    }

    public class WMReportData
    {
        public Int32 WorkItemId { get; set; }
        public String ActivityName { get; set; }
        public String InvoiceNumber { get; set; }
        public String RequestNumber { get; set; }
        public String Comments { get; set; }
        public String UserName { get; set; }
        public DateTime StartedDate { get; set; }

        public Boolean ErrFound { get; set; }

        public DateTime? Created { get; set; }
        public DateTime? Submitted { get; set; }
        public DateTime? ReSubmitted { get; set; }
        public DateTime? ReworkingAgin { get; set; }
        public DateTime? InReview { get; set; }
        public DateTime? Rework { get; set; }
        public DateTime? CorrectedErrors { get; set; }
        public DateTime? Approved { get; set; }
    }

    public class WMHistory
    {
        public Int32 WorkItemId { get; set; }
        public String ActivityName { get; set; }
        public String InvoiceNumber { get; set; }
        public String RequestNumber { get; set; }
        public String Comments { get; set; }
        public String CreatedBy { get; set; }
        public DateTime StartedDate { get; set; }
        public Boolean ErrFound { get; set; }
        public String ReviewerComments { get; set; }
        public String ReviewedBy { get; set; }
        public String PresentStatus { get; set; }
    }

    public class WMReportTimeTaken
    {
        public Int32 WorkItemId { get; set; }
        public String UserId { get; set; }
        public String ActivityName { get; set; }
        public String InvoiceNumber { get; set; }
        public String RequestNumber { get; set; }
        public Int32 TimeTaken_Sec { get; set; }
    }

    public class WMReportReviewerTime
    {
        public String ReviewerName { get; set; }
        public Int32 TimeTaken_Sec { get; set; }
    }

    public class WMReportReviewerData
    {
        public Int32 WorkItemId { get; set; }
        public String UserId { get; set; }
        public String ActivityName { get; set; }
        public String InvoiceNumber { get; set; }
        public String RequestNumber { get; set; }
        public String Comments { get; set; }
        public Boolean ErrorFound { get; set; }
        public String ReviewerName { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public Int32 TimeTaken_Sec { get; set; }
    }

    public class WMReportProcessorSummary
    {
        public String ProcessorName { get; set; }
        public String Description { get; set; }
        public Decimal TotalTimeTaken { get; set; }
    }

    public class WMReportInputs
    {
        public String RptType { get; set; }
        public String UserType { get; set; }
        public Boolean IsGenerating { get; set; }
        public DateTime RptDate { get; set; }
        public String TeamName { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
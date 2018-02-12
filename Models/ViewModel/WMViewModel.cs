using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinTracker.Models.ViewModel
{
    /// <summary>
    /// Class used by the Work Maintenance view
    /// </summary>
    public class WMViewModel: Info
    {
        public WMViewModel(String msg, Boolean flg) : base(msg, flg) { }
        public List<WorkMaint> WorkMaintList { get; set; }
        public ActivityList Activities { get; set; }
        public Int32 TotalRows { get; set; }
    }
    /// <summary>
    /// Used by the update methods which will broadcast the information to other controllers.
    /// </summary>
    public class WMUpdateViewModel : Info
    {
        public WMUpdateViewModel(String msg, Boolean flg) : base(msg, flg) { }
        public Int32 WMCnt { get; set; }
        public String RevieweeId { get; set; }
    }
    /// <summary>
    /// Class that can be used by the users controller view
    /// </summary>
    public class UserViewModel : Info
    {
        public UserViewModel() { }
        public UserViewModel(String msg, Boolean flg) : base(msg, flg) { }
        public List<User> lst { get; set; }
    }
    /// <summary>
    /// Class that can be used by the reviewers view
    /// </summary>
    public class ReviewerMgmt : Info
    {
        public ReviewerMgmt() : base() { }
        public ReviewerMgmt(String msg, Boolean flg) : base(msg, flg) { }
        public List<User> AvailableUsers { get; set; }
        public List<Reviewer> lst { get; set; }
    }

    /// <summary>
    /// Class that contains all the information of work details those are waiting for review
    /// </summary>
    public class WMWaitingReview : Info
    {
         public WMWaitingReview() : base() { }
         public WMWaitingReview(String msg, Boolean flg) : base(msg, flg) { }
         public List<WMDetails> lst { get; set; }
    }
    /// <summary>
    /// Report generation modal - this is used by on screen report.
    /// </summary>
    public class WMRPTErrViewModel : Info
    {
        public WMRPTErrViewModel(String msg, Boolean flg) : base(msg, flg) { }
        /// <summary>
        /// Contains all the data 
        /// </summary>
        public List<WMHistory> lst { get; set; }
        /// <summary>
        /// Contains only the porcessor data
        /// </summary>
        public List<WMReportTimeTaken> plst { get; set; }
        /// <summary>
        /// Contains only the reviewers data
        /// </summary>
        public List<WMReportReviewerTime> rlst { get; set; }
        /// <summary>
        /// Contains the summary information of Processor
        /// </summary>
        public List<WMReportProcessorSummary> psummary { get; set; }
        /// <summary>
        /// Contains the total information of a reviewer for that given dates
        /// </summary>
        public List<WMReportReviewerData> rDlst { get; set; }
    }

    /// <summary>
    /// THis class will be used by the daily report which usually ran by the schedular.
    /// </summary>
    public class WMReportDataViewModal : Info
    {
        public WMReportDataViewModal(String msg, Boolean flg) : base(msg, flg) { }
        /// <summary>
        /// Contains all the report data belongs to AR team
        /// </summary>
        public List<WMReportData_AR> lst_ar { get; set; }
        /// <summary>
        /// Contains all the report data belongs to AP team
        /// </summary>
        public List<WMReportData> list_ap { get; set; }
        /// <summary>
        /// Contains all the report data belongs to expense team
        /// </summary>
        public List<WMReportData> list_ex { get; set; }
        /// <summary>
        /// Contains all the report data belongs to FPNA team
        /// </summary>
        public List<WMReportData> list_fpna { get; set; }


    }
}
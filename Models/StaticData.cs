using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinTracker.Models
{
    public class StaticData
    {
        //GET OPERATIONS
        public const String GET_ACTIVITIES = "usp_getactivities";
        public const String GET_USERS = "usp_getusers";
        public const String GET_WORKMAINTS = "usp_getworkmaintenance"; //Not using as of nov1st2016.
        public const String GET_WORKMAINT_PENDING_SUBMIT = "usp_getworkmaint_submit_pending";
        public const String GET_WORKMAINT_PENDING_REVIEW = "usp_getworkmaint_submit_review";
        public const String GET_REVIEWERS = "usp_getreviewers";
        //CREATE UPDATE DELTE
        public const String CREATE_WORKMAINTENANCE = "usp_createworkmaintenance";
        public const String UPDATE_WORKMAINTENANCE = "usp_updateworkmaintenance";
        public const String UPDATE_REVIEWER_WORKMAINTENANCE = "usp_updateworkmaintenance_review";
        public const String CREATE_REVIEWER_WORKMAINTENANCE = "usp_createworkmaintenance_detail";

        public const String CREATE_USER = "usp_createuser";
        public const String UPDATE_USER = "usp_updateuser";
        public const String DELETE_USER = "usp_deleteuser";

        public const String CREATE_REVIEWER = "usp_createreviewer";
        public const String DELETE_REVIEWER = "usp_deletereviewer";
        
        //REPORT GENERATION
        public const String GET_REPORT_BY_DATE_AR = "usp_finreport_betweendates_ar";
        public const String GET_REPORT_BY_DATE_AP = "usp_finreport_betweendates_ap";

        //DAILY REPORT OUT
        public const String GET_FINANCETEAM_REPORT_DAILY = "usp_finreport_daily";

        //DASHBOARD INFORMATION
        public const String GET_DASHBOARD_DATA = "usp_getproductivity_daily";
    }
}
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Configuration;
using FinTracker.Models.ViewModel;

namespace FinTracker.Models
{
    public class repository
    {
        SqlConnection con = null;
        String conStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        internal string Team { get; set; }
        #region WorkMaintenance
        public Info save(WorkMaint data)
        {
            SqlCommand cmd = null; String team = data.Team;
            if (String.IsNullOrEmpty(data.Team))
                team = Common.GetUser.AssignedTeam;
            Info retVal = new Info(FinTrackRes.WM_SAVE_ERR, false);
            try
            {
                using (con = new SqlConnection(conStr))
                {
                    con.Open();
                    using (cmd = new SqlCommand(StaticData.CREATE_WORKMAINTENANCE, con))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@activityid", data.ActivityId);
                        cmd.Parameters.AddWithValue("@invoicenum", data.InvoiceNumber);
                        cmd.Parameters.AddWithValue("@requestnum", data.RequestNumber);
                        cmd.Parameters.AddWithValue("@comments", data.Comments);
                        cmd.Parameters.AddWithValue("@userid", Common.GetUser.UserId);
                        cmd.Parameters.AddWithValue("@starteddate", DateTime.Now);
                        cmd.Parameters.AddWithValue("@itemstatus", "Created");
                        cmd.Parameters.AddWithValue("@team", team);
                        cmd.ExecuteNonQuery();
                        retVal._message = FinTrackRes.WM_SAVE;
                        retVal._success = true;
                    }
                }
            }
            catch (SqlException ex)
            {
                retVal._message = ex.Message;
                retVal._success = false;
            }
            catch (Exception ex)
            {
                Logger.Log(Logger.LogType.Error, ex.ToString());
            }
            return retVal;
        }
        public WMUpdateViewModel update(WorkMaint data)
        {
            WMUpdateViewModel retVal = new WMUpdateViewModel(FinTrackRes.WM_UPDATE_ERR, false);
            SqlCommand cmd = null;
            try
            {
                using (con = new SqlConnection(conStr))
                {
                    con.Open();
                    using (cmd = new SqlCommand(StaticData.UPDATE_WORKMAINTENANCE, con))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@workitemid", data.WorkItemId);
                        cmd.Parameters.AddWithValue("@userid", Common.GetUser.UserId);
                        cmd.Parameters.AddWithValue("@comments", data.Comments);
                        cmd.Parameters.AddWithValue("@starteddate", DateTime.Now);
                        cmd.Parameters.AddWithValue("@itemstatus", data.Status);
                        cmd.Parameters.AddWithValue("@usertype", "Processor");
                        cmd.Parameters.AddWithValue("@invoicenumber", data.InvoiceNumber);
                        cmd.Parameters.AddWithValue("@requestnumber", data.RequestNumber);
                        SqlDataReader rdr = cmd.ExecuteReader();

                        if (rdr.Read())
                        {
                            retVal.WMCnt = Convert.ToInt32(rdr["revcnt"]);
                        }

                        retVal._success = true;
                        if (data.Status.Contains("Submitted"))
                            retVal._message = FinTrackRes.WM_UPDATE;
                        else
                            retVal._message = String.Empty;

                        CloseReader(rdr);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(Logger.LogType.Error, ex.ToString());
            }
            return retVal;
        }
        private void CloseReader(SqlDataReader rdr)
        {
            if (rdr != null)
            {
                rdr.Close();
                rdr.Dispose();
            }
        }
        public ActivityList getActivities(Int32 IsActive)
        {
            SqlCommand cmd = null;
            ActivityList activities = new ActivityList();
            try
            {
                using (con = new SqlConnection(conStr))
                {
                    con.Open();
                    using (cmd = new SqlCommand(StaticData.GET_ACTIVITIES, con))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@status", Convert.ToBoolean(IsActive));
                        cmd.Parameters.AddWithValue("@userid", Common.GetUser.UserId);
                        SqlDataReader rdr = cmd.ExecuteReader();

                        activities.lst = new List<Activities>();
                        while (rdr.Read())
                        {
                            Activities act = new Activities();
                            act.activityid = Convert.ToInt32(rdr["activityid"]);
                            act.description = Convert.ToString(rdr["description"]);
                            act.isActive = Convert.ToBoolean(rdr["isactive"]);
                            activities.lst.Add(act);
                        }
                        CloseReader(rdr);
                    }
                }
                return activities;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public WMViewModel getList()
        {
            WMViewModel workms = new WMViewModel("", false);
            workms.Activities = new ActivityList();
            List<WorkMaint> wm = new List<WorkMaint>();
            List<Activities> act = new List<Activities>();

            String sqlCmd = String.Empty;
            SqlCommand cmd = null;

            try
            {
                using (con = new SqlConnection(conStr))
                {
                    con.Open();

                    //Check the reviewer is logged in or the user is logged in. based on this we need to generate the list
                    //if (Common.GetUser.IsNormalUser)
                    sqlCmd = StaticData.GET_WORKMAINT_PENDING_SUBMIT;
                    //else sqlCmd = StaticData.GET_WORKMAINT_PENDING_REVIEW;
                    using (cmd = new SqlCommand(sqlCmd, con))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@CreatedBy", Common.GetUser.UserId);
                        cmd.Parameters.AddWithValue("@Team", String.IsNullOrEmpty(Team)? Common.GetUser.AssignedTeam : Team);
                        SqlDataReader rdr = cmd.ExecuteReader();
                        while (rdr.Read())
                        {
                            WorkMaint e = new WorkMaint();
                            e.WorkItemId = Convert.ToInt32(rdr["workitemid"]);
                            e.ActivityId = Convert.ToInt32(rdr["activityid"]);
                            e.InvoiceNumber = Convert.ToString(rdr["invoice_number"]);
                            e.ActivityName = Convert.ToString(rdr["activityname"]);
                            e.RequestNumber = Convert.ToString(rdr["request_number"]);
                            e.Comments = Convert.ToString(rdr["comments"]);
                            e.UserId = Convert.ToString(rdr["userid"]);
                            e.StartedDate = Convert.ToDateTime(rdr["starteddate"]);
                            e.Status = Convert.ToString(rdr["itemstatus"]);
                            wm.Add(e);
                        }
                        workms.WorkMaintList = wm;
                        rdr.NextResult();
                        while (rdr.Read())
                        {
                            Activities a = new Activities();
                            a.activityid = Convert.ToInt32(rdr["activityid"]);
                            a.description = Convert.ToString(rdr["description"]);
                            act.Add(a);
                        }
                        workms.Activities.lst = act;

                        CloseReader(rdr);
                        workms._success = true;
                    }
                }
            }
            catch (Exception ex)
            {
                workms._message = FinTrackRes.WM_RETRIEVE_ERR;
                Logger.Log(Logger.LogType.Error, ex.ToString());
            }
            return workms;
        }
        public void delete(int data)
        {
            con.Open();
            string query = "delete from [tbl_workmaintenance] where workitemid=@id";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@id", data);
            cmd.ExecuteNonQuery();
            con.Close();
        }

        //reviewer related functions
        public WMViewModel getPendingList()
        {
            WMViewModel workms = new WMViewModel("", false);
            workms.Activities = new ActivityList();
            List<WorkMaint> wm = new List<WorkMaint>();
            List<Activities> act = new List<Activities>();

            SqlCommand cmd = null;
            String sqlCmd = StaticData.GET_WORKMAINT_PENDING_REVIEW;

            try
            {
                using (con = new SqlConnection(conStr))
                {
                    con.Open();
                    using (cmd = new SqlCommand(sqlCmd, con))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@CreatedBy", Common.GetUser.UserId);

                        SqlDataReader rdr = cmd.ExecuteReader();
                        while (rdr.Read())
                        {
                            WorkMaint e = new WorkMaint();
                            e.WorkItemId = Convert.ToInt32(rdr["workitemid"]);
                            e.ActivityId = Convert.ToInt32(rdr["activityid"]);
                            e.InvoiceNumber = Convert.ToString(rdr["invoice_number"]);
                            e.ActivityName = Convert.ToString(rdr["activityname"]);
                            e.RequestNumber = Convert.ToString(rdr["request_number"]);
                            e.Comments = Convert.ToString(rdr["comments"]);
                            e.UserId = Convert.ToString(rdr["userid"]);
                            e.StartedDate = Convert.ToDateTime(rdr["starteddate"]);
                            e.Status = Convert.ToString(rdr["itemstatus"]);
                            wm.Add(e);
                        }
                        workms.WorkMaintList = wm;
                        CloseReader(rdr);
                        workms._success = true;
                    }
                }
            }
            catch (Exception ex)
            {
                workms._message = FinTrackRes.WM_RETRIEVE_ERR;
                Logger.Log(Logger.LogType.Error, ex.ToString());
            }
            return workms;
        }
        public WMViewModel getPendingList(FilterAndPagerInfo filterInfo)
        {
            WMViewModel workms = new WMViewModel("", false);
            workms.Activities = new ActivityList();
            List<WorkMaint> wm = new List<WorkMaint>();
            List<Activities> act = new List<Activities>();

            SqlCommand cmd = null;
            String sqlCmd = StaticData.GET_WORKMAINT_PENDING_REVIEW;

            try
            {
                using (con = new SqlConnection(conStr))
                {
                    con.Open();
                    using (cmd = new SqlCommand(sqlCmd, con))
                    {
                        SqlParameter totalRows = new SqlParameter("@TotalRows", System.Data.SqlDbType.Int);
                        totalRows.Direction = System.Data.ParameterDirection.Output;

                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@CreatedBy", Common.GetUser.UserId);
                        if (filterInfo.Filters != null)
                        {
                            foreach (Filter key in filterInfo.Filters)
                            {
                                if(key!=null && key.Key!=null)
                                    cmd.Parameters.AddWithValue("@" + key.Key, key.Value);
                            }
                        }
                        cmd.Parameters.AddWithValue("@PageIndex", filterInfo.PageIndex);
                        cmd.Parameters.AddWithValue("@PageSize", filterInfo.PageSize);
                        cmd.Parameters.Add(totalRows);

                        SqlDataReader rdr = cmd.ExecuteReader();
                        while (rdr.Read())
                        {
                            WorkMaint e = new WorkMaint();
                            e.WorkItemId = Convert.ToInt32(rdr["workitemid"]);
                            e.ActivityId = Convert.ToInt32(rdr["activityid"]);
                            e.InvoiceNumber = Convert.ToString(rdr["invoice_number"]);
                            e.ActivityName = Convert.ToString(rdr["activityname"]);
                            e.RequestNumber = Convert.ToString(rdr["request_number"]);
                            e.Comments = Convert.ToString(rdr["comments"]);
                            e.UserId = Convert.ToString(rdr["userid"]);
                            e.StartedDate = Convert.ToDateTime(rdr["starteddate"]);
                            e.Status = Convert.ToString(rdr["itemstatus"]);
                            wm.Add(e);
                        }
                        workms.WorkMaintList = wm;
                        rdr.NextResult();
                        workms.TotalRows = Convert.ToInt32(totalRows.Value);
                        
                        CloseReader(rdr);
                        workms._success = true;
                    }
                }
            }
            catch (Exception ex)
            {
                workms._message = FinTrackRes.WM_RETRIEVE_ERR;
                Logger.Log(Logger.LogType.Error, ex.ToString());
            }
            return workms;
        }

        public Info insert_reviewerdata(WorkMaint data)
        {
            //This will be called when reviewer clicked on the Review button in the work items grid. so we are not displaying any message.
            Info retVal = new Info("", false);
            SqlCommand cmd = null;
            try
            {
                using (con = new SqlConnection(conStr))
                {
                    con.Open();
                    using (cmd = new SqlCommand(StaticData.CREATE_REVIEWER_WORKMAINTENANCE, con))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@workitemid", data.WorkItemId);
                        cmd.Parameters.AddWithValue("@userid", Common.GetUser.UserId);
                        cmd.Parameters.AddWithValue("@starteddate", DateTime.Now);
                        cmd.Parameters.AddWithValue("@itemstatus", data.Status);
                        cmd.Parameters.AddWithValue("@usertype", "Reviewer"); //Hard coding this because this will be used only by the reviewer.
                        cmd.ExecuteNonQuery();
                        retVal._success = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(Logger.LogType.Error, ex.ToString());
            }
            return retVal;
        }
        public WMUpdateViewModel update_reviewerdata(WorkMaint data)
        {
            WMUpdateViewModel retVal = new WMUpdateViewModel(FinTrackRes.WM_UPDATE_ERR, false);
            SqlCommand cmd = null;
            try
            {
                using (con = new SqlConnection(conStr))
                {
                    con.Open();
                    using (cmd = new SqlCommand(StaticData.UPDATE_REVIEWER_WORKMAINTENANCE, con))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@workitemid", data.WorkItemId);
                        cmd.Parameters.AddWithValue("@userid", Common.GetUser.UserId);
                        cmd.Parameters.AddWithValue("@revcomments", data.ReviewerComments);
                        cmd.Parameters.AddWithValue("@starteddate", DateTime.Now);
                        cmd.Parameters.AddWithValue("@itemstatus", data.Status);
                        cmd.Parameters.AddWithValue("@usertype", "Reviewer"); //Hard coding this because this will be used only by the reviewer.
                        cmd.Parameters.AddWithValue("@iserrorfound", data.ErrFound);
                        SqlDataReader rdr = cmd.ExecuteReader();

                        if (rdr.Read())
                        {
                            retVal.WMCnt = Convert.ToInt32(rdr["wmcnt"]);
                            retVal.RevieweeId = Convert.ToString(rdr["RevieweeId"]);
                        }

                        retVal._success = true;

                        if (data.Status == "Rework")//Rework then display the message.
                            retVal._message = FinTrackRes.WM_UPDATE_REW;
                        else
                            retVal._message = FinTrackRes.WM_UPDATE_APPR_ERR_FIND;

                        CloseReader(rdr);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(Logger.LogType.Error, ex.ToString());
            }
            return retVal;
        }
        public WMViewModel GetWorkMaintData(String status = "")
        {

            WMViewModel workms = new WMViewModel("", false);
            List<WorkMaint> wm = new List<WorkMaint>();

            String sqlCmd = StaticData.GET_WORKMAINTS;
            SqlCommand cmd = null;

            try
            {
                using (con = new SqlConnection(conStr))
                {
                    con.Open();
                    using (cmd = new SqlCommand(sqlCmd, con))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@CreatedBy", Common.GetUser.UserId);
                        if (!String.IsNullOrEmpty(status))
                            cmd.Parameters.AddWithValue("@Status", status);

                        SqlDataReader rdr = cmd.ExecuteReader();
                        while (rdr.Read())
                        {
                            WorkMaint e = new WorkMaint();
                            e.WorkItemId = Convert.ToInt32(rdr["workitemid"]);
                            e.ActivityId = Convert.ToInt32(rdr["activityid"]);
                            e.InvoiceNumber = Convert.ToString(rdr["invoice_number"]);
                            e.ActivityName = Convert.ToString(rdr["activityname"]);
                            e.RequestNumber = Convert.ToString(rdr["request_number"]);
                            e.Comments = Convert.ToString(rdr["comments"]);
                            e.UserId = Convert.ToString(rdr["userid"]);
                            e.StartedDate = Convert.ToDateTime(rdr["starteddate"]);
                            e.Status = Convert.ToString(rdr["itemstatus"]);
                            e.ReviewerComments = Convert.ToString(rdr["reviewercomments"]);
                            e.ReviewerName = Convert.ToString(rdr["reviewername"]);
                            wm.Add(e);
                        }
                        workms._success = true;
                        workms.WorkMaintList = wm;
                        CloseReader(rdr);
                    }
                }

            }
            catch (Exception ex)
            {
                workms._message = FinTrackRes.WM_RETRIEVE_ERR;
                Logger.Log(Logger.LogType.Error, ex.ToString());
            }
            return workms;
        }

        /*Reports related*/
        public WMRPTErrViewModel GetErrorsReport()
        {

            WMRPTErrViewModel objErr = new WMRPTErrViewModel(FinTrackRes.WM_REPORT_LOAD_ERR, false);
            try
            {
                using (con = new SqlConnection(conStr))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand(StaticData.UPDATE_REVIEWER_WORKMAINTENANCE, con);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    //cmd.Parameters.AddWithValue("@workitemid", data.WorkItemId);
                    //cmd.Parameters.AddWithValue("@userid", Common.GetUser.UserId);
                    //cmd.Parameters.AddWithValue("@revcomments", data.ReviewerComments);
                    //cmd.Parameters.AddWithValue("@starteddate", DateTime.Now);
                    //cmd.Parameters.AddWithValue("@itemstatus", data.Status);
                    //cmd.Parameters.AddWithValue("@usertype", "Reviewer"); //Hard coding this because this will be used only by the reviewer.
                    //cmd.Parameters.AddWithValue("@iserrorfound", data.ErrFound);
                    SqlDataReader rdr = cmd.ExecuteReader();
                }
            }
            catch (Exception ex)
            {

                Logger.Log(Logger.LogType.Error, ex.ToString());
            }
            return null;
        }
        #endregion //WorkMaintenance
        #region UserData
        public User GetUserInfo(String uid)
        {
            User objUser = null;
            SqlCommand cmd = null;
            try
            {
                using (con = new SqlConnection(conStr))
                {
                    con.Open();
                    using (cmd = new SqlCommand(StaticData.GET_USERS, con))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@userid", uid);
                        using (SqlDataReader rdr = cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection))
                        {
                            if (rdr.HasRows)
                            {
                                rdr.Read();
                                objUser = new User()
                                {
                                    UserId = rdr["userid"].ToString(),
                                    UserName = rdr["username"].ToString(),
                                    IsActive = Convert.ToBoolean(rdr["isactive"]),
                                    EmailId = Convert.ToString(rdr["emailid"]),
                                    CreatedBy = rdr["createdby"].ToString(),
                                    CreatedDate = Convert.ToDateTime(rdr["createddate"]),
                                    IsReviewer = Convert.ToBoolean(rdr["isreviewer"]),
                                    IsAdmin = Convert.ToBoolean(rdr["isadmin"]),
                                    AssignedTeam = Convert.ToString(rdr["isapexp"])
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(Logger.LogType.Error, ex.ToString());
                throw ex;
            }
            return objUser;
        }
        public UserViewModel GetUsers()
        {
            UserViewModel objLst = new UserViewModel(string.Empty, false);
            SqlCommand cmd = null;
            objLst.lst = new List<User>();
            try
            {
                using (con = new SqlConnection(conStr))
                {
                    con.Open();
                    using (cmd = new SqlCommand(StaticData.GET_USERS, con))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        using (SqlDataReader rdr = cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection))
                        {
                            while (rdr.Read())
                            {
                                objLst.lst.Add(new User()
                                {
                                    UserId = rdr["userid"].ToString(),
                                    UserName = rdr["username"].ToString(),
                                    IsActive = Convert.ToBoolean(rdr["isactive"]),
                                    EmailId = Convert.ToString(rdr["emailid"]),
                                    CreatedBy = rdr["createdby"].ToString(),
                                    CreatedDate = Convert.ToDateTime(rdr["createddate"]),
                                    IsAdmin = Convert.ToBoolean(rdr["isadmin"]),
                                    IsReviewer = Convert.ToBoolean(rdr["isreviewer"])
                                });
                            }
                            objLst._success = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(Logger.LogType.Error, ex.ToString());
                objLst._message = FinTrackRes.USER_RETRIEVE_ERR;
            }
            return objLst;
        }
        public Info save(User data)
        {
            Info retVal = new Info(FinTrackRes.USER_SAVE_ERR, false);
            SqlCommand cmd = null;
            try
            {
                using (con = new SqlConnection(conStr))
                {
                    con.Open();
                    using (cmd = new SqlCommand(StaticData.CREATE_USER, con))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@userid", data.UserId);
                        cmd.Parameters.AddWithValue("@username", data.UserName);
                        cmd.Parameters.AddWithValue("@emailid", data.EmailId);
                        cmd.Parameters.AddWithValue("@createdby", Common.GetUser.UserId);
                        cmd.Parameters.AddWithValue("@createddate", DateTime.Now);
                        cmd.ExecuteNonQuery();
                    }
                }
                        retVal._success = true;
                        retVal._message = FinTrackRes.USER_SAVE;
            }
            catch (Exception ex)
            {
                Logger.Log(Logger.LogType.Error, ex.ToString());
            }
            return retVal;
        }

        internal Info update(User data)
        {
            Info retVal = new Info(FinTrackRes.USER_UPDATE_ERR, false);
            SqlCommand cmd = null;
            try
            {
                using (con = new SqlConnection(conStr))
                {
                    con.Open();
                    using (cmd = new SqlCommand(StaticData.UPDATE_USER, con))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@userid", data.UserId);
                        cmd.Parameters.AddWithValue("@username", data.UserName);
                        cmd.Parameters.AddWithValue("@emailid", data.EmailId);
                        cmd.Parameters.AddWithValue("@isactive", data.IsActive);
                        cmd.Parameters.AddWithValue("@isadmin", data.IsAdmin);
                        cmd.ExecuteNonQuery();
                    }
                }
                    retVal._success = true;
                    retVal._message = FinTrackRes.USER_UPDATE;
            }
            catch (Exception ex)
            {
                Logger.Log(Logger.LogType.Error, ex.ToString());
            }
            return retVal;
        }
        internal Info DeleteUser(String UserId)
        {
            Info retVal = new Info(FinTrackRes.USER_DELETE_ERR, false);
            SqlCommand cmd = null;
            try
            {
                using (con = new SqlConnection(conStr))
                {
                    con.Open();
                    using (cmd = new SqlCommand(StaticData.DELETE_USER, con))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@userid", UserId);
                        cmd.ExecuteNonQuery();
                    }
                }
                retVal._success = true;
                retVal._message = FinTrackRes.USER_DELETE;

            }
            catch (Exception ex)
            {
                Logger.Log(Logger.LogType.Error, ex.ToString());
            }
            return retVal;
        }

        #endregion //User Data
        #region ReviewerMgmt
        public Info save(Reviewer rev)
        {
            Info retVal = new Info(FinTrackRes.REV_SAVE_ERR, false);
            SqlCommand cmd = null;
            try
            {
                using (con = new SqlConnection(conStr))
                {
                    con.Open();
                    using (cmd = new SqlCommand(StaticData.CREATE_REVIEWER, con))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@revieweeid", rev.RevieweeId); //Reviweeid's might come as an array of values.
                        cmd.Parameters.AddWithValue("@reviwerid", rev.ReviewerId);
                        cmd.Parameters.AddWithValue("@isprimary", rev.IsPrimary);
                        cmd.ExecuteNonQuery();
                    }
                }
                retVal._success = true;
                retVal._message = FinTrackRes.REV_SAVE;
            }
            catch (Exception ex)
            {
                Logger.Log(Logger.LogType.Error, ex.ToString());
            }
            return retVal;
        }

        public Info delete(Reviewer rev)
        {
            Info retVal = new Info(FinTrackRes.REV_DELETE_ERR, false);
            SqlCommand cmd = null;
            try
            {
                using (con = new SqlConnection(conStr))
                {
                    con.Open();
                    using (cmd = new SqlCommand(StaticData.DELETE_REVIEWER, con))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@reviewerid", rev.ReviewerId);
                        cmd.ExecuteNonQuery();
                    }
                }
                retVal._success = true;
                retVal._message = FinTrackRes.REV_DELETE;
            }
            catch (Exception ex)
            {
                Logger.Log(Logger.LogType.Error, ex.ToString());
            }
            return retVal;
        }

        public ReviewerMgmt GetReviewers()
        {
            ReviewerMgmt objRm = new ReviewerMgmt("", false);
            List<Reviewer> reviewers = new List<Reviewer>();
            try
            {
                using (con = new SqlConnection(conStr))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(StaticData.GET_REVIEWERS, con))
                    {
                       SqlDataReader rdr = cmd.ExecuteReader();
                       while (rdr.Read())
                       {
                           reviewers.Add(new Reviewer() {
                                ReviewerName = Convert.ToString(rdr["ReviewerName"]),
                                IsPrimary = Convert.ToBoolean(rdr["isprimary"]),
                                ReviewerId = Convert.ToString(rdr["reviewerid"])
                           });
                       }
                    }
                }
                objRm._success = true;
                objRm.lst = reviewers;
            }
            catch (Exception ex)
            {
                objRm._message = FinTrackRes.REV_LOAD_ERR;
                Logger.Log(Logger.LogType.Error, ex.ToString());
            }
            return objRm;
        }
        #endregion //Reviewer Management
        #region Reports
        public WMRPTErrViewModel GetHistory(WMReportInputs input)
        {
            WMRPTErrViewModel objErr = new WMRPTErrViewModel(FinTrackRes.WM_REPORT_LOAD_ERR, false);
            try
            {
                using (con = new SqlConnection(conStr))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand(StaticData.GET_REPORT_BY_DATE_AR, con);
                    //based on the team we can generate the report, same properties can be used for all the teams.
                    if (input.TeamName == "ap")
                        cmd.CommandText = StaticData.GET_REPORT_BY_DATE_AP;

                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.CommandTimeout = 0;
                    cmd.Parameters.AddWithValue("@startdt", input.RptDate);
                    //if end date is there then pass that also
                    cmd.Parameters.Add(new SqlParameter("@enddate", input.EndDate));
                    SqlDataReader rdr = cmd.ExecuteReader();
                    //initialize all the lists.
                    objErr.lst = new List<WMHistory>();
                    objErr.plst = new List<WMReportTimeTaken>();
                    objErr.rlst = new List<WMReportReviewerTime>();
                    objErr.psummary = new List<WMReportProcessorSummary>();
                    objErr.rDlst = new List<WMReportReviewerData>();
                    while (rdr.Read())
                    {
                        objErr.lst.Add(new WMHistory()
                        {
                            WorkItemId = Convert.ToInt32(rdr["Work Item Id"]),
                            ActivityName = Convert.ToString(rdr["Description"]),
                            InvoiceNumber = Convert.ToString(rdr["Invoice#"]),
                            RequestNumber = Convert.ToString(rdr["Request#"]),
                            Comments = Convert.ToString(rdr["Comments"]),
                            CreatedBy = Convert.ToString(rdr["Created By"]),
                            ReviewedBy = Convert.ToString(rdr["Reviewer"]),
                            ReviewerComments = Convert.ToString(rdr["Review Comments"]),
                            ErrFound = Convert.ToBoolean(rdr["Error Found"]),
                            StartedDate = Convert.ToDateTime(rdr["Stamp"]),
                            PresentStatus = Convert.ToString(rdr["status"])
                        });
                    }
                    rdr.NextResult();
                    while (rdr.Read())
                    {
                        objErr.plst.Add(new WMReportTimeTaken()
                        {
                            WorkItemId = Convert.ToInt32(rdr["Work Item Id"]),
                            ActivityName = Convert.ToString(rdr["Description"]),
                            InvoiceNumber = Convert.ToString(rdr["Invoice#"]),
                            RequestNumber = Convert.ToString(rdr["Request#"]),
                            UserId = Convert.ToString(rdr["Created By"]),
                            TimeTaken_Sec = Convert.ToInt32(rdr["timetaken"])
                        });
                    }
                    //new result set which is bringing the Totals of groups.
                    rdr.NextResult();
                    while (rdr.Read())
                    {
                        objErr.psummary.Add(new WMReportProcessorSummary()
                        {
                            ProcessorName = Convert.ToString(rdr["created By"]),
                            Description = Convert.ToString(rdr["Description"]),
                            TotalTimeTaken = Convert.ToDecimal(rdr["Total Timespent"])
                        });
                    }

                    rdr.NextResult();
                    while (rdr.Read())
                    {
                        objErr.rlst.Add(new WMReportReviewerTime()
                        {
                            ReviewerName = Convert.ToString(rdr["Reviewer"]),
                            TimeTaken_Sec = Convert.ToInt32(rdr["timetaken"])
                        });
                    }

                    rdr.NextResult();
                    while (rdr.Read())
                    {
                        objErr.rDlst.Add(new WMReportReviewerData()
                        {
                            UserId = Convert.ToString(rdr["UserId"]),
                            WorkItemId = Convert.ToInt32(rdr["workitemid"]),
                            ActivityName = Convert.ToString(rdr["ActivityName"]),
                            InvoiceNumber = Convert.ToString(rdr["InvoiceNumber"]),
                            RequestNumber = Convert.ToString(rdr["RequestNumber"]),
                            ReviewerName = Convert.ToString(rdr["Reviewer"]),
                            TimeTaken_Sec = Convert.ToInt32(rdr["TimeTaken_Sec"]),
                            Comments = Convert.ToString(rdr["comments"]),
                            StartTime = Convert.ToDateTime(rdr["StartTime"]),
                            EndTime = Convert.ToDateTime(rdr["EndTime"]),
                            ErrorFound = Convert.ToBoolean(rdr["iserrorfound"])
                        });
                    }

                    objErr._success = true;
                    CloseReader(rdr);
                }
            }
            catch (Exception ex)
            {
                Logger.Log(Logger.LogType.Error, ex.ToString());
                throw ex;
            }
            return objErr;
        }
        //Below report will be called from the external program on daily basis
        public WMReportDataViewModal GetRawData(DateTime? startDate, DateTime? endDate)
        {
            WMReportDataViewModal objRep = new WMReportDataViewModal(FinTrackRes.WM_REPORT_LOAD_ERR, false);
            try
            {
                using (con = new SqlConnection(conStr))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand(StaticData.GET_FINANCETEAM_REPORT_DAILY, con);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@startdt", startDate);
                    cmd.Parameters.AddWithValue("@enddate", endDate);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    objRep.lst_ar = new List<WMReportData_AR>();
                    //First list will be the AR team report.
                    while (rdr.Read())
                    {
                        objRep.lst_ar.Add(new WMReportData_AR()
                        { 
                                 UserName = Convert.ToString(rdr["Created By"]),
                                 WorkItemId = Convert.ToInt32(rdr["Work item id"]),
                                 ActivityName = Convert.ToString(rdr["Description"]),
                                 InvoiceNumber = Convert.ToString(rdr["Invoice#"]),
                                 RequestNumber = Convert.ToString(rdr["Request#"]),
                                 ErrFound = Convert.ToBoolean(rdr["Error Found"]),
                                 ReviewedBy = Convert.ToString(rdr["Reviewer"]),
                                 ReviewerComments = Convert.ToString(rdr["Review Comments"]),
                                 Created = rdr.IsDBNull(rdr.GetOrdinal("Created"))? (DateTime?) null : Convert.ToDateTime(rdr["Created"]),
                                 Submitted = rdr.IsDBNull(rdr.GetOrdinal("Submitted"))? (DateTime?) null : Convert.ToDateTime(rdr["Submitted"]),
                                 ReSubmitted = rdr.IsDBNull(rdr.GetOrdinal("ReSubmitted"))? (DateTime?) null : Convert.ToDateTime(rdr["ReSubmitted"]),
                                 ReworkingAgin = rdr.IsDBNull(rdr.GetOrdinal("Reworking agin"))? (DateTime?) null : Convert.ToDateTime(rdr["Reworking agin"]) ,
                                 InReview =  rdr.IsDBNull(rdr.GetOrdinal("In Review"))? (DateTime?) null : Convert.ToDateTime(rdr["In Review"]),
                                 Rework = rdr.IsDBNull(rdr.GetOrdinal("Rework"))? (DateTime?) null : Convert.ToDateTime(rdr["Rework"]),
                                 CorrectedErrors = rdr.IsDBNull(rdr.GetOrdinal("Corrected Errors"))? (DateTime?) null : Convert.ToDateTime(rdr["Corrected Errors"]),
                                 Approved = rdr.IsDBNull(rdr.GetOrdinal("Approved"))? (DateTime?) null : Convert.ToDateTime(rdr["Approved"])
                        });
                    }
                    //Need to bring other 3 data sets and assign them appropriately.
                    rdr.NextResult();
                    objRep.list_ap = FillList(rdr);
                    rdr.NextResult();
                    objRep.list_ex = FillList(rdr);
                    rdr.NextResult();
                    objRep.list_fpna = FillList(rdr);
                    

                    CloseReader(rdr);
                }
            }
            catch (Exception ex)
            {
                Logger.Log(Logger.LogType.Error, ex.ToString());
            }

            return objRep;
        }

        private List<WMReportData> FillList(SqlDataReader rdr)
        {
            List<WMReportData> list = new List<WMReportData>();
            while (rdr.Read())
            {
                list.Add(new WMReportData()
                {
                    UserName = Convert.ToString(rdr["Created By"]),
                    WorkItemId = Convert.ToInt32(rdr["Work item id"]),
                    ActivityName = Convert.ToString(rdr["Description"]),
                    InvoiceNumber = Convert.ToString(rdr["Invoice#"]),
                    RequestNumber = Convert.ToString(rdr["Request#"]),
                    Comments = Convert.ToString(rdr["Comments"]),
                    Created = rdr.IsDBNull(rdr.GetOrdinal("Created")) ? (DateTime?)null : Convert.ToDateTime(rdr["Created"]),
                    Submitted = rdr.IsDBNull(rdr.GetOrdinal("Submitted")) ? (DateTime?)null : Convert.ToDateTime(rdr["Submitted"]),
                    ReSubmitted = rdr.IsDBNull(rdr.GetOrdinal("ReSubmitted")) ? (DateTime?)null : Convert.ToDateTime(rdr["ReSubmitted"]),
                    ReworkingAgin = rdr.IsDBNull(rdr.GetOrdinal("Reworking agin")) ? (DateTime?)null : Convert.ToDateTime(rdr["Reworking agin"]),
                    InReview = rdr.IsDBNull(rdr.GetOrdinal("In Review")) ? (DateTime?)null : Convert.ToDateTime(rdr["In Review"]),
                    Rework = rdr.IsDBNull(rdr.GetOrdinal("Rework")) ? (DateTime?)null : Convert.ToDateTime(rdr["Rework"]),
                    CorrectedErrors = rdr.IsDBNull(rdr.GetOrdinal("Corrected Errors")) ? (DateTime?)null : Convert.ToDateTime(rdr["Corrected Errors"]),
                    Approved = rdr.IsDBNull(rdr.GetOrdinal("Approved")) ? (DateTime?)null : Convert.ToDateTime(rdr["Approved"])
                });
            }
            return list;
        }

        #endregion
        #region Dashboard Charts
        public WMDashboardInfoList GetChartData(FilterAndPagerInfo filterInfo)
        {
            WMDashboardInfoList objDash = new WMDashboardInfoList(FinTrackRes.WM_CHART_ERR_MSG, false);

            try
            {
                objDash.LstByProcAct = new List<WMDashboardInfo>();
                objDash.LstByProc = new List<WMDashboardInfo>();
                objDash.LstByRevAct = new List<WMDashboardInfo>();
                objDash.LstByRev = new List<WMDashboardInfo>();

                using (SqlConnection con = new SqlConnection(conStr))
                {
                    con.Open();
                    using(SqlCommand cmd = new SqlCommand(StaticData.GET_DASHBOARD_DATA, con))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        //we can pass the start date if possible otherwise it will take the current date as start date.
                        if (filterInfo.Filters != null)
                        {
                            foreach (Filter key in filterInfo.Filters)
                            {
                                if (key != null && key.Key != null)
                                    cmd.Parameters.AddWithValue("@" + key.Key, key.Value);
                            }
                        }
                       SqlDataReader rdr = cmd.ExecuteReader();
                        //Processor data
                       while (rdr.Read())
                       {
                           objDash.LstByProcAct.Add(new WMDashboardInfo() { 
                                User = Convert.ToString(rdr["processor"]),
                                Activity = Convert.ToString(rdr["Activity"]),
                                TotalTime = Convert.ToDecimal(rdr["Total Timespent (hrs)"]),
                                Target = Convert.ToInt32(rdr["TargetHrs"])
                           });
                       }

                       rdr.NextResult();
                       while (rdr.Read())
                       {
                           objDash.LstByProc.Add(new WMDashboardInfo()
                           {
                               User = Convert.ToString(rdr["processor"]),
                               TotalTime = Convert.ToDecimal(rdr["Total Timespent (hrs)"]),
                               Target = Convert.ToInt32(rdr["TargetHrs"])
                           });
                       }
                        //Reviewer data
                       rdr.NextResult();
                       while (rdr.Read())
                       {
                           objDash.LstByRevAct.Add(new WMDashboardInfo()
                           {
                               User = Convert.ToString(rdr["Reviewer"]),
                               Activity = Convert.ToString(rdr["Activity"]),
                               TotalTime = Convert.ToDecimal(rdr["Total Timespent (hrs)"]),
                               Target = Convert.ToInt32(rdr["TargetHrs"])
                           });
                       }

                       rdr.NextResult();
                       while (rdr.Read())
                       {
                           objDash.LstByRev.Add(new WMDashboardInfo()
                           {
                               User = Convert.ToString(rdr["Reviewer"]),
                               TotalTime = Convert.ToDecimal(rdr["Total Timespent (hrs)"]),
                               Target = Convert.ToInt32(rdr["TargetHrs"])
                           });
                       }
                       objDash._message = String.Empty;
                       objDash._success = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(Logger.LogType.Error, ex.ToString());
                throw ex;
            }
            return objDash;
        }
        #endregion
    }
}
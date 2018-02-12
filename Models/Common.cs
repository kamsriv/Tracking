using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace FinTracker.Models
{
    public class Common
    {
        internal String m_LoginUser { get; set; }
        //Without creating the object we can access the details by accessing the static information from Session.
        private static User _objCommon;
        public static User GetUser
        {
            get {
                    if (HttpContext.Current.Session["userinfo"] != null)
                    {
                        _objCommon = HttpContext.Current.Session["userinfo"] as User;
                    }
                    else
                    {
                        _objCommon = new User(); //To Avoid User object is null exception.
                    }
                    return _objCommon;
                }
        }

        public void SetUserDetails()
        {
            //Need to assign the revier and admin roles
            repository objRep = new repository();
            HttpContext.Current.Session["userinfo"] = objRep.GetUserInfo(m_LoginUser);
        }
    }

    public class Filter
    {
        public String Key{get;set;}
        public String Value{get;set;}
    }

    public class FilterAndPagerInfo
    {
        public Int32 PageIndex { get; set; }
        public Int32 PageSize { get; set; }

        public Filter[] Filters { get; set; }
    }

    //This class can be used to send the message to the other places, if there are any issues.
    public class Info
    {
        public String _message;
        public Boolean _success;
        public Info() { _message = string.Empty; _success = false; }
        public Info(String msg, Boolean flg)
        {
            _message = msg;
            _success = flg;
        }
    }

    public class Logger
    {
        public enum LogType { Info, Warning, Error };
        public static void Log(LogType type, String msg)
        {
            
            String file = String.Empty;
            String errLine = String.Empty;
            if (HttpContext.Current != null)
            {
                file = String.Format(@"{0}\FinTracker.txt", HttpContext.Current.Request.ServerVariables["APPL_PHYSICAL_PATH"]);
                errLine = String.Format("User {5} - Accessed Date: {0}\r\n{1} :-{4} {2} {3}", DateTime.Now, type.ToString(), Common.GetUser.UserId, msg, Environment.NewLine, HttpContext.Current.User.Identity.Name);
            }
            else
            {
                file = String.Format(@"{0}\FinTracker.txt", AppDomain.CurrentDomain.BaseDirectory);
                errLine = String.Format("Accessed Date: {0} {1} {2}", DateTime.Now, Environment.NewLine, msg);
            }
            
            StreamWriter wr = new StreamWriter(file, true);
            wr.WriteLine("---------------------------------------------");
            if (type == LogType.Error)
                wr.Write("Error Description: ");
            wr.WriteLine(errLine);
            wr.Flush();
            wr.Close();
            wr.Dispose();
        }
    }
}
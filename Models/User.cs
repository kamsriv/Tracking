using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinTracker.Models
{
    public class User
    {
        private String _userid = null;
        /// <summary>
        /// If user is not available then default value is System other wise make it as logged in user.
        /// </summary>
        public String UserId
        {
            get
            {
                return _userid == null ? "SYSTEM" : _userid;
            }
            set
            {
                _userid = value;
            }
        }
        public String UserName { get; set; }
        public Boolean IsActive { get; set; }
        public String EmailId { get; set; }
        public String CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public Boolean IsAdmin { get; set; }

        public String AssignedTeam { get; set; }

        public Boolean IsReviewer { get; set; }
        public Boolean IsNormalUser
        {
            get
            {
                if (!IsAdmin && !IsReviewer)
                    return true;
                else
                    return false;
            }
        }
    }

    
}
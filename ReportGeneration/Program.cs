using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using FinTracker.Models;
using FinTracker.Models.ViewModel;
using FinTracker.Utilities;
using System.Net.Mail;
namespace ReportGeneration
{

    class Program
    {
        static String EMAIL_SUB = ConfigurationManager.AppSettings["subject"];
        static String TO = ConfigurationManager.AppSettings["Email"];
        static String BCC = ConfigurationManager.AppSettings["bccEmail"];
        static Boolean isEmailEnable = Convert.ToBoolean(ConfigurationManager.AppSettings["SendEmail"]);
        static String SALUTATION = ConfigurationManager.AppSettings["Salutation"];
        static String CC = ConfigurationManager.AppSettings["ccEmail"];
        static Int32 Freq = Convert.ToInt32(ConfigurationManager.AppSettings["Frequency"]);
        static void Main(string[] args)
        {
            try
            {
                //Run the report and Create the excel sheet attach it to email and send.
                repository objRepository = new repository();
                //Added below two dates for shashank's request for weekly frequency
                DateTime? startDt, endDt;
                startDt = endDt = null;
                startDt = DateTime.Now.AddDays(-Freq);
                endDt = DateTime.Now;
                String team = Convert.ToString(args[0]);
                WMReportDataViewModal objRpt = objRepository.GetRawData(startDt, endDt);
                XLExport objExport = new XLExport(objRpt);
                String fileName = objExport.GenerateExcel(ConfigurationManager.AppSettings["ExcelPath"], team);
                //Send an email with attachment from this file.
                SendEmail(fileName);
                Logger.Log(Logger.LogType.Info, "Email Has been generated for the day");
            }
            catch (Exception ex)
            {
                Logger.Log(Logger.LogType.Error, ex.ToString());
            } 
        }

        static void SendEmail(string filename)
        {
            if (String.IsNullOrEmpty(filename)) return;//If no file created or empty then don't do anything

            SmtpClient objEmail = new SmtpClient("ryanmail.ryanco.com");
            MailMessage objMsg = new MailMessage();
            //For expense team same day we are sending the report.
            String reportFor = DateTime.Today.AddDays(-1).ToString("MM_dd_yyyy");
            objMsg.Subject = String.Format(EMAIL_SUB, reportFor);
            objMsg.From = new MailAddress("noreply_fintracker@ryan.com");


            if (String.IsNullOrEmpty(TO)) TO = "srinivasarao.kamineni@ryan.com"; 
            if (!String.IsNullOrEmpty(BCC)) objMsg.Bcc.Add(BCC);
            if (!String.IsNullOrEmpty(CC)) objMsg.CC.Add(CC);

            objMsg.To.Add(TO);
            objMsg.Attachments.Add(new Attachment(filename));


            objMsg.IsBodyHtml = true;
            StringBuilder sb = new StringBuilder();
            sb.Append("<style type='text/css'>body{font-family: sans-serif;font-size:12px;} .footer{ font-weight:bold} .paddbottom{padding-bottom:5px;} </style>");
            sb.AppendFormat("<html><body><h5>Dear {0},</h5>", SALUTATION);
            sb.AppendFormat("<div>Attached is the finance team raw data report for the date {0}.", DateTime.Today.AddDays(-1).ToString("MM_dd_yyyy"));
            sb.AppendFormat("<br/>Please review the report and let us know if you need any further information.");
            sb.AppendFormat("<div class='paddbottom'>&nbsp;</div><div class='footer'>Regards,<div>Team FinTracker</div></div></body></html>");
            objMsg.Body = sb.ToString();

            if (isEmailEnable)
                objEmail.Send(objMsg);
        }
    }
}

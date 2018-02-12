using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using NPOI.XSSF;
using NPOI.SS.UserModel;
using System.Collections.Specialized;
using System.Reflection;
using NPOI.XSSF.UserModel;
using FinTracker.Models.ViewModel;
using FinTracker.Models;

namespace FinTracker.Utilities
{
    public class XLExport
    {
        XSSFWorkbook hssfworkbook;
        WMRPTErrViewModel _dataToExport;
        String _chartData;
        WMReportDataViewModal _dailyData;
        
        /// <summary>
        /// Chartdata JSON format constructor
        /// </summary>
        /// <param name="data"></param>
        public XLExport(String data)
        {
            _chartData = data;
        }
        /// <summary>
        /// Schedular report constructor
        /// </summary>
        /// <param name="data"></param>
        public XLExport(WMReportDataViewModal data)
        {
            _dailyData = data;
        }
        /// <summary>
        /// On Screen Report constructor
        /// </summary>
        /// <param name="data"></param>
        public XLExport(WMRPTErrViewModel data)
        {
            _dataToExport = data;
        }
        void InitializeWorkbook()
        {
            hssfworkbook = new XSSFWorkbook();
        }
        #region Methods Used by On Screen Reports
        public string ExcelExport()
        {
            InitializeWorkbook();
            GenerateData();
            string directory = HttpContext.Current.Server.MapPath("../Downloads");
            string fileName = String.Format("{1}\\Report_{0}.xls", DateTime.Now.ToString("yyyyMMdd_hhmss"), directory);
            //Write the stream data of workbook to the root directory
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
            using (FileStream fl = new FileStream(fileName, FileMode.Create))
            {
                hssfworkbook.Write(fl);
            }
            //clear work books which are older than 7 days.
            var p = from string f in Directory.GetFiles(directory) where new FileInfo(f).CreationTime <= DateTime.Now.AddDays(-5) select new FileInfo(f);
            p.ToList().ForEach(s => s.Delete());
            
            return fileName;
        }
        void GenerateData()
        {
            //Check how many lists are available in the dynamic data variable and we need to export each of them in a work sheet.
            //foreach (PropertyInfo prop in _dataToExport.GetType().GetProperties())
            //{
            //    if (prop.Name.Contains("lst"))
            //    {
            //        foreach (PropertyInfo pi in prop.GetType().GetProperties()) //Looping through every property in the 
            //        {

            //        }
            //    }
            //}
            
            if (_dataToExport.lst == null || _dataToExport.plst == null || _dataToExport.rlst == null || _dataToExport.rDlst == null)
                return;
            try
            {
                ISheet sheet1 = hssfworkbook.CreateSheet("ARTeamReport_Mar");

                #region Frame the main report in sheet 1.
                //header for the report
                ICellStyle headerStyle = hssfworkbook.CreateCellStyle();
                IFont headerFont = hssfworkbook.CreateFont();
                headerFont.Boldweight = 800;
                headerStyle.SetFont(headerFont);
                headerStyle.FillBackgroundColor = 10;
                WMHistory rptAll = new WMHistory();
                int i, j;
                i = j = 0;
                IRow row = sheet1.CreateRow(i);
                foreach (PropertyInfo p in rptAll.GetType().GetProperties())
                {
                    row.CreateCell(j).SetCellValue(p.Name);
                    row.GetCell(j).CellStyle.FillBackgroundColor = 2;
                    sheet1.AutoSizeColumn(j);
                    j++;
                }

                i++;
                foreach (WMHistory r in _dataToExport.lst)
                {
                    row = sheet1.CreateRow(i);
                    j = 0;
                    foreach (PropertyInfo p in rptAll.GetType().GetProperties())
                    {
                        row.CreateCell(j);
                        if (p.PropertyType.Name == "Int32")
                        {
                            row.GetCell(j).SetCellType(CellType.Numeric);
                            row.GetCell(j).SetCellValue(Convert.ToInt32(p.GetValue(r, null)));
                        }
                        else
                        {
                            row.GetCell(j).SetCellValue(Convert.ToString(p.GetValue(r, null)));
                        }
                        j++;
                    }
                    i++;
                }
                #endregion
                ISheet sheet2 = hssfworkbook.CreateSheet("Processor");
                i = j = 0;
                #region Frame the processor report in sheet 2.
                //header for the report
                WMReportTimeTaken rptProc = new WMReportTimeTaken();
                row = sheet2.CreateRow(i);
                foreach (PropertyInfo p in rptProc.GetType().GetProperties())
                {
                    row.CreateCell(j).SetCellValue(p.Name);
                    sheet2.AutoSizeColumn(j);
                    j++;
                }

                i++;
                foreach (WMReportTimeTaken r in _dataToExport.plst)
                {
                    row = sheet2.CreateRow(i);
                    j = 0;
                    foreach (PropertyInfo p in rptProc.GetType().GetProperties())
                    {
                        row.CreateCell(j);
                        if (p.PropertyType.Name == "Int32")
                        {
                            row.GetCell(j).SetCellType(CellType.Numeric);
                            row.GetCell(j).SetCellValue(Convert.ToInt32(p.GetValue(r, null)));
                        }
                        else
                            row.GetCell(j).SetCellValue(Convert.ToString(p.GetValue(r, null)));
                        j++;
                    }
                    i++;
                }
                //We have to display the summary information for the processors here.
                i = 0;
                j = j + 5;
                int cellCnt = j;
                WMReportProcessorSummary rptpsummary = new WMReportProcessorSummary();
                row = sheet2.GetRow(i);
                foreach (PropertyInfo p in rptpsummary.GetType().GetProperties())
                {
                    row.CreateCell(j).SetCellValue(p.Name);
                    sheet2.AutoSizeColumn(j);
                    j++;
                }
                i++;
                foreach (WMReportProcessorSummary r in _dataToExport.psummary)
                {
                    row = sheet2.GetRow(i);
                    if (row == null) break;
                    j = cellCnt;
                    foreach (PropertyInfo p in rptpsummary.GetType().GetProperties())
                    {
                        row.CreateCell(j);
                        if (p.PropertyType.Name == "Decimal")
                        {
                            row.GetCell(j).SetCellType(CellType.Numeric);
                            row.GetCell(j).SetCellValue(Convert.ToDouble(p.GetValue(r, null)));
                        }
                        else
                        {
                            string cellVal = Convert.ToString(p.GetValue(r, null));
                            if (cellVal.Equals("Totals")) row.GetCell(j).CellStyle.FillForegroundColor = 20; //Need a differenciation.
                            row.GetCell(j).SetCellValue(cellVal);
                        }
                        j++;
                    }
                    i++;
                }
                //Sum of all the groupings divided by 2.
                row = sheet2.GetRow(i + 1);
                if (row != null) //If no data available in the above tables then this is null
                {
                    row.CreateCell(j - 2).SetCellValue("Grand Total");
                    row.CreateCell(j - 1).SetCellFormula(String.Format("SUM(N2:N{0})/2", i));
                }
                #endregion
                ISheet sheet3 = hssfworkbook.CreateSheet("Reviewer");
                i = j = 0;
                #region Frame the reviewer report in sheet 3.
                //header for the report
                WMReportReviewerTime rptRev = new WMReportReviewerTime();
                row = sheet3.CreateRow(i);
                foreach (PropertyInfo p in rptRev.GetType().GetProperties())
                {
                    row.CreateCell(j).SetCellValue(p.Name);
                    sheet3.AutoSizeColumn(j);
                    j++;
                }

                i++;
                foreach (WMReportReviewerTime r in _dataToExport.rlst)
                {
                    row = sheet3.CreateRow(i);
                    j = 0;
                    foreach (PropertyInfo p in rptRev.GetType().GetProperties())
                    {
                        row.CreateCell(j);
                        if (p.PropertyType.Name == "Int32")
                        {
                            row.GetCell(j).SetCellType(CellType.Numeric);
                            row.GetCell(j).SetCellValue(Convert.ToInt32(p.GetValue(r, null)));
                        }
                        else
                            row.GetCell(j).SetCellValue(Convert.ToString(p.GetValue(r, null)));
                        j++;
                    }
                    i++;
                }
                #endregion
                ISheet sheet4 = hssfworkbook.CreateSheet("Reviewer_Data");
                i = j = 0;
                #region Reviewer Detailed Information Sheet 4
                //header for the report
                WMReportReviewerData rptRevData = new WMReportReviewerData();
                row = sheet4.CreateRow(i);
                foreach (PropertyInfo p in rptRevData.GetType().GetProperties())
                {
                    row.CreateCell(j).SetCellValue(p.Name);
                    sheet4.AutoSizeColumn(j);
                    j++;
                }

                i++;
                foreach (WMReportReviewerData r in _dataToExport.rDlst)
                {
                    row = sheet4.CreateRow(i);
                    j = 0;
                    foreach (PropertyInfo p in rptRevData.GetType().GetProperties())
                    {
                        row.CreateCell(j);
                        if (p.PropertyType.Name == "Int32")
                        {
                            row.GetCell(j).SetCellType(CellType.Numeric);
                            row.GetCell(j).SetCellValue(Convert.ToInt32(p.GetValue(r, null)));
                        }
                        else
                            row.GetCell(j).SetCellValue(Convert.ToString(p.GetValue(r, null)));
                        j++;
                    }
                    i++;
                }

                #endregion
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public string ExcelExportChartData()
        {
            InitializeWorkbook();
            WriteChartData();
            string directory = HttpContext.Current.Server.MapPath("../Downloads");
            string fileName = String.Format("{1}\\ChartReport_{0}.xls", DateTime.Now.ToString("yyyyMMdd_hhmss"), directory);
            //Write the stream data of workbook to the root directory
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
            using (FileStream fl = new FileStream(fileName, FileMode.Create))
            {
                hssfworkbook.Write(fl);
            }
            //clear work books which are older than 7 days.
            var p = from string f in Directory.GetFiles(directory) where new FileInfo(f).CreationTime <= DateTime.Now.AddDays(-5) select new FileInfo(f);
            p.ToList().ForEach(s => s.Delete());

            return fileName;
        }

        void WriteChartData()
        {
           ISheet procSheet = hssfworkbook.CreateSheet("processor");
           Int32 j = 0, i=0;
           IRow row = procSheet.CreateRow(i);
           List<String> hdr = new List<String>() {"Actul Utilization", "Target Hrs", "Total Time Spent" };
           List<String> procData = new List<String>(_chartData.Split('|'));
          
           
           //create a header based on the string
           foreach (String s in hdr)
           {
               row.CreateCell(j).SetCellValue(s);
               j++;
           }
           j = 0;
           
           
        }

        #endregion

        #region Methods Used by Schedular
        /// <summary>
        /// Sends the file name which was populated with all the report information.
        /// </summary>
        /// <param name="Path"></param>
        /// <returns></returns>
        public String GenerateExcel(String Path, String teamName = "ALL")
        {
            string fileName = String.Format("{1}\\Report_{0}.xls", DateTime.Now.ToString("yyyyMMdd_hhmss"), Path);
            InitializeWorkbook();
            WriteData();
            //Write the stream data of workbook to the root directory
            if (!Directory.Exists(Path))
                Directory.CreateDirectory(Path);
            
            //After writing all the data to spread sheet with every team information. We can remove thos sheets which are not necssary based on team
            switch (teamName.ToUpper())
            {
                case "AP":       
                    //for shobha removing other sheets.
                        hssfworkbook.RemoveSheetAt(0);
                        hssfworkbook.RemoveSheetAt(1);
                        hssfworkbook.RemoveSheetAt(1);
                    //Shobha report code comment out if not need
                    break;
                case "AR":
                    //for Kalyani
                    hssfworkbook.RemoveSheetAt(1);
                    hssfworkbook.RemoveSheetAt(1);
                    hssfworkbook.RemoveSheetAt(1);
                    break;
                case "EX":
                    //for Madhu
                    hssfworkbook.RemoveSheetAt(0);
                    hssfworkbook.RemoveSheetAt(0);
                    hssfworkbook.RemoveSheetAt(1);
                    break;
                case "TR":
                    //for Babitha
                    hssfworkbook.RemoveSheetAt(0);
                    hssfworkbook.RemoveSheetAt(0);
                    hssfworkbook.RemoveSheetAt(0);
                    break;
                default: //for shashank don't delete
                    break;
             }


            


            using (FileStream fl = new FileStream(fileName, FileMode.Create))
            {
                hssfworkbook.Write(fl);
            }
            
            //clear work books which are older than 7 days.
            var p = from string f in Directory.GetFiles(Path) where new FileInfo(f).CreationTime <= DateTime.Now.AddDays(-5) select new FileInfo(f);
            p.ToList().ForEach(s => s.Delete());
            
            return fileName;
        }
        public void WriteData()
        {
            int rNum = 0,cNum = 0;
            
            try
            {
                ISheet sheet1 = hssfworkbook.CreateSheet("ARTeamReport");
                IRow row = sheet1.CreateRow(rNum);
                ICell cell;
               NameValueCollection headerNames =new System.Collections.Specialized.NameValueCollection()
                {{"WorkItemId", "Work Item Id"}, {"UserName", "Created By"}, {"ActivityName","Activity Name"},{"InvoiceNumber","Invoice #"},{"RequestNumber","Request #"},
                 {"ReviewedBy","Reviewed By"},{"ReviewerComments","Reviewer Comments"},{"ErrFound","Error Found"},{"Created","Created"},{"Submitted","Submitted"},
                 {"ReworkingAgin","Reworking Again"},{"ReSubmitted","Re Sumbitted"}, {"InReview","In Review"}, {"CorrectedErrors","Corrected Errors"},
                 {"Rework","Rework"},{"Approved","Approved"}};

               NameValueCollection headerNamesExpTeam = new NameValueCollection()
                {{"WorkItemId", "Work Item Id"}, {"UserName", "Created By"}, {"ActivityName","Activity Name"},{"RequestNumber","Completion Status"}, {"InvoiceNumber","Id"},
                 {"Created","Created"},{"Submitted","Submitted"}};
               NameValueCollection headerNamesTRTeam = new NameValueCollection()
                {{"WorkItemId", "Work Item Id"}, {"UserName", "Created By"}, {"ActivityName","Activity Name"}, {"InvoiceNumber","Id"},
                 {"Comments", "Comments"}, {"Created","Created"},{"Submitted","Submitted"}};

               NameValueCollection tmpHederNames = new NameValueCollection();

                ICellStyle headerStyle = hssfworkbook.CreateCellStyle();
                IFont headerFont = hssfworkbook.CreateFont();
                ICellStyle detailStyle = hssfworkbook.CreateCellStyle();
                headerFont.Boldweight = Convert.ToInt16(NPOI.SS.UserModel.FontBoldWeight.Bold);
                headerStyle.SetFont(headerFont);
                
                headerStyle.BorderBottom = BorderStyle.Thin;
                headerStyle.BorderLeft = BorderStyle.Thin;
                headerStyle.BorderRight = BorderStyle.Thin;
                headerStyle.BorderTop = BorderStyle.Thin;

                detailStyle.BorderBottom = BorderStyle.Thin;
                detailStyle.BorderLeft = BorderStyle.Thin;
                detailStyle.BorderRight = BorderStyle.Thin;
                detailStyle.BorderTop = BorderStyle.Thin;

                
                #region AR Team Sheet
                //Fill the header information.
                foreach(String hdr in headerNames)
                {
                    cell = row.CreateCell(cNum);
                    cell.SetCellValue(headerNames[hdr]);
                    cell.CellStyle = headerStyle;
                    cNum++;
                }
                //Fill the data. after resetting the column count
                rNum++;
                cNum = 0;
                List<PropertyInfo> prop =  typeof(WMReportData_AR).GetProperties().ToList<PropertyInfo>();
                foreach (WMReportData_AR data in _dailyData.lst_ar)
                {
                    row = sheet1.CreateRow(rNum);

                    foreach (String hdr in headerNames)
                    {
                        cell = row.CreateCell(cNum);
                        var str = (from p in prop where p.Name==hdr select new {value = (p.GetValue(data,null)==null)?"":p.GetValue(data,null).ToString(),
                                                                                           datatype = p.PropertyType.Name
                                                                                           }).Single(); //Get the property value for that perticular thing
                        if (str.datatype == "Int32")
                            cell.SetCellType(CellType.Numeric);
                        cell.SetCellValue(str.value);
                        cell.CellStyle = detailStyle;
                        cNum++;
                    }
                    rNum++;
                    cNum = 0;
                }
                
                //Auto size the columns to fit its contents
                for(cNum=0;cNum<headerNames.Count;cNum++)
                    sheet1.AutoSizeColumn(cNum);
                #endregion

                #region AP Team Report
                headerNames.Remove("ReviewedBy");
                headerNames.Remove("ReviewerComments");
                headerNames.Remove("InvoiceNumber");
                headerNames.Remove("RequestNumber");
                headerNames.Remove("ErrFound");
                string[] teams = new string[]{"APTeamReport", "ExpenseTeamReport", "FPNATeamReport"}; //These are the teams now generating the report. Same data structure will be used.
                for(int t=0;t<teams.Length;t++)
                {
                    rNum = cNum = 0;
                    List<WMReportData> objRepdata;
                    switch (t)
                    {
                        case 0: objRepdata = _dailyData.list_ap;
                            tmpHederNames = headerNames;
                            break;
                        case 1: objRepdata = _dailyData.list_ex;
                            tmpHederNames = headerNamesExpTeam;
                            break;
                        case 2: objRepdata = _dailyData.list_fpna;
                            tmpHederNames = headerNamesTRTeam;
                            break;
                        default:
                            objRepdata = _dailyData.list_ap;
                            tmpHederNames = headerNames;
                            break;
                    }


                    sheet1 = hssfworkbook.CreateSheet(teams[t]);
                    //Fill the header information.
                    row = sheet1.CreateRow(rNum);
                    foreach (String hdr in tmpHederNames)
                    {
                        cell = row.CreateCell(cNum);
                        cell.SetCellValue(tmpHederNames[hdr]);
                        cell.CellStyle = headerStyle;
                        cNum++;
                    }
                    //Fill the data. after resetting the column count
                    rNum++;
                    cNum = 0;
                    prop = typeof(WMReportData).GetProperties().ToList<PropertyInfo>();
                    foreach (WMReportData data in objRepdata)
                    {
                        row = sheet1.CreateRow(rNum);

                        foreach (String hdr in tmpHederNames)
                        {
                            cell = row.CreateCell(cNum);
                            var str = (from p in prop
                                       where p.Name == hdr
                                       select new
                                       {
                                           value = (p.GetValue(data, null) == null) ? "" : p.GetValue(data, null).ToString(),
                                           datatype = p.PropertyType.Name
                                       }).Single(); //Get the property value for that perticular thing
                            if (str.datatype == "Int32")
                                cell.SetCellType(CellType.Numeric);
                            cell.SetCellValue(str.value);
                            cell.CellStyle = detailStyle;
                            cNum++;
                        }
                        rNum++;
                        cNum = 0;
                    }

                    //Auto size the columns to fit its contents
                    for (cNum = 0; cNum < headerNames.Count; cNum++)
                        sheet1.AutoSizeColumn(cNum);
                }
                #endregion
            }
            catch (Exception ex)
            {
                Logger.Log(Logger.LogType.Error, ex.ToString());
            }
        }
        #endregion
    }


}
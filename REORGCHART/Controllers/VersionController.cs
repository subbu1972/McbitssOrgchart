using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections.Generic;
using System.Dynamic;
using System.Data;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Text;

using Newtonsoft.Json;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;

using Ionic.Zip;

using REORGCHART.Data;
using REORGCHART.Models;
using REORGCHART.Helper;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Core.Objects;
using System.Data.Entity;

// https://gojs.net/latest/intro/layouts.html#LayoutInvalidation
namespace REORGCHART.Controllers
{
    [Authorize]
    public class VersionController : Controller
    {
        Models.DBContext db = new Models.DBContext();
        LevelInfo LI = new LevelInfo();
        Common ComClass = new Common();
        ExcelAPI XlsxAPI = new ExcelAPI();
        string[] UserVersionDetails = null;

        // GET: Version
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public string SetSelectedValues(string KeyDate, string UsedView, string Country, string ShowLevel, string Levels,
                                        string Oper, string Version, string Role)
        {
            LoginUsers UserData = LI.GetLoginUserInfo("");
            var UCA = (from uca in db.UserLastActions
                       where uca.UserId == UserData.UserName
                       select uca).FirstOrDefault();
            if (UCA != null)
            {
                string UserRole = Role;
                var UFH = (from ufh in db.UploadFilesHeaders
                           where ufh.CompanyName == UserData.CompanyName && 
                                 ufh.UserId == UserData.UserName  &&
                                 ufh.Role == UserRole
                           select ufh).FirstOrDefault();
                int UserVersion = UFH.VersionNo;

                var UFD = (from ufd in db.UploadFilesDetails
                           where ufd.VersionNo == UserVersion
                           select ufd).FirstOrDefault();

                UCA.KeyDate = KeyDate;
                UCA.UsedView = UsedView;
                UCA.Country = Country;
                UCA.ShowLevel = UFD.ShowLevel;
                UCA.ParentLevel = "999999";
                UCA.Levels = Levels;
                UCA.Oper = Oper;
                UCA.Version = UserVersion.ToString();
                UCA.Role = Role;

                db.SaveChanges();
            }

            return "Sucess";
        }

        public string DownloadMLPDF()
        {
            LoginUsers UserData = LI.GetLoginUserInfo("");
            var UCA = (from uca in db.UserLastActions
                       where uca.UserId == UserData.UserName
                       select uca).FirstOrDefault();
            if (UCA != null)
            {
                int FinalyzerVersion = Convert.ToInt32(LI.ChooseFinalyzerVersion(UCA.Version));
                var UFH = (from ufh in db.UploadFilesHeaders
                           where ufh.CompanyName == UserData.CompanyName && 
                                 ufh.Role == "Finalyzer" && 
                                 ufh.VersionNo == FinalyzerVersion
                           select ufh).FirstOrDefault();

                // Creating Intermediate step for All PDF
                AllLevelPDF AllPDF = new AllLevelPDF(UCA.ShowLevel, "999999", UCA.Oper.Trim().ToUpper(), UCA.Country, 
                                                     "1", "23/10/2018", 6, "EN", "", "PDF", "POSITION_CALCULATED_COST", 
                                                     UFH.PositionCostField, UserData.CompanyName, UFH.FirstPosition,
                                                     FinalyzerVersion);

                string ValueLevels = "0";
                switch(UCA.Levels)
                {
                    case "One":
                        ValueLevels = "1";
                        break;
                    case "Two":
                        ValueLevels = "2";
                        break;
                    case "Three":
                        ValueLevels = "3";
                        break;
                    case "Four":
                        ValueLevels = "4";
                        break;
                    case "Five":
                        ValueLevels = "5";
                        break;
                }
                AllPDF.ConvertToPDF(UCA.ShowLevel, "VIEW_DEFAULT", "PDF", UserData.CompanyName, ValueLevels);
            }

            return "Success";
        }

        public string DownloadPDF(string ViewFlag, string LevelUp, string CurrentLevel)
        {
            LoginUsers UserData = LI.GetLoginUserInfo("");
            var UCA = (from uca in db.UserLastActions
                       where uca.UserId == UserData.UserName
                       select uca).FirstOrDefault();
            if (UCA != null)
            {
                int FinalyzerVersion = Convert.ToInt32(LI.ChooseFinalyzerVersion(UCA.Version));
                var UFH = (from ufh in db.UploadFilesHeaders
                           where ufh.CompanyName == UserData.CompanyName && 
                                 ufh.Role == "Finalyzer" &&
                                 ufh.VersionNo == FinalyzerVersion
                           select ufh).FirstOrDefault();

                // Creating Intermediate step for All PDF
                string ShowLevelPDF = UCA.ShowLevel;
                if (LevelUp == "Yes") ShowLevelPDF = UCA.ParentLevel == "999999" ? UCA.ShowLevel : UCA.ParentLevel;
                string CurrentLevelPDF = "All";
                if (CurrentLevel == "Yes") CurrentLevelPDF = UCA.Levels;

                AllLevelPDF AllPDF = new AllLevelPDF(ShowLevelPDF, "999999", UCA.Oper.Trim().ToUpper(), UCA.Country, 
                                                     "1", "23/10/2018", 6, "EN", "", "PDF", "POSITION_CALCULATED_COST", 
                                                     UFH.PositionCostField, UserData.CompanyName, UFH.FirstPosition,
                                                     FinalyzerVersion);
                DataSet OrgDataSet = LI.GetOrgChartDataTable(UCA.Role, UCA.Country, ShowLevelPDF, "999999", CurrentLevelPDF, UCA.Oper, UCA.Version, UCA.ShowLevel, LevelUp, UCA.SelectedFunctionalManagerType);
                AllPDF.CreateAllLevelPDFIntermediateResults(OrgDataSet, UCA.Oper.ToUpper(), UCA.Version, "", "PDF");
                OrgDataSet = LI.GetOrgChartDataTable(UCA.Role, UCA.Country, ShowLevelPDF, "999999", CurrentLevelPDF, UCA.Oper, UCA.Version, UCA.ShowLevel, LevelUp, UCA.SelectedFunctionalManagerType);
                AllPDF.CreateAllLevelPDF("PDF", DateTime.Now, "", "PDF");
            }

            return "Success";
        }

        public string DownloadPPT(string ViewFlag, string LevelUp, string CurrentLevel)
        {
            LoginUsers UserData = LI.GetLoginUserInfo("");
            var UCA = (from uca in db.UserLastActions
                       where uca.UserId == UserData.UserName
                       select uca).FirstOrDefault();
            if (UCA != null)
            {
                int FinalyzerVersion = Convert.ToInt32(LI.ChooseFinalyzerVersion(UCA.Version));
                var UFH = (from ufh in db.UploadFilesHeaders
                           where ufh.CompanyName == UserData.CompanyName && 
                                 ufh.Role == "Finalyzer" &&
                                 ufh.VersionNo == FinalyzerVersion
                           select ufh).FirstOrDefault();

                // Creating Intermediate step for All PDF
                string ShowLevelPDF = UCA.ShowLevel;
                if (LevelUp == "Yes") ShowLevelPDF = UCA.ParentLevel == "999999" ? UCA.ShowLevel : UCA.ParentLevel;
                string CurrentLevelPDF = "All";
                if (CurrentLevel == "Yes") CurrentLevelPDF = UCA.Levels;

                AllLevelPDF AllPDF = new AllLevelPDF(ShowLevelPDF, "999999", UCA.Oper.Trim().ToUpper(), UCA.Country, 
                                                     "1", "23/10/2018", 6, "EN", UCA.TemplatePPTX, "PPTX", "POSITION_CALCULATED_COST", 
                                                     UFH.PositionCostField, UserData.CompanyName, UFH.FirstPosition, 
                                                     FinalyzerVersion);
                DataSet OrgDataSet = LI.GetOrgChartDataTable(UCA.Role, UCA.Country, ShowLevelPDF, "999999", CurrentLevelPDF, UCA.Oper, UCA.Version, UCA.ShowLevel, LevelUp, UCA.SelectedFunctionalManagerType);
                AllPDF.CreateAllLevelPDFIntermediateResults(OrgDataSet, UCA.Oper.ToUpper(), UCA.Version, UCA.TemplatePPTX, "PPTX");
                OrgDataSet = LI.GetOrgChartDataTable(UCA.Role, UCA.Country, ShowLevelPDF, "999999", CurrentLevelPDF, UCA.Oper, UCA.Version, UCA.ShowLevel, LevelUp, UCA.SelectedFunctionalManagerType);
                AllPDF.CreateAllLevelTemplatePPT("PPT", DateTime.Now, UCA.TemplatePPTX, "PPTX");
            }

            return "Success";
        }

        public void DownloadAllPDF(string ViewFlag, string LevelUp, string CurrentLevel)
        {
            LoginUsers UserData = LI.GetLoginUserInfo("");
            var UCA = (from uca in db.UserLastActions
                       where uca.UserId == UserData.UserName
                       select uca).FirstOrDefault();
            if (UCA != null)
            {
                var FolderName = new DirectoryInfo(Server.MapPath("~/App_Data/Versions/DownLoadVersion"));
                string FN = UserData.UserName + "_" + UserData.CompanyName + "_" + DateTime.Now.ToString("yyyyMMddhhmmss");
                string FileNameWithPath = FolderName + "\\" + FN + ".PDF";
                int FinalyzerVersion = Convert.ToInt32(LI.ChooseFinalyzerVersion(UCA.Version));

                VersionDetails VD = (from vd in db.VersionDetails
                                     where vd.UserRole == "Finalyzer" &&
                                           vd.CompanyName == UserData.CompanyName &&
                                           vd.ActiveVersion == "Y" &&
                                           vd.VersionNo == FinalyzerVersion
                                     select vd).FirstOrDefault();
                if (VD != null)
                {
                    var UFH = (from ufh in db.UploadFilesHeaders
                                where ufh.CompanyName == UserData.CompanyName && 
                                      ufh.Role == "Finalyzer" &&
                                      ufh.VersionNo == FinalyzerVersion
                               select ufh).FirstOrDefault();
                    if (UFH != null)
                    {
                        string ShowLevelPDF = UCA.ShowLevel;
                        if (LevelUp=="Yes") ShowLevelPDF = UCA.ParentLevel== "999999"?UCA.ShowLevel:UCA.ParentLevel;
                        string CurrentLevelPDF = "All";
                        if (CurrentLevel == "Yes")
                        {
                            CurrentLevelPDF = UCA.Levels;
                            if (LevelUp=="Yes")
                            {
                                if (UCA.ParentLevel != "999999")
                                {
                                    if (CurrentLevelPDF == "One") CurrentLevelPDF = "Two";
                                    else if (CurrentLevelPDF == "Two") CurrentLevelPDF = "Three";
                                    else if (CurrentLevelPDF == "Three") CurrentLevelPDF = "Four";
                                    else if (CurrentLevelPDF == "Four") CurrentLevelPDF = "Five";
                                    else if (CurrentLevelPDF == "Five") CurrentLevelPDF = "Five";
                                    else if (CurrentLevelPDF == "All") CurrentLevelPDF = "All";
                                }
                                else LevelUp = "No";
                            }
                        }
                        if (UCA.ParentLevel == "999999") LevelUp = "No";
                        AllLevelPDF AllPDF = new AllLevelPDF(ShowLevelPDF, "999999", UCA.Oper.Trim().ToUpper(), UCA.Country,
                                                             CurrentLevelPDF, "23/10/2018", 6, "EN", "", "PDF", "POSITION_CALCULATED_COST", 
                                                             UFH.PositionCostField, UserData.CompanyName, UFH.FirstPosition, 
                                                             FinalyzerVersion);
                        DataSet OrgDataSet = LI.GetOrgChartDataTable(UCA.Role, UCA.Country, ShowLevelPDF, "999999", CurrentLevelPDF, UCA.Oper, UCA.Version, UCA.ShowLevel, LevelUp, UCA.SelectedFunctionalManagerType);
                        AllPDF.CreateAllLevelPDFIntermediateResults(OrgDataSet, UCA.Oper.ToUpper(), VD.VersionNo.ToString(), "", "PDF");
                        OrgDataSet = LI.GetOrgChartDataTable(UCA.Role, UCA.Country, ShowLevelPDF, "999999", CurrentLevelPDF, UCA.Oper, UCA.Version, UCA.ShowLevel, LevelUp, UCA.SelectedFunctionalManagerType);
                        AllPDF.CreateAllLevelPDF(OrgDataSet, "PDF", UCA.Company, UCA.UserId, UCA.Oper, UCA.SelectedFMLine, ViewFlag, FileNameWithPath, LevelUp, UCA.ShowLevel);
                    }
                }
            }
        }

        public void DownloadImagePDF()
        {
            string FileNameWithPath = HttpContext.Session["CurrPDF"].ToString();
            HttpContext.Response.Redirect("downloadFile.aspx?msg=" + FileNameWithPath, false);
            HttpContext.ApplicationInstance.CompleteRequest();
        }

        public JsonResult SaveImageSrc(string ImageSrc)
        {
            LoginUsers UserData = LI.GetLoginUserInfo("");
            var UCA = (from uca in db.UserLastActions
                       where uca.UserId == UserData.UserName
                       select uca).FirstOrDefault();
            if (UCA != null)
            {
                var FolderName = new DirectoryInfo(Server.MapPath("~/App_Data/Versions/DownLoadVersion"));
                string FN = UserData.UserName + "_" + UserData.CompanyName + "_" + DateTime.Now.ToString("yyyyMMddhhmmss");
                foreach (var file in FolderName.EnumerateFiles(UserData.UserName + "_" + UserData.CompanyName + "_" + "*.jpeg"))
                    file.Delete();

                //Create image to local machine.
                string FileNameWithPath = FolderName + "\\" + FN + ".jpeg";
                using (FileStream FileStream = new FileStream(FileNameWithPath, FileMode.Create))
                {
                    using (BinaryWriter bw = new BinaryWriter(FileStream))
                    {
                        byte[] DataByte = Convert.FromBase64String(ImageSrc);
                        bw.Write(DataByte);
                        bw.Close();
                    }
                }
                HttpContext.Session["CurrPDF"] = FileNameWithPath;
                AllLevelPDF AllPDF = new AllLevelPDF();
                AllPDF.CreateAllLevelmagePDF(FolderName + "\\" + FN + ".pdf");

                return Json(new
                {
                    Success = "Yes",
                    FileName = FN,
                    ChartData = LI.GetOrgChartData(UCA.Role, UCA.Country, UCA.ShowLevel, UCA.ParentLevel, 
                                                   UCA.Levels, UCA.Oper, UCA.Version,
                                                   UCA.OrgChartType, UCA.SelectedPortraitModeMultipleLevel, UCA.SelectedFunctionalManagerType)[1]
                });
            }

            return Json(new
            {
                Success = "No",
                FileName = "",
                ChartData = ""
            });
        }

        [HttpPost]
        public string SearchOrgTreeView()
        {
            LoginUsers UserData = LI.GetLoginUserInfo("");
            var UCA = (from uca in db.UserLastActions
                       where uca.UserId == UserData.UserName
                       select uca).FirstOrDefault();
            if (UCA != null)
            {
                DataTable retDT = null;

                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 0;
                    cmd.CommandText = "PROC_GET_POSITION_TREEVIEW";

                    cmd.Parameters.Add("@VERSION", SqlDbType.VarChar, 500).Value = UCA.Version;
                    cmd.Parameters.Add("@COMPANYNAME", SqlDbType.VarChar, 500).Value = UserData.CompanyName;
                    cmd.Parameters.Add("@OPER", SqlDbType.VarChar, 500).Value = UCA.Oper;

                    Common csobj = new Common();
                    retDT = csobj.SPReturnDataTable(cmd);
                    retDT.TableName = "Search";
                }

                return JsonConvert.SerializeObject(retDT);
            }

            return "No Data";
        }

        [HttpPost]
        public JsonResult ChangeShowLevel(string ShowLevel, string ParentLevel)
        {
            LoginUsers UserData = LI.GetLoginUserInfo("");
            var Level = ShowLevel;
            var UCA = (from uca in db.UserLastActions
                       where uca.UserId == UserData.UserName
                       select uca).FirstOrDefault();
            if (UCA != null)
            {
                if (UCA.ShowLevel == ShowLevel) Level = ParentLevel;
                if (Level == "999999")
                {
                    return Json(new
                    {
                        Message = "No Changes",
                    });
                }

                UCA.ShowLevel = Level;
                string[] sTableName = LI.GetFinalyzerTable(UCA.Version, UCA.Oper);
                DataTable ShowTable = ComClass.SQLReturnDataTable("SELECT * FROM " + "[" + sTableName[0] + "] WHERE LEVEL_ID='" + Level + "' AND MFLAG <> 'XFM' AND VERSION='" + UCA.Version.ToString() + "'");
                UCA.ParentLevel = ShowTable.Rows[0]["PARENT_LEVEL_ID"].ToString();

                db.SaveChanges();
            }

            string[] ChangeLevelOrgChart = LI.GetOrgChartData(UCA.Role, UCA.Country, UCA.ShowLevel, UCA.ParentLevel, 
                                                            UCA.Levels, UCA.Oper, UCA.Version,
                                                            UCA.OrgChartType, UCA.SelectedPortraitModeMultipleLevel, UCA.SelectedFunctionalManagerType);
            if (Session.Contents[UserData.UserName + "_MyModel"] != null)
            {
                MyModel MyModel = (MyModel)Session.Contents[UserData.UserName + "_MyModel"];
                MyModel.ChartData = ChangeLevelOrgChart[1];
                MyModel.TreeData = ChangeLevelOrgChart[0];
                MyModel.ShowLevel = Level;
                Session.Contents[UserData.UserName + "_MyModel"] = MyModel;
            }

            return Json(new
            {
                Message = "Success",
                UsedDate = DateTime.Now,
                UsedShowLevel = Level,
                ChartData = ChangeLevelOrgChart[1],
                TreeData = ChangeLevelOrgChart[0]
            });
        }

        [HttpPost]
        public JsonResult PartitionChangeShowLevel(string ShowLevel, string ParentLevel, string Version)
        {
            LoginUsers UserData = LI.GetLoginUserInfo("");
            var Level = ShowLevel;
            var UCA = (from uca in db.UserLastActions
                       where uca.UserId == UserData.UserName
                       select uca).FirstOrDefault();
            if (UCA != null)
            {
                if (UCA.PartitionShowLevel == ShowLevel) Level = ParentLevel;
                if (Level == "999999")
                {
                    return Json(new
                    {
                        Message = "No Changes",
                    });
                }
                UCA.PartitionShowLevel = Level;
                string[] sTableName = LI.GetFinalyzerTable(UCA.Version, UCA.Oper);
                DataTable ShowTable = ComClass.SQLReturnDataTable("SELECT * FROM " + "[" + sTableName[0] + "] WHERE LEVEL_ID='" + Level + "' AND MFLAG <> 'XFM' AND VERSION='" + UCA.Version.ToString() + "'");
                UCA.PartitionParentLevel = ShowTable.Rows[0]["PARENT_LEVEL_ID"].ToString();

                db.SaveChanges();
            }

            return Json(new
            {
                Message = "Success",
                UsedDate = DateTime.Now,
                UsedShowLevel = UCA.PartitionShowLevel,
                UsedParentLevel = UCA.PartitionParentLevel,
                UsedVersion = Version,
                ChartData = LI.GetOrgChartData(UCA.Role, UCA.Country, UCA.PartitionShowLevel, UCA.PartitionParentLevel, 
                                               UCA.Levels, UCA.Oper, Version,
                                               UCA.OrgChartType, UCA.SelectedPortraitModeMultipleLevel, UCA.SelectedFunctionalManagerType)[1]
            });
        }

        [HttpPost]
        public string ChangeLevels(string UptoLevel)
        {
            LoginUsers UserData = LI.GetLoginUserInfo("");
            string Levels = "";
            var UCA = (from uca in db.UserLastActions
                       where uca.UserId == UserData.UserName
                       select uca).FirstOrDefault();
            if (UCA != null)
            {
                switch (UptoLevel)
                {
                    case "1":
                        Levels = "One";
                        break;
                    case "2":
                        Levels = "Two";
                        break;
                    case "3":
                        Levels = "Three";
                        break;
                    case "4":
                        Levels = "Four";
                        break;
                    case "5":
                        Levels = "Five";
                        break;
                    case "6":
                        Levels = "All";
                        break;
                }
                if (UCA.Levels == Levels) return "No Changes";
                UCA.Levels = Levels;
                db.SaveChanges();
            }

            string ChangeLevel = LI.GetOrgChartData(UCA.Role, UCA.Country, UCA.ShowLevel, UCA.ParentLevel, 
                                                    UCA.Levels, UCA.Oper, UCA.Version,
                                                    UCA.OrgChartType, UCA.SelectedPortraitModeMultipleLevel, UCA.SelectedFunctionalManagerType)[1];
            if (Session.Contents[UserData.UserName + "_MyModel"] != null)
            {
                MyModel MyModel = (MyModel)Session.Contents[UserData.UserName + "_MyModel"];
                MyModel.ChartData = ChangeLevel;
                Session.Contents[UserData.UserName + "_MyModel"] = MyModel;
            }

            return ChangeLevel;
        }

        [HttpPost]
        public JsonResult ShowTemplates()
        {
            LoginUsers UserData = LI.GetLoginUserInfo("");
            try
            {
                var PPTXlist = (from pptx in db.PPTX_CONFIG_INFO where pptx.ACTIVE_IND == "Y" && pptx.CompanyName == UserData.CompanyName select pptx).ToList();
                string PPTX = JsonConvert.SerializeObject(PPTXlist);

                return Json(new
                {
                    Success = "Yes",
                    ShowMessage = "Display PPTX Templates ",
                    SF = PPTX

                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Success = "No",
                    ShowMessage = "Select Valid template",
                    DDL = "[]"
                });
            }
        }

        [HttpPost]
        public JsonResult ShowTemplatesList()
        {
            LoginUsers UserData = LI.GetLoginUserInfo("");
            try
            {
                var PPTXlist = (from pptx in db.PPTX_CONFIG_INFO where pptx.CompanyName==UserData.CompanyName select pptx).ToList();
                string PPTX = JsonConvert.SerializeObject(PPTXlist);

                return Json(new
                {
                    Success = "Yes",
                    ShowMessage = "Display PPTX Templates ",
                    SF = PPTX

                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Success = "No",
                    ShowMessage = "Select Valid template",
                    DDL = "[]"
                });
            }
        }
        [HttpPost]
        public JsonResult DeleteRevokeTemplate(int TemplateId, string DRType)
        {
            LoginUsers UserData = LI.GetLoginUserInfo("");
            try
            {
                var PPTXlist = db.PPTX_CONFIG_INFO.Where(P => P.ID == TemplateId && P.CompanyName == UserData.CompanyName).FirstOrDefault();

                if (DRType == "D")
                {
                    PPTXlist.ACTIVE_IND = "N";
                }
                else if (DRType == "R")
                {
                    PPTXlist.ACTIVE_IND = "Y";
                }
                db.SaveChanges();

                return Json(new
                {
                    Success = "Yes"
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Success = "No"
                });
            }
        }

        // Save Field information
        [HttpPost]
        public JsonResult SaveSelectedFields(string SetSelectedFields, string TemplatePPTX)
        {
            LoginUsers UserData = LI.GetLoginUserInfo("");
            var UCA = (from uca in db.UserLastActions
                       where uca.UserId == UserData.UserName
                       select uca).FirstOrDefault();
            try
            {
                Common ComObj = new Common();
                if (TemplatePPTX != "")
                {
                    UCA.TemplatePPTX = TemplatePPTX;
                    db.SaveChanges();

                    if (UCA.Oper == "OV")
                        ComObj.ExecuteQuery("UPDATE LEVEL_CONFIG_INFO SET ACTIVE_IND='Y' WHERE TEMPLATE_NAME ='"+ TemplatePPTX + "' AND COMPANY_NAME='" + UserData.CompanyName + "';");
                    else if (UCA.Oper == "LV")
                        ComObj.ExecuteQuery("UPDATE LEGAL_CONFIG_INFO SET ACTIVE_IND='Y' WHERE TEMPLATE_NAME ='" + TemplatePPTX + "' AND COMPANY_NAME='" + UserData.CompanyName + "';");
                }
                else
                {
                    if (UCA.Oper == "OV")
                        ComObj.ExecuteQuery("UPDATE LEVEL_CONFIG_INFO SET ACTIVE_IND='N';UPDATE LEVEL_CONFIG_INFO SET ACTIVE_IND='Y' WHERE FIELD_NAME IN (" + SetSelectedFields + ") AND COMPANY_NAME='"+UserData.CompanyName+"';");
                    else if (UCA.Oper == "LV")
                        ComObj.ExecuteQuery("UPDATE LEGAL_CONFIG_INFO SET ACTIVE_IND='N';UPDATE LEGAL_CONFIG_INFO SET ACTIVE_IND='Y' WHERE FIELD_NAME IN (" + SetSelectedFields + ") AND COMPANY_NAME='" + UserData.CompanyName + "';");
                }

                return Json(new
                {
                    Success = "Yes",
                    ShowMessage = "Display fields set Successfully",
                    SelectFields = (UCA.Oper == "OV" ? JsonConvert.SerializeObject((from sf in db.LEVEL_CONFIG_INFO where sf.DOWNLOAD_TYPE== "PDF" &&
                                                                                                                          sf.COMPANY_NAME == UserData.CompanyName
                                                                                    select new
                                                                                    {
                                                                                        FIELD_NAME = sf.FIELD_NAME,
                                                                                        FIELD_CAPTION = sf.FIELD_CAPTION,
                                                                                        ACTIVE_IND = sf.ACTIVE_IND
                                                                                    }).ToList()) : JsonConvert.SerializeObject((from sf in db.LEGAL_CONFIG_INFO
                                                                                                                                where sf.DOWNLOAD_TYPE == "PDF" &&
                                                                                                                                      sf.COMPANY_NAME == UserData.CompanyName
                                                                                                                                select new
                                                                                                                                {
                                                                                                                                    FIELD_NAME = sf.FIELD_NAME,
                                                                                                                                    FIELD_CAPTION = sf.FIELD_CAPTION,
                                                                                                                                    ACTIVE_IND = sf.ACTIVE_IND
                                                                                                                                }).ToList()))


                });
            }
            catch (Exception ex)
            {

            }
            return Json(new
            {
                Success = "No",
                ShowMessage = "Display fields not set"
            });
        }

        private string GetSearchFields(int VersionNumber)
        {
            LoginUsers UserData = LI.GetLoginUserInfo("");
            string[] ArraySearchFields = ConfigurationManager.AppSettings["SearchFields"].ToString().Split(',');
            var UFH = (from vd in db.UploadFilesHeaders
                       where vd.CompanyName == UserData.CompanyName &&
                             vd.Role=="Finalyzer"
                       select new { vd.ExcelDownLoadFields, vd.FirstPositionField }).Distinct().ToList();

            if (UFH != null)
            {
                if (UFH.Count >= 1)
                {
                    List<ExcelDownLoadField> LstEDL = new List<ExcelDownLoadField>();
                    string[] LstFields = UFH[0].ExcelDownLoadFields.Split(',');
                    foreach (string SF in LstFields)
                    {
                        if (SF != "")
                        {
                            ExcelDownLoadField EDL = new ExcelDownLoadField();

                            EDL.FieldCaption = SF;
                            EDL.SearchField = SF;
                            if (ArraySearchFields.Contains(SF)) EDL.SearchFlag = "Y"; else EDL.SearchFlag = "Y";
                            if (UFH[0].FirstPositionField == SF) EDL.PositionFlag = "Y"; else EDL.PositionFlag = "N";

                            LstEDL.Add(EDL);
                        }
                    }
                    return JsonConvert.SerializeObject(LstEDL);
                }
            }

            return "[]";
        }

        public ActionResult NotAuthorizedPage()
        {
            Models.DBContext db = new Models.DBContext();
            string wcCompanyName = ConfigurationManager.AppSettings["wcCompanyName"].ToString();
            var INI = (from ini in db.InitializeTables
                       where ini.Authentication.ToUpper() == "WINDOWS" && ini.CompanyName == wcCompanyName
                       select ini).FirstOrDefault();
            if (INI != null)
            {
                string[] LogonUser = System.Web.HttpContext.Current.Request.LogonUserIdentity.Name.ToString().Split('\\');
                ViewBag.UserName = LogonUser[1];
            }
            else
            {
                ApplicationUser UserData = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
                ViewBag.UserName = UserData.UserName;
            }
            ViewBag.Title = "Authorization";

            return View();
        }

        private MyModel UserRoleResults(LoginUsers UserData, string Role, string Search)
        {
            string Security = "No";
            string[] AssignedView = LI.CheckUserRoles(Role).Split(',');
            if (Array.IndexOf(AssignedView, Role) != -1) Security = "Yes";

            MyLastAction myla = LI.GetUserCurrentAction(Role, Search);
            int VersionNumber = myla.Version == "" ? 0 : Convert.ToInt32(myla.Version);

            DataTable ShowGridTable = new DataTable();
            ShowGridTable.Columns.Add("Upload", typeof(string));
            DataRow dr = ShowGridTable.NewRow();
            dr["Upload"] = "No data uploaded";
            ShowGridTable.Rows.Add(dr);
            Session["SourceTable"] = ShowGridTable;

            var UFH = (from ufh in db.UploadFilesHeaders
                       where ufh.CompanyName == UserData.CompanyName &&
                             ufh.Role == "Finalyzer"
                       select ufh).FirstOrDefault();

            string FinalyzerVersionFlag = "No";
            VersionDetails VD = (from vd in db.VersionDetails
                                 where vd.CompanyName == UserData.CompanyName &&
                                       vd.Version == myla.SelectedVersion &&
                                       vd.OperType == myla.Oper &&
                                       vd.ActiveVersion == "Y"
                                 select vd).FirstOrDefault();
            if (VD != null)
            {
                if (VD.UserRole == "Finalyzer") FinalyzerVersionFlag = "Yes";
            }

            string[] RetValue = LI.GetOrgChartData(myla.Role, myla.Country, myla.ShowLevel, myla.ParentLevel,
                                               myla.Levels, myla.Oper, myla.Version,
                                               myla.OrgChartType, myla.SelectedPortraitModeMultipleLevel, myla.SelectedFunctionalManagerType);
            var viewModel = new MyModel
            {
                UserId = UserData.UserName,
                UseDate = DateTime.Now,
                CompanyName = UserData.CompanyName,
                KeyDate = myla.KeyDate,
                SelectedInitiative = myla.SelectedInitiative,
                SelectedPopulation = myla.SelectedPopulation,
                SelectedUser = myla.SelectedUser,
                SelectedVersion = myla.SelectedVersion,
                SelectedShape = myla.SelectedShape,
                SelectedSkin = myla.SelectedSkin,
                SelectedShowPicture = myla.SelectedShowPicture,
                SelectedSplitScreen = myla.SelectedSplitScreen,
                SelectedSplitScreenDirection = myla.SelectedSplitScreenDirection,
                SelectedTextColor = myla.SelectedTextColor,
                SelectedBorderColor = myla.SelectedBorderColor,
                SelectedBorderWidth = myla.SelectedBorderWidth,
                SelectedLineColor = myla.SelectedLineColor,
                SelectedBoxWidth = myla.SelectedBoxWidth,
                SelectedFunctionalManagerType = myla.SelectedFunctionalManagerType,
                SelectedFMLine = myla.SelectedFMLine,
                SelectedPortraitModeMultipleLevel = myla.SelectedPortraitModeMultipleLevel,
                OrgChartType = myla.OrgChartType,
                SerialNoFlag = UFH == null ? "N" : UFH.SerialNoFlag,
                CopyPaste = myla.CopyPaste,
                ShowLevel = myla.ShowLevel,
                ParentLevel = myla.ParentLevel,
                Levels = myla.Levels,
                Version = myla.Version,
                Oper = myla.Oper,
                View = myla.View,
                Country = myla.Country,
                Countries = JsonConvert.SerializeObject((from co in db.LegalCountries
                                                         select co).Distinct().ToList()),
                TreeData = RetValue[0],
                ChartData = RetValue[1],
                Role = myla.Role,
                AssignedRole = LI.GetUserRoles(),
                HRCoreVersion = LI.GetHRCoreVersion(myla.Country, myla.Oper),
                Menu = LI.GetMenuItems(myla.Role),
                DDL = JsonConvert.SerializeObject((from vd in db.VersionDetails
                                                   where vd.CompanyName == UserData.CompanyName && vd.ActiveVersion == "Y" && vd.OperType == myla.Oper &&
                                                         (myla.Role == "SuperAdmin" ||
                                                          myla.Role == "Player" ||
                                                          myla.Role == "Finalyzer" ||
                                                          myla.Role == "User" ||
                                                          myla.Role == "EndUser")
                                                   select new { vd.UserName, vd.CompanyName, vd.UserRole, vd.OperType, vd.Country, vd.Initiative, vd.Population, vd.Version }).Distinct().ToList()),
                OVDDL = JsonConvert.SerializeObject((from vd in db.VersionDetails
                                                     where vd.CompanyName == UserData.CompanyName && vd.ActiveVersion == "Y" && vd.OperType == "OV" &&
                                                           (myla.Role == "SuperAdmin" ||
                                                            myla.Role == "Player" ||
                                                            myla.Role == "Finalyzer" ||
                                                            myla.Role == "User" ||
                                                            myla.Role == "EndUser")
                                                     select new { vd.UserName, vd.CompanyName, vd.UserRole, vd.OperType, vd.Country, vd.Initiative, vd.Population, vd.Version }).Distinct().ToList()),
                LVDDL = JsonConvert.SerializeObject((from vd in db.VersionDetails
                                                     where vd.CompanyName == UserData.CompanyName && vd.ActiveVersion == "Y" && vd.OperType == "LV" &&
                                                           ((vd.UserRole == "SuperAdmin" && myla.Role == "SuperAdmin") ||
                                                            (vd.UserRole == "Player" && myla.Role == "Player") ||
                                                             myla.Role == "Finalyzer" ||
                                                             myla.Role == "User" ||
                                                             myla.Role == "EndUser")
                                                     select new { vd.UserName, vd.CompanyName, vd.UserRole, vd.OperType, vd.Country, vd.Initiative, vd.Population, vd.Version }).Distinct().ToList()),
                SelectFields = (myla.Oper == "OV" ? JsonConvert.SerializeObject((from sf in db.LEVEL_CONFIG_INFO
                                                                                 where sf.DOWNLOAD_TYPE == "PDF" &&
                                                                                       sf.COMPANY_NAME == UserData.CompanyName
                                                                                 select new
                                                                                 {
                                                                                     FIELD_NAME = sf.FIELD_NAME,
                                                                                     FIELD_CAPTION = sf.FIELD_CAPTION,
                                                                                     ACTIVE_IND = sf.ACTIVE_IND
                                                                                 }).ToList()) : JsonConvert.SerializeObject((from sf in db.LEGAL_CONFIG_INFO
                                                                                                                             where sf.DOWNLOAD_TYPE == "PDF" &&
                                                                                                                                   sf.COMPANY_NAME == UserData.CompanyName
                                                                                                                             select new
                                                                                                                             {
                                                                                                                                 FIELD_NAME = sf.FIELD_NAME,
                                                                                                                                 FIELD_CAPTION = sf.FIELD_CAPTION,
                                                                                                                                 ACTIVE_IND = sf.ACTIVE_IND
                                                                                                                             }).ToList())),
                SearchFields = GetSearchFields(VersionNumber),
                InitialValues = JsonConvert.SerializeObject((from iv in db.InitializeTables where iv.CompanyName == UserData.CompanyName select iv).FirstOrDefault()),
                FinalyzerVerion = LI.GetFinalyzerVerion(myla.Oper),
                FinalyzerVersionFlag = FinalyzerVersionFlag,
                GridDataTable = ShowGridTable,
                ValidateSecurity = Security
            };
            Session.Contents[UserData.UserName + "_MyModel"] = viewModel;

            return (viewModel);
        }

        // GET: Version
        public ActionResult EndUser(string Search)
        {
            LoginUsers UserData = LI.GetLoginUserInfo("EndUser");
            if (UserData.ValidUser == "No")
            {
                var UserRole = (from usr in db.UserRoles
                                where usr.UserId == UserData.UserName
                                select usr).FirstOrDefault();
                if (UserRole.Role.IndexOf("EndUser") >= 0)
                {
                    HttpContext.Session["LoginUserInf"] = null;
                    UserData = LI.GetLoginUserInfo("EndUser");
                }
                else return RedirectToAction("NotAuthorizedPage", "Version");
            }
            if (Session.Contents[UserData.UserName+"_MyModel"] == null)
            {
                MyModel viewModel = UserRoleResults(UserData, "EndUser", Search);
                return View(viewModel);
            }
            else
            {
                MyModel MyModelRole = (MyModel)Session.Contents[UserData.UserName + "_MyModel"];
                if (MyModelRole.Role!= "EndUser")
                {
                    MyModel viewModel = UserRoleResults(UserData, "EndUser", Search);
                    return View(viewModel);
                }
            }
            MyModel MyModel = (MyModel)Session.Contents[UserData.UserName + "_MyModel"];

            return View(MyModel);
        }


        // GET: Version
        public ActionResult UploadVersion(string Search)
        {
            LoginUsers UserData = LI.GetLoginUserInfo("Player");
            if (Search != null)
            {
                if (Search == "Refresh")
                {
                    Session.Contents[UserData.UserName + "_MyModel"] = null;
                    Search = null;
                }
            }
            if (UserData.ValidUser == "No")
            {
                return RedirectToAction("NotAuthorizedPage", "Version", new { id = UserData.UserName });
            }
            if (Session.Contents[UserData.UserName + "_MyModel"] == null)
            {
                MyModel viewModel = UserRoleResults(UserData, "Player", Search);
                return View(viewModel);
            }
            else
            {
                MyModel MyModelRole = (MyModel)Session.Contents[UserData.UserName + "_MyModel"];
                if (MyModelRole.Role != "Player")
                {
                    MyModel viewModel = UserRoleResults(UserData, "Player", Search);
                    return View(viewModel);
                }
            }
            MyModel MyModel = (MyModel)Session.Contents[UserData.UserName + "_MyModel"];

            return View(MyModel);
        }

        // GET: Version
        public ActionResult UploadData(string Search)
        {
            LoginUsers UserData = LI.GetLoginUserInfo("Finalyzer");
            if (Search != null)
            {
                if (Search == "Refresh")
                {
                    Session.Contents[UserData.UserName + "_MyModel"] = null;
                    Search = null;
                }
            }
            if (UserData.ValidUser == "No")
            {
                return RedirectToAction("NotAuthorizedPage", "Version", new { id = UserData.UserName });
            }
            if (Session.Contents[UserData.UserName + "_MyModel"] == null)
            {
                MyModel viewModel = UserRoleResults(UserData, "Finalyzer", Search);
                return View(viewModel);
            }
            else
            {
                MyModel MyModelRole = (MyModel)Session.Contents[UserData.UserName + "_MyModel"];
                if (MyModelRole.Role != "Finalyzer")
                {
                    MyModel viewModel = UserRoleResults(UserData, "Finalyzer", Search);
                    return View(viewModel);
                }
            }
            MyModel MyModel = (MyModel)Session.Contents[UserData.UserName + "_MyModel"];

            return View(MyModel);
        }

        // Get Version
        public FileResult DownloadVersion()
        {
            LoginUsers UserData = LI.GetLoginUserInfo("");
            MyLastAction myla = LI.GetUserCurrentAction("");
            DataTable dt = LI.GetVersion(myla.Oper, UserData.CompanyName, myla.Version, myla.ShowLevel);

            string FN = UserData.UserName + "_" + UserData.CompanyName + "_" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".xlsx";
            string FilePath = Path.Combine(Server.MapPath("~/App_Data/Downloads/"), FN);

            dt.TableName = UserData.UserName + "_" + UserData.CompanyName + "_" + DateTime.Now.ToString("yyyyMMddhhmmss");
            DataSet ds = dt.DataSet;
            XlsxAPI.ExportDSToExcel(FilePath, ds);
            //XlsxAPI.CreateSpreadsheetWorkbook(FilePath, dt);

            string FileData = "";
            using (StreamReader Reader = new StreamReader(FilePath))
            {
                FileData = Reader.ReadToEnd();
            }

            string extension = new FileInfo(FilePath).Extension;
            if (extension != null || extension != string.Empty)
            {
                switch (extension.ToLower())
                {
                    case ".pdf":
                        return File(FilePath, "application/pdf", string.Format("{0}", FN));
                    case ".txt":
                        return File(FilePath, "application/plain", string.Format("{0}", FN));
                    case ".jpeg":
                        return File(FilePath, "application/jpeg", string.Format("{0}", FN));
                    case ".doc":
                        return File(FilePath, "application/msword", string.Format("{0}", FN));
                    case ".docx":
                        return File(FilePath, "application/msword", string.Format("{0}", FN));
                    case ".xls":
                        return File(FilePath, "application/msexcel", string.Format("{0}", FN));
                    case ".xlsx":
                        return File(FilePath, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", string.Format("{0}", FN));
                    default:
                        return File(FilePath, "application/octet-stream", string.Format("{0}", FN));
                }
            }

            return null;
        }

        public ActionResult DownLoadInitiative()
        {
            LoginUsers UserData = LI.GetLoginUserInfo("");
            MyLastAction myla = LI.GetUserCurrentAction("");
            string FinalyzerVersion = LI.ChooseFinalyzerVersion(myla.Version);

            var FolderName = new DirectoryInfo(Server.MapPath("~/App_Data/Versions/DownLoadVersion"));
            string FN = UserData.UserName + "_" + UserData.CompanyName + "_" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".zip";
            foreach (var file in FolderName.EnumerateFiles(UserData.UserName + "_" + UserData.CompanyName + "_" + "*.xlsx"))
            {
                file.Delete();
            }
            foreach (var file in FolderName.EnumerateFiles("*.pdf"))
            {
                file.Delete();
            }


            var FileList = (from fl in db.UploadFilesDetails
                            where fl.CompanyName == UserData.CompanyName && fl.UserId == UserData.UserName
                            select fl).ToList();
            foreach (var fl in FileList)
            {
                try
                {
                    if (fl.BackUpFile == null)
                    {
                        DataTable retDT = null;
                        using (SqlCommand cmd = new SqlCommand())
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandTimeout = 0;
                            cmd.CommandText = "PROC_GET_POSITION_TREE_BACKUP_VERSION";

                            cmd.Parameters.Add("@VERSION", SqlDbType.VarChar, 150).Value = fl.VersionNo.ToString();
                            cmd.Parameters.Add("@FINALYZER_VERSION", SqlDbType.VarChar, 150).Value = FinalyzerVersion;
                            cmd.Parameters.Add("@COMPANYNAME", SqlDbType.VarChar, 150).Value = UserData.CompanyName;
                            cmd.Parameters.Add("@OPER", SqlDbType.VarChar, 150).Value = myla.Oper;

                            retDT = ComClass.SPReturnDataTable(cmd);
                        }

                        // Remove predefined column
                        retDT.Columns.Remove("USER_ID");
                        retDT.Columns.Remove("LEVEL_ID");
                        retDT.Columns.Remove("PARENT_LEVEL_ID");
                        retDT.Columns.Remove("COUNTRY");
                        retDT.Columns.Remove("VERSION");
                        retDT.Columns.Remove("DATE_UPDATED");
                        retDT.Columns.Remove("FULL_NAME");
                        retDT.Columns.Remove("VERIFY_FLAG");
                        retDT.Columns.Remove("LEVEL_NO");
                        retDT.Columns.Remove("BREAD_GRAM");
                        retDT.Columns.Remove("BREAD_GRAM_NAME");
                        retDT.Columns.Remove("NOR_COUNT");
                        retDT.Columns.Remove("SOC_COUNT");
                        retDT.Columns.Remove("POSITION_CALCULATED_COST");
                        retDT.Columns.Remove("NEXT_LEVEL_FLAG");
                        retDT.Columns.Remove("GRAY_COLORED_FLAG");
                        retDT.Columns.Remove("DOTTED_LINE_FLAG");
                        retDT.Columns.Remove("SHOW_FULL_BOX");
                        retDT.Columns.Remove("LANGUAGE_SELECTED");
                        retDT.Columns.Remove("SORTNO");
                        retDT.Columns.Remove("POSITIONFLAG");
                        retDT.Columns.Remove("FLAG");
                        retDT.Columns.Remove("MFLAG");
                        retDT.Columns.Remove("SHOW_REPORTING_TO_FM");
                        retDT.Columns.Remove("REPORTING_TO_FM");
                        retDT.Columns.Remove("NODE_LOCATION_COL");
                        retDT.Columns.Remove("NODE_LOCATION_ROW");
                        retDT.Columns.Remove("NODE_LOCATION");

                        string BackupFileName = UserData.UserName + "_" + UserData.CompanyName + "_" + fl.VersionNo.ToString() + "_" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".xlsx";
                        string ServerMapPath = System.Web.HttpContext.Current.Server.MapPath("~/App_Data/Versions/" + BackupFileName);
                        retDT.TableName = UserData.UserName + "_" + UserData.CompanyName + "_" + DateTime.Now.ToString("yyyyMMddhhmmss");
                        DataSet ds = retDT.DataSet;
                        XlsxAPI.ExportDSToExcel(ServerMapPath, ds);
                        //XlsxAPI.CreateSpreadsheetWorkbook(ServerMapPath, retDT);

                        fl.BackUpFile = BackupFileName;
                        db.SaveChanges();

                    }
                    System.IO.File.Copy(Server.MapPath("~/App_Data/Versions/") + fl.BackUpFile, Server.MapPath("~/App_Data/Versions/DownLoadVersion/") + fl.BackUpFile);
                }
                catch (Exception ex)
                {
                }
            }

            Response.Clear();
            Response.ContentType = "application/zip";
            Response.AddHeader("content-disposition", "attachment; filename=" + FN);
            using (ZipFile zipfile = new ZipFile())
            {
                zipfile.AddSelectedFiles(UserData.UserName + "_" + UserData.CompanyName + "_" + "*.*", Server.MapPath("~/App_Data/Versions/DownLoadVersion"), false);
                zipfile.Save(Response.OutputStream);
            }
            Response.Flush();
            Response.End();

            return null;
        }

        [HttpPost]
        public JsonResult UploadImage(FormCollection formCollection)
        {
            LoginUsers UserData = LI.GetLoginUserInfo("");
            string PositionID = formCollection["hdnImageFileName"].ToString();
            var UCA = (from uca in db.UserLastActions where uca.UserId == UserData.UserName select uca).FirstOrDefault();

            if (Request.Files.Count > 0)
            {
                var file = Request.Files[0];
                string fileExt = System.IO.Path.GetExtension(file.FileName);
                if (fileExt == ".jpg" || fileExt == ".JPG")
                {
                    if (file != null && file.ContentLength > 0)
                    {
                        var fileName = Path.GetFileName(file.FileName);
                        string Imagepath = Server.MapPath("~/Content/Images/PHOTOS/") + PositionID + ".jpg";
                        if (System.IO.File.Exists(Imagepath)) System.IO.File.Delete(Imagepath);
                        string path = Server.MapPath("~/Content/Images/PHOTOS/") + PositionID + ".jpg";
                        file.SaveAs(path);

                        string ImagePath = ConfigurationManager.AppSettings["ImagePath"].ToString();
                        fileName = Path.GetFileName(file.FileName);
                        Imagepath = ImagePath + PositionID + ".jpg";
                        if (System.IO.File.Exists(Imagepath)) System.IO.File.Delete(Imagepath);
                        file.SaveAs(Imagepath);
                    }
                    return Json(new
                    {
                        Success = "Yes",
                        Message = "Picture Uploded successfully",
                        ChartData = LI.GetOrgChartData(UCA.Role, UCA.Country, UCA.ShowLevel, UCA.ParentLevel, 
                                                       UCA.Levels, UCA.Oper, UCA.Version,
                                                       UCA.OrgChartType, UCA.SelectedPortraitModeMultipleLevel, UCA.SelectedFunctionalManagerType)
                    });
                }
                else
                {
                    return Json(new
                    {
                        Success = "No",
                        Message = "Please select JPG formate file",

                    });
                }
            }
            else
            {
                return Json(new
                {
                    Success = "No",
                    Message = "Please select JPG formate file",

                });
            }
        }

        [HttpPost]
        public ActionResult GridViewPostPartialFM()
        {
            MyLastAction myla = LI.GetUserCurrentAction("");
            string[] sTableName = LI.GetFinalyzerTable(myla.Version, myla.Oper);

            DataTable ShowTableFM = (DataTable)Session["ShowTableFM"];
            if (ShowTableFM == null)
            {
                ShowTableFM = new DataTable();
                ShowTableFM.Columns.Add("FM", typeof(string));
                DataRow dr = ShowTableFM.NewRow();
                dr["FM"] = "No Functional Manager";
                ShowTableFM.Rows.Add(dr);
            }

            return PartialView("ShowFunctionalManagerPartial", ShowTableFM);
        }

        [HttpGet]
        public ActionResult ShowFunctionalManagerPartial()
        {
            LoginUsers UserData = LI.GetLoginUserInfo("");
            MyLastAction myla = LI.GetUserCurrentAction("");
            DataTable ShowTableFM = null;

            string[] sTableName = LI.GetFinalyzerTable(myla.Version, myla.Oper);
            if (sTableName[0] != "")
            {
                int Version = Convert.ToInt32(sTableName[1]);
                var UFH = (from ufh in db.UploadFilesHeaders
                           where ufh.CompanyName == UserData.CompanyName &&
                                 ufh.Role == "Finalyzer" &&
                                 ufh.VersionNo == Version
                           select ufh).FirstOrDefault();

                if (UFH != null) ShowTableFM = ComClass.SQLReturnDataTable("SELECT LEVEL_ID, FULL_NAME, MFLAG" + UFH.ExcelDownLoadFields + " " +
                    "                                                           FROM " + "[" + sTableName[0] + "] " +
                    "                                                           WHERE NOR_COUNT >= 1 AND DOTTED_LINE_FLAG = 'N' AND VERSION = '"+ myla.Version + "'" +
                    "                                                           ORDER BY PARENT_LEVEL_ID;");
            }
            Session.Add("ShowTableFM", ShowTableFM);
            if (ShowTableFM == null)
            {
                ShowTableFM = new DataTable();
                ShowTableFM.Columns.Add("FM", typeof(string));
                DataRow dr = ShowTableFM.NewRow();
                dr["FM"] = "No Functional Manager";
                ShowTableFM.Rows.Add(dr);
            }


            return PartialView("ShowFunctionalManagerPartial", ShowTableFM);
        }

        [HttpPost]
        public ActionResult GridViewPostUploadData()
        {
            MyLastAction myla = LI.GetUserCurrentAction("");

            DataTable ShowTable = (DataTable)Session["SourceTable"];
            if (ShowTable == null)
            {
                ShowTable = new DataTable();
                ShowTable.Columns.Add("Upload", typeof(string));
                DataRow dr = ShowTable.NewRow();
                dr["Upload"] = "No data uploaded";
                ShowTable.Rows.Add(dr);
            }

            var viewModel = new MyModel
            {
               GridDataTable=ShowTable
            };

            return PartialView("ShowUploadData", viewModel);
        }

        [HttpGet]
        public ActionResult ShowUploadData()
        {
            MyLastAction myla = LI.GetUserCurrentAction("");

            DataTable ShowTable = (DataTable)Session["SourceTable"];
            if (ShowTable == null)
            {
                ShowTable = new DataTable();
                ShowTable.Columns.Add("Upload", typeof(string));
                DataRow dr = ShowTable.NewRow();
                dr["Upload"] = "No data uploaded";
                ShowTable.Rows.Add(dr);
            }

            var viewModel = new MyModel
            {
                GridDataTable = ShowTable
            };

            return PartialView("ShowUploadData", viewModel);
        }


        [HttpPost]
        public ActionResult GridViewPostPartial()
        {
            MyLastAction myla = LI.GetUserCurrentAction("");
            string[] sTableName = LI.GetFinalyzerTable(myla.Version, myla.Oper);

            DataTable ShowTable = (DataTable)Session["ShowTable"];
            if (ShowTable == null)
            {
                ShowTable = new DataTable();
                ShowTable.Columns.Add("Search", typeof(string));
                DataRow dr = ShowTable.NewRow();
                dr["Search"] = "No Search Operation happened";
                ShowTable.Rows.Add(dr);
            }

            return PartialView("ShowSearchMenu", ShowTable);
        }

        [HttpGet]
        public ActionResult ShowSearchMenu()
        {
            LoginUsers UserData = LI.GetLoginUserInfo("");
            MyLastAction myla = LI.GetUserCurrentAction("");
            DataTable ShowTable = null;

            string[] sTableName = LI.GetFinalyzerTable(myla.Version, myla.Oper);
            if (sTableName[0] != "")
            {
                int Version = Convert.ToInt32(sTableName[1]);
                var UFH = (from ufh in db.UploadFilesHeaders
                           where ufh.CompanyName == UserData.CompanyName &&
                                 ufh.Role == "Finalyzer" &&
                                 ufh.VersionNo == Version
                           select ufh).FirstOrDefault();

                if (UFH != null) ShowTable = ComClass.SQLReturnDataTable("SELECT LEVEL_ID, FULL_NAME, MFLAG" + UFH.ExcelDownLoadFields + " FROM " + "[" + sTableName[0] + "]");
            }
            Session.Add("ShowTable", ShowTable);
            if (ShowTable == null)
            {
                ShowTable = new DataTable();
                ShowTable.Columns.Add("Search", typeof(string));
                DataRow dr = ShowTable.NewRow();
                dr["Search"] = "No Search Operation happened";
                ShowTable.Rows.Add(dr);
            }

            return PartialView("ShowSearchMenu", ShowTable);
        }

        [HttpPost]
        public string ShowSearchInformation(string IV)
        {
            LoginUsers UserData = LI.GetLoginUserInfo("");
            MyLastAction myla = LI.GetUserCurrentAction("");
            string WhereCondition = "";
            List<SearchField> IVs = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<List<SearchField>>(IV);
            foreach (SearchField sf in IVs)
            {
                if (sf.FieldValue != "")
                {
                    WhereCondition += " OR " + sf.FieldName + " like '%" + sf.FieldValue + "%'";
                }
            }
            if (WhereCondition != "")
                WhereCondition = " WHERE VERSION='" + myla.Version + "' AND (" + WhereCondition.Substring(4) + ")";
            else
                WhereCondition = " WHERE VERSION='" + myla.Version + "'";

            string[] sTableName = LI.GetFinalyzerTable(myla.Version, myla.Oper);
            int Version = Convert.ToInt32(sTableName[1]);
            var UFH = (from ufh in db.UploadFilesHeaders
                       where ufh.CompanyName == UserData.CompanyName &&
                             ufh.Role == "Finalyzer" &&
                             ufh.VersionNo == Version
                       select ufh).FirstOrDefault();

            DataTable ShowTable = null;
            ShowTable = ComClass.SQLReturnDataTable("SELECT LEVEL_ID, FULL_NAME, MFLAG" + UFH.ExcelDownLoadFields + " FROM " + "[" + sTableName[0] + "] " + WhereCondition + "");
            Session.Add("ShowTable", ShowTable);
            if (ShowTable == null)
            {
                ShowTable = new DataTable();
                ShowTable.Columns.Add("Search", typeof(string));
                DataRow dr = ShowTable.NewRow();
                dr["Search"] = "No Search Operation happened";
                ShowTable.Rows.Add(dr);
            }

            return "Success";
        }

        public ActionResult ShowSearchPosition(string id)
        {
            var Level = id;
            LoginUsers UserData = LI.GetLoginUserInfo("");
            var UCA = (from uca in db.UserLastActions
                       where uca.UserId == UserData.UserName
                       select uca).FirstOrDefault();
            if (UCA != null)
            {
                string[] sTableName = LI.GetFinalyzerTable(UCA.Version, UCA.Oper);

                DataTable ShowTable = ComClass.SQLReturnDataTable("SELECT * FROM " + "[" + sTableName[0] + "] WHERE LEVEL_ID='" + id + "' AND VERSION='" + UCA.Version.ToString() + "'");
                if (ShowTable.Rows[0]["PARENT_LEVEL_ID"].ToString() == "999999")
                {
                    Level = ShowTable.Rows[0]["LEVEL_ID"].ToString();
                    UCA.ShowLevel = Level; UCA.ParentLevel = "999999";
                }
                else
                {
                    Level = ShowTable.Rows[0]["PARENT_LEVEL_ID"].ToString();
                    UCA.ShowLevel = Level;
                    ShowTable = ComClass.SQLReturnDataTable("SELECT * FROM " + "[" + sTableName[0] + "] WHERE LEVEL_ID='" + Level + "' AND MFLAG <> 'XFM' AND VERSION='" + UCA.Version.ToString() + "'");
                    UCA.ParentLevel = ShowTable.Rows[0]["PARENT_LEVEL_ID"].ToString();
                }
                
                db.SaveChanges();
            }

            Session.Contents[UserData.UserName + "_MyModel"] = null;
            if (UCA.Role == "EndUser")
                return RedirectToAction("EndUser", "Version", new { Search = id });
            else if (UCA.Role == "SuperAdmin")
                return RedirectToAction("SuperAdminVersion", "Version", new { Search = id });
            else if (UCA.Role == "Player")
                return RedirectToAction("UploadVersion", "Version", new { Search = id });
            else if (UCA.Role == "Finalyzer")
                return RedirectToAction("UploadData", "Version", new { Search = id });

            return RedirectToAction("Index", "Home", new { Search = id });
        }

        [HttpPost]
        public ActionResult GridViewEmployeePostPartial()
        {
            DataTable ShowTable = (DataTable)Session["ShowEmployeeTable"];
            if (ShowTable == null)
            {
                ShowTable = new DataTable();
                ShowTable.Columns.Add("Search", typeof(string));
                DataRow dr = ShowTable.NewRow();
                dr["Search"] = "No Search Operation happened";
                ShowTable.Rows.Add(dr);
            }

            return PartialView("ShowEmployeeView", ShowTable);
        }

        public ActionResult ShowEmployeeView()
        {
            DataTable ShowTable = null;
            LoginUsers UserData = LI.GetLoginUserInfo("");

            var UCA = (from uca in db.UserLastActions
                       where uca.UserId == UserData.UserName
                       select uca).FirstOrDefault();
            if (UCA != null)
            {
                string[] sTableName = LI.GetFinalyzerTable(UCA.Version, UCA.Oper);
                if (sTableName[0] != "")
                {
                    int Version = Convert.ToInt32(sTableName[1]);
                    var UFH = (from ufh in db.UploadFilesHeaders
                               where ufh.CompanyName == UserData.CompanyName &&
                                     ufh.Role == "Finalyzer" &&
                                     ufh.VersionNo == Version
                               select ufh).FirstOrDefault();
                    if (UFH != null)
                    {
                        ShowTable = ComClass.SQLReturnDataTable("SELECT TOP 1 LEVEL_ID" + UFH.ExcelDownLoadFields + " " +
                                                                "     FROM " + "[" + sTableName[0] + "] WHERE VERSION='" + UCA.Version.ToString() + "'");
                    }
                }
            }
            
            if (ShowTable == null)
            {
                ShowTable = new DataTable();
                ShowTable.Columns.Add("Search", typeof(string));
                DataRow dr = ShowTable.NewRow();
                dr["Search"] = "No Employee information available";
                ShowTable.Rows.Add(dr);
            }
            Session.Add("ShowEmployeeTable", ShowTable);

            return PartialView("ShowEmployeeView", ShowTable);
        }

        public ActionResult ShowOrgchart()
        {
            MyLastAction myla = LI.GetUserCurrentAction("");
            LoginUsers UserData = LI.GetLoginUserInfo("");

            var viewModel = new MyModel
            {
                UseDate = DateTime.Now,
                KeyDate = myla.KeyDate,
                ShowLevel = myla.ShowLevel,
                ParentLevel = myla.ParentLevel,
                Levels = myla.Levels,
                Version = myla.Version,
                Oper = myla.Oper,
                View = myla.View,
                Country = myla.Country,
                Countries = JsonConvert.SerializeObject((from co in db.LegalCountries
                                                         select co).Distinct().ToList()),
                ChartData = LI.GetOrgChartData(myla.Role, myla.Country, myla.ShowLevel, myla.ParentLevel, 
                                               myla.Levels, myla.Oper, myla.Version,
                                               myla.OrgChartType, myla.SelectedPortraitModeMultipleLevel, myla.SelectedFunctionalManagerType)[1],
                Role = myla.Role,
                HRCoreVersion = LI.GetHRCoreVersion(myla.Country, myla.Oper),
                DDL = JsonConvert.SerializeObject((from vd in db.VersionDetails
                                                   where vd.CompanyName == UserData.CompanyName && vd.ActiveVersion == "Y" && vd.OperType == myla.Oper &&
                                                          ((vd.UserRole == "SuperAdmin" && myla.Role == "SuperAdmin") ||
                                                           (vd.UserRole == "Player" && myla.Role == "Player") ||
                                                            myla.Role == "Finalyzer" || 
                                                            myla.Role == "User" || 
                                                            myla.Role == "EndUser")
                                                   select new { vd.UserName, vd.CompanyName, vd.UserRole, vd.OperType, vd.Country, vd.Initiative, vd.Population, vd.Version }).Distinct().ToList())
            };

            return View(viewModel);
        }

        public ActionResult ShowOrgchartStatic()
        {
            MyLastAction myla = LI.GetUserCurrentAction("");
            LoginUsers UserData = LI.GetLoginUserInfo("");

            var viewModel = new MyModel
            {
                UseDate = DateTime.Now,
                KeyDate = myla.KeyDate,
                ShowLevel = myla.ShowLevel,
                ParentLevel = myla.ParentLevel,
                Levels = myla.Levels,
                Version = myla.Version,
                Oper = myla.Oper,
                View = myla.View,
                Country = myla.Country,
                Countries = JsonConvert.SerializeObject((from co in db.LegalCountries
                                                         select co).Distinct().ToList()),
                ChartData = LI.GetOrgChartData(myla.Role, myla.Country, myla.ShowLevel, myla.ParentLevel, 
                                               myla.Levels, myla.Oper, myla.Version,
                                               myla.OrgChartType, myla.SelectedPortraitModeMultipleLevel, myla.SelectedFunctionalManagerType)[1],
                Role = myla.Role,
                HRCoreVersion = LI.GetHRCoreVersion(myla.Country, myla.Oper),
                DDL = JsonConvert.SerializeObject((from vd in db.VersionDetails
                                                   where vd.CompanyName == UserData.CompanyName && vd.ActiveVersion == "Y" && vd.OperType == myla.Oper &&
                                                          ((vd.UserRole == "SuperAdmin" && myla.Role == "SuperAdmin") ||
                                                           (vd.UserRole == "Player" && myla.Role == "Player") ||
                                                            myla.Role == "Finalyzer" || 
                                                            myla.Role == "User" || 
                                                            myla.Role == "EndUser")
                                                   select new { vd.UserName, vd.CompanyName, vd.UserRole, vd.OperType, vd.Country, vd.Initiative, vd.Population, vd.Version }).Distinct().ToList())
            };

            return View(viewModel);
        }

        public ActionResult UpdateSettings()
        {
            MyLastAction myla = LI.GetUserCurrentAction("");
            LoginUsers UserData = LI.GetLoginUserInfo("");

            var viewModel = new MyModel
            {
                UseDate = DateTime.Now,
                KeyDate = myla.KeyDate,
                ShowLevel = myla.ShowLevel,
                ParentLevel = myla.ParentLevel,
                Levels = myla.Levels,
                Version = myla.Version,
                Oper = myla.Oper,
                View = myla.View,
                Country = myla.Country,
                Countries = JsonConvert.SerializeObject((from co in db.LegalCountries
                                                         select co).Distinct().ToList()),
                ChartData = LI.GetOrgChartData(myla.Role, myla.Country, myla.ShowLevel, myla.ParentLevel, 
                                               myla.Levels, myla.Oper, myla.Version,
                                               myla.OrgChartType, myla.SelectedPortraitModeMultipleLevel, myla.SelectedFunctionalManagerType)[1],
                Role = myla.Role,
                HRCoreVersion = LI.GetHRCoreVersion(myla.Country, myla.Oper),
                DDL = JsonConvert.SerializeObject((from vd in db.VersionDetails
                                                   where vd.CompanyName == UserData.CompanyName && vd.ActiveVersion == "Y" && vd.OperType == myla.Oper &&
                                                         ((vd.UserRole == "SuperAdmin" && myla.Role == "SuperAdmin") ||
                                                          (vd.UserRole == "Player" && myla.Role == "Player") ||
                                                           myla.Role == "Finalyzer" || 
                                                           myla.Role == "User" || 
                                                           myla.Role == "EndUser")
                                                   select new { vd.UserName, vd.CompanyName, vd.UserRole, vd.OperType, vd.Country, vd.Initiative, vd.Population, vd.Version }).Distinct().ToList())
            };

            return View(viewModel);
        }

        public ActionResult ShowSettingsOrgchart()
        {
            return PartialView();
        }

        public ActionResult UserVersion()
        {
            MyLastAction myla = LI.GetUserCurrentAction("");
            LoginUsers UserData = LI.GetLoginUserInfo("");

            var viewModel = new MyModel
            {
                UseDate = DateTime.Now,
                KeyDate = myla.KeyDate,
                ShowLevel = myla.ShowLevel,
                ParentLevel = myla.ParentLevel,
                Levels = myla.Levels,
                Version = myla.Version,
                Oper = myla.Oper,
                View = myla.View,
                Country = myla.Country,
                Countries = JsonConvert.SerializeObject((from co in db.LegalCountries
                                                         select co).Distinct().ToList()),
                ChartData = LI.GetOrgChartData(myla.Role, myla.Country, myla.ShowLevel, myla.ParentLevel, 
                                               myla.Levels, myla.Oper, myla.Version,
                                               myla.OrgChartType, myla.SelectedPortraitModeMultipleLevel, myla.SelectedFunctionalManagerType)[1],
                Role = myla.Role,
                HRCoreVersion = LI.GetHRCoreVersion(myla.Country, myla.Oper),
                DDL = JsonConvert.SerializeObject((from vd in db.VersionDetails
                                                   where vd.CompanyName == UserData.CompanyName && vd.ActiveVersion == "Y" && vd.OperType == myla.Oper &&
                                                         ((vd.UserRole == "SuperAdmin" && myla.Role == "SuperAdmin") ||
                                                          (vd.UserRole == "Player" && myla.Role == "Player") ||
                                                           myla.Role == "Finalyzer" || 
                                                           myla.Role == "User" || 
                                                           myla.Role == "EndUser")
                                                   select new { vd.UserName, vd.CompanyName, vd.UserRole, vd.OperType, vd.Country, vd.Initiative, vd.Population, vd.Version }).Distinct().ToList())
            };

            return View(viewModel);
        }

        private void AddProperty(ExpandoObject expando, string propertyName, object propertyValue)
        {
            // ExpandoObject supports IDictionary so we can extend it like this
            var expandoDict = expando as IDictionary<string, object>;
            if (expandoDict.ContainsKey(propertyName))
                expandoDict[propertyName] = propertyValue;
            else
                expandoDict.Add(propertyName, propertyValue);
        }

        private ExpandoObject SearchProperty(List<dynamic> lstDyn, string propertyName, string propertyValue)
        {
            foreach (ExpandoObject eo in lstDyn)
            {
                var expandoDict = eo as IDictionary<string, object>;
                if (expandoDict[propertyName].ToString() == propertyValue) return eo;
            }

            return null;
        }

        public JsonResult CopyPasteEmployeeInfo(string CopyPaste, string CopyInfo)
        {
            LoginUsers UserData = LI.GetLoginUserInfo("");
            var UCA = (from uca in db.UserLastActions
                       where uca.UserId == UserData.UserName
                       select uca).FirstOrDefault();
            if (UCA != null)
            {
                int Version = Convert.ToInt32(UCA.Version);
                var UFH = (from ufh in db.UploadFilesHeaders
                           where ufh.CompanyName == UserData.CompanyName &&
                                 ufh.Role == "Finalyzer"
                           select ufh).FirstOrDefault();

                if (CopyPaste == "Copy")
                {
                    UCA.CopyPaste = CopyInfo;
                    db.SaveChanges();

                    return Json(new
                    {
                        Success = "Yes",
                        Message = "Copied the employee information"
                    });
                }
                else if (CopyPaste == "Paste")
                {
                    return Json(new
                    {
                        Success = "Yes",
                        IB = UCA.CopyPaste,
                        PF = UFH.FirstPositionField,
                        InitialValues = JsonConvert.SerializeObject((from iv in db.InitializeTables where iv.CompanyName == UserData.CompanyName select iv).FirstOrDefault())
                    });
                }
            }

            return Json(new
            {
                Success = "No",
                Message = "User did not Exists"
            });
        }

        public JsonResult ShowFunctionalManager(string LevelId, string ParentLevelId, string NewParentLevelId, string OldParentName)
        {
            LoginUsers UserData = LI.GetLoginUserInfo("");
            var UCA = (from uca in db.UserLastActions
                       where uca.UserId == UserData.UserName
                       select uca).FirstOrDefault();
            if (UCA != null)
            {
                UserVersionDetails = LI.GetVersionDetails(Convert.ToInt32(UCA.Version), UCA.Role, UCA.Oper);
                if (UserVersionDetails[0] == "Success")
                {
                    string[] sTableName = LI.GetFinalyzerTable(UCA.Version, UCA.Oper);
                    string InsertData = "", FieldName = "";
                    string Query = "SELECT * FROM " + sTableName[0] + " WHERE LEVEL_ID='" + LevelId + "' AND PARENT_LEVEL_ID='" + ParentLevelId + "' AND VERSION='" + UCA.Version + "';";
                    DataTable dtQuery = ComClass.SQLReturnDataTable(Query);
                    Query = "SELECT * FROM " + sTableName[0] + " WHERE LEVEL_ID='" + NewParentLevelId + "' AND VERSION='" + UCA.Version + "';";
                    DataTable dtNP = ComClass.SQLReturnDataTable(Query);

                    if (dtQuery != null)
                    {
                        if (dtQuery.Rows.Count >= 1)
                        {
                            try
                            {
                                int VersionNo = Convert.ToInt32(UCA.Version);
                                string KeyField = "", ParentField = "";
                                var UFH = (from ufh in db.UploadFilesHeaders
                                           where ufh.CompanyName == UserData.CompanyName &&
                                                 ufh.VersionNo == VersionNo
                                           select ufh).FirstOrDefault();
                                if (UFH.SerialNoFlag == "N")
                                {
                                    KeyField = UFH.KeyField;
                                    ParentField = UFH.ParentField;
                                }

                                UCA.ShowLevel = NewParentLevelId;
                                DataTable ShowTable = ComClass.SQLReturnDataTable("SELECT * FROM " + "[" + sTableName[0] + "] WHERE LEVEL_ID='" + NewParentLevelId + "' AND MFLAG <> 'XFM' AND VERSION='" + UCA.Version.ToString() + "'");
                                UCA.ParentLevel = ShowTable.Rows[0]["PARENT_LEVEL_ID"].ToString();

                                db.SaveChanges();

                                dtQuery.Rows[0]["VERIFY_FLAG"] = "Y";
                                dtQuery.Rows[0]["BREAD_GRAM"] = dtNP.Rows[0]["BREAD_GRAM"].ToString() + "-->" + dtQuery.Rows[0]["LEVEL_ID"].ToString();
                                dtQuery.Rows[0]["BREAD_GRAM_NAME"] = dtNP.Rows[0]["BREAD_GRAM_NAME"].ToString() + "-->" + dtQuery.Rows[0]["FULL_NAME"].ToString();
                                dtQuery.Rows[0]["PARENT_LEVEL_ID"] = NewParentLevelId;
                                dtQuery.Rows[0]["LEVEL_NO"] = (Convert.ToInt32(dtNP.Rows[0]["LEVEL_NO"]) + 1).ToString();
                                if (ParentField != "") dtQuery.Rows[0][ParentField] = NewParentLevelId;
                                dtQuery.Rows[0]["DOTTED_LINE_FLAG"] = "Y";
                                dtQuery.Rows[0]["MFLAG"] = "XFM";

                                foreach (DataColumn dr in dtQuery.Columns)
                                {
                                    FieldName += "," + dr.ColumnName;
                                    InsertData += ",'" + dtQuery.Rows[0][dr.ColumnName].ToString().Replace("'", "''") + "'";
                                }

                                InitializeTables InitializeTables = (from iv in db.InitializeTables where iv.CompanyName == UserData.CompanyName select iv).FirstOrDefault();
                                Query += "INSERT INTO " + sTableName[0] + " (" + FieldName.Substring(1) + ") VALUES(" + InsertData.Substring(1) + ");";
                                Query += "UPDATE " + sTableName[0] + 
                                         " SET REPORTING_TO_FM='" + ShowTable.Rows[0][InitializeTables.FunctionalManagerKey].ToString() + "' ," +
                                         "     SHOW_REPORTING_TO_FM='"+ ShowTable.Rows[0]["LEVEL_ID"].ToString() + "'"+
                                         " WHERE LEVEL_ID='"+ dtQuery.Rows[0]["LEVEL_ID"].ToString() + "' AND VERSION='" + UCA.Version.ToString() + "';";
                                ComClass.ExecuteQuery(Query);
                            }
                            catch (Exception ex)
                            {
                                return Json(new
                                {
                                    Message = "No",
                                    ShowMessage = "DB issue: "+ex.Message
                                });
                            }
                        }
                    }
                    else
                    {
                        return Json(new
                        {
                            Message = "No",
                            ShowMessage = "Parent position not found to set functional manager"
                        });
                    }
                }
                else
                {
                    return Json(new
                    {
                        Message = "Failure",
                        ShowMessage = "Please select you own version and also ensure OrgPlanner/OrgAdmin"
                    });
                }
            }

            string FuncationalOrgChart = LI.GetOrgChartData(UCA.Role, UCA.Country, UCA.ShowLevel, UCA.ParentLevel, 
                                                            UCA.Levels, UCA.Oper, UCA.Version,
                                                            UCA.OrgChartType, UCA.SelectedPortraitModeMultipleLevel, UCA.SelectedFunctionalManagerType)[1];
            if (Session.Contents[UserData.UserName + "_MyModel"] != null)
            {
                MyModel MyModel = (MyModel)Session.Contents[UserData.UserName + "_MyModel"];
                MyModel.ChartData = FuncationalOrgChart;
                MyModel.ShowLevel = UCA.ShowLevel;
                MyModel.ParentLevel = UCA.ParentLevel;
                MyModel.Version = UCA.Version;
                Session.Contents[UserData.UserName + "_MyModel"] = MyModel;
            }

            return Json(new
            {
                Message = "Success",
                ShowMessage = "Functional Manager created",
                UsedDate = DateTime.Now,
                UsedShowLevel = UCA.ShowLevel,
                UsedParentLevel = UCA.ParentLevel,
                UsedVersion = UCA.Version,
                ChartData = FuncationalOrgChart
            });
        }

        public JsonResult ShowOutsideLegalEntity(string LevelId, string ParentLevelId)
        {
            string OutsideLegalEntity = "";
            LoginUsers UserData = LI.GetLoginUserInfo("");
            var UCA = (from uca in db.UserLastActions
                       where uca.UserId == UserData.UserName
                       select uca).FirstOrDefault();
            try
            {
                if (UCA != null)
                {
                    UserVersionDetails = LI.GetVersionDetails(Convert.ToInt32(UCA.Version), UCA.Role, UCA.Oper);
                    if (UserVersionDetails[0] == "Success")
                    {
                        string[] sTableName = LI.GetFinalyzerTable(UCA.Version, UCA.Oper);
                        string InsertData = "", FieldName = "";
                        string Query = "SELECT * FROM " + sTableName[0] + " WHERE LEVEL_ID='" + LevelId + "' AND PARENT_LEVEL_ID='" + ParentLevelId + "' AND VERSION='" + UCA.Version + "';";
                        DataTable dtQuery = ComClass.SQLReturnDataTable(Query);

                        if (dtQuery != null)
                        {
                            if (dtQuery.Rows.Count >= 1)
                            {
                                dtQuery.Rows[0]["VERIFY_FLAG"] = "Y";
                                if (dtQuery.Rows[0]["GRAY_COLORED_FLAG"].ToString() == "Y")
                                {
                                    dtQuery.Rows[0]["GRAY_COLORED_FLAG"] = "N";
                                    OutsideLegalEntity = "unset";
                                }
                                else
                                {
                                    dtQuery.Rows[0]["GRAY_COLORED_FLAG"] = "Y";
                                    OutsideLegalEntity = "set";
                                }

                                foreach (DataColumn dr in dtQuery.Columns)
                                {
                                    FieldName += "," + dr.ColumnName;
                                    InsertData += ",'" + dtQuery.Rows[0][dr.ColumnName] + "'";
                                }

                                Query += "UPDATE " + sTableName[0] + " SET GRAY_COLORED_FLAG='" + dtQuery.Rows[0]["GRAY_COLORED_FLAG"].ToString() + "' WHERE LEVEL_ID='" + LevelId + "' AND PARENT_LEVEL_ID='" + ParentLevelId + "' AND VERSION='" + UCA.Version + "';";
                                ComClass.ExecuteQuery(Query);
                            }
                        }
                    }
                    else
                    {
                        return Json(new
                        {
                            Message = "Failure",
                            ShowMessage = "Please select you own version and also ensure OrgPlanner/OrgAdmin"
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Message = "No",
                    ShowMessage = "DB issue: "+ex.Message
                });
            }

            return Json(new
            {
                Message = "Success",
                ShowMessage = "Outside legal entity is " + OutsideLegalEntity + " to this associate( "+ LevelId + " )",
                UsedDate = DateTime.Now,
                UsedShowLevel = UCA.ShowLevel,
                UsedParentLevel = UCA.ParentLevel,
                UsedVersion = UCA.Version,
                ChartData = LI.GetOrgChartData(UCA.Role, UCA.Country, UCA.ShowLevel, UCA.ParentLevel, 
                                               UCA.Levels, UCA.Oper, UCA.Version,
                                               UCA.OrgChartType, UCA.SelectedPortraitModeMultipleLevel, UCA.SelectedFunctionalManagerType)[1]
            });
        }

        public JsonResult CheckUnCheckAssistance(string ShowLevel, string ParentLevel)
        {
            LoginUsers UserData = LI.GetLoginUserInfo("");
            var UCA = (from uca in db.UserLastActions
                       where uca.UserId == UserData.UserName
                       select uca).FirstOrDefault();
            if (UCA != null)
            {
                try { 
                    string[] sTableName = LI.GetFinalyzerTable(UCA.Version, UCA.Oper);
                    string Query = "";
                    Query += "UPDATE " + sTableName[0] + " SET FLAG=(CASE WHEN FLAG='Y' THEN 'N' ELSE 'Y' END) WHERE LEVEL_ID='" + ShowLevel + "' AND PARENT_LEVEL_ID='" + ParentLevel + "' AND VERSION='" + UCA.Version + "';";
                    ComClass.ExecuteQuery(Query);
                }
                catch (Exception ex)
                {
                    return Json(new
                    {
                        Message = "Failure",
                        ShowMessage = "DB issue: " + ex.Message
                    });
                }
            }

            return Json(new
            {
                Success = "Success",
                ShowMessage = "Check/Uncheck Assistance saved",
                UsedDate = DateTime.Now,
                UsedShowLevel = UCA.ShowLevel,
                UsedParentLevel = UCA.ParentLevel,
                UsedVersion = UCA.Version,
                ChartData = LI.GetOrgChartData(UCA.Role, UCA.Country, UCA.ShowLevel, UCA.ParentLevel, 
                                               UCA.Levels, UCA.Oper, UCA.Version,
                                               UCA.OrgChartType, UCA.SelectedPortraitModeMultipleLevel, UCA.SelectedFunctionalManagerType)[1]
            });
        }


        public JsonResult SaveSorting(string SortedData, string SortLevel)
        {
            LoginUsers UserData = LI.GetLoginUserInfo("");
            var UCA = (from uca in db.UserLastActions
                       where uca.UserId == UserData.UserName
                       select uca).FirstOrDefault();
            if (UCA != null)
            {
                try
                {
                    string[] sTableName = LI.GetFinalyzerTable(UCA.Version, UCA.Oper);
                    string[] ArraySortedData = SortedData.Split(',');
                    string Query = ""; int Index = 1;
                    foreach (string asd in ArraySortedData)
                    {
                        Query += "UPDATE " + sTableName[0] + " SET SORTNO='" + Index.ToString() + "' WHERE LEVEL_ID='" + asd + "' AND PARENT_LEVEL_ID='" + SortLevel + "' AND VERSION='" + UCA.Version + "';";
                        Index++;
                    }
                    ComClass.ExecuteQuery(Query);

                    UCA.ShowLevel = SortLevel;
                    DataTable ShowTable = ComClass.SQLReturnDataTable("SELECT * FROM " + "[" + sTableName[0] + "] WHERE LEVEL_ID='" + SortLevel + "' AND MFLAG <> 'XFM' AND VERSION='" + UCA.Version.ToString() + "'");
                    UCA.ParentLevel = ShowTable.Rows[0]["PARENT_LEVEL_ID"].ToString();

                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    return Json(new
                    {
                        Message = "Failure",
                        ShowMessage = "DB issue: " + ex.Message
                    });
                }
            }

            return Json(new
            {
                Success = "Success",
                ShowMessage = "Current level is sorted",
                UsedDate = DateTime.Now,
                UsedShowLevel = UCA.ShowLevel,
                UsedParentLevel = UCA.ParentLevel,
                UsedVersion = UCA.Version,
                ChartData = LI.GetOrgChartData(UCA.Role, UCA.Country, UCA.ShowLevel, UCA.ParentLevel, 
                                               UCA.Levels, UCA.Oper, UCA.Version,
                                               UCA.OrgChartType, UCA.SelectedPortraitModeMultipleLevel, UCA.SelectedFunctionalManagerType)[1]
            });
        }

        public JsonResult GetSortLevel(string ShowLevel)
        {
            MyLastAction myla = LI.GetUserCurrentAction("");

            string[] sTableName = LI.GetFinalyzerTable(myla.Version, myla.Oper);
            DataTable ShowTable = ComClass.SQLReturnDataTable("SELECT * FROM " + "[" + sTableName[0] + "] " +
                                                              "     WHERE PARENT_LEVEL_ID='" + ShowLevel + "' AND VERSION='" + myla.Version.ToString() + "' " +
                                                              "     ORDER BY CAST(SORTNO AS INT), PARENT_LEVEL_ID, LEVEL_ID");

            return Json(new
            {
                Success = "Yes",
                IB = JsonConvert.SerializeObject(ShowTable)
            });
        }

        public JsonResult SaveEmployeeInformation(string EmployeeInformation, string ShowLevel)
        {
            try
            {
                LoginUsers UserData = LI.GetLoginUserInfo("");
                MyLastAction myla = LI.GetUserCurrentAction("");
                string[] MandatoryFields = ConfigurationManager.AppSettings["MiscFields"].ToString().Split(',');

                UserVersionDetails = LI.GetVersionDetails(Convert.ToInt32(myla.Version), myla.Role, myla.Oper);
                if (UserVersionDetails[0] == "Success")
                {
                    string FullNameFields = UserVersionDetails[11];
                    string UseFields = UserVersionDetails[17];
                    string KeyField = UserVersionDetails[9];
                    string ParentField = UserVersionDetails[10];

                    string[] sTableName = LI.GetFinalyzerTable(myla.Version, myla.Oper);
                    string Query = "UPDATE " + sTableName[0] + " SET ", QueryFields = "";
                    dynamic array = JsonConvert.DeserializeObject(EmployeeInformation);
                    foreach (var item in array.data)
                    {
                        string FULL_NAME = "";
                        if (FullNameFields != "")
                        {
                            string[] FN = FullNameFields.Split(',');
                            foreach (string strFN in FN)
                                FULL_NAME += " " + item[strFN];
                        }
                        string[] UF = UseFields.Split(',');
                        foreach (string strUF in UF)
                        {
                            if (Array.IndexOf(MandatoryFields, strUF) == -1 && KeyField != strUF && ParentField != strUF)
                                QueryFields += "," + strUF + "='" + item[strUF] + "'";
                        }
                        QueryFields += ",FULL_NAME='" + FULL_NAME + "'";
                    }
                    Query += (QueryFields == "" ? "" : QueryFields.Substring(1)) + " WHERE LEVEL_ID = '" + ShowLevel + "' AND VERSION = '" + myla.Version + "'; ";
                    ComClass.ExecuteQuery(Query);
                }
                else
                {
                    return Json(new
                    {
                        Success = "Failure",
                        ChartData = LI.GetOrgChartData(myla.Role, myla.Country, myla.ShowLevel, myla.ParentLevel, 
                                                       myla.Levels, myla.Oper, myla.Version,
                                                       myla.OrgChartType, myla.SelectedPortraitModeMultipleLevel, myla.SelectedFunctionalManagerType)[1],
                        ShowMessage = "Error: Employee Information not saved, Please select your own version to edit employee information"
                    });
                }

                return Json(new
                {
                    Success = "Success",
                    ChartData = LI.GetOrgChartData(myla.Role, myla.Country, myla.ShowLevel, myla.ParentLevel, 
                                                   myla.Levels, myla.Oper, myla.Version,
                                                   myla.OrgChartType, myla.SelectedPortraitModeMultipleLevel, myla.SelectedFunctionalManagerType)[1],
                    ShowMessage = "Employee Information Saved"
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Success = "No",
                    ShowMessage = "Error: " + ex.Message
                });
            }

        }

        public JsonResult ShowEmployeeInformationFields(string ShowLevel)
        {
            try
            {
                LoginUsers UserData = LI.GetLoginUserInfo("");
                MyLastAction myla = LI.GetUserCurrentAction("");

                string[] sTableName = LI.GetFinalyzerTable(myla.Version, myla.Oper);
                int Version = Convert.ToInt32(sTableName[1]);
                var UFH = (from ufh in db.UploadFilesHeaders
                           where ufh.CompanyName == UserData.CompanyName &&
                                 ufh.Role == "Finalyzer" &&
                                 ufh.VersionNo == Version
                           select ufh).FirstOrDefault();
                DataTable ShowTable = ComClass.SQLReturnDataTable("SELECT " + UFH.ExcelDownLoadFields.Substring(1) + " FROM " + "[" + sTableName[0] + "] WHERE LEVEL_ID='" + ShowLevel + "' AND VERSION='" + myla.Version.ToString() + "'");

                return Json(new
                {
                    Success = "Yes",
                    IB = JsonConvert.SerializeObject(ShowTable),
                    PF = UFH.FirstPositionField
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Success = "No",
                    ShowMessage = ex.Message
                });
            }
        }


        public JsonResult ShowLevelInfo(string ShowLevel)
        {
            try
            {
                LoginUsers UserData = LI.GetLoginUserInfo("");
                MyLastAction myla = LI.GetUserCurrentAction("");

                string[] sTableName = LI.GetFinalyzerTable(myla.Version, myla.Oper);
                int Version = Convert.ToInt32(sTableName[1]);
                var UFH = (from ufh in db.UploadFilesHeaders
                           where ufh.CompanyName == UserData.CompanyName &&
                                 ufh.Role == "Finalyzer" &&
                                 ufh.VersionNo == Version
                           select ufh).FirstOrDefault();
                DataTable ShowTable = ComClass.SQLReturnDataTable("SELECT * FROM " + "[" + sTableName[0] + "] WHERE LEVEL_ID='" + ShowLevel + "' AND VERSION='" + myla.Version.ToString() + "'");

                return Json(new
                {
                    Success = "Yes",
                    IB = JsonConvert.SerializeObject(ShowTable),
                    PF = UFH.FirstPositionField
                });
            }
            catch(Exception ex)
            {
                return Json(new
                {
                    Success = "No",
                    ShowMessage = ex.Message
                });
            }
        }

        private string InsertDynamicDataToDB(List<dynamic> lstDynamic, string VersionNumber, string Oper, string ShowLevel, 
                                             string CountryName, string PositionCostField)
        {
            LoginUsers UserData = LI.GetLoginUserInfo("");
            string VersionNo = "", ErrorString = "", sFields = "", sValues = "", sQuery="";
            string sTableName = UserData.CompanyName.ToString().Trim().ToUpper().Replace(" ", "_") + "_" + VersionNumber + "_LevelInfos";
            if (Oper == "LV") sTableName = UserData.CompanyName.ToString().Trim().ToUpper().Replace(" ", "_") + "_" + VersionNumber + "_LegalInfos";

            sQuery = "DROP TABLE IF EXISTS [dbo].[" + UserData.CompanyName.ToString().Trim().ToUpper().Replace(" ", "_") + "_" + VersionNumber + "_DIRECT_REPORT];";
            ComClass.ExecuteQuery(sQuery);
            sQuery = " CREATE TABLE[dbo].[" + UserData.CompanyName.ToString().Trim().ToUpper().Replace(" ", "_") + "_" + VersionNumber + "_DIRECT_REPORT]" +
                     "(" +
                     "    [ID][int] IDENTITY(1,1) NOT NULL," +
                     "    [PositionID] [varchar] (50) NULL," +
                     "    [KeyDate] [varchar] (50) NULL," +
                     "    [DirectReport] [varchar] (max) NULL," +
                     "    [NextLevel] [varchar] (max) NULL," +
                     "    [DrType] [varchar] (50) NULL," +
                     "    [Country] [varchar] (50) NULL," +
                     "    [LevelNo] [int] NULL," +
                     "    [SOC] [int] NULL," +
                     "    [MaxLevel] [int] NULL," +
                     "    [TemplateName] [varchar] (500) NULL," +
                     "    [DownloadType] [varchar] (50) NULL," +
                     "    [CompanyName] [varchar] (500) NULL," +
                     "    CONSTRAINT[PK_" + UserData.CompanyName.ToString().Trim().ToUpper().Replace(" ", "_") + "_" + VersionNumber + "_DIRECT_REPORT] PRIMARY KEY CLUSTERED" +
                     "    (" +
                     "         [ID] ASC" +
                     "    )" +
                     ")";
            ComClass.ExecuteQuery(sQuery);

            sQuery = "DROP TABLE IF EXISTS [dbo].[" + UserData.CompanyName.ToString().Trim().ToUpper().Replace(" ", "_") + "_" + VersionNumber + "_DIRECT_REPORT_DATA];";
            ComClass.ExecuteQuery(sQuery);
            sQuery = "CREATE TABLE [dbo].[" + UserData.CompanyName.ToString().Trim().ToUpper().Replace(" ", "_") + "_" + VersionNumber + "_DIRECT_REPORT_DATA]( " +
                     "    [ID][int] IDENTITY(1, 1) NOT NULL,   " +
                     "    [DRIndex] [int] NULL,   " +
                     "    [PositionID] [varchar] (50) NULL,   " +
                     "    [DrType] [varchar] (50) NULL,   " +
                     "    [Data1] [varchar] (max) NULL,   " +
                     "    [Data2] [varchar] (max) NULL,   " +
                     "    [Data3] [varchar] (max) NULL,   " +
                     "    [Data4] [varchar] (max) NULL,   " +
                     "    [UpdatedDate] [varchar] (50) NULL,   " +
                     "    [TemplateName] [varchar] (500) NULL,   " +
                     "    [DownloadType] [varchar] (50) NULL,   " +
                     "    [CompanyName] [varchar] (500) NULL,   " +
                     "  CONSTRAINT[PK_" + UserData.CompanyName.ToString().Trim().ToUpper().Replace(" ", "_") + "_" + VersionNumber + "_DIRECT_REPORT_DATA] PRIMARY KEY CLUSTERED   " +
                     "  (   " +
                     "     [ID] ASC   " +
                     "  )   " +
                     ")";
            ComClass.ExecuteQuery(sQuery);

            sQuery = "DROP TABLE IF EXISTS [dbo].[" + UserData.CompanyName.ToString().Trim().ToUpper().Replace(" ", "_") + "_" + VersionNumber + "_DIRECT_REPORT_LV];";
            ComClass.ExecuteQuery(sQuery);
            sQuery = " CREATE TABLE[dbo].[" + UserData.CompanyName.ToString().Trim().ToUpper().Replace(" ", "_") + "_" + VersionNumber + "_DIRECT_REPORT_LV]" +
                     "(" +
                     "    [ID][int] IDENTITY(1,1) NOT NULL," +
                     "    [PositionID] [varchar] (50) NULL," +
                     "    [KeyDate] [varchar] (50) NULL," +
                     "    [DirectReport] [varchar] (max) NULL," +
                     "    [NextLevel] [varchar] (max) NULL," +
                     "    [DrType] [varchar] (50) NULL," +
                     "    [Country] [varchar] (50) NULL," +
                     "    [LevelNo] [int] NULL," +
                     "    [SOC] [int] NULL," +
                     "    [MaxLevel] [int] NULL," +
                     "    [TemplateName] [varchar] (500) NULL," +
                     "    [DownloadType] [varchar] (50) NULL," +
                     "    [CompanyName] [varchar] (500) NULL," +
                     "    CONSTRAINT[PK_" + UserData.CompanyName.ToString().Trim().ToUpper().Replace(" ", "_") + "_" + VersionNumber + "_DIRECT_REPORT_LV] PRIMARY KEY CLUSTERED" +
                     "    (" +
                     "         [ID] ASC" +
                     "    )" +
                     ")";
            ComClass.ExecuteQuery(sQuery);

            sQuery = "DROP TABLE IF EXISTS  [dbo].[" + UserData.CompanyName.ToString().Trim().ToUpper().Replace(" ", "_") + "_" + VersionNumber + "_DIRECT_REPORT_DATA_LV];";
            ComClass.ExecuteQuery(sQuery);
            sQuery = "CREATE TABLE [dbo].[" + UserData.CompanyName.ToString().Trim().ToUpper().Replace(" ", "_") + "_" + VersionNumber + "_DIRECT_REPORT_DATA_LV]( " +
                     "    [ID][int] IDENTITY(1, 1) NOT NULL,   " +
                     "    [DRIndex] [int] NULL,   " +
                     "    [PositionID] [varchar] (50) NULL,   " +
                     "    [DrType] [varchar] (50) NULL,   " +
                     "    [Data1] [varchar] (max) NULL,   " +
                     "    [Data2] [varchar] (max) NULL,   " +
                     "    [Data3] [varchar] (max) NULL,   " +
                     "    [Data4] [varchar] (max) NULL,   " +
                     "    [UpdatedDate] [varchar] (50) NULL,   " +
                     "    [TemplateName] [varchar] (500) NULL,   " +
                     "    [DownloadType] [varchar] (50) NULL,   " +
                     "    [CompanyName] [varchar] (500) NULL,   " +
                     "  CONSTRAINT[PK_" + UserData.CompanyName.ToString().Trim().ToUpper().Replace(" ", "_") + "_" + VersionNumber + "_DIRECT_REPORT_DATA_LV] PRIMARY KEY CLUSTERED   " +
                     "  (   " +
                     "     [ID] ASC   " +
                     "  )   " +
                     ")";
            ComClass.ExecuteQuery(sQuery);

            sQuery = "DROP TABLE IF EXISTS [dbo].[" + sTableName + "]";
            ComClass.ExecuteQuery(sQuery);
            sQuery = "CREATE TABLE [dbo].[" + sTableName + "] (";
            var expandoField = (IDictionary<string, object>)lstDynamic[0];
            foreach (var pairKV in expandoField)
            {
                if (pairKV.Key == "VERSION" || pairKV.Key == "LEVEL_ID" || pairKV.Key == "PARENT_LEVEL_ID" || pairKV.Key == "USER_ID" || pairKV.Key == "MFLAG" || pairKV.Key == "DATE_UPDATED")
                    sFields += ", [" + pairKV.Key.ToString().Trim().ToUpper().Replace(" ", "_") + "][varchar](100)";
                else if (pairKV.Key == "VERIFY_FLAG")
                    sFields += ", [" + pairKV.Key.ToString().Trim().ToUpper().Replace(" ", "_") + "][varchar](200) NULL";
                else if (pairKV.Key == "BREAD_GRAM_NAME")
                    sFields += ", [" + pairKV.Key.ToString().Trim().ToUpper().Replace(" ", "_") + "][varchar](MAX) NULL";
                else
                    sFields += ", [" + pairKV.Key.ToString().Trim().ToUpper().Replace(" ", "_") + "][varchar](200) NULL";
            }
            sQuery += sFields.Substring(1) + ", CONSTRAINT [PK_" + sTableName + "] PRIMARY KEY CLUSTERED ( [USER_ID] ASC, [VERSION] ASC, [LEVEL_ID] ASC, [PARENT_LEVEL_ID] ASC, [MFLAG] ASC, [DATE_UPDATED] ASC) )  ON [PRIMARY]";
            ComClass.ExecuteQuery(sQuery);

            sFields = ""; sValues = "";
            sQuery = "DROP TABLE IF EXISTS [dbo].[BACKUP_" + sTableName + "] ";
            ComClass.ExecuteQuery(sQuery);

            sQuery = "CREATE TABLE [dbo].[BACKUP_" + sTableName + "] (";
            expandoField = (IDictionary<string, object>)lstDynamic[0];
            foreach (var pairKV in expandoField)
            {
                if (pairKV.Key == "VERSION" || pairKV.Key == "LEVEL_ID" || pairKV.Key == "PARENT_LEVEL_ID" || pairKV.Key == "USER_ID" || pairKV.Key == "MFLAG" || pairKV.Key == "DATE_UPDATED")
                    sFields += ", [" + pairKV.Key.ToString().Trim().ToUpper().Replace(" ", "_") + "][varchar](100)";
                else if (pairKV.Key == "VERIFY_FLAG")
                    sFields += ", [" + pairKV.Key.ToString().Trim().ToUpper().Replace(" ", "_") + "][varchar](200) NULL";
                else if (pairKV.Key == "BREAD_GRAM_NAME")
                    sFields += ", [" + pairKV.Key.ToString().Trim().ToUpper().Replace(" ", "_") + "][varchar](MAX) NULL";
                else
                    sFields += ", [" + pairKV.Key.ToString().Trim().ToUpper().Replace(" ", "_") + "][varchar](200) NULL";
            }
            sQuery += sFields.Substring(1) + ", CONSTRAINT [PK_BACKUP_" + sTableName + "] PRIMARY KEY CLUSTERED ( [USER_ID] ASC, [VERSION] ASC, [LEVEL_ID] ASC, [PARENT_LEVEL_ID] ASC, [MFLAG] ASC, [DATE_UPDATED] ASC) )  ON [PRIMARY]";
            ComClass.ExecuteQuery(sQuery);

            sFields = ""; sValues = "";
            sQuery = "DROP TABLE IF EXISTS [dbo].[TEMP_" + sTableName + "] ";
            ComClass.ExecuteQuery(sQuery);

            sQuery = "CREATE TABLE [dbo].[TEMP_" + sTableName + "] (";
            expandoField = (IDictionary<string, object>)lstDynamic[0];
            foreach (var pairKV in expandoField)
            {
                if (pairKV.Key == "VERSION" || pairKV.Key == "LEVEL_ID" || pairKV.Key == "PARENT_LEVEL_ID" || pairKV.Key == "USER_ID" || pairKV.Key == "MFLAG" || pairKV.Key == "DATE_UPDATED")
                    sFields += ", [" + pairKV.Key.ToString().Trim().ToUpper().Replace(" ", "_") + "][varchar](100)";
                else if (pairKV.Key == "VERIFY_FLAG")
                    sFields += ", [" + pairKV.Key.ToString().Trim().ToUpper().Replace(" ", "_") + "][varchar](200) NULL";
                else if (pairKV.Key == "BREAD_GRAM_NAME")
                    sFields += ", [" + pairKV.Key.ToString().Trim().ToUpper().Replace(" ", "_") + "][varchar](MAX) NULL";
                else
                    sFields += ", [" + pairKV.Key.ToString().Trim().ToUpper().Replace(" ", "_") + "][varchar](200) NULL";
            }
            sQuery += sFields.Substring(1) + ", CONSTRAINT [PK_TEMP_" + sTableName + "] PRIMARY KEY CLUSTERED ( [USER_ID] ASC, [VERSION] ASC, [LEVEL_ID] ASC, [PARENT_LEVEL_ID] ASC, [MFLAG] ASC, [DATE_UPDATED] ASC) )  ON [PRIMARY]";
            ComClass.ExecuteQuery(sQuery);

            int recCount = 0; sQuery = "";
            foreach (ExpandoObject recObj in lstDynamic)
            {
                try
                {
                    var expandoRecord = (IDictionary<string, object>)recObj;
                    sQuery += "INSERT INTO [dbo].[" + sTableName + "] VALUES( ";
                    foreach (var pairKV in expandoRecord)
                    {
                        if (pairKV.Value == null)
                        {
                            if (pairKV.Key == "VERSION") VersionNo = "";
                            sValues += ",''";
                        }
                        else
                        {
                            if (pairKV.Key == "VERSION") VersionNo = pairKV.Value.ToString().Trim();
                            sValues += ",'" + pairKV.Value.ToString().Replace("'", "''") + "'";
                        }
                    }
                    sQuery += sValues.Substring(1) + " );";
                    sValues = "";
                }
                catch(Exception ex)
                {

                }

                recCount++;
                if (recCount >= 100)
                {
                    try
                    {
                        ComClass.ExecuteQuery(sQuery);
                    }
                    catch(Exception ex)
                    {
                        ErrorString += ex.Message;
                    }
                    recCount = 0; sQuery = "";
                }
            }
            if (recCount >= 1)
            {
                try
                {
                    ComClass.ExecuteQuery(sQuery);
                }
                catch(Exception ex)
                {
                    ErrorString += ex.Message;
                }
            }

            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                cmd.CommandText = "PROC_GET_TREE_OPERATIONALCHART_NOR_SOC";

                cmd.Parameters.Add("@POSITION_COST", SqlDbType.VarChar, 150).Value = PositionCostField;
                cmd.Parameters.Add("@FINALYZER_VERSION", SqlDbType.VarChar, 150).Value = VersionNo;
                cmd.Parameters.Add("@VERSION", SqlDbType.VarChar, 150).Value = VersionNo;
                cmd.Parameters.Add("@USERTYPE", SqlDbType.VarChar, 50).Value = "Finalyzer";
                cmd.Parameters.Add("@COMPANYNAME", SqlDbType.VarChar, 150).Value = UserData.CompanyName.ToString().Trim().ToUpper();
                cmd.Parameters.Add("@USERID", SqlDbType.VarChar, 150).Value = UserData.UserName.ToString().Trim().ToUpper();
                cmd.Parameters.Add("@OPER", SqlDbType.VarChar, 150).Value = Oper.Trim().ToUpper();

                Common csobj = new Common();
                csobj.SPReturnDataTable(cmd);
            }

            return ErrorString;
        }

        private string InsertPlayerDataToDB(List<dynamic> lstDynamic, string BaseVersion, string VersionNo, string OldVersion, string Oper, 
                                            string ShowLevel, string PositionCostField, string RoleType)
        {
            LoginUsers UserData = LI.GetLoginUserInfo("");
            string sValues = "", ErrorString = "", sQuery = "", sTableName = "";
            int IntOldVersion = Convert.ToInt32(VersionNo), VersionNumber = Convert.ToInt32(BaseVersion);

            VersionDetails VD = (from vd in db.VersionDetails
                                 where vd.VersionNo == IntOldVersion &&
                                       vd.ActiveVersion == "Y"
                                 select vd).FirstOrDefault();
            if (VD != null)
            {
                if (VD.UserRole == "SuperAdmin" || VD.UserRole == "Player")
                {
                    sQuery = "DELETE FROM [dbo].[" + sTableName + "] WHERE USER_ID='" + UserData.UserName + "' AND VERSION='" + OldVersion + "'";
                    ComClass.ExecuteQuery(sQuery);
                    VersionNumber = (int)VD.FinalyzeVersionNo;
                }
            }
            sTableName = UserData.CompanyName.ToString().Trim().ToUpper().Replace(" ", "_") + "_" + VersionNumber + "_LevelInfos";
            if (Oper == "LV") sTableName = UserData.CompanyName.ToString().Trim().ToUpper().Replace(" ", "_") + "_" + VersionNumber + "_LegalInfos";

            int recCount = 0; sQuery = "";
            foreach (ExpandoObject recObj in lstDynamic)
            {
                try
                {
                    var expandoRecord = (IDictionary<string, object>)recObj;
                    sQuery += "INSERT INTO [dbo].[" + sTableName + "] VALUES( ";
                    foreach (var pairKV in expandoRecord)
                        sValues += ",'" + pairKV.Value.ToString().Replace("'", "''") + "'";
                    sQuery += sValues.Substring(1) + " );";
                    sValues = "";
                }
                catch(Exception ex)
                {
                }

                recCount++;
                if (recCount >= 100)
                {
                    try
                    {
                        ComClass.ExecuteQuery(sQuery);
                    }
                    catch(Exception ex)
                    {
                        ErrorString += ex.Message;
                    }
                    recCount = 0; sQuery = "";
                }
            }
            if (recCount >= 1)
            {
                try
                {
                    ComClass.ExecuteQuery(sQuery);
                }
                catch(Exception ex)
                {
                    ErrorString += ex.Message;
                }
            }

            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                cmd.CommandText = "PROC_GET_TREE_OPERATIONALCHART_NOR_SOC";

                cmd.Parameters.Add("@POSITION_COST", SqlDbType.VarChar, 150).Value = PositionCostField;
                cmd.Parameters.Add("@FINALYZER_VERSION", SqlDbType.VarChar, 150).Value = BaseVersion;
                cmd.Parameters.Add("@VERSION", SqlDbType.VarChar, 150).Value = VersionNo;
                cmd.Parameters.Add("@USERTYPE", SqlDbType.VarChar, 50).Value = RoleType;
                cmd.Parameters.Add("@COMPANYNAME", SqlDbType.VarChar, 150).Value = UserData.CompanyName.ToString().Trim().ToUpper();
                cmd.Parameters.Add("@USERID", SqlDbType.VarChar, 150).Value = UserData.UserName.ToString().Trim().ToUpper();
                cmd.Parameters.Add("@OPER", SqlDbType.VarChar, 150).Value = Oper.Trim().ToUpper();

                Common csobj = new Common();
                csobj.SPReturnDataTable(cmd);
            }

            return ErrorString;
        }

        private string UpdateTableWithJSON(string RoleType, string BaseVersion, string VersionNo, string OldVersion,
                                         string Oper, string Country, string ShowLevel)
        {
            LoginUsers UserData = LI.GetLoginUserInfo("");
            int iShowCount = 0, iNullCount = 0, iKey = 0;
            string jsonData = "";
            List<string> lstParentName = new List<string>();
            List<dynamic> lstDynamic = new List<dynamic>();

            var UploadExcelFile = (from uef in db.UploadFilesHeaders
                                   where uef.CompanyName == UserData.CompanyName && 
                                         uef.UserId == UserData.UserName && 
                                         uef.Role == RoleType
                                   select uef).OrderByDescending(x => x.CreatedDate).FirstOrDefault();

            string ServerMapPath = System.Web.HttpContext.Current.Server.MapPath("~/App_Data/Uploads/" + UploadExcelFile.UploadFileName);
            if (UploadExcelFile.FileType == "JSON")
            {
                using (StreamReader reader = new StreamReader(ServerMapPath))
                {
                    jsonData = reader.ReadToEnd();
                }
            }
            else if (UploadExcelFile.FileType == "XLSX")
            {
                ExcelAPI API = new ExcelAPI();
                string[] RetValue = { "", "" };
                RetValue = API.CreateJSONFromExcel(ServerMapPath, (UploadExcelFile != null) ? UploadExcelFile.ShowFieldType : "NONE");
                jsonData = RetValue[0];
            }

            string SUP_DISPLAY_NAME = UploadExcelFile.ParentField;
            string RNUM = UploadExcelFile.FirstPositionField;
            string SNUM = "";

            dynamic array = JsonConvert.DeserializeObject(jsonData);
            foreach (var item in array.data)
            {
                if (item[SUP_DISPLAY_NAME] != null) iShowCount++;
                if (item[SUP_DISPLAY_NAME] == null) iNullCount++;

                if (UploadExcelFile.SerialNoFlag == "Y")
                    SNUM = (100000 + Convert.ToInt32(item[RNUM])).ToString();
                else
                    SNUM = (100000 + iKey++).ToString();
                if (item[SUP_DISPLAY_NAME] != null)
                {
                    if (lstParentName.Count() >= 1)
                    {
                        var match = lstParentName.FirstOrDefault(stringToCheck => stringToCheck.Contains(item[SUP_DISPLAY_NAME].ToString().Trim()));
                        if (match == null) lstParentName.Add(item[SUP_DISPLAY_NAME].ToString().Trim());
                    }
                    else lstParentName.Add(item[SUP_DISPLAY_NAME].ToString().Trim());
                }

                // Employee Details
                if (UploadExcelFile.UseFields != "")
                {
                    string FULL_NAME = "";
                    if (UploadExcelFile.FullNameFields != "")
                    {
                        string[] FN = UploadExcelFile.FullNameFields.Split(',');
                        foreach (string strFN in FN)
                            FULL_NAME += " " + item[strFN];
                    }
                    dynamic DyObj = new ExpandoObject();
                    string[] UF = UploadExcelFile.UseFields.Split(',');
                    foreach (string strUF in UF)
                    {
                        string strField = strUF.Trim().ToUpper().Replace(" ", "_");
                        if (strField == "LEVEL_ID" && UploadExcelFile.SerialNoFlag == "Y") AddProperty(DyObj, strField, SNUM);
                        else if (strField == "LEVEL_ID" && UploadExcelFile.SerialNoFlag == "N") AddProperty(DyObj, strField, item[UploadExcelFile.KeyField]);
                        else if (strField == "PARENT_LEVEL_ID" && UploadExcelFile.SerialNoFlag == "Y") AddProperty(DyObj, strField, "999999");
                        else if (strField == "PARENT_LEVEL_ID" && UploadExcelFile.SerialNoFlag == "N") AddProperty(DyObj, strField, item[UploadExcelFile.ParentField]);
                        else if (strField == "COUNTRY") AddProperty(DyObj, strField, "CH");
                        else if (strField == "VERSION") AddProperty(DyObj, strField, VersionNo);
                        else if (strField == "FULL_NAME") AddProperty(DyObj, strField, (FULL_NAME == "") ? "" : FULL_NAME.Substring(1));
                        else if (strField == "VERIFY_FLAG") AddProperty(DyObj, strField, "Y");
                        else if (strField == "LEVEL_NO") AddProperty(DyObj, strField, "0");
                        else if (strField == "BREAD_GRAM") AddProperty(DyObj, strField, "");
                        else if (strField == "BREAD_GRAM_NAME") AddProperty(DyObj, strField, "");
                        else if (strField == "NOR_COUNT") AddProperty(DyObj, strField, "0");
                        else if (strField == "SOC_COUNT") AddProperty(DyObj, strField, "0");
                        else if (strField == "POSITION_CALCULATED_COST") AddProperty(DyObj, strField, item[UploadExcelFile.PositionCostField]);
                        else if (strField == "NEXT_LEVEL_FLAG") AddProperty(DyObj, strField, "N");
                        else if (strField == "GRAY_COLORED_FLAG") AddProperty(DyObj, strField, "N");
                        else if (strField == "DOTTED_LINE_FLAG") AddProperty(DyObj, strField, "N");
                        else if (strField == "SHOW_FULL_BOX") AddProperty(DyObj, strField, "Y");
                        else if (strField == "LANGUAGE_SELECTED") AddProperty(DyObj, strField, "EN");
                        else if (strField == "SORTNO") AddProperty(DyObj, strField, "0");
                        else if (strField == "POSITIONFLAG") AddProperty(DyObj, strField, "0");
                        else if (strField == "FLAG") AddProperty(DyObj, strField, "N");
                        else if (strField == "MFLAG") AddProperty(DyObj, strField, "XXX");
                        else if (strField == "REPORTING_TO_FM") AddProperty(DyObj, strField, "0");
                        else if (strField == "SHOW_REPORTING_TO_FM") AddProperty(DyObj, strField, "0");
                        else if (strField == "NODE_LOCATION_COL") AddProperty(DyObj, strField, "0");
                        else if (strField == "NODE_LOCATION_ROW") AddProperty(DyObj, strField, "0");
                        else if (strField == "NODE_LOCATION") AddProperty(DyObj, strField, "");
                        else if (strField == "DATE_UPDATED") AddProperty(DyObj, strField, DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss"));
                        else if (strField == "USER_ID") AddProperty(DyObj, strField, (RoleType == "SuperAdmin" || RoleType == "Player") ? UserData.UserName : UserData.UserName);
                        else AddProperty(DyObj, strField, ((item[strUF] == null) ? "" : item[strUF]));
                    }
                    lstDynamic.Add(DyObj);
                }
            }

            // Gets the Parent name
            try
            {
                int Index = 0;
                foreach (string pn in lstParentName)
                {
                    ExpandoObject ObjId = lstDynamic.Where(Obj => Obj.FULL_NAME == pn).FirstOrDefault();
                    if (ObjId != null)
                    {
                        var expandoLEVEL_ID = (IDictionary<string, object>)ObjId;

                        Index++;
                        var Objects = lstDynamic.Where(Obj => Obj.SUP_DISPLAY_NAME == pn).ToList();
                        foreach (ExpandoObject md in Objects)
                        {
                            var expandoPARENT_ID = (IDictionary<string, object>)md;
                            expandoPARENT_ID["PARENT_LEVEL_ID"] = expandoLEVEL_ID["LEVEL_ID"].ToString();
                        }
                    }
                }
            }
            catch(Exception ex)
            {

            }

            // Insert the data into SQL table
            string ErrorString = "";
            if (RoleType == "SuperAdmin" || RoleType == "Player")
            {
                BackupExistingVersion(OldVersion);
                ErrorString=InsertPlayerDataToDB(lstDynamic, BaseVersion, VersionNo, OldVersion, Oper, ShowLevel, UploadExcelFile.PositionCostField, RoleType);
            }
            else if (RoleType == "Finalyzer")
            {
                ErrorString=InsertDynamicDataToDB(lstDynamic, VersionNo, Oper, ShowLevel, Country, UploadExcelFile.PositionCostField);
            }

            return ErrorString;
        }

        private void BackupExistingVersion(string OldVersion)
        {
            LoginUsers UserData = LI.GetLoginUserInfo("");
            int IntOldVersion = Convert.ToInt32(OldVersion);
            VersionDetails VD = (from vd in db.VersionDetails
                                 where vd.VersionNo == IntOldVersion &&
                                       vd.ActiveVersion == "Y"
                                 select vd).FirstOrDefault();
            if (VD != null)
            {
                if (VD.UserRole == "Player" || VD.UserRole == "SuperAdmin")
                {
                    DataTable retDT = null;
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.CommandText = "PROC_GET_POSITION_TREE_BACKUP_VERSION";

                        cmd.Parameters.Add("@VERSION", SqlDbType.VarChar, 150).Value = OldVersion;
                        cmd.Parameters.Add("@COMPANYNAME", SqlDbType.VarChar, 150).Value = UserData.CompanyName;
                        cmd.Parameters.Add("@OPER", SqlDbType.VarChar, 150).Value = VD.OperType;

                        retDT = ComClass.SPReturnDataTable(cmd);
                    }

                    // Remove predefined column
                    retDT.Columns.Remove("USER_ID");
                    retDT.Columns.Remove("LEVEL_ID");
                    retDT.Columns.Remove("PARENT_LEVEL_ID");
                    retDT.Columns.Remove("COUNTRY");
                    retDT.Columns.Remove("VERSION");
                    retDT.Columns.Remove("DATE_UPDATED");
                    retDT.Columns.Remove("FULL_NAME");
                    retDT.Columns.Remove("VERIFY_FLAG");
                    retDT.Columns.Remove("LEVEL_NO");
                    retDT.Columns.Remove("BREAD_GRAM");
                    retDT.Columns.Remove("BREAD_GRAM_NAME");
                    retDT.Columns.Remove("NOR_COUNT");
                    retDT.Columns.Remove("SOC_COUNT");
                    retDT.Columns.Remove("POSITION_CALCULATED_COST");
                    retDT.Columns.Remove("NEXT_LEVEL_FLAG");
                    retDT.Columns.Remove("GRAY_COLORED_FLAG");
                    retDT.Columns.Remove("DOTTED_LINE_FLAG");
                    retDT.Columns.Remove("SHOW_FULL_BOX");
                    retDT.Columns.Remove("LANGUAGE_SELECTED");
                    retDT.Columns.Remove("SORTNO");
                    retDT.Columns.Remove("POSITIONFLAG");
                    retDT.Columns.Remove("FLAG");
                    retDT.Columns.Remove("MFLAG");
                    retDT.Columns.Remove("REPORTING_TO_FM");
                    retDT.Columns.Remove("SHOW_REPORTING_TO_FM");
                    retDT.Columns.Remove("NODE_LOCATION_COL");
                    retDT.Columns.Remove("NODE_LOCATION_ROW");
                    retDT.Columns.Remove("NODE_LOCATION");

                    string BackupFileName = UserData.UserName + "_" + UserData.CompanyName + "_" + OldVersion + "_" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".xlsx";
                    string ServerMapPath = System.Web.HttpContext.Current.Server.MapPath("~/App_Data/Versions/" + BackupFileName);
                    XlsxAPI.CreateSpreadsheetWorkbook(ServerMapPath, retDT);

                    var UploadFilesDetails = (from ufd in db.UploadFilesDetails
                                              where ufd.UserId == UserData.UserName &&
                                                   ufd.CompanyName == UserData.CompanyName &&
                                                   (ufd.Role == "SuperAdmin" || ufd.Role == "Player") &&
                                                   ufd.VersionNo == IntOldVersion
                                              select ufd).FirstOrDefault();
                    if (UploadFilesDetails != null)
                    {
                        UploadFilesDetails.BackUpFile = BackupFileName;
                        db.SaveChanges();
                    }
                }
            }
        }

        [HttpPost]
        public ActionResult SaveInitiative(string Name, string Description)
        {
            LoginUsers UserData = LI.GetLoginUserInfo("");
            var UCA = (from uca in db.UserLastActions
                       where uca.UserId == UserData.UserName
                       select uca).FirstOrDefault();
            if (UCA != null)
            {
                var FinalyzerVD = (from vd in db.VersionDetails
                             where vd.CompanyName == UserData.CompanyName && 
                                   vd.ActiveVersion == "Y" && 
                                   vd.OperType == UCA.Oper &&
                                   vd.UserRole == "Finalyzer"
                             select new { vd.UserName, vd.CompanyName, vd.UserRole, vd.OperType, vd.Country, vd.Initiative, vd.Population, vd.Version }).FirstOrDefault();
                if (FinalyzerVD == null && UCA.Role == "Player")
                {
                    return Json(new
                    {
                        Success = "No",
                        ShowMessage = "Finalyzer not yet started",
                        DDL = "[]"
                    });
                }

                VersionDetails VD = (from vd in db.VersionDetails
                                     where vd.CompanyName == UserData.CompanyName &&
                                           vd.UserName == UserData.UserName &&
                                           vd.Initiative == Name &&
                                           vd.Population == null &&
                                           vd.UserName == null &&
                                           vd.Version == null &&
                                           vd.ActiveVersion == "Y"
                                     select vd).FirstOrDefault();
                if (VD == null)
                {
                    MyLastAction myla = LI.GetUserCurrentAction("");

                    VD = new VersionDetails();
                    VD.Initiative = Name;
                    VD.IDescription = Description;
                    VD.UserName = UserData.UserName;
                    VD.CompanyName = UserData.CompanyName;
                    VD.UserRole = UCA.Role;
                    VD.Country = UCA.Country;
                    VD.OperType = UCA.Oper;
                    VD.ActiveVersion = "Y";

                    db.VersionDetails.Add(VD);
                    try
                    {
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        return Json(new
                        {
                            Success = "No",
                            ShowMessage = ex.Message,
                            DDL = "[]"
                        });
                    }

                    var lstVD = (from vd in db.VersionDetails
                                 where vd.CompanyName == UserData.CompanyName && vd.ActiveVersion == "Y" && vd.OperType == UCA.Oper &&
                                       ((vd.UserRole == "SuperAdmin" && UCA.Role == "SuperAdmin") ||
                                        (vd.UserRole == "Player" && UCA.Role == "Player") ||
                                         UCA.Role == "Finalyzer" || 
                                         UCA.Role == "User" || 
                                         UCA.Role == "EndUser")
                                 select new { vd.UserName, vd.CompanyName, vd.UserRole, vd.OperType, vd.Country, vd.Initiative, vd.Population, vd.Version }).Distinct().ToList();
                    return Json(new
                    {
                        Success = "Yes",
                        ShowMessage = "Initiate saved",
                        DDL = JsonConvert.SerializeObject(lstVD)
                    });
                }
                return Json(new
                {
                    Success = "No",
                    ShowMessage = "Initiative already exist",
                    DDL = "[]"
                });
            }

            return Json(new
            {
                Success = "No",
                ShowMessage = "Cannot create initiative",
                DDL = "[]"
            });
        }

        [HttpPost]
        public JsonResult LoadSelectedPartitionVersion(string Country, string Initiative, string Population, string UserName, string VersionName)
        {
            LoginUsers UserData = LI.GetLoginUserInfo("");
            var UCA = (from uca in db.UserLastActions
                       where uca.UserId == UserData.UserName
                       select uca).FirstOrDefault();
            if (UCA != null)
            {
                VersionDetails VD = (from vd in db.VersionDetails
                                     where vd.CompanyName == UserData.CompanyName &&
                                           vd.Version == VersionName &&
                                           vd.OperType == UCA.Oper &&
                                           vd.ActiveVersion == "Y"
                                     select vd).FirstOrDefault();
                if (VD != null)
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.CommandText = "PROC_GET_POSITION_TREE_RESTORE_VERSION";

                        cmd.Parameters.Add("@VERSION", SqlDbType.VarChar, 150).Value = VD.VersionNo.ToString();
                        cmd.Parameters.Add("@COMPANYNAME", SqlDbType.VarChar, 150).Value = UserData.CompanyName.ToString().Trim().ToUpper();
                        cmd.Parameters.Add("@OPER", SqlDbType.VarChar, 150).Value = UCA.Oper.Trim().ToUpper();

                        Common csobj = new Common();
                        csobj.SPReturnDataTable(cmd);
                    }
                    return Json(new
                    {
                        ChartData = LI.GetOrgChartData(VD.UserRole, VD.Country, VD.ShowLevel, "999999", 
                                                       UCA.Levels, UCA.Oper, VD.VersionNo.ToString(),
                                                       UCA.OrgChartType, UCA.SelectedPortraitModeMultipleLevel, UCA.SelectedFunctionalManagerType)[1],
                        UsedRole = VD.UserRole,
                        UsedShowLevel = VD.ShowLevel,
                        UsedParentLevel = "999999",
                        UsedVersion = VD.VersionNo,
                        HRCoreVersion = LI.GetHRCoreVersion(VD.Country, UCA.Oper),
                    });
                }
            }

            MyLastAction myla = LI.GetUserCurrentAction("");
            return Json(new
            {
                UsedDate = DateTime.Now,
                UsedKeyDate = myla.KeyDate,
                UsedShowLevel = myla.ShowLevel,
                UsedParentLevel = myla.ParentLevel,
                UsedLevels = myla.Levels,
                UsedVersion = myla.Version,
                UsedOper = myla.Oper,
                UsedView = myla.View,
                UsedCountry = myla.Country,
                ChartData = LI.GetOrgChartData(myla.Role, myla.Country, myla.ShowLevel, myla.ParentLevel, 
                                               myla.Levels, myla.Oper, myla.Version,
                                               myla.OrgChartType, myla.SelectedPortraitModeMultipleLevel, myla.SelectedFunctionalManagerType)[1],
                ChartHRCoreData = JsonConvert.SerializeObject(""),
                UsedRole = myla.Role,
                HRCoreVersion = LI.GetHRCoreVersion(myla.Country, myla.Oper),
                DDL = JsonConvert.SerializeObject("")
            });
        }

        [HttpPost]
        public JsonResult LoadSelectedUserVersion(string Country, string Initiative, string Population, string UserName, string VersionName)
        {
            LoginUsers UserData = LI.GetLoginUserInfo("");
            var UCA = (from uca in db.UserLastActions
                       where uca.UserId == UserData.UserName
                       select uca).FirstOrDefault();
            if (UCA != null)
            {
                VersionDetails VD = (from vd in db.VersionDetails
                                     where vd.CompanyName == UserData.CompanyName &&
                                           vd.Version == VersionName &&
                                           vd.OperType == UCA.Oper &&
                                           vd.ActiveVersion == "Y"
                                     select vd).FirstOrDefault();
                if (VD != null)
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.CommandText = "PROC_GET_POSITION_TREE_RESTORE_VERSION";

                        cmd.Parameters.Add("@VERSION", SqlDbType.VarChar, 150).Value = VD.VersionNo.ToString();
                        cmd.Parameters.Add("@COMPANYNAME", SqlDbType.VarChar, 150).Value = UserData.CompanyName.ToString().Trim().ToUpper();
                        cmd.Parameters.Add("@OPER", SqlDbType.VarChar, 150).Value = UCA.Oper.Trim().ToUpper();

                        Common csobj = new Common();
                        csobj.SPReturnDataTable(cmd);
                    }
                    return Json(new
                    {
                        ChartData = LI.GetOrgChartData(VD.UserRole, VD.Country, VD.ShowLevel, "999999", 
                                                       UCA.Levels, UCA.Oper, VD.VersionNo.ToString(),
                                                       UCA.OrgChartType, UCA.SelectedPortraitModeMultipleLevel, UCA.SelectedFunctionalManagerType)[1],
                        UsedRole = VD.UserRole,
                        HRCoreVersion = LI.GetHRCoreVersion(VD.Country, UCA.Oper),
                    });
                }
            }

            MyLastAction myla = LI.GetUserCurrentAction("");
            return Json(new
            {
                UsedDate = DateTime.Now,
                UsedKeyDate = myla.KeyDate,
                UsedShowLevel = myla.ShowLevel,
                UsedParentLevel = myla.ParentLevel,
                UsedLevels = myla.Levels,
                UsedVersion = myla.Version,
                UsedOper = myla.Oper,
                UsedView = myla.View,
                UsedCountry = myla.Country,
                ChartData = LI.GetOrgChartData(myla.Role, myla.Country, myla.ShowLevel, myla.ParentLevel, 
                                               myla.Levels, myla.Oper, myla.Version,
                                               myla.OrgChartType, myla.SelectedPortraitModeMultipleLevel, myla.SelectedFunctionalManagerType)[1],
                ChartHRCoreData = JsonConvert.SerializeObject(""),
                UsedRole = myla.Role,
                HRCoreVersion = LI.GetHRCoreVersion(myla.Country, myla.Oper),
                DDL = JsonConvert.SerializeObject("")
            });
        }

        //public void Save(object entity)
        //{
        //    using (var transaction = Connection.BeginTransaction())
        //    {
        //        try
        //        {
        //            SaveChanges();
        //            transaction.Commit();
        //        }
        //        catch (OptimisticConcurrencyException)
        //        {
        //            if (ObjectStateManager.GetObjectStateEntry(entity).State == EntityState.Deleted || ObjectStateManager.GetObjectStateEntry(entity).State == EntityState.Modified)
        //                this.Refresh(RefreshMode.StoreWins, entity);
        //            else if (ObjectStateManager.GetObjectStateEntry(entity).State == EntityState.Added)
        //                Detach(entity);
        //            AcceptAllChanges();
        //            transaction.Commit();
        //        }
        //    }
        //}

        private void SaveDBChanges()
        {
            bool saveFailed;
            do
            {
                saveFailed = false;
                try
                {
                    db.SaveChanges();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    saveFailed = true;

                    // Update the values of the entity that failed to save from the store
                    ex.Entries.Single().Reload();
                }
                catch (Exception ex)
                {

                }
            }
            while (saveFailed);
        }

        [HttpPost]
        public JsonResult LoadEndUserVersion(int VersionName)
        {
            string FinalyzerVersion = "No";
            LoginUsers UserData = LI.GetLoginUserInfo("");
            var UCA = (from uca in db.UserLastActions
                       where uca.UserId == UserData.UserName
                       select uca).FirstOrDefault();
            if (UCA != null)
            {
                VersionDetails VD = (from vd in db.VersionDetails
                                     where vd.CompanyName == UserData.CompanyName &&
                                           vd.VersionNo == VersionName &&
                                           vd.OperType == UCA.Oper &&
                                           vd.ActiveVersion == "Y"
                                     select vd).FirstOrDefault();
                if (VD != null)
                {
                    UCA.SelectedInitiative = VD.Initiative;
                    UCA.SelectedPopulation = VD.Population;
                    UCA.SelectedUser = UserData.UserName;
                    UCA.SelectedVersion = VD.Version;

                    if (VD.UserRole == "Finalyzer") FinalyzerVersion = "Yes";
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.CommandText = "PROC_GET_POSITION_TREE_RESTORE_VERSION";

                        cmd.Parameters.Add("@VERSION", SqlDbType.VarChar, 150).Value = VD.VersionNo.ToString();
                        cmd.Parameters.Add("@COMPANYNAME", SqlDbType.VarChar, 150).Value = UserData.CompanyName.ToString().Trim().ToUpper();
                        cmd.Parameters.Add("@OPER", SqlDbType.VarChar, 150).Value = UCA.Oper.Trim().ToUpper();

                        Common csobj = new Common();
                        csobj.SPReturnDataTable(cmd);
                    }
                    UCA.Version = VD.VersionNo.ToString();
                    UCA.ShowLevel = VD.ShowLevel;
                    UCA.ParentLevel = "999999";
                }
            }
            SaveDBChanges();

            MyLastAction myla = LI.GetUserCurrentAction("");
            return Json(new
            {
                UsedDate = DateTime.Now,
                UsedKeyDate = myla.KeyDate,
                UsedInitiative = myla.SelectedInitiative,
                UsedPopulation = myla.SelectedPopulation,
                UsedUser = myla.SelectedUser,
                UsedVersionName = myla.SelectedVersion,
                UsedShowLevel = myla.ShowLevel,
                UsedParentLevel = myla.ParentLevel,
                UsedLevels = myla.Levels,
                UsedVersion = myla.Version,
                UsedOper = myla.Oper,
                UsedView = myla.View,
                UsedCountry = myla.Country,
                ChartData = LI.GetOrgChartData(myla.Role, myla.Country, myla.ShowLevel, myla.ParentLevel, 
                                               myla.Levels, myla.Oper, myla.Version,
                                               myla.OrgChartType, myla.SelectedPortraitModeMultipleLevel, myla.SelectedFunctionalManagerType)[1],
                ChartHRCoreData = JsonConvert.SerializeObject(""),
                UsedRole = myla.Role,
                HRCoreVersion = LI.GetHRCoreVersion(myla.Country, myla.Oper),
                FV = FinalyzerVersion,
                DDL = JsonConvert.SerializeObject("")
            });
        }


        [HttpPost]
        public JsonResult LoadSelectedVersion(string Country, string Initiative, string Population, string UserName, string VersionName)
        {
            string FinalyzerVersion = "No";
            LoginUsers UserData = LI.GetLoginUserInfo("");
            var UCA = (from uca in db.UserLastActions
                       where uca.UserId == UserData.UserName
                       select uca).FirstOrDefault();
            if (UCA != null)
            {
                UCA.SelectedInitiative = Initiative;
                UCA.SelectedPopulation = Population;
                UCA.SelectedUser = UserName;
                UCA.SelectedVersion = VersionName;

                VersionDetails VD = (from vd in db.VersionDetails
                                     where vd.CompanyName == UserData.CompanyName &&
                                           vd.Version == VersionName &&
                                           vd.OperType == UCA.Oper &&
                                           vd.ActiveVersion == "Y"
                                     select vd).FirstOrDefault();
                if (VD != null)
                {
                    if (VD.UserRole== "Finalyzer") FinalyzerVersion = "Yes";
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.CommandText = "PROC_GET_POSITION_TREE_RESTORE_VERSION";

                        cmd.Parameters.Add("@VERSION", SqlDbType.VarChar, 150).Value = VD.VersionNo.ToString();
                        cmd.Parameters.Add("@COMPANYNAME", SqlDbType.VarChar, 150).Value = UserData.CompanyName.ToString().Trim().ToUpper();
                        cmd.Parameters.Add("@OPER", SqlDbType.VarChar, 150).Value = UCA.Oper.Trim().ToUpper();

                        Common csobj = new Common();
                        csobj.SPReturnDataTable(cmd);
                    }
                    UCA.Version = VD.VersionNo.ToString();
                    if (UCA.Oper == "LV")
                    {
                        if (Country != "SelectCountry" && VD.UserRole != "Player" && VD.UserRole != "SuperAdmin")
                        {
                            var SL = (from sl in db.LegalCountries where sl.CountryCode == Country select sl).FirstOrDefault();
                            UCA.ShowLevel = SL.OrgUnit;
                            UCA.Country = Country;
                        }
                        else
                        {
                            UCA.ShowLevel = VD.ShowLevel;
                            UCA.Country = VD.Country;
                        }
                    }
                    else
                    {
                        UCA.ShowLevel = VD.ShowLevel;
                        UCA.ParentLevel = "999999";
                    }

                    if (UCA.Role == "Player" || UCA.Role == "SuperAdmin")
                    {
                        var UploadFilesHeader = (from ufh in db.UploadFilesHeaders
                                                 where ufh.UserId == UserData.UserName &&
                                                       ufh.CompanyName == UserData.CompanyName &&
                                                       ufh.Role == UCA.Role
                                                 select ufh).FirstOrDefault();
                        if (UploadFilesHeader != null)
                        {
                            var UFHFinalyzer = (from ufhf in db.UploadFilesHeaders
                                                where ufhf.UserId == UserData.UserName &&
                                                      ufhf.CompanyName == UserData.CompanyName &&
                                                      ufhf.VersionNo == VD.FinalyzeVersionNo &&
                                                      ufhf.Role == "Finalyzer"
                                                select ufhf).FirstOrDefault();
                            if (UFHFinalyzer != null)
                            {
                                UploadFilesHeader.SerialNoFlag = UFHFinalyzer.SerialNoFlag;
                                UploadFilesHeader.FirstPositionField = UFHFinalyzer.FirstPositionField;
                                UploadFilesHeader.FirstPosition = UFHFinalyzer.FirstPosition;
                                UploadFilesHeader.KeyField = UFHFinalyzer.KeyField;
                                UploadFilesHeader.ParentField = UFHFinalyzer.ParentField;
                                UploadFilesHeader.FullNameFields = UFHFinalyzer.FullNameFields;
                                UploadFilesHeader.FileType = UFHFinalyzer.FileType;
                                UploadFilesHeader.UploadFileName = UFHFinalyzer.UploadFileName;
                                UploadFilesHeader.VersionNo = Convert.ToInt32(UFHFinalyzer.VersionNo);
                                UploadFilesHeader.CurrentVersionNo = Convert.ToInt32(UFHFinalyzer.CurrentVersionNo);
                                UploadFilesHeader.VersionName = UFHFinalyzer.VersionName;
                                UploadFilesHeader.VersionDesc = UFHFinalyzer.VersionDesc;
                                UploadFilesHeader.ExcelDownLoadFields = UFHFinalyzer.ExcelDownLoadFields;
                                UploadFilesHeader.UseFields = UFHFinalyzer.UseFields;
                                UploadFilesHeader.PositionCostField = UFHFinalyzer.PositionCostField;
                                UploadFilesHeader.ShowFieldType = UFHFinalyzer.ShowFieldType;
                                UploadFilesHeader.PositionCostType = UFHFinalyzer.PositionCostType;
                            }
                        }
                    }
                }
            }
            SaveDBChanges();

            MyLastAction myla = LI.GetUserCurrentAction("");
            string HRCV = LI.GetHRCoreVersion(myla.Country, myla.Oper);
            string[] SelectedChartData = LI.GetOrgChartData(myla.Role, myla.Country, myla.ShowLevel, myla.ParentLevel, 
                                                            myla.Levels, myla.Oper, myla.Version,
                                                            myla.OrgChartType, myla.SelectedPortraitModeMultipleLevel, myla.SelectedFunctionalManagerType);
            if (Session.Contents[UserData.UserName + "_MyModel"] != null)
            {
                MyModel MyModel = (MyModel)Session.Contents[UserData.UserName + "_MyModel"];
                MyModel.ChartData = SelectedChartData[1];
                MyModel.TreeData = SelectedChartData[0];
                MyModel.ShowLevel = myla.ShowLevel;
                MyModel.ParentLevel = myla.ParentLevel;
                MyModel.Levels = myla.Levels;
                MyModel.SelectedInitiative = myla.SelectedInitiative;
                MyModel.SelectedPopulation = myla.SelectedPopulation;
                MyModel.SelectedUser = myla.SelectedUser;
                MyModel.SelectedVersion = myla.SelectedVersion;
                MyModel.FinalyzerVerion = FinalyzerVersion;
                MyModel.HRCoreVersion = HRCV;

                Session.Contents[UserData.UserName + "_MyModel"] = MyModel;
            }

            return Json(new
            {
                UsedDate = DateTime.Now,
                UsedKeyDate = myla.KeyDate,
                UsedInitiative = myla.SelectedInitiative,
                UsedPopulation = myla.SelectedPopulation,
                UsedUser = myla.SelectedUser,
                UsedVersionName = myla.SelectedVersion,
                UsedShowLevel = myla.ShowLevel,
                UsedParentLevel = myla.ParentLevel,
                UsedLevels = myla.Levels,
                UsedVersion = myla.Version,
                UsedOper = myla.Oper,
                UsedView = myla.View,
                UsedCountry = myla.Country,
                ChartData = SelectedChartData[1],
                TreeData = SelectedChartData[0],
                ChartHRCoreData = JsonConvert.SerializeObject(""),
                UsedRole = myla.Role,
                HRCoreVersion = HRCV,
                FV= FinalyzerVersion,
                DDL = JsonConvert.SerializeObject("")
            });
        }

        [HttpPost]
        public ActionResult SaveVersionTitle(string Initiative, string Population, string UserName, string VersionSelected,
                                             string Name, string Description)
        {
            LoginUsers UserData = LI.GetLoginUserInfo("");
            MyLastAction myla = LI.GetUserCurrentAction("");
            if (VersionSelected == "SelectVersion")
            {
                VersionDetails VD = (from vd in db.VersionDetails
                                     where vd.CompanyName == UserData.CompanyName &&
                                           vd.UserName == UserData.UserName &&
                                           vd.Initiative == Initiative &&
                                           vd.Population == Population &&
                                           vd.UserName == UserName &&
                                           vd.Version == null &&
                                           vd.ActiveVersion == "Y"
                                     select vd).FirstOrDefault();
                if (VD != null)
                {
                    VersionDetails VDExists = (from vd in db.VersionDetails
                                               where vd.Version == Name
                                               select vd).FirstOrDefault();
                    if (VDExists == null)
                    {
                        VD.Version = Name;
                        VD.VDescription = Description;

                        var UCA = (from uca in db.UserLastActions
                                   where uca.UserId == UserData.UserName
                                   select uca).FirstOrDefault();
                        if (UCA != null)
                        {
                            UCA.SelectedInitiative = Initiative;
                            UCA.SelectedPopulation = Population;
                            UCA.SelectedUser = UserName;
                            UCA.SelectedVersion = Name;
                        }

                        try
                        {
                            db.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            return Json(new
                            {
                                Success = "No",
                                ShowMessage = ex.Message,
                                DDL = "[]"
                            });
                        }

                        var lstVD = (from vd in db.VersionDetails
                                     where vd.CompanyName == UserData.CompanyName && vd.ActiveVersion == "Y" && vd.OperType == myla.Oper &&
                                           (myla.Role == "SuperAdmin" ||
                                            myla.Role == "Player" ||
                                            myla.Role == "Finalyzer" ||
                                            myla.Role == "User" || 
                                            myla.Role == "EndUser")
                                     select new { vd.UserName, vd.CompanyName, vd.UserRole, vd.OperType, vd.Country, vd.Initiative, vd.Population, vd.Version }).Distinct().ToList();

                        string[] SelectedChartData = LI.GetOrgChartData(myla.Role, myla.Country, myla.ShowLevel, myla.ParentLevel,
                                                           myla.Levels, myla.Oper, myla.Version,
                                                           myla.OrgChartType, myla.SelectedPortraitModeMultipleLevel, myla.SelectedFunctionalManagerType);
                        if (Session.Contents[UserData.UserName + "_MyModel"] != null)
                        {
                            MyModel MyModel = (MyModel)Session.Contents[UserData.UserName + "_MyModel"];
                            MyModel.ChartData = SelectedChartData[1];
                            MyModel.TreeData = SelectedChartData[0];
                            MyModel.ShowLevel = VD.ShowLevel;
                            MyModel.ParentLevel = "999999";
                            MyModel.Levels = myla.Levels;
                            MyModel.SelectedInitiative = VD.Initiative;
                            MyModel.SelectedPopulation = VD.Population;
                            MyModel.SelectedUser = VD.UserName;
                            MyModel.SelectedVersion = VD.Version;
                            MyModel.Version = VD.VersionNo.ToString();
                            MyModel.DDL = JsonConvert.SerializeObject(lstVD);

                            Session.Contents[UserData.UserName + "_MyModel"] = MyModel;
                        }
                        return Json(new
                        {
                            Success = "Yes",
                            ShowMessage = "Version saved",
                            UsedVersion = VD.VersionNo,
                            UsedShowLevel = VD.ShowLevel,
                            UsedParentLevel = "999999",
                            Initiative = VD.Initiative,
                            Population = VD.Population,
                            UserName = VD.UserName,
                            Version = VD.Version,
                            ChartData = SelectedChartData[1],
                            TreeData = SelectedChartData[0],
                            DDL = JsonConvert.SerializeObject(lstVD)
                        });
                    }
                    else
                    {
                        return Json(new
                        {
                            Success = "No",
                            ShowMessage = "Vesion already exists",
                            DDL = "[]"
                        });
                    }
                }

                return Json(new
                {
                    Success = "No",
                    ShowMessage = "Combination of Initiative, Population & User did not exist",
                    DDL = "[]"
                });
            }
            else
            {
                VersionDetails VD = (from vd in db.VersionDetails
                                     where vd.CompanyName == UserData.CompanyName &&
                                           vd.Version == VersionSelected &&
                                           vd.ActiveVersion == "Y"
                                     select vd).FirstOrDefault();
                if (VD != null)
                {
                    VersionDetails VDExist = (from vd in db.VersionDetails
                                         where vd.CompanyName == UserData.CompanyName &&
                                               vd.UserName == UserData.UserName &&
                                               vd.Initiative == Initiative &&
                                               vd.Population == Population &&
                                               vd.UserName == UserName &&
                                               vd.Version == null &&
                                               vd.ActiveVersion == "Y"
                                         select vd).FirstOrDefault();
                    if (VDExist == null)
                    {
                        int OldVersionNo = Convert.ToInt32(VD.VersionNo), MaxVersionNo = 0;
                        var UploadFilesDetailsVNO = (from ufd in db.UploadFilesDetails
                                                     select ufd).OrderByDescending(p => p.VersionNo).FirstOrDefault();
                        if (UploadFilesDetailsVNO == null) MaxVersionNo = 1; else MaxVersionNo = UploadFilesDetailsVNO.VersionNo + 1;

                        var UploadFilesHeaders = (from ufd in db.UploadFilesHeaders
                                                  where ufd.CompanyName == UserData.CompanyName &&
                                                        ufd.VersionNo == OldVersionNo &&
                                                        ufd.UserId == UserData.UserName &&
                                                        (myla.Role == "SuperAdmin" || myla.Role == "Player") && 
                                                        ufd.Role == myla.Role
                                                  select ufd).FirstOrDefault();
                        if (UploadFilesHeaders != null)
                        {

                            var UploadFilesDetails = (from ufd in db.UploadFilesDetails
                                                      where ufd.CompanyName == UserData.CompanyName &&
                                                            ufd.VersionNo == OldVersionNo
                                                      select ufd).FirstOrDefault();

                            if (UploadFilesDetails != null)
                            {
                                UploadFilesHeaders.VersionName = Name;
                                UploadFilesHeaders.VersionDesc = Description;
                                UploadFilesHeaders.VersionNo = MaxVersionNo;
                                UploadFilesHeaders.CurrentVersionNo = MaxVersionNo;

                                UploadFilesDetails UFD = new UploadFilesDetails();

                                UFD.UploadFileName = UploadFilesDetails.UploadFileName;
                                UFD.KeyDate = DateTime.Now;
                                UFD.VersionNo = MaxVersionNo;
                                UFD.VersionStatus = "P";
                                UFD.VersionName = Name;
                                UFD.VersionDesc = Description;
                                UFD.Role = UploadFilesDetails.Role;
                                UFD.CompanyName = UploadFilesDetails.CompanyName;
                                UFD.UserId = UserData.UserName;
                                UFD.ShowLevel = UploadFilesDetails.ShowLevel;
                                UFD.OperType = UploadFilesDetails.OperType;
                                UFD.Country = UploadFilesDetails.Country;

                                db.UploadFilesDetails.Add(UFD);

                                VersionDetails VDInf = new VersionDetails();

                                VDInf.UserRole = VD.UserRole;
                                VDInf.Country = VD.Country;
                                VDInf.Initiative = VD.Initiative;
                                VDInf.IDescription = VD.IDescription;
                                VDInf.Population = VD.Population;
                                VDInf.PDescription = VD.PDescription;
                                VDInf.UserName = UserData.UserName;
                                VDInf.CompanyName = VD.CompanyName;
                                VDInf.ShowLevel = VD.ShowLevel;
                                VDInf.OperType = VD.OperType;
                                VDInf.Version = Name;
                                VDInf.VersionNo = MaxVersionNo;
                                VDInf.VDescription = Description;
                                VDInf.ActiveVersion = VD.ActiveVersion;
                                VDInf.FinalyzeVersionNo = VD.FinalyzeVersionNo; 

                                db.VersionDetails.Add(VDInf);

                                var UserLastAction = (from ula in db.UserLastActions
                                                      where ula.UserId == UserData.UserName && ula.Company == UserData.CompanyName
                                                      select ula).FirstOrDefault();

                                if (UserLastAction != null)
                                {
                                    UserLastAction.Version = MaxVersionNo.ToString();
                                    UserLastAction.ShowLevel = VDInf.ShowLevel;
                                    UserLastAction.ParentLevel = "999999";
                                    UserLastAction.SelectedInitiative = VDInf.Initiative;
                                    UserLastAction.SelectedPopulation = VDInf.Population;
                                    UserLastAction.SelectedUser = VDInf.UserName;
                                    UserLastAction.SelectedVersion = VDInf.Version;
                                }

                                try
                                {
                                    db.SaveChanges();

                                    using (SqlCommand cmd = new SqlCommand())
                                    {
                                        cmd.CommandType = CommandType.StoredProcedure;
                                        cmd.CommandTimeout = 0;
                                        cmd.CommandText = "PROC_POSIITON_TREE_CREATE_NEW_VERSION";

                                        cmd.Parameters.Add("@FINALYZER_VERSION", SqlDbType.VarChar, 150).Value = VD.FinalyzeVersionNo.ToString();
                                        cmd.Parameters.Add("@VERSION", SqlDbType.VarChar, 150).Value = VD.VersionNo.ToString();
                                        cmd.Parameters.Add("@NEWVERSION", SqlDbType.VarChar, 150).Value = MaxVersionNo.ToString();
                                        cmd.Parameters.Add("@USERID", SqlDbType.VarChar, 150).Value = UserData.UserName.ToString().Trim().ToUpper();
                                        cmd.Parameters.Add("@COMPANYNAME", SqlDbType.VarChar, 150).Value = UserData.CompanyName.ToString().Trim().ToUpper();
                                        cmd.Parameters.Add("@OPER", SqlDbType.VarChar, 150).Value = UserLastAction.Oper.Trim().ToUpper();

                                        Common csobj = new Common();
                                        csobj.SPReturnDataTable(cmd);
                                    }
                                }
                                catch (DbEntityValidationException e)
                                {
                                    string ErrorString = "";
                                    foreach (var eve in e.EntityValidationErrors)
                                    {
                                        ErrorString += "Entity of type \"" + eve.Entry.Entity.GetType().Name + "\" in state \"" + eve.Entry.State + "\" has the following validation errors:\n";
                                        foreach (var ve in eve.ValidationErrors)
                                        {
                                            ErrorString += "- Property: \"" + ve.PropertyName + "\", Error: \"" + ve.ErrorMessage + "\"\n";
                                        }
                                    }

                                    return Json(new
                                    {
                                        Success = "No",
                                        ShowMessage = ErrorString,
                                        DDL = "[]"
                                    });
                                }

                                myla = LI.GetUserCurrentAction("");
                                var lstVD = (from vd in db.VersionDetails
                                             where vd.CompanyName == UserData.CompanyName && vd.ActiveVersion == "Y" && vd.OperType == myla.Oper &&
                                                   (myla.Role == "SuperAdmin" ||
                                                    myla.Role == "Player" ||
                                                    myla.Role == "Finalyzer" || 
                                                    myla.Role == "User" || 
                                                    myla.Role == "EndUser")
                                             select new { vd.UserName, vd.CompanyName, vd.UserRole, vd.OperType, vd.Country, vd.Initiative, vd.Population, vd.Version }).Distinct().ToList();
                                return Json(new
                                {
                                    Success = "Yes",
                                    ShowMessage = "Version saved",
                                    UsedVersion = VDInf.VersionNo,
                                    UsedShowLevel = VDInf.ShowLevel,
                                    UsedParentLevel = "999999",
                                    Initiative = VDInf.Initiative,
                                    Population = VDInf.Population,
                                    UserName = VDInf.UserName,
                                    Version = VDInf.Version,
                                    ChartData = LI.GetOrgChartData(VDInf.UserRole, VDInf.Country, VDInf.ShowLevel, "999999", 
                                                                   myla.Levels, myla.Oper, VDInf.VersionNo.ToString(),
                                                                   myla.OrgChartType, myla.SelectedPortraitModeMultipleLevel, myla.SelectedFunctionalManagerType)[1],
                                    DDL = JsonConvert.SerializeObject(lstVD)
                                });
                            }
                        }
                    }
                    else
                    {
                        return Json(new
                        {
                            Success = "No",
                            ShowMessage = "Vesion already exists",
                            DDL = "[]"
                        });
                    }
                }
            }

            return Json(new
            {
                Success = "No",
                ShowMessage = "Please select proper Initiative, Population & User",
                DDL = "[]"
            });
        }

        // Save Population
        [HttpPost]
        public JsonResult SaveVersionInfo(string selVFP, string txtVFP, string chkVSN,
                                          string selVNL, string selVPL, string txtVUN,
                                          string chkVFT, string txtVFN,
                                          string UseFields, string ExcelDownLoadFields,
                                          string selINI, string VersionName, string VersionDesc,
                                          string selPCF, string selSFT, string selPCT, string BaseVersion)
        {
            string ErrorString = "";
            string ShowVersionNo = "0";
            LoginUsers UserData = LI.GetLoginUserInfo("");

            if (selPCF == null)
            {
                var UploadFilesHeaderPCF = (from ufh in db.UploadFilesHeaders
                                            where ufh.CompanyName == UserData.CompanyName &&
                                                  ufh.Role == "Finalyzer"
                                            select ufh).FirstOrDefault();

                selPCF = UploadFilesHeaderPCF.PositionCostField;
                selSFT = UploadFilesHeaderPCF.ShowFieldType;
                selPCT = UploadFilesHeaderPCF.PositionCostType;
            }
            BaseVersion = String.IsNullOrEmpty(BaseVersion) ? "0" : BaseVersion;

            var UCA = (from uca in db.UserLastActions
                       where uca.UserId == UserData.UserName
                       select uca).FirstOrDefault();

            VersionDetails VD = (from vd in db.VersionDetails
                                 where vd.CompanyName == UserData.CompanyName &&
                                       vd.UserName == UserData.UserName &&
                                       vd.Initiative == selINI &&
                                       vd.Population == null &&
                                       vd.PDescription == null &&
                                       vd.Version == null &&
                                       vd.ActiveVersion == "Y"
                                 select vd).FirstOrDefault();
            if (VD == null)
            {
                VD = (from vd in db.VersionDetails
                      where vd.CompanyName == UserData.CompanyName &&
                            vd.UserName == UserData.UserName &&
                            vd.Initiative == selINI &&
                            vd.ActiveVersion == "Y"
                      select vd).FirstOrDefault();

                VersionDetails VDNew = new VersionDetails();
                VDNew.Initiative = VD.Initiative;
                VDNew.IDescription = VD.IDescription;
                VDNew.UserName = UserData.UserName;
                VDNew.CompanyName = UserData.CompanyName;
                VDNew.UserRole = UCA.Role;
                VDNew.Country = UCA.Country;
                VDNew.OperType = UCA.Oper;
                VDNew.ActiveVersion = "Y";

                db.VersionDetails.Add(VDNew);
                db.SaveChanges();

                VD = (from vd in db.VersionDetails
                      where vd.CompanyName == UserData.CompanyName &&
                            vd.UserName == UserData.UserName &&
                            vd.Initiative == selINI &&
                            vd.Population == null &&
                            vd.PDescription == null &&
                            vd.Version == null &&
                            vd.ActiveVersion == "Y"
                      select vd).FirstOrDefault();
            }

            VD.Population = VersionName;
            VD.PDescription = VersionDesc;

            try
            {
                db.SaveChanges();

                int MaxVersionNo = 0, DeleteMaxVersionNo = 0;
                var UploadFilesDetailsVNO = (from ufd in db.UploadFilesDetails
                                                select ufd).OrderByDescending(p => p.VersionNo).FirstOrDefault();
                if (UploadFilesDetailsVNO == null) MaxVersionNo = 0; else MaxVersionNo = UploadFilesDetailsVNO.VersionNo;
                var DeleteUploadFilesDetailsVNO = (from ufd in db.DeleteUploadFilesDetails
                                             select ufd).OrderByDescending(p => p.VersionNo).FirstOrDefault();
                if (DeleteUploadFilesDetailsVNO == null) DeleteMaxVersionNo = 0; else DeleteMaxVersionNo = DeleteUploadFilesDetailsVNO.VersionNo;
                if (MaxVersionNo <= DeleteMaxVersionNo) MaxVersionNo = DeleteMaxVersionNo;
                ShowVersionNo = (MaxVersionNo + 1).ToString();

                string OldVersion = "0";
                var UserLastAction = (from ula in db.UserLastActions
                                        where ula.UserId == UserData.UserName && ula.Company == UserData.CompanyName
                                        select ula).FirstOrDefault();

                if (UserLastAction != null)
                {
                    var UploadFilesDetails = (from ufd in db.UploadFilesDetails
                                                where ufd.UserId == UserData.UserName &&
                                                    ufd.CompanyName == UserData.CompanyName
                                                select ufd).OrderByDescending(p => p.VersionNo).FirstOrDefault();

                    var UploadFilesHeader = (from ufh in db.UploadFilesHeaders
                                                where ufh.UserId == UserData.UserName &&
                                                      ufh.CompanyName == UserData.CompanyName &&
                                                      ufh.Role == UserLastAction.Role
                                                select ufh).FirstOrDefault();
                    if (UserLastAction.Role == "Finalyzer")
                    {
                        UploadFilesHeaders UFH = new UploadFilesHeaders();

                        UFH.SerialNoFlag = chkVSN;
                        UFH.FirstPositionField = selVFP;
                        UFH.FirstPosition = txtVFP;
                        UFH.KeyField = selVNL;
                        UFH.ParentField = selVPL;
                        UFH.FullNameFields = txtVUN;
                        UFH.FileType = chkVFT;
                        UFH.UploadFileName = txtVFN;
                        UFH.Role = UserLastAction.Role;
                        UFH.VersionNo = MaxVersionNo + 1;
                        UFH.CurrentVersionNo = MaxVersionNo + 1;
                        UFH.CreatedDate = DateTime.Now;
                        UFH.CompanyName = UserData.CompanyName;
                        UFH.UserId = UserData.UserName;
                        UFH.ExcelDownLoadFields = ExcelDownLoadFields;
                        UFH.VersionName = VersionName;
                        UFH.VersionDesc = VersionDesc;
                        UFH.UseFields = UseFields;
                        UFH.PositionCostField = selPCF;
                        UFH.ShowFieldType = selSFT;
                        UFH.PositionCostType = selPCT;
                        UserLastAction.Version = (MaxVersionNo + 1).ToString();
                        UserLastAction.ShowLevel = (chkVSN == "Y") ? (100000 + Convert.ToInt32(txtVFP)).ToString() : txtVFP;
                        UserLastAction.ParentLevel = "999999";

                        db.UploadFilesHeaders.Add(UFH);
                    }
                    else
                    {
                        if (UploadFilesHeader != null)
                        {
                            UploadFilesHeader.SerialNoFlag = chkVSN;
                            UploadFilesHeader.FirstPositionField = selVFP;
                            UploadFilesHeader.FirstPosition = txtVFP;
                            UploadFilesHeader.KeyField = selVNL;
                            UploadFilesHeader.ParentField = selVPL;
                            UploadFilesHeader.FullNameFields = txtVUN;
                            UploadFilesHeader.FileType = chkVFT;
                            UploadFilesHeader.UploadFileName = txtVFN;
                            UploadFilesHeader.UseFields = UseFields;
                            UploadFilesHeader.ExcelDownLoadFields = ExcelDownLoadFields;
                            UploadFilesHeader.VersionName = VersionName;
                            UploadFilesHeader.VersionDesc = VersionDesc;
                            UploadFilesHeader.VersionNo = MaxVersionNo + 1;
                            UploadFilesHeader.CurrentVersionNo = UploadFilesHeader.VersionNo;
                            UploadFilesHeader.PositionCostField = selPCF;
                            UploadFilesHeader.ShowFieldType = selSFT;
                            UploadFilesHeader.PositionCostType = selPCT;

                            OldVersion = UserLastAction.Version;
                            UserLastAction.Version = UploadFilesHeader.VersionNo.ToString();
                            UserLastAction.ShowLevel = (chkVSN == "Y") ? (100000 + Convert.ToInt32(txtVFP)).ToString() : txtVFP;
                            UserLastAction.ParentLevel = "999999";
                        }
                        else
                        {
                            UploadFilesHeaders UFH = new UploadFilesHeaders();

                            UFH.SerialNoFlag = chkVSN;
                            UFH.FirstPositionField = selVFP;
                            UFH.FirstPosition = txtVFP;
                            UFH.KeyField = selVNL;
                            UFH.ParentField = selVPL;
                            UFH.FullNameFields = txtVUN;
                            UFH.FileType = chkVFT;
                            UFH.UploadFileName = txtVFN;
                            UFH.Role = UserLastAction.Role;
                            UFH.VersionNo = MaxVersionNo + 1;
                            UFH.CurrentVersionNo = MaxVersionNo + 1;
                            UFH.CreatedDate = DateTime.Now;
                            UFH.CompanyName = UserData.CompanyName;
                            UFH.UserId = UserData.UserName;
                            UFH.ExcelDownLoadFields = ExcelDownLoadFields;
                            UFH.VersionName = VersionName;
                            UFH.VersionDesc = VersionDesc;
                            UFH.UseFields = UseFields;
                            UFH.PositionCostField = selPCF;
                            UFH.ShowFieldType = selSFT;
                            UFH.PositionCostType = selPCT;
                            UserLastAction.Version = (MaxVersionNo + 1).ToString();
                            UserLastAction.ShowLevel = (chkVSN == "Y") ? (100000 + Convert.ToInt32(txtVFP)).ToString() : txtVFP;
                            UserLastAction.ParentLevel = "999999";

                            db.UploadFilesHeaders.Add(UFH);
                        }
                    }

                    UploadFilesDetails UFD = new UploadFilesDetails();

                    UFD.UploadFileName = txtVFN;
                    UFD.KeyDate = DateTime.Now;
                    UFD.VersionNo = Convert.ToInt32(UserLastAction.Version);
                    UFD.VersionStatus = "P";
                    UFD.VersionName = VersionName;
                    UFD.VersionDesc = VersionDesc;
                    UFD.Role = UserLastAction.Role;
                    UFD.CompanyName = UserData.CompanyName;
                    UFD.UserId = UserData.UserName;
                    UFD.ShowLevel = UserLastAction.ShowLevel;
                    UFD.OperType = UserLastAction.Oper;
                    UFD.Country = UserLastAction.Country;

                    db.UploadFilesDetails.Add(UFD);

                    try
                    {
                        db.SaveChanges();
                    }
                    catch (DbEntityValidationException e)
                    {
                        foreach (var eve in e.EntityValidationErrors)
                        {
                            ErrorString += "Entity of type \"" + eve.Entry.Entity.GetType().Name + "\" in state \"" + eve.Entry.State + "\" has the following validation errors:\n";
                            foreach (var ve in eve.ValidationErrors)
                            {
                                ErrorString += "- Property: \"" + ve.PropertyName + "\", Error: \"" + ve.ErrorMessage + "\"\n";
                            }
                        }

                        return Json(new
                        {
                            Success = "No",
                            ShowMessage = ErrorString,
                            DDL = "[]"
                        });
                    }
                }

                // Updates the Table with new Version
                if (chkVFT == "JSON")
                    ErrorString += UpdateTableWithJSON(UserLastAction.Role, BaseVersion, UserLastAction.Version, OldVersion, UserLastAction.Oper, UserLastAction.Country, UserLastAction.ShowLevel);
                else if (chkVFT == "XLSX")
                    ErrorString += UpdateTableWithJSON(UserLastAction.Role, BaseVersion, UserLastAction.Version, OldVersion, UserLastAction.Oper, UserLastAction.Country, UserLastAction.ShowLevel);

                VD = (from vd in db.VersionDetails
                        where vd.CompanyName == UserData.CompanyName &&
                            vd.UserName == UserData.UserName &&
                            vd.Initiative == selINI &&
                            vd.Population == VersionName &&
                            vd.Version == null &&
                            vd.ActiveVersion == "Y"
                        select vd).FirstOrDefault();
                if (VD != null)
                {
                    VD.VersionNo = Convert.ToInt32(UserLastAction.Version);
                    VD.FinalyzeVersionNo = Convert.ToInt32(ShowVersionNo);
                    if (UserLastAction.Role == "SuperAdmin" || UserLastAction.Role == "Player") {
                        VD.FinalyzeVersionNo = Convert.ToInt32(BaseVersion);
                    }
                    VD.ShowLevel = UserLastAction.ShowLevel;
                    VD.Country = UserLastAction.Country;
                    db.SaveChanges();
                }

                MyLastAction myla = LI.GetUserCurrentAction("");
                var lstVD = (from vd in db.VersionDetails
                                where vd.CompanyName == UserData.CompanyName && vd.ActiveVersion == "Y" && vd.OperType == myla.Oper &&
                                    ((vd.UserRole == "SuperAdmin" && myla.Role == "SuperAdmin") ||
                                     (vd.UserRole == "Player" && myla.Role == "Player") ||
                                      myla.Role == "Finalyzer" || 
                                      myla.Role == "User")
                                select new { vd.UserName, vd.CompanyName, vd.UserRole, vd.OperType, vd.Country, vd.Initiative, vd.Population, vd.Version }).Distinct().ToList();
                return Json(new
                {
                    Success = "Yes",
                    ShowMessage = (ErrorString == "" ? "Population saved" : ErrorString),
                    Data = LI.GetOrgChartData(myla.Role, myla.Country, myla.ShowLevel, myla.ParentLevel, 
                                              myla.Levels, myla.Oper, myla.Version,
                                              myla.OrgChartType, myla.SelectedPortraitModeMultipleLevel, myla.SelectedFunctionalManagerType),
                    HRData = LI.GetOrgChartHrCoreData(myla.Role, myla.Country, myla.ShowLevel, myla.Levels, myla.Oper, myla.Version, myla.SelectedFunctionalManagerType)[1],
                    DDL = JsonConvert.SerializeObject(lstVD)
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Success = "No",
                    ShowMessage = ex.Message,
                    DDL = "[]"
                });
            }
        }

        [HttpPost]
        public ActionResult FinalyseVersion()
        {
            LoginUsers UserData = LI.GetLoginUserInfo("");
            var UCA = (from uca in db.UserLastActions
                       where uca.UserId == UserData.UserName
                       select uca).FirstOrDefault();
            if (UCA != null)
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 0;
                    cmd.CommandText = "PROC_FINALIZE_VERSION_LIST";

                    cmd.Parameters.Add("@COMPANYNAME", SqlDbType.VarChar, 150).Value = UserData.CompanyName;
                    cmd.Parameters.Add("@USERID", SqlDbType.VarChar, 150).Value = UserData.UserName;
                    cmd.Parameters.Add("@PVERSIONNO", SqlDbType.VarChar, 150).Value = UCA.Version;

                    Common csobj = new Common();
                    csobj.SPReturnDataTable(cmd);
                }
            }

            return Json(new
            {
                Success = "Yes"
            });
        }

        [HttpPost]
        public string ShowVersionInfo(string VersionNo, string Role)
        {
            LoginUsers UserData = LI.GetLoginUserInfo("");
            var UCA = (from uca in db.UserLastActions
                       where uca.UserId == UserData.UserName
                       select uca).FirstOrDefault();
            if (UCA != null)
            {
                UCA.Version = VersionNo;
                UCA.Role = Role;

                db.SaveChanges();
            }

            return "Sucess";
        }

        private string UpdateFields(string strField, dynamic array)
        {
            foreach (var item in array)
            {
                Dictionary<string, string> AEs = JsonConvert.DeserializeObject<Dictionary<string, string>>(item.ToString());
                foreach (var AE in AEs)
                {
                    if (AE.Key.ToUpper() == strField) return AE.Value;
                }
            }

            return "FIELD_NOMATCH";
        }

        private void DeleteVersionDetailsInfo(VersionDetails VD, string Role, string RE)
        {
            LoginUsers UserData = LI.GetLoginUserInfo("");
            if (RE.ToUpper() == "INITIATIVE")
            {
                DeleteVersionDetails VDDelete = new DeleteVersionDetails();
                VDDelete.UserName = VD.UserName;
                VDDelete.CompanyName = VD.CompanyName;
                VDDelete.UserRole = VD.UserRole;
                VDDelete.Country = VD.Country;
                VDDelete.ShowLevel = VD.ShowLevel;
                VDDelete.OperType = VD.OperType;
                VDDelete.Initiative = VD.Initiative;
                VDDelete.IDescription = VD.IDescription;
                VDDelete.Population = VD.Population;
                VDDelete.PDescription = VD.PDescription;
                VDDelete.VersionNo = VD.VersionNo;
                VDDelete.Version = VD.Version;
                VDDelete.VDescription = VD.VDescription;
                VDDelete.ActiveVersion = "N";

                db.DeleteVersionDetails.Add(VDDelete);
                db.VersionDetails.Remove(VD);
            }
            else if (RE.ToUpper() == "POPULATION")
            {
                VD.Population = "";
                VD.PDescription = "";
                VD.VersionNo = VD.VersionNo;
                VD.Version = "";
                VD.VDescription = "";
                VD.ActiveVersion = "Y";
            }
            else if (RE.ToUpper() == "VERSION")
            {
                VD.Version = "";
                VD.VDescription = "";
                VD.ActiveVersion = "Y";
            }

            if (Role == "Finalyzer")
            {
                var UFHeaders = (from ufh in db.UploadFilesHeaders
                                      where ufh.CompanyName == UserData.CompanyName && 
                                            (ufh.Role == "SuperAdmin" || ufh.Role == "Player" || ufh.Role == "Finalyzer")
                                      select ufh).ToList();

                foreach (UploadFilesHeaders UFH in UFHeaders)
                {
                    DeleteUploadFilesHeaders UFHDelete = new DeleteUploadFilesHeaders();

                    UFHDelete.SerialNoFlag = UFH.SerialNoFlag;
                    UFHDelete.FirstPositionField = UFH.FirstPositionField;
                    UFHDelete.FirstPosition = UFH.FirstPosition;
                    UFHDelete.KeyField = UFH.KeyField;
                    UFHDelete.ParentField = UFH.ParentField;
                    UFHDelete.FullNameFields = UFH.FullNameFields;
                    UFHDelete.FileType = UFH.FileType;
                    UFHDelete.UploadFileName = UFH.UploadFileName;
                    UFHDelete.Role = UFH.Role;
                    UFHDelete.VersionNo = UFH.VersionNo;
                    UFHDelete.CurrentVersionNo = UFH.CurrentVersionNo;
                    UFHDelete.CreatedDate = UFH.CreatedDate;
                    UFHDelete.CompanyName = UFH.CompanyName;
                    UFHDelete.UserId = UFH.UserId;
                    UFHDelete.ExcelDownLoadFields = UFH.ExcelDownLoadFields;
                    UFHDelete.VersionName = UFH.VersionName;
                    UFHDelete.VersionDesc = UFH.VersionDesc;
                    UFHDelete.UseFields = UFH.UseFields;
                    UFHDelete.PositionCostField = UFH.PositionCostField;
                    UFHDelete.ShowFieldType = UFH.ShowFieldType;
                    UFHDelete.PositionCostType = UFH.PositionCostType;

                    db.DeleteUploadFilesHeaders.Add(UFHDelete);
                    db.UploadFilesHeaders.Remove(UFH);
                }

                var UFDetails = (from ufd in db.UploadFilesDetails
                                 where ufd.CompanyName == UserData.CompanyName && 
                                       (ufd.Role == "SuperAdmin" || ufd.Role == "Player" || ufd.Role == "Finalyzer")
                                 select ufd).ToList();

                foreach (UploadFilesDetails UFD in UFDetails)
                {
                    DeleteUploadFilesDetails UFDDelete = new DeleteUploadFilesDetails();

                    UFDDelete.UploadFileName = UFD.UploadFileName;
                    UFDDelete.KeyDate = UFD.KeyDate;
                    UFDDelete.VersionNo = UFD.VersionNo;
                    UFDDelete.VersionStatus = UFD.VersionStatus;
                    UFDDelete.VersionName = UFD.VersionName;
                    UFDDelete.VersionDesc = UFD.VersionDesc;
                    UFDDelete.Role = UFD.Role;
                    UFDDelete.CompanyName = UFD.CompanyName;
                    UFDDelete.UserId = UFD.UserId;
                    UFDDelete.ShowLevel = UFD.ShowLevel;
                    UFDDelete.OperType = UFD.OperType;
                    UFDDelete.Country = UFD.Country;

                    db.DeleteUploadFilesDetails.Add(UFDDelete);
                    db.UploadFilesDetails.Remove(UFD);
                }
            }
        }

        [HttpPost]
        public JsonResult RemoveEntities(string RE, string EV)
        {
            DataTable retDT = null;
            LoginUsers UserData = LI.GetLoginUserInfo("");
            MyLastAction myla = LI.GetUserCurrentAction("");

            try {
                UserVersionDetails = LI.GetVersionDetails(Convert.ToInt32(myla.Version), myla.Role, myla.Oper);
                if (UserVersionDetails[0] == "Success")
                {
                    string FinalyzerVersion = LI.ChooseFinalyzerVersion(myla.Version);
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.CommandText = "PROC_REMOVE_ENTITY";

                        cmd.Parameters.Add("@REMOVE_ENTITY", SqlDbType.VarChar, 500).Value = RE;
                        cmd.Parameters.Add("@COMPANYNAME", SqlDbType.VarChar, 150).Value = UserData.CompanyName;
                        cmd.Parameters.Add("@USERID", SqlDbType.VarChar, 150).Value = UserData.UserName;
                        cmd.Parameters.Add("@USERROLE", SqlDbType.VarChar, 50).Value = myla.Role;
                        cmd.Parameters.Add("@FINALYZER_VERSION", SqlDbType.VarChar, 150).Value = FinalyzerVersion;
                        cmd.Parameters.Add("@VERSION", SqlDbType.VarChar, 150).Value = myla.Version;

                        Common csobj = new Common();
                        retDT = csobj.SPReturnDataTable(cmd);
                    }

                    if (retDT.Rows[0]["Result"].ToString() == "Success")
                    {
                        var UserLastAction = (from ula in db.UserLastActions
                                              where ula.UserId == UserData.UserName &&
                                                    ula.Company == UserData.CompanyName &&
                                                    (ula.Role == "SuperAdmin" || ula.Role == "Player" || ula.Role == "Finalyzer")
                                              select ula).FirstOrDefault();
                        if (UserLastAction != null)
                        {
                            UserLastAction.SelectedInitiative = "SelectInitiative";
                            UserLastAction.SelectedPopulation = "SelectPopulation";
                            UserLastAction.SelectedUser = UserData.UserName;
                            UserLastAction.SelectedVersion = "SelectVersion";

                            db.SaveChanges();

                            //  Refresh changes
                            var context = ((IObjectContextAdapter)db).ObjectContext;
                            var refreshableObjects = (from entry in context.ObjectStateManager.GetObjectStateEntries(
                                                                       EntityState.Added
                                                                       | EntityState.Deleted
                                                                       | EntityState.Modified
                                                                       | EntityState.Unchanged)
                                                      where entry.EntityKey != null
                                                      select entry.Entity).ToList();
                            context.Refresh(RefreshMode.StoreWins, refreshableObjects);

                            myla = LI.GetUserCurrentAction("");
                        }

                        var lstVD = (from vd in db.VersionDetails
                                     where vd.CompanyName == UserData.CompanyName && vd.ActiveVersion == "Y" && vd.OperType == myla.Oper &&
                                           ((vd.UserRole == "Player" && myla.Role == "Player") ||
                                             myla.Role == "Finalyzer" ||
                                             myla.Role == "User" ||
                                             myla.Role == "EndUser")
                                     select new { vd.UserName, vd.CompanyName, vd.UserRole, vd.OperType, vd.Country, vd.Initiative, vd.Population, vd.Version }).Distinct().ToList();

                        Session.Contents[UserData.UserName + "_MyModel"] = null;
                        return Json(new
                        {
                            Success = "Success",
                            ShowMessage = RE + " Removed",
                            UsedVersion = myla.Version,
                            UsedShowLevel = myla.ShowLevel,
                            UsedParentLevel = "999999",
                            Initiative = myla.SelectedInitiative,
                            Population = myla.SelectedPopulation,
                            UserName = myla.SelectedUser,
                            Version = myla.Version,
                            ChartData = LI.GetOrgChartData(myla.Role, myla.Country, myla.ShowLevel, myla.ParentLevel,
                                                           myla.Levels, myla.Oper, myla.Version,
                                                           myla.OrgChartType, myla.SelectedPortraitModeMultipleLevel, myla.SelectedFunctionalManagerType)[1],
                            DDL = JsonConvert.SerializeObject(lstVD)
                        });
                    }
                    else if (retDT.Rows[0]["Result"].ToString() == "Failure")
                    {
                        return Json(new
                        {
                            Success = "Failure",
                            ShowMessage = "DB Issuse: " + retDT.Rows[0]["ErrorMessage"].ToString(),
                            DDL = "[]"
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Success = "Failure",
                    ShowMessage = ex.Message,
                    DDL = "[]"
                });
            }

            return Json(new
            {
                Success = "Failure",
                ShowMessage = "Please select your own Version and also ensure OrgAdmin/OrgPlanner role.",
                DDL = "[]"
            });
        }

        [HttpPost]
        public JsonResult SaveNewPosition(string VersionName, string PositionId, string NextPositionId, 
                                          string FirstName, string LastName, string PositionCost,
                                          string AssignEmployee, string SelectedDiv)
        {
            DataTable dtTable = null;
            LoginUsers UserData = LI.GetLoginUserInfo("");
            string FirstNameAE = "", LastNameAE = "", PositionCostAE = "0.0", PositionFieldAE = "", UpdateFieldAE="";
            MyLastAction myla = LI.GetUserCurrentAction("");

            var UserLastAction = (from ula in db.UserLastActions
                                  where ula.UserId == UserData.UserName && 
                                        ula.Company == UserData.CompanyName && 
                                        (ula.Role == "SuperAdmin" || ula.Role == "Player" || ula.Role == "Finalyzer")
                                  select ula).FirstOrDefault();

            if (UserLastAction != null)
            {
                string sQuery = "";

                UserVersionDetails = LI.GetVersionDetails(Convert.ToInt32(UserLastAction.Version), myla.Role, myla.Oper);
                if (UserVersionDetails[0] == "Success")
                {
                    string[] sTableName = { UserVersionDetails[5], UserVersionDetails[4] };
                    int BaseVersion = Convert.ToInt32(sTableName[1]);
                    if (BaseVersion != 999999 && VersionName != "SelectVersion")
                    {
                        if (SelectedDiv == "divAssignEmployee")
                        {
                            try
                            {
                                var UploadHeader = (from uef in db.UploadFilesHeaders
                                                    where uef.CompanyName == UserData.CompanyName &&
                                                          uef.Role == "Finalyzer" &&
                                                          uef.VersionNo == BaseVersion
                                                    select uef).OrderByDescending(x => x.CreatedDate).FirstOrDefault();

                                Dictionary<string, string> KeyValuePair = null;
                                string[] FullNameFields = UploadHeader.FullNameFields.Split(',');
                                dynamic array = JsonConvert.DeserializeObject(AssignEmployee);
                                foreach (var item in array)
                                {
                                    KeyValuePair = JsonConvert.DeserializeObject<Dictionary<string, string>>(item.ToString());
                                    foreach (var KV in KeyValuePair)
                                    {
                                        if (KV.Key.ToUpper() == UploadHeader.PositionCostField) PositionCostAE = KV.Value;
                                        if (KV.Key.ToUpper() == UploadHeader.FirstPositionField) PositionFieldAE = KV.Value;
                                        if (FullNameFields.Length >= 2)
                                        {
                                            if (KV.Key.ToUpper() == FullNameFields[0] || KV.Key.ToUpper() == FullNameFields[1])
                                            {
                                                if (KV.Key.ToUpper() == FullNameFields[0])
                                                    FirstNameAE = KV.Value;
                                                else if (KV.Key.ToUpper() == FullNameFields[1])
                                                    LastNameAE = KV.Value;
                                            }
                                        }
                                        else if (FullNameFields.Length == 1 && KV.Key.ToUpper() == UploadHeader.FullNameFields.ToUpper())
                                        {
                                            FirstNameAE = KV.Value;
                                        }
                                    }
                                }

                                PositionId = PositionFieldAE;
                                if (UploadHeader.SerialNoFlag == "Y")
                                    PositionId = (100000 + Convert.ToInt32(PositionFieldAE)).ToString();

                                if (FullNameFields.Length >= 2)
                                {
                                    FirstName = FirstNameAE;
                                    LastName = LastNameAE;
                                }
                                else FirstName = FirstNameAE;
                                PositionCost = PositionCostAE;

                                sQuery = "SELECT * FROM [dbo].[" + sTableName[0] + "] " +
                                         "  WHERE VERSION='" + myla.Version + "' AND USER_ID='" + UserData.UserName + "' AND LEVEL_ID='" + PositionId + "'";

                                using (SqlCommand cmd = new SqlCommand())
                                {
                                    cmd.CommandType = CommandType.Text;
                                    cmd.CommandTimeout = 0;
                                    cmd.CommandText = sQuery;

                                    Common csobj = new Common();
                                    dtTable = csobj.SPReturnDataTable(cmd);
                                }
                                if (dtTable.Rows.Count >= 1)
                                {
                                    return Json(new
                                    {
                                        Success = "No",
                                        ShowMessage = "* Assign employee not working, Employee ID already exist"
                                    });
                                }

                                string[] UF = UploadHeader.UseFields.Split(',');
                                foreach (string strUF in UF)
                                {
                                    string strField = strUF.Trim().ToUpper().Replace(" ", "_");
                                    if (strField == "LEVEL_ID") UpdateFieldAE += "," + strField + "='" + PositionId + "'";
                                    else if (strField == "PARENT_LEVEL_ID") UpdateFieldAE += "," + strField + "='" + NextPositionId + "'";
                                    else if (strField == "COUNTRY") UpdateFieldAE += "," + strField + "='CH'";
                                    else if (strField == "VERSION") UpdateFieldAE += "," + strField + "='" + myla.Version + "'";
                                    else if (strField == "FULL_NAME" && FullNameFields.Length >= 2) UpdateFieldAE += "," + strField + "='" + FirstName + " " + LastName + "'";
                                    else if (strField == "FULL_NAME" && FullNameFields.Length == 1) UpdateFieldAE += "," + strField + "='" + FirstName + "'";
                                    else if (strField == "VERIFY_FLAG") UpdateFieldAE += "," + strField + "='Y'";
                                    else if (strField == "NEXT_LEVEL_FLAG") UpdateFieldAE += "," + strField + "='N'";
                                    else if (strField == "GRAY_COLORED_FLAG") UpdateFieldAE += "," + strField + "='N'";
                                    else if (strField == "DOTTED_LINE_FLAG") UpdateFieldAE += "," + strField + "='N'";
                                    else if (strField == "SHOW_FULL_BOX") UpdateFieldAE += "," + strField + "='N'";
                                    else if (strField == "LANGUAGE_SELECTED") UpdateFieldAE += "," + strField + "='EN'";
                                    else if (strField == "SORTNO") UpdateFieldAE += "," + strField + "='0'";
                                    else if (strField == "POSITIONFLAG") UpdateFieldAE += "," + strField + "='0'";
                                    else if (strField == "FLAG") UpdateFieldAE += "," + strField + "=''";
                                    else if (strField == "MFLAG") UpdateFieldAE += "," + strField + "=''";
                                    else if (strField == "REPORTING_TO_FM") UpdateFieldAE += "," + strField + "='0'";
                                    else if (strField == "SHOW_REPORTING_TO_FM") UpdateFieldAE += "," + strField + "='0'";
                                    else if (strField == "NODE_LOCATION_COL") UpdateFieldAE += "," + strField + "='0'";
                                    else if (strField == "NODE_LOCATION_ROW") UpdateFieldAE += "," + strField + "='0'";
                                    else if (strField == "NODE_LOCATION") UpdateFieldAE += "," + strField + "=''";
                                    else if (strField == "DATE_UPDATED") UpdateFieldAE += "," + strField + "='" + DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss") + "'";
                                    else if (strField == "USER_ID") UpdateFieldAE += "," + strField + "='" + UserData.UserName + "'";
                                    else
                                    {
                                        string UFI = UpdateFields(strField, array);
                                        if (UFI != "FIELD_NOMATCH") UpdateFieldAE += "," + strField + "='" + UFI + "'";
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                return Json(new
                                {
                                    Success = "No",
                                    ShowMessage = "* Assign Employee is not working, Please look at the message: " + ex.Message
                                });
                            }
                        }

                        var UCA = (from uca in db.VersionDetails
                                   where uca.Version == VersionName &&
                                           uca.ActiveVersion == "Y"
                                   select uca).FirstOrDefault();
                        using (SqlCommand cmd = new SqlCommand())
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandTimeout = 0;
                            cmd.CommandText = "PROC_PLAYER_SAVE_NEW_POSITION";

                            cmd.Parameters.Add("@FINALYZER_VERSION", SqlDbType.VarChar, 150).Value = BaseVersion.ToString();
                            cmd.Parameters.Add("@COMPANYNAME", SqlDbType.VarChar, 150).Value = UserData.CompanyName;
                            cmd.Parameters.Add("@USERID", SqlDbType.VarChar, 50).Value = UserData.UserName;
                            cmd.Parameters.Add("@USER_ROLE", SqlDbType.VarChar, 150).Value = myla.Role.ToString().Trim().ToUpper();
                            cmd.Parameters.Add("@VERSIONNO", SqlDbType.VarChar, 50).Value = UCA.VersionNo;
                            cmd.Parameters.Add("@POSITIONID", SqlDbType.VarChar, 50).Value = PositionId;
                            cmd.Parameters.Add("@NEXTPOSITIONID", SqlDbType.VarChar, 50).Value = NextPositionId;
                            cmd.Parameters.Add("@COUNTRY", SqlDbType.VarChar, 50).Value = UCA.Country;
                            cmd.Parameters.Add("@FIRSTNAME", SqlDbType.VarChar, 500).Value = FirstName;
                            cmd.Parameters.Add("@LASTNAME", SqlDbType.VarChar, 500).Value = LastName;
                            cmd.Parameters.Add("@POSITIONCOST", SqlDbType.VarChar, 50).Value = PositionCost;
                            cmd.Parameters.Add("@CREATEDDATE", SqlDbType.VarChar, 50).Value = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss");
                            cmd.Parameters.Add("@OPER", SqlDbType.VarChar, 50).Value = UCA.OperType;

                            Common csobj = new Common();
                            dtTable = csobj.SPReturnDataTable(cmd);
                            if (dtTable.Rows[0]["Result"].ToString() != "Success")
                            {
                                return Json(new
                                {
                                    Success = "No",
                                    ShowMessage = dtTable.Rows[0]["ErrorMessage"].ToString()
                                });
                            }
                            else if (dtTable.Rows[0]["Result"].ToString() == "Success")
                            {
                                InitializeTables InitializeTables = (from iv in db.InitializeTables where iv.CompanyName == UserData.CompanyName select iv).FirstOrDefault();
                                if (InitializeTables != null)
                                {
                                    if (UCA.OperType == "OV")
                                        InitializeTables.OprLevelId = (Convert.ToInt32(InitializeTables.OprLevelId) + 1).ToString();
                                    else if (UCA.OperType == "LV")
                                        InitializeTables.LglLevelId = (Convert.ToInt32(InitializeTables.LglLevelId) + 1).ToString();

                                    db.SaveChanges();
                                }

                                if (SelectedDiv == "divAssignEmployee")
                                {
                                    try
                                    {
                                        sQuery = "UPDATE [dbo].[" + sTableName + "] SET " + UpdateFieldAE.Substring(1) + " WHERE VERSION='" + myla.Version + "' AND USER_ID='" + UserData.UserName + "' AND LEVEL_ID='" + PositionId + "'";
                                        ComClass.ExecuteQuery(sQuery);
                                    }
                                    catch (Exception ex)
                                    {
                                        return Json(new
                                        {
                                            Success = "No",
                                            ShowMessage = "* Assign Employee is not working, Please look at the message: " + ex.Message
                                        });
                                    }
                                }
                                else if (SelectedDiv == "divNewPosition")
                                {
                                }

                                UserLastAction.ShowLevel = NextPositionId;
                                DataTable ShowTable = ComClass.SQLReturnDataTable("SELECT * FROM " + "[" + sTableName[0] + "] WHERE LEVEL_ID='" + NextPositionId + "' AND MFLAG <> 'XFM' AND VERSION='" + UserLastAction.Version.ToString() + "'");
                                UserLastAction.ParentLevel = ShowTable.Rows[0]["PARENT_LEVEL_ID"].ToString();
                                db.SaveChanges();

                                return Json(new
                                {
                                    Success = "Success",
                                    ShowMessage = "New Position added successfully.",
                                    Message = LI.GetOrgChartData(UserLastAction.Role, UserLastAction.Country, 
                                                                 UserLastAction.ShowLevel, UserLastAction.ParentLevel, 
                                                                 UserLastAction.Levels, UserLastAction.Oper, UserLastAction.Version,
                                                                 UserLastAction.OrgChartType, UserLastAction.SelectedPortraitModeMultipleLevel, 
                                                                 UserLastAction.SelectedFunctionalManagerType),
                                    InitialValues = JsonConvert.SerializeObject((from iv in db.InitializeTables where iv.CompanyName == UserData.CompanyName select iv).FirstOrDefault())
                                });
                            }
                        }
                    }
                    else
                    {
                        return Json(new
                        {
                            Success = "Failure",
                            ShowMessage = "* New position not added, Please select version(your own version) and also ensure OrgAdmin/OrgPlanner role"
                        });

                    }
                }
                else
                {
                    return Json(new
                    {
                        Success = "Failure",
                        ShowMessage = "* New position not added, Please select version(your own version) and also ensure OrgAdmin/OrgPlanner role"
                    });

                }
            }

            return Json(new
            {
                Success = "Failure",
                ShowMessage = (UserLastAction == null ? "* New position not added, Please select version(your own version) and also ensure OrgAdmin/OrgPlanner role" : "* New position not added")
            });
        }

        public int GetUserVerion(int Version, MyLastAction myla)
        {
            string Oper = myla.Oper;
            LoginUsers UserData = LI.GetLoginUserInfo("");
            VersionDetails myIV = (from iv in db.VersionDetails
                                   where iv.CompanyName == UserData.CompanyName && 
                                         (iv.UserRole == "SuperAdmin" || iv.UserRole == "Player" || iv.UserRole == "Finalyzer") && 
                                         iv.VersionNo == Version &&
                                         iv.ActiveVersion == "Y" && 
                                         iv.OperType == Oper
                                   select iv).FirstOrDefault();
            if (myIV!=null)
            {
                if (myIV.UserName == UserData.UserName) return (int)myIV.FinalyzeVersionNo;
            }

            return 999999;
        }

        [HttpPost]
        public JsonResult SaveVersion(string VersionData, string MoveToParent, string[] ChangeType, string OperType)
        {
            LoginUsers UserData = LI.GetLoginUserInfo("");
            MyLastAction myla = LI.GetUserCurrentAction("");
            List<dynamic> lstDynamic = new List<dynamic>();

            var UserLastAction = (from ula in db.UserLastActions
                                  where ula.UserId == UserData.UserName && 
                                        ula.Company == UserData.CompanyName && 
                                        (ula.Role == "SuperAdmin" || ula.Role == "Player" || ula.Role == "Finalyzer")
                                  select ula).FirstOrDefault();

            if (UserLastAction != null)
            {
                UserVersionDetails = LI.GetVersionDetails(Convert.ToInt32(UserLastAction.Version), myla.Role, myla.Oper);
                if (UserVersionDetails[0] == "Success")
                {
                    int BaseVersion = Convert.ToInt32(UserVersionDetails[4]);
                    if (BaseVersion != 999999)
                    {
                        var UploadHeader = (from uef in db.UploadFilesHeaders
                                            where uef.CompanyName == UserData.CompanyName &&
                                                  uef.Role == "Finalyzer" &&
                                                  uef.VersionNo == BaseVersion
                                            select uef).OrderByDescending(x => x.CreatedDate).FirstOrDefault();

                        if (UserLastAction != null)
                        {
                            dynamic array = JsonConvert.DeserializeObject(VersionData);
                            foreach (var item in array)
                            {
                                string DestinationVersion = UserLastAction.Version.ToString();
                                string SourceVersion = item["VERSION"].ToString();
                                string PositionID = item["LEVEL_ID"].ToString();
                                string ParentPositionID = MoveToParent;
                                string ChildField = UploadHeader.KeyField.ToString();
                                string ParentField = UploadHeader.ParentField.ToString();

                                using (SqlCommand cmd = new SqlCommand())
                                {
                                    cmd.CommandType = CommandType.StoredProcedure;
                                    cmd.CommandTimeout = 0;
                                    cmd.CommandText = "PROC_SAVE_PLAYER_VERSION_TREE_OPERATIONALCHART";

                                    cmd.Parameters.Add("@FINALYZER_VERSION", SqlDbType.VarChar, 150).Value = BaseVersion;
                                    cmd.Parameters.Add("@SOURCE_VERSION", SqlDbType.VarChar, 150).Value = SourceVersion;
                                    cmd.Parameters.Add("@DESTINATION_VERSION", SqlDbType.VarChar, 50).Value = DestinationVersion;
                                    cmd.Parameters.Add("@POSITION_ID", SqlDbType.VarChar, 150).Value = PositionID;
                                    cmd.Parameters.Add("@COMPANYNAME", SqlDbType.VarChar, 150).Value = UserData.CompanyName.ToString().Trim().ToUpper();
                                    cmd.Parameters.Add("@USERID", SqlDbType.VarChar, 150).Value = UserData.UserName.ToString().Trim().ToUpper();
                                    cmd.Parameters.Add("@USER_ROLE", SqlDbType.VarChar, 150).Value = myla.Role.ToString().Trim().ToUpper();
                                    cmd.Parameters.Add("@PARENT_POSITION_ID", SqlDbType.VarChar, 150).Value = ParentPositionID.Trim().ToUpper();
                                    cmd.Parameters.Add("@CHILD_FIELD", SqlDbType.VarChar, 150).Value = ChildField.Trim().ToUpper();
                                    cmd.Parameters.Add("@PARENT_FIELD", SqlDbType.VarChar, 150).Value = ParentField.Trim().ToUpper();
                                    cmd.Parameters.Add("@OPER", SqlDbType.VarChar, 150).Value = OperType.Trim().ToUpper();

                                    Common csobj = new Common();
                                    DataTable retDT = csobj.SPReturnDataTable(cmd);
                                    if (retDT.Rows[0]["Result"].ToString()== "Failure")
                                    {
                                        return Json(new
                                        {
                                            Success = "Failure",
                                            ShowMessage = "DB Issuse: " + retDT.Rows[0]["ErrorMessage"].ToString(),
                                            UsedDate = DateTime.Now,
                                            UsedShowLevel = UserLastAction.ShowLevel,
                                            UsedParentLevel = UserLastAction.ParentLevel,
                                            UsedVersion = UserLastAction.Version,
                                            ChartData = LI.GetOrgChartData(myla.Role, myla.Country, UserLastAction.ShowLevel, UserLastAction.ParentLevel, 
                                                                           myla.Levels, myla.Oper, myla.Version,
                                                                           myla.OrgChartType, myla.SelectedPortraitModeMultipleLevel, myla.SelectedFunctionalManagerType)[1]
                                        });

                                    }
                                }

                                UserLastAction.ShowLevel = ParentPositionID;
                                string[] sTableName = LI.GetFinalyzerTable(UserLastAction.Version, UserLastAction.Oper);
                                DataTable ShowTable = ComClass.SQLReturnDataTable("SELECT * FROM " + "[" + sTableName[0] + "] WHERE LEVEL_ID='" + ParentPositionID + "' AND MFLAG <> 'XFM' AND VERSION='" + UserLastAction.Version.ToString() + "'");
                                UserLastAction.ParentLevel = ShowTable.Rows[0]["PARENT_LEVEL_ID"].ToString();

                                db.SaveChanges();

                                string[] SaveVersion = LI.GetOrgChartData(myla.Role, myla.Country, UserLastAction.ShowLevel, UserLastAction.ParentLevel,
                                                                   myla.Levels, myla.Oper, myla.Version,
                                                                   myla.OrgChartType, myla.SelectedPortraitModeMultipleLevel, myla.SelectedFunctionalManagerType);
                                if (Session.Contents[UserData.UserName + "_MyModel"] != null)
                                {
                                    MyModel MyModel = (MyModel)Session.Contents[UserData.UserName + "_MyModel"];
                                    MyModel.ChartData = SaveVersion[1];
                                    MyModel.TreeData = SaveVersion[0];
                                    MyModel.ShowLevel = UserLastAction.ShowLevel;
                                    MyModel.ParentLevel = UserLastAction.ParentLevel;
                                    MyModel.Version = UserLastAction.Version;
                                    Session.Contents[UserData.UserName + "_MyModel"] = MyModel;
                                }

                                myla = LI.GetUserCurrentAction("");
                                return Json(new
                                {
                                    Success = "Success",
                                    ShowMessage = "Changes applied",
                                    UsedDate = DateTime.Now,
                                    UsedShowLevel = UserLastAction.ShowLevel,
                                    UsedParentLevel = UserLastAction.ParentLevel,
                                    UsedVersion = UserLastAction.Version,
                                    ChartData = SaveVersion[1],
                                    TreeData = SaveVersion[0]
                                });
                            }
                        }
                    }
                }
            }

            return Json(new
            {
                Success = "Failure",
                ShowMessage = "Choose Finalyzer/Player role and your own version to Drag and drop and to add new position",
                ChartData = LI.GetOrgChartData(myla.Role, myla.Country, myla.ShowLevel, myla.ParentLevel, 
                                               myla.Levels, myla.Oper, myla.Version,
                                               myla.OrgChartType, myla.SelectedPortraitModeMultipleLevel, myla.SelectedFunctionalManagerType)[1]
            });
        }

        [HttpPost]
        public JsonResult FinalyzePlayerVersion()
        {
            LoginUsers UserData = LI.GetLoginUserInfo("");
            MyLastAction myla = LI.GetUserCurrentAction("");
            var UserLastAction = (from ula in db.UserLastActions
                                  where ula.UserId == UserData.UserName && 
                                        ula.Company == UserData.CompanyName && 
                                        ula.Role == "Finalyzer"
                                  select ula).FirstOrDefault();
            if (UserLastAction != null)
            {
                int BaseVersion = GetUserVerion(Convert.ToInt32(UserLastAction.Version), myla);
                if (BaseVersion != 999999)
                {
                    string DestinationVersion = UserLastAction.Version.ToString();
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.CommandText = "PROC_SAVE_PLAYER_COMPLETED_VERSION";

                        cmd.Parameters.Add("@FINALYZER_VERSION", SqlDbType.VarChar, 150).Value = BaseVersion;
                        cmd.Parameters.Add("@COMPANY_NAME", SqlDbType.VarChar, 150).Value = UserData.CompanyName;
                        cmd.Parameters.Add("@SOURCE_VERSION", SqlDbType.VarChar, 150).Value = UserLastAction.Version.ToString();
                        cmd.Parameters.Add("@OPER", SqlDbType.VarChar, 150).Value = UserLastAction.Oper.ToUpper();

                        Common csobj = new Common();
                        csobj.SPReturnDataTable(cmd);
                    }
                }

                myla = LI.GetUserCurrentAction("");
                VersionDetails myIV = (from iv in db.VersionDetails
                                       where iv.CompanyName == UserData.CompanyName && 
                                             iv.UserRole == "Finalyzer" && 
                                             iv.ActiveVersion == "Y" && 
                                             iv.OperType == myla.Oper
                                       select iv).FirstOrDefault();
                return Json(new
                {
                    Success = "Yes",
                    Message = "Changes applied",
                    Data = LI.GetOrgChartData(myla.Role, myla.Country, myla.ShowLevel, myla.ParentLevel, 
                                              myla.Levels, myla.Oper, myla.Version,
                                              myla.OrgChartType, myla.SelectedPortraitModeMultipleLevel, myla.SelectedFunctionalManagerType)[1],
                    IVData = LI.GetOrgChartData(myIV.UserRole, myIV.Country, myIV.ShowLevel, "999999", 
                                                myla.Levels, myla.Oper, myIV.VersionNo.ToString(),
                                                myla.OrgChartType, myla.SelectedPortraitModeMultipleLevel, myla.SelectedFunctionalManagerType)[1]
                });
            }

            myla = LI.GetUserCurrentAction("");
            return Json(new
            {
                Success = "No",
                Message = "Choose Admin/Player role",
            });
        }

        [HttpPost]
        public JsonResult FinalyzeFinalyzerVersion()
        {
            LoginUsers UserData = LI.GetLoginUserInfo("");
            MyLastAction myla = null;
            var UserLastAction = (from ula in db.UserLastActions
                                  where ula.UserId == UserData.UserName && ula.Company == UserData.CompanyName && ula.Role == "Finalyzer"
                                  select ula).FirstOrDefault();
            if (UserLastAction != null)
            {
                string DestinationVersion = UserLastAction.Version.ToString();
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 0;
                    cmd.CommandText = "PROC_SAVE_FINALYZER_COMPLETED_VERSION";

                    cmd.Parameters.Add("@SOURCE_VERSION", SqlDbType.VarChar, 150).Value = UserLastAction.Version.ToString();
                    cmd.Parameters.Add("@OPER", SqlDbType.VarChar, 150).Value = UserLastAction.Oper.ToUpper();

                    Common csobj = new Common();
                    csobj.SPReturnDataTable(cmd);
                }

                myla = LI.GetUserCurrentAction("");
                return Json(new
                {
                    Success = "Yes",
                    Message = "Changes applied",
                    Data = LI.GetOrgChartData(myla.Role, myla.Country, myla.ShowLevel, myla.ParentLevel, 
                                              myla.Levels, myla.Oper, myla.Version,
                                              myla.OrgChartType, myla.SelectedPortraitModeMultipleLevel, myla.SelectedFunctionalManagerType)[1]
                });
            }

            myla = LI.GetUserCurrentAction("");
            return Json(new
            {
                Success = "No",
                Message = "Choose Player role",
                Data = LI.GetOrgChartData(myla.Role, myla.Country, myla.ShowLevel, myla.ParentLevel, 
                                          myla.Levels, myla.Oper, myla.Version,
                                          myla.OrgChartType, myla.SelectedPortraitModeMultipleLevel, myla.SelectedFunctionalManagerType)[1]
            });
        }

        public string[] CheckFields(string FileName)
        {
            string[] FieldsInf = { "", "" };
            string MissingFields = "", ShowFields = "";
            LoginUsers UserData = LI.GetLoginUserInfo("");

            var UserLastAction = (from ula in db.UserLastActions
                                  where ula.UserId == UserData.UserName && ula.Company == UserData.CompanyName
                                  select ula).FirstOrDefault();

            var UploadExcelFile = (from uef in db.UploadFilesHeaders
                                   where uef.CompanyName == UserData.CompanyName && 
                                         uef.UserId == UserData.UserName && 
                                         uef.Role == UserLastAction.Role
                                   select uef).OrderByDescending(x => x.CreatedDate).FirstOrDefault();

            using (StreamReader reader = new StreamReader(FileName))
            {
                string jsonData = reader.ReadToEnd();
                dynamic array = JsonConvert.DeserializeObject(jsonData);
                int Index = 0, ErrCount = 0;
                foreach (var item in array.data)
                {
                    if (UploadExcelFile.UseFields != "")
                    {
                        Index++;
                        string[] UF = UploadExcelFile.UseFields.Split(',');
                        foreach (string strUF in UF)
                        {
                            string strField = strUF.Trim().ToUpper().Replace(" ", "_");
                            if (!(strField == "LEVEL_ID" || strField == "PARENT_LEVEL_ID" || strField == "VERSION" ||
                                  strField == "FULL_NAME" || strField == "DATE_UPDATED" || strField == "USER_ID" ||
                                  strField == "VERIFY_FLAG" || strField == "LEVEL_NO" || strField == "POSITION_CALCULATED_COST" ||
                                  strField == "BREAD_GRAM" || strField == "BREAD_GRAM_NAME" ||
                                  strField == "NOR_COUNT" || strField == "SOC_COUNT" || strField == "COUNTRY" ||
                                  strField == "NEXT_LEVEL_FLAG" || strField == "GRAY_COLORED_FLAG" || strField == "DOTTED_LINE_FLAG" ||
                                  strField == "SHOW_FULL_BOX" || strField == "LANGUAGE_SELECTED" || strField == "SORTNO" ||
                                  strField == "POSITIONFLAG" || strField == "FLAG" || strField == "MFLAG" ||
                                  strField == "REPORTING_TO_FM" || strField == "SHOW_REPORTING_TO_FM" ||
                                  strField == "NODE_LOCATION_COL" || strField == "NODE_LOCATION_ROW" || strField == "NODE_LOCATION"))
                            {
                                try
                                {
                                    if (item[strUF] == null)
                                    {
                                        if (ErrCount <= 100) MissingFields += "," + Index.ToString() + "~:~" + strUF;
                                        ErrCount++;
                                    }
                                    else
                                    {
                                        if (Index == 1) ShowFields += "," + strUF;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    if (ErrCount <= 100) MissingFields += "," + Index.ToString() + "~:~" + strUF;
                                    ErrCount++;
                                }
                            }
                        }
                    }

                }
            }

            FieldsInf[0] = (MissingFields == "") ? "" : MissingFields.Substring(1);
            FieldsInf[1] = (ShowFields == "") ? "" : ShowFields.Substring(1);

            return FieldsInf;
        }

        public ActionResult DownloadErrorFile(string FN)
        {
            string ErrorData = "";
            string ErrorFilePath = Path.Combine(Server.MapPath("~/App_Data/ErrorFile/"), FN);
            using (StreamReader Reader = new StreamReader(ErrorFilePath))
            {
                ErrorData = Reader.ReadToEnd();
            }
            return File(Encoding.UTF8.GetBytes(ErrorData), "text/plain", string.Format("{0}", FN));
        }

        private string GetFirstPosition(string PostionField, string ParentPositionField) {

            try {
                DataTable ShowTable = (DataTable)Session["SourceTable"];
                if (ShowTable != null)
                {
                    DataRow[] drs = ShowTable.Select(ParentPositionField+"='999999'");
                    if (drs != null)
                    {
                        if (drs.Count() >= 1)
                        {
                            return drs[0][PostionField].ToString();
                        }
                    }

                }
            }
            catch(Exception ex)
            {

            }

            return "";
        }

        public string[] CheckFieldsErrorFile(string FileName, string SkipDataError, int BaseVersion)
        {
            DataTable SourceTable = new DataTable();
            string[] FieldsInf = { "", "", "" };
            string[] RetJsonValue = { "", "" };
            string MissingFields = "", ShowFields = "", ErrorFile = "", ErrorFilePath = "", UsedFields = "", DataErrorFlag="N";
            int LineNumber = 0;

            LoginUsers UserData = LI.GetLoginUserInfo("");

            ErrorFile = "ERROR_FILE_" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".txt";
            ErrorFilePath = Path.Combine(Server.MapPath("~/App_Data/ErrorFile/"), ErrorFile);
            using (StreamWriter Writer = new StreamWriter(ErrorFilePath))
            {
                try
                {
                    var UserLastAction = (from ula in db.UserLastActions
                                          where ula.UserId == UserData.UserName && ula.Company == UserData.CompanyName
                                          select ula).FirstOrDefault();

                    var UploadExcelFile = (from uef in db.UploadFilesHeaders
                                           where uef.CompanyName == UserData.CompanyName &&
                                                 uef.Role == "Finalyzer" && 
                                                 (uef.VersionNo==BaseVersion || BaseVersion==0)
                                           select uef).OrderByDescending(x => x.CreatedDate).FirstOrDefault();


                    string jsonData = "";
                    string[] ExtFile = FileName.Split('.');
                    if (ExtFile[1].ToUpper() == "JSON")
                    {
                        using (StreamReader reader = new StreamReader(FileName))
                        {
                            jsonData = reader.ReadToEnd();
                        }
                    }
                    else if (ExtFile[1].ToUpper() == "XLSX")
                    {
                        ExcelAPI API = new ExcelAPI();
                        RetJsonValue = API.CreateJSONFromExcel(FileName, (UploadExcelFile != null) ? UploadExcelFile.ShowFieldType : "NONE");
                        jsonData = RetJsonValue[0];

                        // If error in excel data
                        if (RetJsonValue[1] != "")
                        {
                            DataErrorFlag = "Y";
                            Writer.WriteLine(RetJsonValue[1]);
                        }
                    }
                    dynamic array = JsonConvert.DeserializeObject(jsonData);
                    int Index = 0, ErrCount = 0;

                    if (UserLastAction.Role=="Finalyzer")
                    {
                        UsedFields = "";
                        foreach (var item in array.data)
                        {
                            Dictionary<string, string> KeyValuePair = JsonConvert.DeserializeObject<Dictionary<string, string>>(item.ToString());
                            foreach (var KV in KeyValuePair)
                            {
                                UsedFields += "," + KV.Key.ToUpper();
                            }
                            if (UsedFields != "") UsedFields = UsedFields.Substring(1);
                            break;
                        }
                    }
                    else UsedFields = UploadExcelFile.UseFields;

                    string[] UF_List = UsedFields.Split(',');
                    foreach (string strUF in UF_List)
                    {
                        string strField = strUF.Trim().ToUpper().Replace(" ", "_");
                        if (!(strField == "LEVEL_ID" || strField == "PARENT_LEVEL_ID" || strField == "VERSION" ||
                              strField == "FULL_NAME" || strField == "DATE_UPDATED" || strField == "USER_ID" ||
                              strField == "VERIFY_FLAG" || strField == "LEVEL_NO" || strField == "POSITION_CALCULATED_COST" ||
                              strField == "BREAD_GRAM" || strField == "BREAD_GRAM_NAME" ||
                              strField == "NOR_COUNT" || strField == "SOC_COUNT" || strField == "COUNTRY" ||
                              strField == "NEXT_LEVEL_FLAG" || strField == "GRAY_COLORED_FLAG" || strField == "DOTTED_LINE_FLAG" ||
                              strField == "SHOW_FULL_BOX" || strField == "LANGUAGE_SELECTED" || strField == "SORTNO" ||
                              strField == "POSITIONFLAG" || strField == "FLAG" || strField == "MFLAG" ||
                              strField == "REPORTING_TO_FM" || strField == "SHOW_REPORTING_TO_FM" ||
                              strField == "NODE_LOCATION_COL" || strField == "NODE_LOCATION_ROW" || strField == "NODE_LOCATION"))
                        {
                            SourceTable.Columns.Add(strField, typeof(string));
                        }
                    }

                    foreach (var item in array.data)
                    {
                        if (UsedFields != "")
                        {
                            Index++;
                            string[] UF = UsedFields.Split(',');

                            // generate the data you want to insert
                            DataRow ToInsert = SourceTable.NewRow();
                            foreach (string strUF in UF)
                            {
                                string strField = strUF.Trim().ToUpper().Replace(" ", "_");
                                if (!(strField == "LEVEL_ID" || strField == "PARENT_LEVEL_ID" || strField == "VERSION" ||
                                      strField == "FULL_NAME" || strField == "DATE_UPDATED" || strField == "USER_ID" ||
                                      strField == "VERIFY_FLAG" || strField == "LEVEL_NO" || strField == "POSITION_CALCULATED_COST" ||
                                      strField == "BREAD_GRAM" || strField == "BREAD_GRAM_NAME" ||
                                      strField == "NOR_COUNT" || strField == "SOC_COUNT" || strField == "COUNTRY" ||
                                      strField == "NEXT_LEVEL_FLAG" || strField == "GRAY_COLORED_FLAG" || strField == "DOTTED_LINE_FLAG" ||
                                      strField == "SHOW_FULL_BOX" || strField == "LANGUAGE_SELECTED" || strField == "SORTNO" ||
                                      strField == "POSITIONFLAG" || strField == "FLAG" || strField == "MFLAG" ||
                                      strField == "REPORTING_TO_FM" || strField == "SHOW_REPORTING_TO_FM" ||
                                      strField == "NODE_LOCATION_COL" || strField == "NODE_LOCATION_ROW" || strField == "NODE_LOCATION"))
                                {
                                    try
                                    {
                                        if (item[strField] == null)
                                        {
                                            ToInsert[strField] = "";
                                            if (ErrCount <= 1000)
                                            {
                                                if (LineNumber != Index)
                                                {
                                                    LineNumber = Index;
                                                    MissingFields += "\n";
                                                    MissingFields += "Line Number : " + Index.ToString() + "\n";
                                                }
                                                MissingFields += strUF + " has null value\n";

                                                if (Index % 100 == 0)
                                                {
                                                    Writer.WriteLine(MissingFields);
                                                    MissingFields = "";
                                                }

                                                ErrCount++;
                                            }
                                        }
                                        else
                                        {
                                            ToInsert[strField] = item[strField];
                                            if (Index == 1) ShowFields += "," + strUF;
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        if (ErrCount <= 1000)
                                        {
                                            if (LineNumber != Index)
                                            {
                                                MissingFields += "\n";
                                                MissingFields += "Line Number : " + Index.ToString() + "\n";
                                                LineNumber = Index;
                                            }
                                            MissingFields += strUF + " not belongs the field list\n";

                                            if (Index % 100 == 0)
                                            {
                                                Writer.WriteLine(MissingFields);
                                                MissingFields = "";
                                            }
                                            ErrCount++;
                                        }
                                    }
                                }
                            }
                            // Rows Added
                            SourceTable.Rows.Add(ToInsert);
                        }
                    }
                    if (MissingFields != "") Writer.Write(MissingFields);

                    if (DataErrorFlag == "Y" && SkipDataError=="N")
                    {
                        FieldsInf[0] = "Error";
                        FieldsInf[1] = (ShowFields == "") ? "" : ShowFields.Substring(1);
                        FieldsInf[2] = ErrorFile;
                    }
                    else if ((DataErrorFlag == "N") || (DataErrorFlag == "Y" && SkipDataError == "Y"))
                    {
                        FieldsInf[0] = (MissingFields == "") ? "" : MissingFields.Substring(1);
                        FieldsInf[1] = (ShowFields == "") ? "" : ShowFields.Substring(1);
                        FieldsInf[2] = ErrorFile;
                    }
                }
                catch (Exception ex)
                {
                    Writer.Write(ex.Message);

                    FieldsInf[0] = "Error";
                    FieldsInf[1] = (ShowFields == "") ? "" : ShowFields.Substring(1);
                    FieldsInf[2] = ErrorFile;
                }
            }

            Session.Contents["SourceTable"] = SourceTable;

            return FieldsInf;
        }

        [HttpPost]
        public ActionResult UploadFile()
        {
            string[] Fields = { "", "", "" };
            string UploadFileName = "";
            string SkipDataError = (Request.Form["SkipDataError"]);
            LoginUsers UserData = LI.GetLoginUserInfo("");

            var UserLastAction = (from ula in db.UserLastActions
                                  where ula.UserId == UserData.UserName && ula.Company == UserData.CompanyName
                                  select ula).FirstOrDefault();
            int BaseVerion = 0;
            if (UserLastAction.Role=="SuperAdmin" || UserLastAction.Role == "Player")
                BaseVerion = Convert.ToInt32(Request.Form["selFV"]);

            // Checking no of files injected in Request object  
            if (Request.Files.Count > 0)
            {
                try
                {
                    //  Get all files from Request object  
                    HttpFileCollectionBase files = Request.Files;
                    for (int Idx = 0; Idx < files.Count; Idx++)
                    {
                        //string path = AppDomain.CurrentDomain.BaseDirectory + "Uploads/";  
                        //string filename = Path.GetFileName(Request.Files[i].FileName);  

                        HttpPostedFileBase file = files[Idx];
                        string fname;

                        // Checking for Internet Explorer  
                        if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                        {
                            string[] testfiles = file.FileName.Split(new char[] { '\\' });
                            fname = testfiles[testfiles.Length - 1];
                        }
                        else
                        {
                            fname = file.FileName;
                        }
                        UploadFileName = fname;

                        // Get the complete folder path and store the file inside it.  
                        fname = Path.Combine(Server.MapPath("~/App_Data/Uploads/"), fname);
                        file.SaveAs(fname);

                        Fields = CheckFieldsErrorFile(fname, SkipDataError, BaseVerion);
                    }

                    var UploadExcelFile = (from uef in db.UploadFilesHeaders
                                           where uef.CompanyName == UserData.CompanyName &&
                                                 uef.Role == "Finalyzer" &&
                                                 (uef.VersionNo == BaseVerion || BaseVerion==0)
                                           select uef).OrderByDescending(x => x.CreatedDate).FirstOrDefault();
                    if (UploadExcelFile != null)
                    {
                        // Returns message that successfully uploaded  
                        return Json(new
                        {
                            Success = (Fields[0] == "") ? "Yes" : "No",
                            Message = (Fields[0] != "") ? "Missing Fields" : "",
                            MF = Fields[0],
                            SF = Fields[1],
                            FN = UploadFileName,
                            EF = Fields[2],
                            SerialNoFlag = UploadExcelFile.SerialNoFlag,
                            FirstPositionField = UploadExcelFile.FirstPositionField,
                            FirstPosition = GetFirstPosition(UploadExcelFile.FirstPositionField, UploadExcelFile.ParentField),
                            ChildField = UploadExcelFile.KeyField,
                            ParentField = UploadExcelFile.ParentField,
                            PositionCostField = UploadExcelFile.PositionCostField,
                            PositionCostType = UploadExcelFile.PositionCostType,
                            FullNameFields = UploadExcelFile.FullNameFields
                        });
                    }

                    // Returns message that successfully uploaded  
                    if (Fields[0] != "")
                    {
                        return Json(new
                        {
                            Success = "No",
                            Message = (Fields[0] != "") ? "Missing Fields" : "",
                            MF = Fields[0],
                            SF = Fields[1],
                            FN = UploadFileName,
                            EF = Fields[2],
                            SerialNoFlag = "N",
                            FirstPositionField = "",
                            FirstPosition = "",
                            ChildField = "",
                            ParentField = "",
                            PositionCostField = "",
                            PositionCostType = "",
                            FullNameFields = ""
                        });
                    }
                    else
                    {
                        return Json(new
                        {
                            Success = "Yes",
                            Message = (Fields[0] != "") ? "Missing Fields" : "",
                            MF = Fields[0],
                            SF = Fields[1],
                            FN = UploadFileName,
                            EF = Fields[2],
                            SerialNoFlag = "N",
                            FirstPositionField = "",
                            FirstPosition = "",
                            ChildField = "",
                            ParentField = "",
                            PositionCostField = "",
                            PositionCostType = "",
                            FullNameFields = ""
                        });
                    }
                }
                catch (Exception ex)
                {
                    return Json(new
                    {
                        Success = "No",
                        Message = ex.Message,
                        MF = "",
                        SF = Fields[1],
                        FN = UploadFileName,
                        EF = Fields[2]
                    });
                }
            }
            else
            {
                return Json(new
                {
                    Success = "No",
                    Message = "No files selected.",
                    MF = "",
                    SF = Fields[1],
                    FN = UploadFileName,
                    EF = Fields[2]
                });
            }
        }

        [HttpPost]
        public ActionResult GetVersionList()
        {
            LoginUsers UserData = LI.GetLoginUserInfo("");
            var UserLastAction = (from ula in db.UserLastActions
                                  where ula.UserId == UserData.UserName && ula.Company == UserData.CompanyName
                                  select ula).FirstOrDefault();

            DataTable retDT = null;

            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 0;
            cmd.CommandText = "PROC_GET_DETAILS_VERSION_LIST";

            cmd.Parameters.Add("@USERID", SqlDbType.VarChar, 150).Value = UserData.UserName;
            cmd.Parameters.Add("@ROLE", SqlDbType.VarChar, 150).Value = UserLastAction.Role;

            Common csobj = new Common();
            retDT = csobj.SPReturnDataTable(cmd);

            return Json(new
            {
                Success = "Yes",
                VL = JsonConvert.SerializeObject(retDT)
            });
        }

        public ActionResult LegalView()
        {
            LoginUsers UserData = LI.GetLoginUserInfo("");
            MyLastAction myla = LI.GetUserCurrentAction("");

            var viewModel = new MyModel
            {
                UseDate = DateTime.Now,
                KeyDate = myla.KeyDate,
                ShowLevel = myla.ShowLevel,
                ParentLevel = myla.ParentLevel,
                Levels = myla.Levels,
                Version = myla.Version,
                Oper = myla.Oper,
                View = myla.View,
                Country = myla.Country,
                ChartData = LI.GetOrgChartData(myla.Role, myla.Country, myla.ShowLevel, myla.ParentLevel, 
                                               myla.Levels, myla.Oper, myla.Version,
                                               myla.OrgChartType, myla.SelectedPortraitModeMultipleLevel, myla.SelectedFunctionalManagerType)[1],
                Role = myla.Role,
                HRCoreVersion = LI.GetHRCoreVersion(myla.Country, myla.Oper),
                DDL = JsonConvert.SerializeObject((from vd in db.VersionDetails
                                                   where vd.CompanyName == UserData.CompanyName && vd.ActiveVersion == "Y" && vd.OperType == myla.Oper &&
                                                         ((vd.UserRole == "SuperAdmin" && myla.Role == "SuperAdmin") ||
                                                          (vd.UserRole == "Player" && myla.Role == "Player") ||
                                                           myla.Role == "Finalyzer" || 
                                                           myla.Role == "User" || 
                                                           myla.Role == "EndUser")
                                                   select new { vd.UserName, vd.CompanyName, vd.UserRole, vd.OperType, vd.Country, vd.Initiative, vd.Population, vd.Version }).Distinct().ToList())

            };
            return View("LegalView", viewModel);
        }

        [HttpPost]
        public ActionResult UploadPPTXFile()
        {
            string[] Fields = { "", "", "" };
            string UploadFileName = "";
            LoginUsers UserData = LI.GetLoginUserInfo("");

            // Checking no of files injected in Request object  
            if (Request.Files.Count > 0)
            {
                try
                {
                    string UsedFields = "", Mappingfileds = "";

                    //  Get all files from Request object  
                    HttpFileCollectionBase Files = Request.Files;
                    for (int Idx = 0; Idx < Files.Count; Idx++)
                    {
                        HttpPostedFileBase File = Files[Idx];
                        string FName;

                        // Checking for Internet Explorer  
                        if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                        {
                            string[] TestFiles = File.FileName.Split(new char[] { '\\' });
                            FName = TestFiles[TestFiles.Length - 1];
                        }
                        else
                        {
                            FName = File.FileName;
                        }
                        UploadFileName = FName;

                        UsedFields = new InsertImage().GetNodeAllTextBoxes(FName, File);
                        Mappingfileds = db.UploadFilesHeaders
                                          .Where(x => x.CompanyName == UserData.CompanyName)
                                          .Select(x => x.UseFields)
                                          .FirstOrDefault();
                    }
                    UsedFields = UsedFields.Substring(1);
                    // Returns message that successfully uploaded  
                    return Json(new
                    {
                        Success = (Fields[0] == "") ? "Yes" : "No",
                        Message = (Fields[0] != "") ? "Missing Fields" : "",
                        MF = Mappingfileds,
                        SF = UsedFields,
                        FN = UploadFileName,
                        EF = Fields[2]
                    });

                }
                catch (Exception ex)
                {
                    return Json(new
                    {
                        Success = "No",
                        Message = ex.Message,
                        MF = "",
                        SF = Fields[1],
                        FN = UploadFileName,
                        EF = Fields[2]
                    });
                }
            }
            else
            {
                return Json(new
                {
                    Success = "No",
                    Message = "No files selected.",
                    MF = "",
                    SF = Fields[1],
                    FN = UploadFileName,
                    EF = Fields[2]
                });
            }
        }


        public JsonResult SavePPTXInfo(string txtPFN, string PPTXFields, string PPTXTemplateName, string PPTXTemplateDesc, string NodeCount)
        {
            try
            {
                LoginUsers UserData = LI.GetLoginUserInfo("");
                PPTX_CONFIG_INFO PPTX = (from pptx in db.PPTX_CONFIG_INFO where pptx.TemplateName == PPTXTemplateName && pptx.CompanyName==UserData.CompanyName select pptx).FirstOrDefault();
                if (PPTX == null)
                {
                    PPTX_CONFIG_INFO PCI = new PPTX_CONFIG_INFO();
                    PCI.Description = PPTXTemplateDesc;
                    PCI.FieldsInfo = PPTXFields;
                    PCI.FileName = txtPFN;
                    PCI.TemplateName = PPTXTemplateName;
                    PCI.CompanyName = UserData.CompanyName;
                    PCI.NodeCount = Convert.ToInt32(NodeCount);
                    PCI.ACTIVE_IND = "Y";

                    db.PPTX_CONFIG_INFO.Add(PCI);
                    db.SaveChanges();

                    var UserLastAction = (from ula in db.UserLastActions
                                          where ula.UserId == UserData.UserName && ula.Company == UserData.CompanyName
                                          select ula).FirstOrDefault();

                    string TableName = "LEGAL_CONFIG_INFO";
                    if (UserLastAction.Oper=="OV") TableName = "LEVEL_CONFIG_INFO";

                    string sMapFields = "DELETE FROM "+ TableName + " WHERE TEMPLATE_NAME='"+ PPTXTemplateName + "' AND DOWNLOAD_TYPE='PPTX' AND COMPANY_NAME='"+UserData.CompanyName+"';";
                    string FieldNames = "VIEW_ID, FIELD_WIDTH, FIELD_CAPTION, FIELD_NAME, FIELD_ROW, FIELD_ROW_TYPE, FIELD_COL, FIELD_COL_TYPE, " +
                                         "WRAP, FONT_NAME, FONT_SIZE, FONT_COLOR, FONT_STYLE, FONT_FLOAT, ACTIVE_IND, TABLE_IND, ADJUSTMENT, " +
                                         "SAMPLE_DATA, TEMPLATE_NAME, DOWNLOAD_TYPE, COMPANY_NAME";
                    string FieldValues = "'VIEW_DEFAULT', '140', 'FieldCaption', 'FieldName','60', 'px', '90', 'px', 'N', 'Arial'," +
                                         "'8', '#ff0000', 'bold-ul', 'center', 'N', 'Y', 'N', 'MapValue', 'TemplateName','DownLoadType','"+UserData.CompanyName+"'";
                    List<PPTXTemplateFields> PPTXAllFields = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<List<PPTXTemplateFields>>(PPTXFields);

                    foreach(PPTXTemplateFields pptx in PPTXAllFields)
                    {
                        sMapFields = sMapFields + "INSERT INTO " + TableName + " ("+ FieldNames + ") VALUES ("+ FieldValues + ");";
                        sMapFields = sMapFields.Replace("FieldCaption", pptx.MappedFields);
                        sMapFields = sMapFields.Replace("FieldName", pptx.MappedFields);
                        sMapFields = sMapFields.Replace("MapValue", pptx.MappedFields);
                        sMapFields = sMapFields.Replace("TemplateName", PPTXTemplateName);
                        sMapFields = sMapFields.Replace("DownLoadType", "PPTX");
                    }
                    ComClass.ExecuteQuery(sMapFields);
                }
                else
                {
                    return Json(new
                    {
                        Success = "No",
                        ShowMessage = "Place Valid template information or Template already exist",
                        DDL = "[]"
                    });
                }

                return Json(new
                {
                    Success = "Yes",
                    ShowMessage = "PPTX Template INFO saved",

                });
            }
            catch (Exception)
            {
                return Json(new
                {
                    Success = "No",
                    ShowMessage = "Place Valid template information",
                    DDL = "[]"
                });
            }
        }

    }
}
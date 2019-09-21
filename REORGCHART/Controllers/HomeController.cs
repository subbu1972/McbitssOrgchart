using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using System.IO;
using System.ComponentModel;
using System.Configuration;
using System.Data.SqlClient;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Web.Script.Serialization;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml;
using System.Dynamic;
using Newtonsoft.Json.Converters;
using System.Reflection;

using REORGCHART.Models;
using REORGCHART.Helper;
using REORGCHART.Data;
using OfficeOpenXml;
using System.Web.Configuration;
using System.DirectoryServices.AccountManagement;

namespace REORGCHART.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        Models.DBContext db = new Models.DBContext();
        LevelInfo LI = new LevelInfo();

        public ActionResult Index(string Search)
        {
            string Security = "No";
            string[] AssignedView = LI.GetUserRoles().Split(',');
            if (Array.IndexOf(AssignedView, "User") != -1) Security = "Yes";

            MyLastAction myla = LI.GetUserCurrentAction("User", Search);
            int VersionNumber = myla.Version == "" ? 0 : Convert.ToInt32(myla.Version);

            DataTable ShowGridTable = new DataTable();
            ShowGridTable.Columns.Add("Upload", typeof(string));
            DataRow dr = ShowGridTable.NewRow();
            dr["Upload"] = "No data uploaded";
            ShowGridTable.Rows.Add(dr);
            Session["SourceTable"] = ShowGridTable;

            if (AssignedView[0] == "Player")
                return RedirectToAction("UploadVersion", "Version", new { area = "" });
            else if (AssignedView[0] == "Finalyzer")
                return RedirectToAction("UploadData", "Version", new { area = "" });
            LoginUsers UserData = LI.GetLoginUserInfo("User");
            if (UserData.ValidUser == "No")
            {
                return RedirectToAction("NotAuthorizedPage", "Version");
            }

            var UFH = (from ufh in db.UploadFilesHeaders
                       where ufh.CompanyName == UserData.CompanyName &&
                             ufh.Role == "Finalyzer"
                       select ufh).FirstOrDefault();
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
                OrgChartType = myla.OrgChartType,
                ShowLevel = myla.ShowLevel,
                Levels = myla.Levels,
                Version = myla.Version,
                Oper = myla.Oper,
                View = myla.View,
                SerialNoFlag = UFH==null?"N":UFH.SerialNoFlag,
                CopyPaste = myla.CopyPaste,
                Country = myla.Country,
                Countries = JsonConvert.SerializeObject((from co in db.LegalCountries
                                                         select co).Distinct().ToList()),
                ChartData =LI. GetOrgChartData(myla.Role, myla.Country, myla.ShowLevel, myla.ParentLevel, 
                                               myla.Levels, myla.Oper, myla.Version, 
                                               myla.OrgChartType, myla.SelectedPortraitModeMultipleLevel, myla.SelectedFunctionalManagerType)[1],
                Role = myla.Role,
                AssignedRole = LI.GetUserRoles(),
                HRCoreVersion = LI.GetHRCoreVersion(myla.Country, myla.Oper),
                Menu = LI.GetMenuItems(myla.Role),
                DDL = JsonConvert.SerializeObject((from vd in db.VersionDetails
                                                   where vd.CompanyName == UserData.CompanyName && vd.ActiveVersion == "Y" && vd.OperType == myla.Oper &&
                                                         (myla.Role == "Player" || myla.Role == "Finalyzer" || myla.Role == "User")
                                                   select new { vd.UserName, vd.CompanyName, vd.UserRole, vd.OperType, vd.Country, vd.Initiative, vd.Population, vd.Version }).Distinct().ToList()),
                OVDDL = JsonConvert.SerializeObject((from vd in db.VersionDetails
                                                     where vd.CompanyName == UserData.CompanyName && vd.ActiveVersion == "Y" && vd.OperType == "OV" &&
                                                           ((vd.UserRole == "Player" && myla.Role == "Player") || myla.Role == "Finalyzer" || myla.Role == "User")
                                                     select new { vd.UserName, vd.CompanyName, vd.UserRole, vd.OperType, vd.Country, vd.Initiative, vd.Population, vd.Version }).Distinct().ToList()),
                LVDDL = JsonConvert.SerializeObject((from vd in db.VersionDetails
                                                     where vd.CompanyName == UserData.CompanyName && vd.ActiveVersion == "Y" && vd.OperType == "LV" &&
                                                           ((vd.UserRole == "Player" && myla.Role == "Player") || myla.Role == "Finalyzer" || myla.Role == "User")
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
                GridDataTable = ShowGridTable,
                ValidateSecurity = Security
            };

            return View(viewModel);
        }

        private string GetSearchFields(int VersionNumber)
        {
            string[] ArraySearchFields = ConfigurationManager.AppSettings["SearchFields"].ToString().Split(',');
            var UFH = (from vd in db.UploadFilesHeaders
                       where vd.VersionNo == VersionNumber
                       select new { vd.ExcelDownLoadFields, vd.FirstPositionField }).Distinct().ToList();

            if (UFH != null) {
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


        [HttpPost]
        public JsonResult SetSelectedValues(string KeyDate, string UsedView, string Country, string ShowLevel, string Levels, string Oper, string Version, string Role,
                                        string SelectedShape, string SelectedSkin, string SelectedShowPicture, string SelectedSplitScreen, string SelectedSplitScreenDirection,
                                        string SelectedTextColor, string SelectedBorderColor, string SelectedBorderWidth, string SelectedLineColor, string SelectedBoxWidth,
                                        string SelectedPortraitModeMultipleLevel, string SelectedFunctionalManagerType, string OrgChartType, string Type)
        {
            LoginUsers UserData = LI.GetLoginUserInfo("");
            var UCA = (from uca in db.UserLastActions
                       where uca.UserId == UserData.UserName
                       select uca).FirstOrDefault();
            if (UCA != null)
            {
                if (Type == "Role")
                {
                    string UserRole = (Role == "User") ? "Finalyzer" : Role;
                    UploadFilesHeaders UFH = null;
                    if (UserRole != "Finalyzer")
                    {
                        UFH = (from ufh in db.UploadFilesHeaders
                               where ufh.CompanyName == UserData.CompanyName &&
                                     ufh.UserId== UserData.UserName &&                                
                                     ufh.Role == UserRole
                               select ufh).FirstOrDefault();
                    }
                    else
                    {
                        UFH = (from ufh in db.UploadFilesHeaders
                               where ufh.CompanyName == UserData.CompanyName &&
                                     ufh.Role == UserRole
                               select ufh).FirstOrDefault();
                    }

                    if (UFH != null)
                    {
                        UCA.Version = UFH.VersionNo.ToString();
                        UCA.Role = Role;

                        int UserVersion = UFH.VersionNo;

                        var UFD = (from ufd in db.UploadFilesDetails
                                   where ufd.VersionNo == UserVersion
                                   select ufd).FirstOrDefault();
                        if (UFD != null) UCA.ShowLevel = UFD.ShowLevel;
                    }
                    else
                    {
                        UCA.Version = "-999";
                        UCA.Role = Role;
                    }

                    Session.Contents["MyModel"] = null;
                    return Json(new
                    {
                        Success = "Yes",
                        ChartData = "Role Change",
                        TreeData = ""
                    });
                }
                else if (Type == "View") UCA.UsedView = UsedView;
                else if (Type == "Level") UCA.Levels = Levels;
                else if (Type == "Type") UCA.Oper = Oper;
                else if (Type == "Settings")
                {
                    UCA.UsedView = UsedView;
                    UCA.SelectedShape = SelectedShape;
                    UCA.SelectedSkin = SelectedSkin;
                    UCA.SelectedShowPicture = SelectedShowPicture;
                    UCA.SelectedSplitScreen = SelectedSplitScreen;
                    UCA.SelectedSplitScreenDirection = SelectedSplitScreenDirection;
                    UCA.SelectedTextColor = SelectedTextColor;
                    UCA.SelectedBorderColor = SelectedBorderColor;
                    UCA.SelectedBorderWidth = SelectedBorderWidth;
                    UCA.SelectedLineColor = SelectedLineColor;
                    UCA.SelectedBoxWidth = SelectedBoxWidth;
                    UCA.SelectedPortraitModeMultipleLevel = SelectedPortraitModeMultipleLevel;
                    UCA.SelectedFunctionalManagerType = SelectedFunctionalManagerType;
                    UCA.OrgChartType = OrgChartType;
                    UCA.Levels = Levels;
                }
                else if (Type == "")
                {
                    UCA.KeyDate = KeyDate;
                    UCA.Country = Country;
                    UCA.ShowLevel = ShowLevel;
                }

                db.SaveChanges();
            }

            string[] ChangeLevel = LI.GetOrgChartData(UCA.Role, UCA.Country, UCA.ShowLevel, UCA.ParentLevel, 
                                                    UCA.Levels, UCA.Oper, UCA.Version, 
                                                    UCA.OrgChartType, UCA.SelectedPortraitModeMultipleLevel, UCA.SelectedFunctionalManagerType);
            if (Session.Contents["MyModel"] != null)
            {
                MyModel MyModel = (MyModel)Session.Contents["MyModel"];
                MyModel.ChartData = ChangeLevel[1];
                if (Type == "Settings")
                {
                    MyModel.View = UsedView;
                    MyModel.SelectedShape = SelectedShape;
                    MyModel.SelectedSkin = SelectedSkin;
                    MyModel.SelectedShowPicture = SelectedShowPicture;
                    MyModel.SelectedSplitScreen = SelectedSplitScreen;
                    MyModel.SelectedSplitScreenDirection = SelectedSplitScreenDirection;
                    MyModel.SelectedTextColor = SelectedTextColor;
                    MyModel.SelectedBorderColor = SelectedBorderColor;
                    MyModel.SelectedBorderWidth = SelectedBorderWidth;
                    MyModel.SelectedBoxWidth = SelectedBoxWidth;
                    MyModel.SelectedLineColor = SelectedLineColor;
                    MyModel.SelectedPortraitModeMultipleLevel = SelectedPortraitModeMultipleLevel;
                    MyModel.SelectedFunctionalManagerType = SelectedFunctionalManagerType;
                    MyModel.OrgChartType = OrgChartType;
                    MyModel.Levels = Levels;
                }
                else if (Type == "")
                {
                    MyModel.KeyDate = KeyDate;
                    MyModel.Country = Country;
                    MyModel.ShowLevel = ShowLevel;
                }
                Session.Contents["MyModel"] = MyModel;
            }

            return Json(new
            {
                Success = "Yes",
                ChartData = ChangeLevel[1],
                TreeData = ChangeLevel[0]
            });
        }

        public ActionResult ShowSearchMenu()
        {
            LoginUsers UserData = LI.GetLoginUserInfo("");
            MyLastAction myla = LI.GetUserCurrentAction("");

            string sTableName = UserData.CompanyName.ToString().Trim().ToUpper().Replace(" ", "_") + "_LevelInfos";
            if (myla.Oper == "LV") sTableName = UserData.CompanyName.ToString().Trim().ToUpper().Replace(" ", "_") + "_LegalInfos";

            int Version = Convert.ToInt32(myla.Version);
            var UFH = (from ufh in db.UploadFilesHeaders where ufh.VersionNo == Version select ufh).FirstOrDefault();

            DataTable ShowTable = null;
            if (UFH != null)
            {
                Common ComClass = new Common();
                ShowTable = ComClass.SQLReturnDataTable("SELECT LEVEL_ID," + UFH.ExcelDownLoadFields + " FROM " + "[" + sTableName + "]");
            }
            if (ShowTable == null)
            {
                ShowTable = new DataTable();
                ShowTable.Columns.Add("Dummy", typeof(string));
                DataRow dr = ShowTable.NewRow();
                dr["Dummy"] = "None";
                ShowTable.Rows.Add(dr);
            }

            return PartialView("ShowSearchMenu", ShowTable); ;
        }

        public ActionResult ShowEmployeeView()
        {
            LoginUsers UserData = LI.GetLoginUserInfo("");
            DataTable ShowTable = null;

            var UCA = (from uca in db.UserLastActions
                       where uca.UserId == UserData.UserName
                       select uca).FirstOrDefault();
            if (UCA != null)
            {
                string sTableName = UserData.CompanyName.ToString().Trim().ToUpper().Replace(" ", "_") + "_LevelInfos";
                if (UCA.Oper == "LV") sTableName = UserData.CompanyName.ToString().Trim().ToUpper().Replace(" ", "_") + "_LegalInfos";

                int Version = Convert.ToInt32(UCA.Version);
                var UFH = (from ufh in db.UploadFilesHeaders where ufh.VersionNo == Version select ufh).FirstOrDefault();

                if (UFH != null)
                {
                    Common ComClass = new Common();
                    ShowTable = ComClass.SQLReturnDataTable("SELECT LEVEL_ID," + UFH.ExcelDownLoadFields + " FROM " + "[" + sTableName + "] WHERE VERSION='" + UCA.Version.ToString() + "'");
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

            return LI.GetOrgChartData(UCA.Role, UCA.Country, UCA.ShowLevel, UCA.ParentLevel, 
                                      UCA.Levels, UCA.Oper, UCA.Version,
                                      UCA.OrgChartType, UCA.SelectedPortraitModeMultipleLevel, UCA.SelectedFunctionalManagerType)[1];
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult LogOut()
        {
            return Redirect("~/account/LogOff");
        }

        public ActionResult UserList()
        {
            DBContext db = new DBContext();
            LoginUsers UserData = LI.GetLoginUserInfo("");

            SqlConnection conn = new SqlConnection();
            string sqlConn = WebConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString;
            conn.ConnectionString = sqlConn;

            string sqlQuery = "SELECT * FROM [AspNetUsers]  WHERE CompanyName = '" + UserData.CompanyName + "'";
            SqlCommand cmd = new SqlCommand(sqlQuery, conn);
            conn.Open();
            SqlDataReader dr = cmd.ExecuteReader();
            List<UserListDetails> list = new List<UserListDetails>();

            while (dr.Read())
            {
                list.Add(new UserListDetails()
                {
                    UserName = dr["UserName"].ToString(),
                    CompanyName = dr["CompanyName"].ToString(),
                    Id = dr["Id"].ToString()
                });

            }
            dr.Close();
            conn.Close();

            return PartialView("UserListPartial", list);
        }
    }
}
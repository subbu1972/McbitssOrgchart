using System;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Data;
using System.Collections.Generic;

using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;

using REORGCHART.Data;
using REORGCHART.Models;
using System.Configuration;

namespace REORGCHART.Helper
{
    // Org chart creation 
    public class LevelInfo
    {
        Models.DBContext db = new Models.DBContext();
        public string ChooseFinalyzerVersion(string Version)
        {
            LoginUsers UserData = GetLoginUserInfo("");
            int VersionNo = Convert.ToInt32(Version);
            string FinalyzerVersion = "0";

            VersionDetails VD = (from vd in db.VersionDetails
                                 where vd.CompanyName == UserData.CompanyName &&
                                       vd.VersionNo == VersionNo
                                 select vd).FirstOrDefault();
            if (VD != null) FinalyzerVersion = VD.FinalyzeVersionNo.ToString();

            return FinalyzerVersion;
        }

        public string[] GetFinalyzerTable(string Version, string Oper)
        {
            LoginUsers UserData = GetLoginUserInfo("");
            string[] FinalyzerTable = { "", "" };
            int VersionNo = Convert.ToInt32(Version);
            string FinalyzerVersion = "0";

            VersionDetails VD = (from vd in db.VersionDetails
                                 where vd.CompanyName == UserData.CompanyName &&
                                       vd.VersionNo == VersionNo
                                 select vd).FirstOrDefault();
            if (VD != null)
            {
                FinalyzerVersion = (VD.FinalyzeVersionNo==null?0: VD.FinalyzeVersionNo).ToString();

                string sTableName = UserData.CompanyName.ToString().Trim().ToUpper().Replace(" ", "_") + "_" + FinalyzerVersion + "_LevelInfos";
                if (Oper == "LV") sTableName = UserData.CompanyName.ToString().Trim().ToUpper().Replace(" ", "_") + "_" + FinalyzerVersion + "_LegalInfos";

                FinalyzerTable[0] = sTableName;
                FinalyzerTable[1] = FinalyzerVersion;
            }

            return FinalyzerTable;
        }


        public DataSet GetLevelInfo(string UserId, string CompanyName, string UserType,
                                      string ShowLevel, string ParentLevel, string Levels, string Version, string OperType)
        {
            DataSet dsLI = null;
            if (ShowLevel == ParentLevel) ParentLevel = "999999";
            string BreadGram = (ParentLevel == null ? "" : ParentLevel);
            if (BreadGram == "" || BreadGram == "999999") BreadGram = ShowLevel; else BreadGram += "-->" + ShowLevel;

            string FinalyzerVersion = ChooseFinalyzerVersion(Version);
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                cmd.CommandText = "PROC_GET_POSITION_TREE_OPERATIONALCHART";

                cmd.Parameters.Add("@STARTPOSITION", SqlDbType.VarChar, 50).Value = ShowLevel;
                cmd.Parameters.Add("@BREADGRAM", SqlDbType.VarChar, 50).Value = BreadGram;
                cmd.Parameters.Add("@DEPTH", SqlDbType.VarChar, 15).Value = Levels;
                cmd.Parameters.Add("@FINALYZER_VERSION", SqlDbType.VarChar, 150).Value = FinalyzerVersion;
                cmd.Parameters.Add("@VERSION", SqlDbType.VarChar, 150).Value = Version;
                cmd.Parameters.Add("@USERTYPE", SqlDbType.VarChar, 50).Value = UserType;
                cmd.Parameters.Add("@COMPANYNAME", SqlDbType.VarChar, 150).Value = CompanyName;
                cmd.Parameters.Add("@USERID", SqlDbType.VarChar, 150).Value = UserId;
                cmd.Parameters.Add("@OPER", SqlDbType.VarChar, 150).Value = OperType;

                Common csobj = new Common();
                dsLI = csobj.SPReturnDataSet(cmd);
            }

            return dsLI;
        }

        public DataTable GetVersion(string OperType, string CompanyName, string Version, string ShowLevel)
        {
            DataTable retDT = null;
            string FinalyzerVersion = ChooseFinalyzerVersion(Version);

            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 0;
            cmd.CommandText = "PROC_GET_VERSION_TREE_OPERATIONALCHART";

            cmd.Parameters.Add("@VERSION", SqlDbType.VarChar, 150).Value = Version;
            cmd.Parameters.Add("@FINALYZER_VERSION", SqlDbType.VarChar, 150).Value = FinalyzerVersion;
            cmd.Parameters.Add("@COMPANYNAME", SqlDbType.VarChar, 150).Value = CompanyName;
            cmd.Parameters.Add("@OPER", SqlDbType.VarChar, 150).Value = OperType;
            cmd.Parameters.Add("@SHOWLEVEL", SqlDbType.VarChar, 150).Value = ShowLevel;

            Common csobj = new Common();
            retDT = csobj.SPReturnDataTable(cmd);

            // Remove predefined column
            if (retDT.Columns.Count >= 1)
            {
                try
                {
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
                }
                catch (Exception ex)
                {
                }
            }

            return retDT;
        }

        public MyLastAction GetUserCurrentAction(string RoleMethod, string Search)
        {
            LoginUsers UserData = GetLoginUserInfo("");
            var UCA = (from uca in db.UserLastActions
                       where uca.UserId == UserData.UserName
                       select uca).FirstOrDefault();

            MyLastAction viewModel = new MyLastAction();
            if (UCA == null)
            {
                UserLastActions uca = new UserLastActions();
                uca.Company = UserData.CompanyName;
                uca.KeyDate = "2018/07/11";
                uca.UserId = UserData.UserName;
                uca.Version = "1";
                uca.UsedView = "Normal";
                uca.Oper = "OV";
                uca.Levels = "One";
                uca.ShowLevel = "101212";
                uca.ParentLevel = "999999";
                uca.PartitionShowLevel = "101212";
                uca.PartitionParentLevel = "101212";
                uca.Country = "CH";
                uca.Role = RoleMethod;
                uca.SelectedInitiative = "SelectInitiative";
                uca.SelectedPopulation = "SelectPopulation";
                uca.SelectedUser = UserData.UserName;
                uca.SelectedVersion = "SelectVersion";
                uca.SelectedShape = "RoundedRectangle";
                uca.SelectedSkin = "white";
                uca.SelectedShowPicture = "Yes";
                uca.SelectedSplitScreen = "Yes";
                uca.SelectedSplitScreenDirection = "Vertical";
                uca.SelectedTextColor = "black";
                uca.SelectedBorderColor = "cyan";
                uca.SelectedBorderWidth = "3";
                uca.SelectedLineColor = "#634329";
                uca.SelectedPortraitModeMultipleLevel = "Yes";
                uca.SelectedFunctionalManagerType = "ShowFM";
                uca.SelectedFMLine = "PN";
                uca.OrgChartType = "OD";
                uca.CopyPaste = "";
                db.UserLastActions.Add(uca);

                db.SaveChanges();

                viewModel.ShowLevel = "101212";
                viewModel.ParentLevel = "999999";
                viewModel.PartitionShowLevel = "101212";
                viewModel.PartitionParentLevel = "999999";
                viewModel.KeyDate = "2018/07/11";
                viewModel.Levels = "One";
                viewModel.Version = "1";
                viewModel.Oper = "OV";
                viewModel.View = "Normal";
                viewModel.Country = "CH";
                viewModel.SelectedInitiative = "SelectInitiative";
                viewModel.SelectedPopulation = "SelectPopulation";
                viewModel.SelectedUser = UserData.UserName;
                viewModel.SelectedVersion = "SelectVersion";
                viewModel.SelectedShape = "RoundedRectangle";
                viewModel.SelectedSkin = "white";
                viewModel.SelectedShowPicture = "Yes";
                viewModel.SelectedSplitScreen = "Yes";
                viewModel.SelectedSplitScreenDirection = "Vertical";
                viewModel.SelectedTextColor = "black";
                viewModel.SelectedBorderColor = "cyan";
                viewModel.SelectedBorderWidth = "3";
                viewModel.SelectedLineColor = "#634329";
                viewModel.SelectedPortraitModeMultipleLevel = "Yes";
                viewModel.SelectedFunctionalManagerType = "ShowFM";
                viewModel.SelectedFMLine = "PN";
                viewModel.OrgChartType = "OD";
                viewModel.CopyPaste = "";
                viewModel.Role = RoleMethod;
            }
            else
            {
                viewModel.KeyDate = UCA.KeyDate;
                viewModel.SelectedInitiative = UCA.SelectedInitiative;
                viewModel.SelectedPopulation = UCA.SelectedPopulation;
                viewModel.SelectedUser = UCA.SelectedUser;
                viewModel.SelectedVersion = UCA.SelectedVersion;
                viewModel.SelectedShape = UCA.SelectedShape;
                viewModel.SelectedSkin = UCA.SelectedSkin;
                viewModel.SelectedShowPicture = UCA.SelectedShowPicture;
                viewModel.SelectedSplitScreen = UCA.SelectedSplitScreen;
                viewModel.SelectedSplitScreenDirection = UCA.SelectedSplitScreenDirection;
                viewModel.SelectedTextColor = UCA.SelectedTextColor;
                viewModel.SelectedBorderColor = UCA.SelectedBorderColor;
                viewModel.SelectedBorderWidth = UCA.SelectedBorderWidth;
                viewModel.SelectedLineColor = UCA.SelectedLineColor;
                viewModel.SelectedPortraitModeMultipleLevel = UCA.SelectedPortraitModeMultipleLevel;
                viewModel.SelectedFunctionalManagerType = UCA.SelectedFunctionalManagerType;
                viewModel.SelectedFMLine = UCA.SelectedFMLine;
                viewModel.OrgChartType = UCA.OrgChartType;
                viewModel.Levels = UCA.Levels;
                viewModel.Oper = UCA.Oper;
                viewModel.View = UCA.UsedView;
                viewModel.Country = UCA.Country;
                viewModel.Role = UCA.Role;
                viewModel.ShowLevel = UCA.ShowLevel;
                viewModel.ParentLevel = UCA.ParentLevel;
                viewModel.PartitionShowLevel = UCA.PartitionShowLevel;
                viewModel.PartitionParentLevel = UCA.PartitionParentLevel;
                if (UCA.Version=="")
                {
                    UCA.Version = "1";
                    db.SaveChanges();
                }
                viewModel.Version = UCA.Version;

                if (RoleMethod != "" && string.IsNullOrEmpty(Search))
                {
                    var VersionDetails = (from vd in db.VersionDetails
                                            where vd.CompanyName == UserData.CompanyName && 
                                                  vd.ActiveVersion == "Y" && 
                                                  vd.OperType == UCA.Oper && 
                                                  vd.UserRole == "Finalyzer"
                                            select vd).FirstOrDefault();
                    if (VersionDetails != null)
                    {
                        UCA.SelectedInitiative = VersionDetails.Initiative;
                        UCA.SelectedPopulation = VersionDetails.Population;
                        UCA.SelectedVersion = VersionDetails.Version;
                        UCA.Version= (VersionDetails.VersionNo == null ?1 : VersionDetails.VersionNo).ToString();
                        UCA.ShowLevel = VersionDetails.ShowLevel;
                        UCA.ParentLevel = "999999";

                        viewModel.ShowLevel = VersionDetails.ShowLevel;
                        viewModel.PartitionShowLevel = VersionDetails.ShowLevel;
                        viewModel.Version = (VersionDetails.VersionNo==null?1: VersionDetails.VersionNo).ToString();

                        viewModel.SelectedInitiative = UCA.SelectedInitiative;
                        viewModel.SelectedPopulation = UCA.SelectedPopulation;
                        viewModel.SelectedVersion = UCA.SelectedVersion;

                        UCA.Role = RoleMethod;
                        viewModel.Role = RoleMethod;
                        db.SaveChanges();
                    }
                    else
                    {
                        UCA.Company = UserData.CompanyName;
                        UCA.KeyDate = "2018/07/11";
                        UCA.UserId = UserData.UserName;
                        UCA.Version = "1";
                        UCA.UsedView = "Normal";
                        UCA.Oper = "OV";
                        UCA.Levels = "One";
                        UCA.ShowLevel = "101212";
                        UCA.ParentLevel = "999999";
                        UCA.PartitionShowLevel = "101212";
                        UCA.PartitionParentLevel = "999999";
                        UCA.Country = "CH";
                        UCA.Role = RoleMethod;
                        UCA.SelectedInitiative = "SelectInitiative";
                        UCA.SelectedPopulation = "SelectPopulation";
                        UCA.SelectedUser = UserData.UserName;
                        UCA.SelectedVersion = "SelectVersion";
                        UCA.SelectedShape = "RoundedRectangle";
                        UCA.SelectedSkin = "white";
                        UCA.SelectedShowPicture = "Yes";
                        UCA.SelectedSplitScreen = "Yes";
                        UCA.SelectedSplitScreenDirection = "Vertical";
                        UCA.SelectedTextColor = "black";
                        UCA.SelectedBorderColor = "cyan";
                        UCA.SelectedBorderWidth = "3";
                        UCA.SelectedLineColor = "#634329";
                        UCA.SelectedPortraitModeMultipleLevel = "Yes";
                        UCA.SelectedFunctionalManagerType = "ShowFM";
                        UCA.SelectedFMLine = "PN";
                        UCA.OrgChartType = "OD";
                        UCA.CopyPaste = "";

                        db.SaveChanges();

                        viewModel.ShowLevel = "101212";
                        viewModel.ParentLevel = "999999";
                        viewModel.PartitionShowLevel = "101212";
                        viewModel.PartitionParentLevel = "101212";
                        viewModel.KeyDate = "2018/07/11";
                        viewModel.Levels = "One";
                        viewModel.Version = "1";
                        viewModel.Oper = "OV";
                        viewModel.View = "Normal";
                        viewModel.Country = "CH";
                        viewModel.SelectedInitiative = "SelectInitiative";
                        viewModel.SelectedPopulation = "SelectPopulation";
                        viewModel.SelectedUser = UserData.UserName;
                        viewModel.SelectedVersion = "SelectVersion";
                        viewModel.SelectedShape = "RoundedRectangle";
                        viewModel.SelectedSkin = "white";
                        viewModel.SelectedShowPicture = "Yes";
                        viewModel.SelectedSplitScreen = "Yes";
                        viewModel.SelectedSplitScreenDirection = "Vertical";
                        viewModel.SelectedTextColor = "black";
                        viewModel.SelectedBorderColor = "cyan";
                        viewModel.SelectedBorderWidth = "3";
                        viewModel.SelectedLineColor = "#634329";
                        viewModel.SelectedPortraitModeMultipleLevel = "Yes";
                        viewModel.SelectedFunctionalManagerType = "ShowFM";
                        viewModel.SelectedFMLine = "PN";
                        viewModel.OrgChartType = "OD";
                        viewModel.CopyPaste = "";
                        viewModel.Role = RoleMethod;
                    }
                }
            }

            return viewModel;
        }

        public MyLastAction GetUserCurrentAction(string RoleMethod)
        {
            LoginUsers UserData = GetLoginUserInfo("");
            var UCA = (from uca in db.UserLastActions
                       where uca.UserId == UserData.UserName
                       select uca).FirstOrDefault();

            MyLastAction viewModel = new MyLastAction();
            if (UCA == null)
            {
                UserLastActions uca = new UserLastActions();
                uca.Company = UserData.CompanyName;
                uca.KeyDate = "2018/07/11";
                uca.UserId = UserData.UserName;
                uca.Version = "1";
                uca.UsedView = "Normal";
                uca.Oper = "OV";
                uca.Levels = "One";
                uca.ShowLevel = "101212";
                uca.PartitionShowLevel = "101212";
                uca.Country = "CH";
                uca.Role = RoleMethod;
                uca.SelectedInitiative = "SelectInitiative";
                uca.SelectedPopulation = "SelectPopulation";
                uca.SelectedUser = UserData.UserName;
                uca.SelectedVersion = "SelectVersion";
                uca.SelectedShape = "RoundedRectangle";
                uca.SelectedSkin = "white";
                uca.SelectedShowPicture = "Yes";
                uca.SelectedSplitScreen = "Yes";
                uca.SelectedSplitScreenDirection = "Vertical";
                uca.SelectedTextColor = "black";
                uca.SelectedBorderColor = "cyan";
                uca.SelectedBorderWidth = "3";
                uca.SelectedLineColor = "#634329";
                uca.SelectedPortraitModeMultipleLevel = "Yes";
                uca.SelectedFunctionalManagerType = "ShowFM";
                uca.SelectedFMLine = "PN";
                uca.OrgChartType = "OD";
                uca.CopyPaste = "";
                db.UserLastActions.Add(uca);

                db.SaveChanges();

                viewModel.ShowLevel = "101212";
                viewModel.PartitionShowLevel = "101212";
                viewModel.KeyDate = "2018/07/11";
                viewModel.Levels = "One";
                viewModel.Version = "1";
                viewModel.Oper = "OV";
                viewModel.View = "Normal";
                viewModel.Country = "CH";
                viewModel.SelectedInitiative = "SelectInitiative";
                viewModel.SelectedPopulation = "SelectPopulation";
                viewModel.SelectedUser = UserData.UserName;
                viewModel.SelectedVersion = "SelectVersion";
                viewModel.SelectedShape = "RoundedRectangle";
                viewModel.SelectedSkin = "white";
                viewModel.SelectedShowPicture = "Yes";
                viewModel.SelectedSplitScreen = "Yes";
                viewModel.SelectedSplitScreenDirection = "Vertical";
                viewModel.SelectedTextColor = "black";
                viewModel.SelectedBorderColor = "cyan";
                viewModel.SelectedBorderWidth = "3";
                viewModel.SelectedLineColor = "#634329";
                viewModel.SelectedPortraitModeMultipleLevel = "Yes";
                viewModel.SelectedFunctionalManagerType = "ShowFM";
                viewModel.SelectedFMLine = "PN";
                viewModel.OrgChartType = "OD";
                viewModel.CopyPaste = "";
                viewModel.Role = RoleMethod;
            }
            else
            {
                viewModel.KeyDate = UCA.KeyDate;
                viewModel.SelectedInitiative = UCA.SelectedInitiative;
                viewModel.SelectedPopulation = UCA.SelectedPopulation;
                viewModel.SelectedUser = UCA.SelectedUser;
                viewModel.SelectedVersion = UCA.SelectedVersion;
                viewModel.SelectedShape = UCA.SelectedShape;
                viewModel.SelectedSkin = UCA.SelectedSkin;
                viewModel.SelectedShowPicture = UCA.SelectedShowPicture;
                viewModel.SelectedSplitScreen = UCA.SelectedSplitScreen;
                viewModel.SelectedSplitScreenDirection = UCA.SelectedSplitScreenDirection;
                viewModel.SelectedTextColor = UCA.SelectedTextColor;
                viewModel.SelectedBorderColor = UCA.SelectedBorderColor;
                viewModel.SelectedBorderWidth = UCA.SelectedBorderWidth;
                viewModel.SelectedLineColor = UCA.SelectedLineColor;
                viewModel.SelectedPortraitModeMultipleLevel = UCA.SelectedPortraitModeMultipleLevel;
                viewModel.SelectedFunctionalManagerType = UCA.SelectedFunctionalManagerType;
                viewModel.SelectedFMLine = UCA.SelectedFMLine;
                viewModel.OrgChartType = UCA.OrgChartType;
                viewModel.Levels = UCA.Levels;
                viewModel.Oper = UCA.Oper;
                viewModel.View = UCA.UsedView;
                viewModel.Country = UCA.Country;
                viewModel.Role = UCA.Role;
                viewModel.ShowLevel = UCA.ShowLevel;
                viewModel.PartitionShowLevel = UCA.PartitionShowLevel;
                if (UCA.Version == "")
                {
                    UCA.Version = "1";
                    db.SaveChanges();
                }
                viewModel.Version = UCA.Version;

                if (RoleMethod != "")
                {
                    var VersionDetails = (from vd in db.VersionDetails
                                          where vd.CompanyName == UserData.CompanyName &&
                                                vd.ActiveVersion == "Y" &&
                                                vd.OperType == UCA.Oper &&
                                                vd.UserRole == "Finalyzer"
                                          select vd).FirstOrDefault();
                    if (VersionDetails != null)
                    {
                        UCA.SelectedInitiative = VersionDetails.Initiative;
                        UCA.SelectedPopulation = VersionDetails.Population;
                        UCA.SelectedVersion = VersionDetails.Version;
                        UCA.Version = (VersionDetails.VersionNo == null ? 1 : VersionDetails.VersionNo).ToString();
                        UCA.ShowLevel = VersionDetails.ShowLevel;

                        viewModel.ShowLevel = VersionDetails.ShowLevel;
                        viewModel.PartitionShowLevel = VersionDetails.ShowLevel;
                        viewModel.Version = (VersionDetails.VersionNo == null ? 1 : VersionDetails.VersionNo).ToString();

                        viewModel.SelectedInitiative = UCA.SelectedInitiative;
                        viewModel.SelectedPopulation = UCA.SelectedPopulation;
                        viewModel.SelectedVersion = UCA.SelectedVersion;

                        UCA.Role = RoleMethod;
                        viewModel.Role = RoleMethod;
                        db.SaveChanges();
                    }
                    else
                    {
                        UCA.Company = UserData.CompanyName;
                        UCA.KeyDate = "2018/07/11";
                        UCA.UserId = UserData.UserName;
                        UCA.Version = "1";
                        UCA.UsedView = "Normal";
                        UCA.Oper = "OV";
                        UCA.Levels = "One";
                        UCA.ShowLevel = "101212";
                        UCA.PartitionShowLevel = "101212";
                        UCA.Country = "CH";
                        UCA.Role = RoleMethod;
                        UCA.SelectedInitiative = "SelectInitiative";
                        UCA.SelectedPopulation = "SelectPopulation";
                        UCA.SelectedUser = UserData.UserName;
                        UCA.SelectedVersion = "SelectVersion";
                        UCA.SelectedShape = "RoundedRectangle";
                        UCA.SelectedSkin = "white";
                        UCA.SelectedShowPicture = "Yes";
                        UCA.SelectedSplitScreen = "Yes";
                        UCA.SelectedSplitScreenDirection = "Vertical";
                        UCA.SelectedTextColor = "black";
                        UCA.SelectedBorderColor = "cyan";
                        UCA.SelectedBorderWidth = "3";
                        UCA.SelectedLineColor = "#634329";
                        UCA.SelectedPortraitModeMultipleLevel = "Yes";
                        UCA.SelectedFunctionalManagerType = "ShowFM";
                        UCA.SelectedFMLine = "PN";
                        UCA.OrgChartType = "OD";
                        UCA.CopyPaste = "";

                        db.SaveChanges();

                        viewModel.ShowLevel = "101212";
                        viewModel.PartitionShowLevel = "101212";
                        viewModel.KeyDate = "2018/07/11";
                        viewModel.Levels = "One";
                        viewModel.Version = "1";
                        viewModel.Oper = "OV";
                        viewModel.View = "Normal";
                        viewModel.Country = "CH";
                        viewModel.SelectedInitiative = "SelectInitiative";
                        viewModel.SelectedPopulation = "SelectPopulation";
                        viewModel.SelectedUser = UserData.UserName;
                        viewModel.SelectedVersion = "SelectVersion";
                        viewModel.SelectedShape = "RoundedRectangle";
                        viewModel.SelectedSkin = "white";
                        viewModel.SelectedShowPicture = "Yes";
                        viewModel.SelectedSplitScreen = "Yes";
                        viewModel.SelectedSplitScreenDirection = "Vertical";
                        viewModel.SelectedTextColor = "black";
                        viewModel.SelectedBorderColor = "cyan";
                        viewModel.SelectedBorderWidth = "3";
                        viewModel.SelectedLineColor = "#634329";
                        viewModel.SelectedPortraitModeMultipleLevel = "Yes";
                        viewModel.SelectedFunctionalManagerType = "ShowFM";
                        viewModel.SelectedFMLine = "PN";
                        viewModel.OrgChartType = "OD";
                        viewModel.CopyPaste = "";
                        viewModel.Role = RoleMethod;
                    }
                }
            }

            return viewModel;
        }

        public string GetHRCoreVersion(string Country, string Oper)
        {
            LoginUsers UserData = GetLoginUserInfo("");
            VersionDetails VD = (from vd in db.VersionDetails
                                 where vd.CompanyName == UserData.CompanyName &&
                                     vd.OperType == Oper &&
                                     vd.UserRole == "Finalyzer" &&
                                     vd.ActiveVersion == "Y"
                                 select vd).OrderByDescending(x => x.VersionNo).FirstOrDefault();

            if (VD != null) return VD.Version;

            return "NOT AN HR CORE VERSION";
        }

        public string GetUserRoles()
        {
            LoginUsers UserData = GetLoginUserInfo("");
            List<UserRoles> URoles = (from ur in db.UserRoles where ur.UserId == UserData.UserName select ur).ToList();
            InitializeTables Initialize = (from ur in db.InitializeTables where ur.CompanyName == UserData.CompanyName select ur).FirstOrDefault();
            string[] ArrayCR = Initialize.CompanyRoles.Split(',');
            foreach (string CR in ArrayCR) {
                foreach (UserRoles UR in URoles)
                {
                    if (UR.Role.IndexOf(CR) != -1) return CR;
                }
            }

            return "NoUser";
        }

        public string CheckUserRoles(string CheckRole)
        {
            LoginUsers UserData = GetLoginUserInfo("");
            List<UserRoles> URoles = (from ur in db.UserRoles where ur.UserId == UserData.UserName select ur).ToList();
            InitializeTables Initialize = (from ur in db.InitializeTables where ur.CompanyName == UserData.CompanyName select ur).FirstOrDefault();
            string[] ArrayCR = Initialize.CompanyRoles.Split(',');
            foreach (string CR in ArrayCR)
            {
                foreach (UserRoles UR in URoles)
                {
                    if (UR.Role.IndexOf(CR) != -1)
                    {
                        if (CheckRole==CR) return CR;
                    }
                }
            }

            return "NoUser";
        }

        public DataSet GetOrgChartDataTable(string UserType, string Country, string ShowLevel, string ParentLevel, 
                                            string Levels, string Oper, string Version, string CurrentLevel, 
                                            string OneLevelUp, string FlagFM)
        {
            DataSet RetTable = new DataSet();
            LoginUsers UserData = GetLoginUserInfo("");
            try
            {
                DataTable OrgChartData = null;

                if (ShowLevel == null) ShowLevel = "";
                if (Levels == null || Levels == "" || Levels == " ") Levels = "1";

                // Operation & Legal chart details              
                OrgChartData = GetLevelInfo(UserData.UserName, UserData.CompanyName, UserType, ShowLevel, ParentLevel, Levels, Version, Oper).Tables[0];
                DataTable OrgChartDataFM = new DataTable();
                foreach(DataColumn dc in OrgChartData.Columns)
                {
                    OrgChartDataFM.Columns.Add(dc.ColumnName, dc.DataType);
                }
                if (FlagFM == "HideFM")
                {
                    foreach (DataRow dr in OrgChartData.Rows)
                    {
                        if (dr["DOTTED_LINE_FLAG"].ToString() == "Y")
                        {
                            OrgChartDataFM.Rows.Add(dr.ItemArray);
                        }
                    }
                }
                int COUNT = OrgChartData.Rows.Count;
                if (COUNT >= 1)
                {
                    for (int Idr = COUNT - 1; Idr >= 0; Idr--)
                    {
                        if (OrgChartData.Rows[Idr]["DOTTED_LINE_FLAG"].ToString() == "Y")
                            OrgChartData.Rows.Remove(OrgChartData.Rows[Idr]);
                    }
                }

                if (OneLevelUp=="Yes")
                {
                    COUNT = OrgChartData.Rows.Count;
                    if (COUNT >= 1)
                    {
                        for (int Idr = COUNT - 1; Idr >= 0; Idr--)
                        {
                            if (OrgChartData.Rows[Idr]["PARENT_LEVEL_ID"].ToString() == ShowLevel &&
                                OrgChartData.Rows[Idr]["LEVEL_ID"].ToString() != CurrentLevel)
                                OrgChartData.Rows.Remove(OrgChartData.Rows[Idr]);
                        }
                    }

                    COUNT = OrgChartDataFM.Rows.Count;
                    if (COUNT >= 1)
                    {
                        for (int Idr = COUNT - 1; Idr >= 0; Idr--)
                        {
                            if (OrgChartDataFM.Rows[Idr]["PARENT_LEVEL_ID"].ToString() == ShowLevel &&
                                OrgChartDataFM.Rows[Idr]["LEVEL_ID"].ToString() != CurrentLevel)
                                OrgChartDataFM.Rows.Remove(OrgChartDataFM.Rows[Idr]);
                        }
                    }
                }

                if (OrgChartData.DataSet != null)
                {
                    DataSet RemoveDataTable = OrgChartData.DataSet;
                    RemoveDataTable.Tables.Remove(OrgChartData);
                }
                OrgChartData.TableName = "OrgChartData";
                RetTable.Tables.Add(OrgChartData);
                if (OrgChartDataFM.DataSet != null)
                {
                    DataSet RemoveDataTable = OrgChartDataFM.DataSet;
                    RemoveDataTable.Tables.Remove(OrgChartDataFM);
                }
                OrgChartDataFM.TableName = "OrgChartDataFM";
                RetTable.Tables.Add(OrgChartDataFM);

                return RetTable;
            }
            catch (Exception ex)
            {
                Dictionary<string, string> Info = new Dictionary<string, string>();
                Info.Add("WebHTML", "Error");
                Info.Add("Message", ex.Message);
                Info.Add("StackTrace", ex.StackTrace);

                return null;
            }
        }

        public string[] GetOrgChartData(string UserType, string Country, string ShowLevel, string ParentLevel, 
                                      string Levels, string Oper, string Version, 
                                      string OrgType, string PortraitModeMultipleLevel, string FlagFM)
        {
            string[] RetOrgChart = { "", "" };
            LoginUsers UserData = GetLoginUserInfo("");
            try
            {
                DataSet orgChartDataSet = null;
                DataTable orgChartData = null;

                if (ShowLevel == null) ShowLevel = "";
                if (Levels == null || Levels == "" || Levels == " ") Levels = "1";

                // Operation & Legal chart details              
                orgChartDataSet = GetLevelInfo(UserData.UserName, UserData.CompanyName, UserType, ShowLevel, ParentLevel, Levels, Version, Oper);
                DataTable OrgTreeData = orgChartDataSet.Tables[1];
                orgChartData= orgChartDataSet.Tables[0];
                RetOrgChart[0] = JsonConvert.SerializeObject(OrgTreeData);

                if (orgChartData.Rows.Count >= 500)
                {
                    orgChartDataSet = GetLevelInfo(UserData.UserName, UserData.CompanyName, UserType, ShowLevel, ParentLevel, "One", Version, Oper);
                    OrgTreeData = orgChartDataSet.Tables[1];
                    orgChartData = orgChartDataSet.Tables[0];
                    RetOrgChart[0] = JsonConvert.SerializeObject(OrgTreeData);
                }

                if (FlagFM == "HideFM")
                {
                    int COUNT = orgChartData.Rows.Count;
                    if (COUNT >= 1)
                    {
                        for (int Idr = COUNT - 1; Idr >= 0; Idr--)
                        {
                            if (orgChartData.Rows[Idr]["DOTTED_LINE_FLAG"].ToString() == "Y")
                                orgChartData.Rows.Remove(orgChartData.Rows[Idr]);
                        }
                    }
                }

                if (OrgType == "OD" && PortraitModeMultipleLevel == "No")
                {
                    int COUNT = orgChartData.Rows.Count;
                    if (COUNT >= 1)
                    {
                        for (int Idr = COUNT - 1; Idr >= 0; Idr--)
                        {
                            if (!(orgChartData.Rows[Idr]["PARENT_LEVEL_ID"].ToString() == ShowLevel ||
                                  orgChartData.Rows[Idr]["LEVEL_ID"].ToString() == ShowLevel))
                                orgChartData.Rows.Remove(orgChartData.Rows[Idr]);
                        }
                    }
                }

                RetOrgChart[1] = JsonConvert.SerializeObject(orgChartData);

                return RetOrgChart;
            }
            catch (Exception ex)
            {
                Dictionary<string, string> Info = new Dictionary<string, string>();
                Info.Add("WebHTML", "Error");
                Info.Add("Message", ex.Message);
                Info.Add("StackTrace", ex.StackTrace);

                RetOrgChart[0] = "Error";
                RetOrgChart[1] = JsonConvert.SerializeObject(Info);

                return RetOrgChart;
            }
        }

        public string GetOrgChartHrCoreData(string UserType, string Country, string ShowLevel, string Levels, string Oper, string Version, string FlagFM)
        {
            LoginUsers UserData = GetLoginUserInfo("");
            try
            {
                DataTable orgChartData = null;

                if (ShowLevel == null) ShowLevel = "";
                if (Levels == null || Levels == "" || Levels == " ") Levels = "1";

                VersionDetails VD = (from vd in db.VersionDetails
                                     where vd.CompanyName == UserData.CompanyName &&
                                         vd.Country == Country &&
                                         vd.OperType == Oper &&
                                         vd.UserRole == "Finalyzer" &&
                                         vd.ActiveVersion == "Y"
                                     select vd).FirstOrDefault();
                if (VD != null)
                {
                    // Operation chart details              
                    orgChartData = GetLevelInfo(UserData.UserName, UserData.CompanyName, "Finalyzer", VD.ShowLevel, "999999", "All", VD.VersionNo.ToString(), Oper).Tables[0];
                    if (FlagFM == "HideFM")
                    {
                        int COUNT = orgChartData.Rows.Count;
                        if (COUNT >= 1)
                        {
                            for (int Idr = COUNT - 1; Idr >= 0; Idr--)
                            {
                                if (orgChartData.Rows[Idr]["DOTTED_LINE_FLAG"].ToString() == "Y")
                                    orgChartData.Rows.Remove(orgChartData.Rows[Idr]);
                            }
                        }
                    }
                }

                return JsonConvert.SerializeObject(orgChartData);
            }
            catch (Exception ex)
            {
                Dictionary<string, string> Info = new Dictionary<string, string>();
                Info.Add("WebHTML", "Error");
                Info.Add("Message", ex.Message);
                Info.Add("StackTrace", ex.StackTrace);

                return JsonConvert.SerializeObject(Info);
            }
        }

        public string GetMenuItems(string UserRole)
        {
            LoginUsers UserData = GetLoginUserInfo("");
            DataSet retDS = null;

            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                cmd.CommandText = "PROC_GET_USER_MENU_LIST";

                cmd.Parameters.Add("@USER_ID", SqlDbType.VarChar, 500).Value = UserData.UserName;
                cmd.Parameters.Add("@COMPANY_NAME", SqlDbType.VarChar, 500).Value = UserData.CompanyName;

                Common csobj = new Common();
                retDS = csobj.SPReturnDataSet(cmd);
                retDS.Tables[0].TableName = "MT";
                retDS.Tables[1].TableName = "VT";
            }

            return JsonConvert.SerializeObject(retDS);
        }

        public string GetFinalyzerVerion(string Oper)
        {
            LoginUsers UserData = GetLoginUserInfo("");
            string VersionOption = "";
            var myIV = (from iv in db.VersionDetails
                                   where iv.CompanyName == UserData.CompanyName &&
                                         iv.UserRole == "Finalyzer" &&
                                         iv.ActiveVersion == "Y" &&
                                         iv.OperType == Oper
                        select new { iv.Initiative, iv.Population, iv.Version, iv.VersionNo}).Distinct().ToList();
            if (myIV != null)
            {
                for(int Idx=0; Idx<= myIV.Count()-1; Idx++)
                {
                    if (myIV[Idx].Version != null)
                    {
                        VersionOption += ",{\"VersionName\": \"" + myIV[Idx].Version +
                                           "\",\"VersionNo\": \"" + myIV[Idx].VersionNo + "\" }";
                    }
                }
            }

            return "["+((VersionOption=="")?"":VersionOption.Substring(1))+"]";
        }

        public string[] GetVersionDetails(int Version, string UserRole, string Oper)
        {
            string[] GetUserResult = { "", "", "", "", "", "", "", "", "", "", "", "",
                                       "", "", "", "", "", "", "", "", "", "", "", "",
                                       "", "", "", "" };
            LoginUsers UserData = GetLoginUserInfo("");
            InitializeTables Initialize = (from ur in db.InitializeTables
                                           where ur.CompanyName == UserData.CompanyName
                                           select ur).FirstOrDefault();
            VersionDetails myIV = (from iv in db.VersionDetails
                                   where iv.CompanyName == UserData.CompanyName &&
                                         (iv.UserRole == "Player" || iv.UserRole == "Finalyzer") &&
                                         iv.VersionNo == Version &&
                                         iv.ActiveVersion == "Y" &&
                                         iv.OperType == Oper
                                   select iv).FirstOrDefault();
            if (myIV != null)
            {
                string FinalyzerVersion = (myIV.FinalyzeVersionNo == null ? 0 : myIV.FinalyzeVersionNo).ToString();
                string sTableName = UserData.CompanyName.ToString().Trim().ToUpper().Replace(" ", "_") + "_" + FinalyzerVersion + "_LevelInfos";
                if (Oper == "LV") sTableName = UserData.CompanyName.ToString().Trim().ToUpper().Replace(" ", "_") + "_" + FinalyzerVersion + "_LegalInfos";

                if (UserRole == "Player")
                {
                    if (myIV.UserName == UserData.UserName && myIV.UserRole == "Player")
                    {
                        GetUserResult[0] = "Success";
                        GetUserResult[1] = UserData.UserName;
                        GetUserResult[2] = myIV.UserRole;
                        GetUserResult[3] = myIV.VersionNo.ToString();
                        GetUserResult[4] = myIV.FinalyzeVersionNo.ToString();
                        GetUserResult[5] = sTableName;
                    }
                    else
                    {
                        GetUserResult[0] = "Failure";
                        GetUserResult[1] = "Please select your own version";
                    }
                }
                else if (UserRole == "Finalyzer")
                {
                    if (myIV.UserName == UserData.UserName || Initialize.FinalyzerCanChange=="Yes")
                    {
                        GetUserResult[0] = "Success";
                        GetUserResult[1] = UserData.UserName;
                        GetUserResult[2] = myIV.UserRole;
                        GetUserResult[3] = myIV.VersionNo.ToString();
                        GetUserResult[4] = myIV.FinalyzeVersionNo.ToString();
                        GetUserResult[5] = sTableName;
                    }
                    else
                    {
                        GetUserResult[0] = "Failure";
                        GetUserResult[1] = "Please select your own version";
                    }
                }
                else if (UserRole == "Anonymous")
                {
                    GetUserResult[0] = "Success";
                    GetUserResult[1] = myIV.UserName;
                    GetUserResult[2] = myIV.UserRole;
                    GetUserResult[3] = myIV.VersionNo.ToString();
                    GetUserResult[4] = myIV.FinalyzeVersionNo.ToString();
                    GetUserResult[5] = sTableName;
                }
                if (GetUserResult[0] == "Success")
                {
                    int BaseVersion = Convert.ToInt32(GetUserResult[4]);
                    var UFH = (from uef in db.UploadFilesHeaders
                               where uef.CompanyName == UserData.CompanyName &&
                                       uef.Role == "Finalyzer" &&
                                       uef.VersionNo == BaseVersion
                               select uef).OrderByDescending(x => x.CreatedDate).FirstOrDefault();
                    if (UFH != null)
                    {
                        GetUserResult[6] = UFH.SerialNoFlag;
                        GetUserResult[7] = UFH.FirstPositionField;
                        GetUserResult[8] = UFH.FirstPosition;
                        GetUserResult[9] = UFH.KeyField;
                        GetUserResult[10] = UFH.ParentField;
                        GetUserResult[11] = UFH.FullNameFields;
                        GetUserResult[12] = UFH.FileType;
                        GetUserResult[13] = UFH.UploadFileName;
                        GetUserResult[14] = UFH.ExcelDownLoadFields;
                        GetUserResult[15] = UFH.VersionName;
                        GetUserResult[16] = UFH.VersionDesc;
                        GetUserResult[17] = UFH.UseFields;
                        GetUserResult[18] = UFH.PositionCostField;
                        GetUserResult[19] = UFH.ShowFieldType;
                        GetUserResult[20] = UFH.PositionCostType;
                    }
                }
            }
            else
            {
                GetUserResult[0] = "Failure";
                GetUserResult[1] = "Invalid version/data corrupted";
            }

            return GetUserResult;
        }

        public LoginUsers GetLoginUserInfo(string Role)
        {
            LoginUsers LoginUser = new LoginUsers();

            if (HttpContext.Current.Session["LoginUserInf"] == null)
            {
                Models.DBContext db = new Models.DBContext();
                string wcCompanyName = ConfigurationManager.AppSettings["wcCompanyName"].ToString();
                var INI = (from ini in db.InitializeTables
                           where ini.Authentication.ToUpper() == "WINDOWS" && ini.CompanyName == wcCompanyName
                           select ini).FirstOrDefault();
                if (INI != null)
                {
                    string[] LogonUser = HttpContext.Current.Request.LogonUserIdentity.Name.ToString().Split('\\');
                    string UserName = LogonUser[1];

                    var UserData = (from usr in db.UserRoles
                                    where usr.UserId == UserName
                                    select usr).FirstOrDefault();
                    if (UserData != null)
                    {
                        LoginUser.CompanyName = UserData.CompanyName;
                        LoginUser.UserName = UserData.UserId;
                        LoginUser.Email = UserData.Email;
                        LoginUser.Roles = UserData.Role;
                    }
                    else
                    {
                        LoginUser.CompanyName = wcCompanyName;
                        LoginUser.UserName = UserName;
                        LoginUser.Email = UserName+"@gmail.com";
                        LoginUser.Roles = "User,EndUser";

                        UserRoles URoles = new UserRoles();
                        URoles.CompanyName = LoginUser.CompanyName;
                        URoles.UserId = LoginUser.UserName;
                        URoles.Email = LoginUser.Email;
                        URoles.Role = LoginUser.Roles;
                        db.UserRoles.Add(URoles);

                        db.SaveChanges();
                    }
                }
                else
                {
                    ApplicationUser UserData = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
                    var UserRoles = (from usr in db.UserRoles
                                     where usr.UserId == UserData.UserName
                                     select usr).FirstOrDefault();

                    LoginUser.CompanyName = UserData.CompanyName;
                    LoginUser.UserName = UserData.UserName;
                    LoginUser.Email = UserData.Email;
                    LoginUser.Roles = UserRoles.Role;
                }
                HttpContext.Current.Session["LoginUserInf"] = LoginUser.CompanyName + ";" +
                                                              LoginUser.UserName + ";" +
                                                              LoginUser.Email + ";" +
                                                              LoginUser.Roles + ";";
            }
            else
            {
                string[] LoginUserInf = HttpContext.Current.Session["LoginUserInf"].ToString().Split(';');
                LoginUser.CompanyName = LoginUserInf[0];
                LoginUser.UserName = LoginUserInf[1];
                LoginUser.Email = LoginUserInf[2];
                LoginUser.Roles = LoginUserInf[3];
            }

            LoginUser.ValidUser = "No";
            if (LoginUser.Roles.LastIndexOf(Role) != -1 || Role == "") LoginUser.ValidUser = "Yes";

            return LoginUser;
        }
    }
}



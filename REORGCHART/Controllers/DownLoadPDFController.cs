using REORGCHART.Helper;
using REORGCHART.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Http.WebHost;
using System.Web.Routing;
using System.Web.Script.Serialization;
using System.Web.SessionState;

namespace REORGCHART.Controllers
{
    public class SessionControllerHandler : HttpControllerHandler, IRequiresSessionState
    {
        public SessionControllerHandler(RouteData routeData)
            : base(routeData)
        { }
    }

    public class SessionHttpControllerRouteHandler : HttpControllerRouteHandler
    {
        protected override IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            return new SessionControllerHandler(requestContext.RouteData);
        }
    }

    public class DownloadPDFInfo
    {
        public string ViewFlag { get; set; }
        public string LevelUp { get; set; }
        public string CurrentLevel { get; set; }
    }

    public class BindJson : System.Web.Http.Filters.ActionFilterAttribute
    {
        Type _type;
        string _queryStringKey;
        public BindJson(Type type, string queryStringKey)
        {
            _type = type;
            _queryStringKey = queryStringKey;
        }
        public override void OnActionExecuting(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            var json = actionContext.Request.RequestUri.ParseQueryString()[_queryStringKey];
            var serializer = new JavaScriptSerializer();
            actionContext.ActionArguments[_queryStringKey] = serializer.Deserialize(json, _type);
        }
    }

    public class DownLoadPDFController : ApiController
    {
        [HttpGet]
        [ActionName("DownLoadOrgChartPDF")]
        [BindJson(typeof(DownloadPDFInfo), "Info")]
        public HttpResponseMessage DownloadOrgChartPDF(DownloadPDFInfo Info)
        {
            string ErrorString = "ERROR_NONE", ErrorMessage = "", PDF_FileName="";
            try
            {
                LevelInfo LI = new LevelInfo();
                string AuthHeader = HttpContext.Current.Request.Headers["Authorization"];
            
                if (AuthHeader != null && AuthHeader.StartsWith("Basic"))
                {
                    //Extract credentials
                    string EncodedUsernamePassword = AuthHeader.Substring("Basic ".Length).Trim();

                    //The coding should be iso or you could use ASCII and UTF-8 decoder
                    Encoding Encoding = Encoding.GetEncoding("iso-8859-1");
                    string UsernamePassword = Encoding.GetString(Convert.FromBase64String(EncodedUsernamePassword));

                    int SeperatorIndex = UsernamePassword.IndexOf(':');

                    string UserName = UsernamePassword.Substring(0, SeperatorIndex);
                    string Password = UsernamePassword.Substring(SeperatorIndex + 1);

                    using (Models.DBContext db = new Models.DBContext())
                    {
                        LoginUsers UserData = LI.GetLoginUserWebAPI("", UserName);
                        var UCA = (from uca in db.UserLastActions
                                    where uca.UserId == UserData.UserName
                                    select uca).FirstOrDefault();
                        if (UCA != null)
                        {
                            var FolderName = new DirectoryInfo(HttpContext.Current.Server.MapPath("~/App_Data/Versions/DownLoadVersion"));
                            string FN = UserData.UserName + "_" + UserData.CompanyName + "_" + DateTime.Now.ToString("yyyyMMddhhmmss");
                            string FileNameWithPath = FolderName + "\\" + FN + ".PDF";
                            int FinalyzerVersion = Convert.ToInt32(LI.ChooseFinalyzerVersionWebAPI(UCA.Version, UserName));

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
                                    if (Info.LevelUp == "Yes") ShowLevelPDF = UCA.ParentLevel == "999999" ? UCA.ShowLevel : UCA.ParentLevel;
                                    string CurrentLevelPDF = "All";
                                    if (Info.CurrentLevel == "Yes")
                                    {
                                        CurrentLevelPDF = UCA.Levels;
                                        if (Info.LevelUp == "Yes")
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
                                            else Info.LevelUp = "No";
                                        }
                                    }
                                    if (UCA.ParentLevel == "999999") Info.LevelUp = "No";
                                    AllLevelPDF AllPDF = new AllLevelPDF(ShowLevelPDF, "999999", UCA.Oper.Trim().ToUpper(), UCA.Country,
                                                                            CurrentLevelPDF, "23/10/2018", 6, "EN", "", "PDF", "POSITION_CALCULATED_COST",
                                                                            UFH.PositionCostField, UserData.CompanyName, UFH.FirstPosition,
                                                                            FinalyzerVersion);
                                    DataSet OrgDataSet = LI.GetOrgChartDataTable(UCA.Role, UCA.Country, ShowLevelPDF, "999999", CurrentLevelPDF, UCA.Oper, UCA.Version, UCA.ShowLevel, Info.LevelUp, UCA.SelectedFunctionalManagerType);
                                    AllPDF.CreateAllLevelPDFIntermediateResults(OrgDataSet, UCA.Oper.ToUpper(), VD.VersionNo.ToString(), "", "PDF");
                                    OrgDataSet = LI.GetOrgChartDataTable(UCA.Role, UCA.Country, ShowLevelPDF, "999999", CurrentLevelPDF, UCA.Oper, UCA.Version, UCA.ShowLevel, Info.LevelUp, UCA.SelectedFunctionalManagerType);
                                    PDF_FileName=AllPDF.CreateAllLevelPDF(OrgDataSet, "PDF", UCA.Company, UCA.UserId, UCA.Oper, UCA.SelectedFMLine, Info.ViewFlag, FileNameWithPath, Info.LevelUp, UCA.ShowLevel);
                                }
                            }
                        }
                    }
                }
                else
                {
                    //Handle what happens if that isn't the case
                    ErrorString = "ERROR_AUTH";
                }
            }
            catch (Exception ex)
            {
                // Log exception code goes here  
                ErrorString = "ERROR_IN";
                ErrorMessage = ex.Message;
            }

            if (ErrorString == "ERROR_IN")
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Error occured while executing DownloadOrgChartPDF(" + ErrorMessage + ").");
            }
            else if (ErrorString == "ERROR_AUTH")
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "The authorization header is either empty or isn't Basic.");
            }

            return Request.CreateResponse(HttpStatusCode.OK, PDF_FileName);
        }
    }
}
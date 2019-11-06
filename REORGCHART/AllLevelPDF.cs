//***********************************************************************************************//
// Name                : Subramanian(CHANDSUI)                                                   //
// Incident No.        : Orgcharts: Creating PDF for All level Data (3100077738)                 //
// CR No               : 1100060563                                                              //
// CD No               : 1400024591                                                              //
// Date                : 30-Jan-2017                                                             // 
// Description         : Creating PDF for All level Data                                         //
//***********************************************************************************************//

using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Runtime.Serialization;
using System.Drawing.Imaging;
using System.Data.SqlClient;

using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;

using ceTe.DynamicPDF;
using Newtonsoft.Json.Linq;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Presentation;
using ppt=DocumentFormat.OpenXml.Presentation;
using DocumentFormat.OpenXml.Drawing;
using Dgm = DocumentFormat.OpenXml.Drawing.Diagrams;
using ceTe.DynamicPDF.PageElements;

using REORGCHART.Data;
using REORGCHART.Models;
using REORGCHART.Helper;
using System.Xml;
using DocumentFormat.OpenXml;
using System.Text;
using DocumentFormat.OpenXml.Office.Drawing;
using pptoffice = DocumentFormat.OpenXml.Office.Drawing;
using System.Configuration;

namespace REORGCHART
{
    // Org chart Information
    [DataContract]
    public class ObjectInfPDF
    { 
        [DataMember]
        public string Id { get; set; }
        [DataMember]
        public string Title { get; set; }
        [DataMember]
        public string PId { get; set; }
        [DataMember]
        public string Level { get; set; }
        [DataMember]
        public int Row { get; set; }
        [DataMember]
        public int Col { get; set; }
        [DataMember]
        public int Width { get; set; }
        [DataMember]
        public int Height { get; set; }
        [DataMember]
        public int Cwidth { get; set; }
        [DataMember]
        public int Cheight { get; set; }
        [DataMember]
        public int Owidth { get; set; }
        [DataMember]
        public int Oheight { get; set; }
        [DataMember]
        public string NextLevelFlag { get; set; }
        [DataMember]
        public string GrayColourFlag { get; set; }
        [DataMember]
        public string DottedLineFlag { get; set; }
        [DataMember]
        public string ShowFullBox { get; set; }
        [DataMember]
        public string Language { get; set; }
        [DataMember]
        public string SortNo { get; set; }
        [DataMember]
        public int NOR { get; set; }
        [DataMember]
        public int SOC { get; set; }
        [DataMember]
        public string PositionFlag { get; set; }
        [DataMember]
        public string ColorFlag { get; set; }
        [DataMember]
        public string BackColor { get; set; }
        [DataMember]
        public string Flag { get; set; }
        [DataMember]
        public string BreadGram { get; set; }
        [DataMember]
        public string BreadGramName { get; set; }
        [DataMember]
        public int ActualLevelNo { get; set; }
        [DataMember]
        public int RealCol { get; set; }
        [DataMember]
        public int RealRow { get; set; }
        [DataMember]
        public int RealBoxWidth { get; set; }
        [DataMember]
        public int RealBoxHeight { get; set; }
        [DataMember]
        public int NodeIndex { get; set; }
        [DataMember]
        public int NewRow { get; set; }
        [DataMember]
        public int NewPage { get; set; }

        public ObjectInfPDF()
        {
        }

        public ObjectInfPDF(string pId, string pTitle, string pPId,
                         string pLevel, int pRow, int pCol,
                         int pWidth, int pHeight, int pCwidth, int pCheight, int pOwidth, int pOheight,
                         string pNLF, string pGCF, string pDLF, string pSFB,
                         string pLN, string pSortNo, string NOR_COUNT, string SOC_COUNT, string pPositionFlag,
                         string pColorFlag, string pBackColor, string pFlag, string pBreadGram, string pBreaGramName, int pActualLevelNo,
                         int RC, int RR, int RBW, int RBH, int NI)
        {
            Id = pId;
            Title = pTitle;
            PId = pPId;
            Level = pLevel;
            Row = pRow;
            Col = pCol;
            Width = pWidth;
            Height = pHeight;
            Cwidth = pCwidth;
            Cheight = pCheight;
            Owidth = pOwidth;
            Oheight = pOheight;
            NextLevelFlag = pNLF;
            GrayColourFlag = pGCF;
            DottedLineFlag = pDLF;
            ShowFullBox = pSFB;
            Language = pLN;
            SortNo = pSortNo;
            PositionFlag = pPositionFlag;
            ColorFlag = pColorFlag;
            BackColor = pBackColor;
            Flag = pFlag;
            NOR = Convert.ToInt32(NOR_COUNT);
            SOC = Convert.ToInt32(SOC_COUNT);
            BreadGram = pBreadGram;
            BreadGramName = pBreaGramName;
            ActualLevelNo = pActualLevelNo;
            RealCol = RC;
            RealRow = RR;
            RealBoxWidth = RBW;
            RealBoxHeight = RBH;
            NodeIndex = NI;
            NewRow = 0;
            NewPage = 0;
        }
    }

    [DataContract]
    public class ObjectIWPDF
    {
        [DataMember]
        public string Id { get; set; }
        [DataMember]
        public string PId { get; set; }
        [DataMember]
        public int Width { get; set; }
        [DataMember]
        public int Height { get; set; }
        [DataMember]
        public float TotalWidth { get; set; }
        [DataMember]
        public float TotalHeight { get; set; }
        [DataMember]
        public string UsedFlag { get; set; }

        public ObjectIWPDF()
        {
        }

        public ObjectIWPDF(string pId, string pPId,
                         int pWidth, int pHeight, string pUsedFlag)
        {
            Id = pId;
            PId = pPId;
            Width = pWidth;
            Height = pHeight;
            UsedFlag = pUsedFlag;
        }
    }

    // Org chart Information
    [DataContract]
    public class PageObjectInf
    {
        [DataMember]
        public string Id { get; set; }
        [DataMember]
        public int Col { get; set; }
        [DataMember]
        public int Row { get; set; }
        [DataMember]
        public int CurPageNo { get; set; }
        [DataMember]
        public int PageNo { get; set; }

        public PageObjectInf(string pId, int pCol, int pRow, int pCurPageNo, int pPageNo)
        {
            Id = pId;
            Col = pCol;
            Row = pRow;
            CurPageNo = pCurPageNo;
            PageNo = pPageNo;
        }

    }

    // Org chart Information
    [DataContract]
    public class ObjectInf
    {
        [DataMember]
        public string Id { get; set; }
        [DataMember]
        public string Title { get; set; }
        [DataMember]
        public string PId { get; set; }
        [DataMember]
        public string Level { get; set; }
        [DataMember]
        public int Row { get; set; }
        [DataMember]
        public int Col { get; set; }
        [DataMember]
        public int Width { get; set; }
        [DataMember]
        public int Height { get; set; }
        [DataMember]
        public int Owidth { get; set; }
        [DataMember]
        public int Oheight { get; set; }
        [DataMember]
        public string NextLevelFlag { get; set; }
        [DataMember]
        public string GrayColourFlag { get; set; }
        [DataMember]
        public string DottedLineFlag { get; set; }
        [DataMember]
        public string ShowFullBox { get; set; }
        [DataMember]
        public string Language { get; set; }
        [DataMember]
        public string SortNo { get; set; }
        [DataMember]
        public string PositionFlag { get; set; }
        [DataMember]
        public string ColorFlag { get; set; }
        [DataMember]
        public string BackColor { get; set; }
        [DataMember]
        public string Flag { get; set; }
        [DataMember]
        public string PDFLevel { get; set; }
        [DataMember]
        public string GDDBID { get; set; }

        public ObjectInf()
        {
        }

        public ObjectInf(string pId, string pTitle, string pPId,
                         string pLevel, int pRow, int pCol,
                         int pWidth, int pHeight, int pOwidth, int pOheight,
                         string pNLF, string pGCF, string pDLF, string pSFB,
                         string pLN, string pSortNo, string pPositionFlag,
                         string pColorFlag, string pBackColor,
                         string pFlag, string pPDFLevel, string pGDDBID)
        {
            Id = pId;
            Title = pTitle;
            PId = pPId;
            Level = pLevel;
            Row = pRow;
            Col = pCol;
            Width = pWidth;
            Height = pHeight;
            Owidth = pOwidth;
            Oheight = pOheight;
            NextLevelFlag = pNLF;
            GrayColourFlag = pGCF;
            DottedLineFlag = pDLF;
            ShowFullBox = pSFB;
            Language = pLN;
            SortNo = pSortNo;
            PositionFlag = pPositionFlag;
            ColorFlag = pColorFlag;
            BackColor = pBackColor;
            Flag = pFlag;
            PDFLevel = pPDFLevel;
            GDDBID = pGDDBID;
        }
    }

    // All Org chart Information
    [DataContract]
    public class AllObjectInf
    {
        [DataMember]
        public string Id { get; set; }
        [DataMember]
        public string OprId { get; set; }
        [DataMember]
        public string LevelInfo { get; set; }
        [DataMember]
        public string LevelFlag { get; set; }
        [DataMember]
        public int Row { get; set; }
        [DataMember]
        public int Col { get; set; }
        [DataMember]
        public int Height { get; set; }
        [DataMember]
        public int Page { get; set; }
        [DataMember]
        public int ColIndex { get; set; }
        [DataMember]
        public string LevelType { get; set; }
        [DataMember]
        public int ItemCount { get; set; }
        [DataMember]
        public string LevelEnd { get; set; }

        public AllObjectInf(string pId, string pOprId, string pLevelInfo, string pLevelFlag,
                            int pRow, int pCol, int pHeight, int pPage,
                            int pColIndex, string pLevelType, int pItemCount, string pLevelEnd)
        {
            Id = pId;
            OprId = pOprId;
            LevelInfo = pLevelInfo;
            LevelFlag = pLevelFlag;
            Row = pRow;
            Col = pCol;
            Height = pHeight;
            Page = pPage;
            ColIndex = pColIndex;
            LevelType = pLevelType;
            ItemCount = pItemCount;
            LevelEnd = pLevelEnd;
        }
    }

    [DataContract]
    public class ORG_CONFIG_INFO
    {
        [DataMember]
        public string LEVEL_ID { get; set; }
        [DataMember]
        public string BOX_HEIGHT { get; set; }
        [DataMember]
        public string TEMPLATE_URL { get; set; }
        [DataMember]
        public string VIEW_NAME { get; set; }
        [DataMember]
        public string VIEW_TYPE { get; set; }
        [DataMember]
        public string BOX_FILE { get; set; }
        [DataMember]
        public string VIEW_DEFAULT { get; set; }
        [DataMember]
        public string LINE_COLOR { get; set; }
        [DataMember]
        public string LINE_WIDTH { get; set; }

        public ORG_CONFIG_INFO(string json)
        {
            JToken jUser = JObject.Parse(json.Substring(1, json.Length - 2));
            LEVEL_ID = (string)jUser["LEVEL_ID"];
            BOX_HEIGHT = (string)jUser["BOX_HEIGHT"];
            TEMPLATE_URL = (string)jUser["TEMPLATE_URL"];
            VIEW_NAME = (string)jUser["VIEW_NAME"];
            VIEW_TYPE = (string)jUser["VIEW_TYPE"];
            BOX_FILE = (string)jUser["BOX_FILE"];
            VIEW_DEFAULT = (string)jUser["VIEW_DEFAULT"];
            LINE_COLOR = (string)jUser["LINE_COLOR"];
            LINE_WIDTH = (string)jUser["LINE_WIDTH"];
        }

        public ORG_CONFIG_INFO(string pLEVEL_ID, string pBOX_HEIGHT, string pTEMPLATE_URL,
                               string pVIEW_NAME, string pVIEW_TYPE, string pBOX_FILE,
                               string pVIEW_DEFAULT, string pLINE_COLOR, string pLINE_WIDTH)
        {
            LEVEL_ID = pLEVEL_ID;
            BOX_HEIGHT = pBOX_HEIGHT;
            TEMPLATE_URL = pTEMPLATE_URL;
            VIEW_NAME = pVIEW_NAME;
            VIEW_TYPE = pVIEW_TYPE;
            BOX_FILE = pBOX_FILE;
            VIEW_DEFAULT = pVIEW_DEFAULT;
            LINE_COLOR = pLINE_COLOR;
            LINE_WIDTH = pLINE_WIDTH;
        }
    }

    [DataContract]
    public class LEVEL_CONFIG_INFO
    {
        [DataMember]
        public string ID { get; set; }
        [DataMember]
        public string FIELD_CAPTION { get; set; }
        [DataMember]
        public string FIELD_NAME { get; set; }
        [DataMember]
        public string FIELD_ROW { get; set; }
        [DataMember]
        public string FIELD_ROW_TYPE { get; set; }
        [DataMember]
        public string FIELD_COL { get; set; }
        [DataMember]
        public string FIELD_COL_TYPE { get; set; }
        [DataMember]
        public string WRAP { get; set; }
        [DataMember]
        public string FONT_NAME { get; set; }
        [DataMember]
        public string FONT_SIZE { get; set; }
        [DataMember]
        public string FONT_COLOR { get; set; }
        [DataMember]
        public string FONT_STYLE { get; set; }
        [DataMember]
        public string FONT_FLOAT { get; set; }
        [DataMember]
        public string ACTIVE_IND { get; set; }
        [DataMember]
        public string TABLE_IND { get; set; }
        [DataMember]
        public string FIELD_WIDTH { get; set; }
        [DataMember]
        public string ADJUSTMENT { get; set; }
        [DataMember]
        public string SAMPLE_DATA { get; set; }


        public LEVEL_CONFIG_INFO(string pID, string pFIELD_CAPTION, string pFIELD_NAME,
                                 string pFIELD_ROW, string pFIELD_ROW_TYPE,
                                 string pFIELD_COL, string pFIELD_COL_TYPE,
                                 string pWRAP,
                                 string pFONT_NAME, string pFONT_SIZE, string pFONT_COLOR, string pFONT_STYLE, string pFONT_FLOAT,
                                 string pACTIVE_IND, string pTABLE_IND, string pFIELD_WIDTH, string pADJUSTMENT, string pSAMPLE_DATA)
        {
            ID = pID;
            FIELD_CAPTION = pFIELD_CAPTION;
            FIELD_NAME = pFIELD_NAME;
            FIELD_ROW = pFIELD_ROW;
            FIELD_ROW_TYPE = pFIELD_ROW_TYPE;
            FIELD_COL = pFIELD_COL;
            FIELD_COL_TYPE = pFIELD_COL_TYPE;
            WRAP = pWRAP;
            FONT_NAME = pFONT_NAME;
            FONT_SIZE = pFONT_SIZE;
            FONT_COLOR = pFONT_COLOR;
            FONT_STYLE = pFONT_STYLE;
            FONT_FLOAT = pFONT_FLOAT;
            ACTIVE_IND = pACTIVE_IND;
            TABLE_IND = pTABLE_IND;
            FIELD_WIDTH = pFIELD_WIDTH;
            ADJUSTMENT = pADJUSTMENT;
            SAMPLE_DATA = pSAMPLE_DATA;
        }
    }

    [DataContract]
    public class RECT_OBJ
    {
        [DataMember]
        public int Col { get; set; }
        [DataMember]
        public int Row { get; set; }
        [DataMember]
        public int Width { get; set; }
        [DataMember]
        public int Height { get; set; }
    }

    public class AllLevelPDF
    {
        string OrgConnection = System.Configuration.ConfigurationManager.ConnectionStrings["ORG_CHART"].ConnectionString.ToString();
        string LevelUpto = "", Height = "", TemplateURL = "", strOutput = "HTML", sAllPDF = "N";
        string[] Suppress = new string[10], LevelIds;
        List<int> PageSizeNos = new List<int>();
        int Original_Height = 100, Original_Height_10 = 0, Adjustment_Height = 0, FieldCount = 11, Output_Height = 901;
        int CurPage = 0, MaxPage = 0, PDFPage = 0, iLevel = 0, iTotalPage = 0;

        ceTe.DynamicPDF.Document MyDocument = null;
        ceTe.DynamicPDF.Page[] MyPage = new ceTe.DynamicPDF.Page[100000];
        ceTe.DynamicPDF.Page[] MyAllPage = new ceTe.DynamicPDF.Page[100000];
        ceTe.DynamicPDF.PageElements.Image MyImage = null;
        ceTe.DynamicPDF.PageElements.Rectangle MyRect = null;
        ceTe.DynamicPDF.PageElements.Rectangle MyLabelRect = null;
        ceTe.DynamicPDF.PageElements.Line MyLine = null;
        ceTe.DynamicPDF.PageElements.Label MyLabel = null;
        ceTe.DynamicPDF.PageElements.Circle MyCircle = null;
        ceTe.DynamicPDF.PageElements.Link MyLink = null;

        DataTable dtFieldInformation = null, dtFieldActive = null, dtconf = null;

        ObjectInf PreviousObj = null;
        Bitmap BmpURL = null, BmpUpArrow = null, BmpDnArrow = null;

        List<PageObjectInf> thePageObjectInf = new List<PageObjectInf>();

        List<ObjectInf> lstObjLevel0 = new List<ObjectInf>();
        List<ObjectInf> lstObjLevel1 = new List<ObjectInf>();
        List<ObjectInf> lstObjLevel2 = new List<ObjectInf>();
        List<ObjectInf> lstObjLevel = new List<ObjectInf>();

        // Places the IDs in the List for Creating All Level PDF and PPT
        List<string> lstID = new List<string>();
        List<string> lstLevel = new List<string>();
        List<string> pageLevel = new List<string>();
        List<string> lstPageNo = new List<string>();

        List<ObjectInfPDF> lstObjectInf = new List<ObjectInfPDF>();
        List<ObjectInfPDF> lstObjectPDF = new List<ObjectInfPDF>();

        // Variable used when BRD object created.
        RECT_OBJ RectPPT = new RECT_OBJ();

        // Create font and brush for Connector.
        System.Drawing.Font drawFont = new System.Drawing.Font("Calibri", 12, FontStyle.Regular, GraphicsUnit.Pixel);
        SolidBrush drawBrush = new SolidBrush(System.Drawing.Color.Black);

        // Create font and brush for Label.
        System.Drawing.Font drawFontText = new System.Drawing.Font("Calibri", 12, FontStyle.Regular, GraphicsUnit.Pixel);
        SolidBrush drawBrushText = new SolidBrush(System.Drawing.Color.Black);

        // Create point for upper-left corner of drawing.
        PointF drawPoint = new PointF(150.0F, 150.0F);

        // Black pen to draw line
        Pen blackPen = new Pen(System.Drawing.Color.DarkGray, 1);

        Bitmap[] ImageOut = new Bitmap[100];
        Graphics[] GraphicImg = new Graphics[100];

        Bitmap[] ImagePIC = new Bitmap[10000];
        Graphics[] GraphicPIC = new Graphics[10000];

        public string Level { get; set; }
        public string PreviousLevel { get; set; }
        public string View { get; set; }
        public string FlagFM { get; set; }
        public string Country { get; set; }
        public string LineColor { get; set; }
        public string BoxColor { get; set; }
        public string LineWidth { get; set; }
        public string LevelFlag { get; set; }
        public string LevelDate { get; set; }
        public string Language { get; set; }
        public string PositionTotalCostField { get; set; }
        public string PositionCostField { get; set; }
        public string CompanyName { get; set; }
        public string StartPosition { get; set; }
        public int FinalyzerVersion { get; set; }
        public int LevelCount { get; set; }
        public string TableHTML { get; set; }
        public string JsonFieldWidth { get; set; }
        public string JsonFieldInfo { get; set; }

        string dtHD_DIRECT_REPORT ="_DIRECT_REPORT";
        string dtHD_DIRECT_REPORT_DATA = "DIRECT_REPORT_DATA";
        string dtHD_DIRECT_REPORT_LV = "DIRECT_REPORT_LV";
        string dtHD_DIRECT_REPORT_DATA_LV = "DIRECT_REPORT_DATA_LV";

        string OPR_LEVEL_ID = "LEVEL_ID";
        string OPR_PARENT_ID = "PARENT_LEVEL_ID";
        string LGL_LEVEL_ID = "LEVEL_ID";
        string LGL_PARENT_ID = "PARENT_LEVEL_ID";

        Common csobj = new Common();
        LevelInfo LI = new LevelInfo();

        // Constructor
        public AllLevelPDF()
        {
        }

        public AllLevelPDF(string LI, string PLI, string Vtype, string CountryName, string LvlFlag, string LvlDate, int LvlCount, string Lang, 
                           string TemplateName, string DownloadType, string PositionTCF, string PositionCF, string CN, 
                           string LColor, string BColor)
        {
            Level = LI;
            PreviousLevel = PLI;
            View = Vtype;
            Country = CountryName;
            LevelUpto = LvlFlag;
            LevelDate = LvlDate;
            LevelCount = LvlCount;
            Language = Lang;
            PositionTotalCostField = PositionTCF;
            PositionCostField = PositionCF;
            CompanyName = CN;
            LineColor = LColor;
            BoxColor = BColor;

            HttpContext.Current.Session["ID"] = Level;
            HttpContext.Current.Session["LEVEL_DATE"] = LevelDate;
            if (Vtype == "OV")
            {
                dtconf = csobj.SQLReturnDataTable("SELECT * " +
                                                  " FROM LEVEL_CONFIG_INFO " +
                                                  " WHERE VIEW_ID='VIEW_DEFAULT' AND ACTIVE_IND='Y' AND TEMPLATE_NAME='" + TemplateName + "' AND " +
                                                  "       DOWNLOAD_TYPE='" + DownloadType + "' AND COMPANY_NAME='" + CompanyName + "'");
            }
            else if (Vtype == "LV")
            {
                dtconf = csobj.SQLReturnDataTable("SELECT * " +
                                                  " FROM LEGAL_CONFIG_INFO " +
                                                  " WHERE VIEW_ID='VIEW_DEFAULT' AND ACTIVE_IND='Y' AND TEMPLATE_NAME='" + TemplateName + "' AND " +
                                                  "       DOWNLOAD_TYPE='" + DownloadType + "' AND COMPANY_NAME='" + CompanyName + "'");
            }
        }

        public AllLevelPDF(string LI, string PLI, string Vtype, string CountryName, string LvlFlag, string LvlDate, 
                           int LvlCount, string Lang, string TemplateName, string DownloadType, string PositionTCF, 
                           string PositionCF, string CN, string SP, int FV)
        {
            Level = LI;
            PreviousLevel = PLI;
            View = Vtype;
            Country = CountryName;
            LevelUpto = LvlFlag;
            LevelDate = LvlDate;
            LevelCount = LvlCount;
            Language = Lang;
            PositionTotalCostField = PositionTCF;
            PositionCostField = PositionCF;
            CompanyName = CN;
            StartPosition = SP;
            FinalyzerVersion = FV;

            dtHD_DIRECT_REPORT = CompanyName.ToUpper() + "_" + FV.ToString() + "_DIRECT_REPORT";
            dtHD_DIRECT_REPORT_DATA = CompanyName.ToUpper() + "_" + FV.ToString() + "_DIRECT_REPORT_DATA";
            dtHD_DIRECT_REPORT_LV = CompanyName.ToUpper() + "_" + FV.ToString() + "_DIRECT_REPORT_LV";
            dtHD_DIRECT_REPORT_DATA_LV = CompanyName.ToUpper() + "_" + FV.ToString() + "_DIRECT_REPORT_DATA_LV";


            HttpContext.Current.Session["ID"] = Level;
            HttpContext.Current.Session["LEVEL_DATE"] = LevelDate;
            if (Vtype == "OV")
            {
                dtconf = csobj.SQLReturnDataTable("SELECT * " +
                                                  " FROM LEVEL_CONFIG_INFO " +
                                                  " WHERE VIEW_ID='VIEW_DEFAULT' AND ACTIVE_IND='Y' AND TEMPLATE_NAME='" + TemplateName + "' AND " +
                                                  "       DOWNLOAD_TYPE='" + DownloadType + "' AND COMPANY_NAME='" + CompanyName + "'");
            }
            else if (Vtype == "LV")
            {
                dtconf = csobj.SQLReturnDataTable("SELECT * " +
                                                  " FROM LEGAL_CONFIG_INFO " +
                                                  " WHERE VIEW_ID='VIEW_DEFAULT' AND ACTIVE_IND='Y' AND TEMPLATE_NAME='" + TemplateName + "' AND " +
                                                  "       DOWNLOAD_TYPE='" + DownloadType + "' AND COMPANY_NAME='" + CompanyName + "'");
            }
        }

        private System.Drawing.Color GetDrawingLineColor(string LineColor)
        {
            if (LineColor == "#D3D3D3") return System.Drawing.Color.LightGray;
            else if (LineColor == "#DCDCDC") return System.Drawing.Color.Gainsboro;
            else if (LineColor == "#C0C0C0") return System.Drawing.Color.Silver;
            else if (LineColor == "#A9A9A9") return System.Drawing.Color.DarkGray;
            else if (LineColor == "#808080") return System.Drawing.Color.Gray;
            else if (LineColor == "#696969") return System.Drawing.Color.DimGray;
            else if (LineColor == "#778899") return System.Drawing.Color.LightSlateGray;
            else if (LineColor == "#708090") return System.Drawing.Color.SlateGray;
            else if (LineColor == "#2F4F4F") return System.Drawing.Color.DarkGray;
            else if (LineColor == "#000000") return System.Drawing.Color.Black;

            return System.Drawing.Color.LightGray;
        }

        private ceTe.DynamicPDF.Color GetPDFLineColor(string LineColor)
        {
            if (LineColor == "#D3D3D3") return ceTe.DynamicPDF.RgbColor.LightGrey;
            else if (LineColor == "#DCDCDC") return ceTe.DynamicPDF.RgbColor.Gainsboro;
            else if (LineColor == "#C0C0C0") return ceTe.DynamicPDF.RgbColor.Silver;
            else if (LineColor == "#A9A9A9") return ceTe.DynamicPDF.RgbColor.DarkGray;
            else if (LineColor == "#808080") return ceTe.DynamicPDF.RgbColor.Gray;
            else if (LineColor == "#696969") return ceTe.DynamicPDF.RgbColor.DimGray;
            else if (LineColor == "#778899") return ceTe.DynamicPDF.RgbColor.LightSlateGray;
            else if (LineColor == "#708090") return ceTe.DynamicPDF.RgbColor.SlateGray;
            else if (LineColor == "#2F4F4F") return ceTe.DynamicPDF.RgbColor.DarkGray;
            else if (LineColor == "#000000") return ceTe.DynamicPDF.RgbColor.Black;

            return ceTe.DynamicPDF.RgbColor.LightGrey;
        }

        //Set the Page number
        private void SetPositionPageNo(string sID, int PageNo)
        {
            if (thePageObjectInf.Count >= 1)
            {
                foreach (PageObjectInf PObj in thePageObjectInf)
                {
                    if (PObj.Id == sID) PObj.PageNo = PageNo;
                }
            }
        }

        // Gets row page
        private int GetRowPage(int Row)
        {
            int StartPage = 0, EndPage = Output_Height - 2, RowPage = 0;

            RowPage = 0;
            for (int Idx = 0; Idx <= PageSizeNos.Count - 1; Idx++)
            {
                if (PageSizeNos.Count >= 2)
                {
                    StartPage = PageSizeNos[Idx];
                    if ((Idx + 1) != PageSizeNos.Count) EndPage = PageSizeNos[Idx + 1] - 1; else EndPage = PageSizeNos[Idx] + Output_Height - 2;
                }
                if ((Row >= StartPage) && (Row <= EndPage)) RowPage = Idx;
            }

            return RowPage;
        }

        // Get the Page existence 
        private bool GetPageExistence(int Row)
        {
            int StartPage = 0, EndPage = Output_Height - 2;

            for (int Idx = 0; Idx <= PageSizeNos.Count - 1; Idx++)
            {
                if (PageSizeNos.Count >= 2)
                {
                    StartPage = PageSizeNos[Idx];
                    if ((Idx + 1) != PageSizeNos.Count) EndPage = PageSizeNos[Idx + 1] - 1; else EndPage = PageSizeNos[Idx] + Output_Height - 2;
                }
                if ((Row >= StartPage) && (Row <= EndPage)) return true;
            }

            return false;
        }

        // Gets the page into which object is to be drawn
        private int GetPageRowPosition(int Row)
        {
            int StartPage = 0, EndPage = Output_Height - 2;

            CurPage = PDFPage;
            for (int Idx = 0; Idx <= PageSizeNos.Count - 1; Idx++)
            {
                if (PageSizeNos.Count >= 2)
                {
                    StartPage = PageSizeNos[Idx];
                    if ((Idx + 1) != PageSizeNos.Count) EndPage = PageSizeNos[Idx + 1] - 1; else EndPage = PageSizeNos[Idx] + Output_Height - 2;
                }
                if ((Row >= StartPage) && (Row <= EndPage)) CurPage = Idx;
            }

            return Row - PageSizeNos[CurPage];
        }

        // Gets the page into which object is to be drawn
        private int GetPageRowStartPosition(int Row)
        {
            int StartPage = 0, EndPage = Output_Height - 2;

            for (int Idx = 0; Idx <= PageSizeNos.Count - 1; Idx++)
            {
                if (PageSizeNos.Count >= 2)
                {
                    StartPage = PageSizeNos[Idx];
                    if ((Idx + 1) != PageSizeNos.Count) EndPage = PageSizeNos[Idx + 1] - 1; else EndPage = PageSizeNos[Idx] + Output_Height - 2;
                }
                if ((Row >= StartPage) && (Row <= EndPage)) return PageSizeNos[Idx];
            }

            return PageSizeNos[0];
        }

        // DrawLine X1, Y1, X2, Y2
        private void DrawLine(string Connector, int X1, int Y1, int X2, int Y2)
        {
            int StartPage = GetRowPage(Y1), EndPage = GetRowPage(Y2), LWidth = Convert.ToInt16(LineWidth);
            if (StartPage != EndPage)
            {
                if (Connector.Substring(0, 6) != "Middle")
                {
                    for (int Idx = StartPage; Idx <= EndPage; Idx++)
                    {
                        if (Idx == StartPage)
                        {
                            if (strOutput == "PDF")
                            {
                                MyLine = new ceTe.DynamicPDF.PageElements.Line(X1, GetPageRowPosition(Y1) + 15, X2, Output_Height - 1, GetPDFLineColor(LineColor));
                                MyLine.Width = LWidth;
                                MyPage[Idx].Elements.Add(MyLine);
                                MyRect = new ceTe.DynamicPDF.PageElements.Rectangle(X1 - 20, Output_Height - 1, 50, 12);
                                MyPage[Idx].Elements.Add(MyRect);
                                MyLabel = new ceTe.DynamicPDF.PageElements.Label(Connector, X1 - 20, Output_Height + 1, 48, 8, ceTe.DynamicPDF.Font.Helvetica, 7, ceTe.DynamicPDF.TextAlign.Center);
                                MyPage[Idx].Elements.Add(MyLabel);
                            }
                            else if (strOutput == "PPT")
                            {
                                GraphicImg[Idx].DrawLine(blackPen, X1, GetPageRowPosition(Y1) + 15, X2, Output_Height - 1);
                                GraphicImg[Idx].DrawRectangle(blackPen, X1 - 20, Output_Height - 1, 65, 17);
                                GraphicImg[Idx].DrawString(Connector, drawFont, drawBrush, X1 - 20, Output_Height + 1);
                            }
                        }
                        else if (Idx == EndPage)
                        {
                            if (strOutput == "PDF")
                            {
                                MyLine = new ceTe.DynamicPDF.PageElements.Line(X1, 12, X2, GetPageRowPosition(Y2) + 15, GetPDFLineColor(LineColor));
                                MyLine.Width = LWidth;
                                MyPage[Idx].Elements.Add(MyLine);
                                MyRect = new ceTe.DynamicPDF.PageElements.Rectangle(X1 - 20, 0, 50, 12);
                                MyPage[Idx].Elements.Add(MyRect);
                                MyLabel = new ceTe.DynamicPDF.PageElements.Label(Connector, X1 - 18, 2, 48, 8, ceTe.DynamicPDF.Font.Helvetica, 7, ceTe.DynamicPDF.TextAlign.Center);
                                MyPage[Idx].Elements.Add(MyLabel);
                            }
                            else if (strOutput == "PPT")
                            {
                                GraphicImg[Idx].DrawLine(blackPen, X1, 12, X2, GetPageRowPosition(Y2) + 15);
                                GraphicImg[Idx].DrawRectangle(blackPen, X1 - 20, 0, 65, 17);
                                GraphicImg[Idx].DrawString(Connector, drawFont, drawBrush, X1 - 18, 2);
                            }
                        }
                        else
                        {
                            if (strOutput == "PDF")
                            {
                                MyLine = new ceTe.DynamicPDF.PageElements.Line(X1, 12, X2, Output_Height - 1, GetPDFLineColor(LineColor));
                                MyLine.Width = LWidth;
                                MyPage[Idx].Elements.Add(MyLine);

                                MyRect = new ceTe.DynamicPDF.PageElements.Rectangle(X1 - 20, 0, 50, 12);
                                MyPage[Idx].Elements.Add(MyRect);
                                MyLabel = new ceTe.DynamicPDF.PageElements.Label(Connector, X1 - 18, 2, 48, 8, ceTe.DynamicPDF.Font.Helvetica, 7, ceTe.DynamicPDF.TextAlign.Center);
                                MyPage[Idx].Elements.Add(MyLabel);

                                MyRect = new ceTe.DynamicPDF.PageElements.Rectangle(X1 - 20, Output_Height - 1, 50, 12);
                                MyPage[Idx].Elements.Add(MyRect);
                                MyLabel = new ceTe.DynamicPDF.PageElements.Label(Connector, X1 - 20, Output_Height + 1, 48, 8, ceTe.DynamicPDF.Font.Helvetica, 7, ceTe.DynamicPDF.TextAlign.Center);
                                MyPage[Idx].Elements.Add(MyLabel);
                            }
                            else if (strOutput == "PPT")
                            {
                                GraphicImg[Idx].DrawLine(blackPen, X1, 12, X2, Output_Height - 1);

                                GraphicImg[Idx].DrawRectangle(blackPen, X1 - 20, 0, 65, 17);
                                GraphicImg[Idx].DrawString(Connector, drawFont, drawBrush, X1 - 18, 2);

                                GraphicImg[Idx].DrawRectangle(blackPen, X1 - 20, Output_Height - 1, 65, 17);
                                GraphicImg[Idx].DrawString(Connector, drawFont, drawBrush, X1 - 20, Output_Height + 1);
                            }
                        }
                    }
                }
                else
                {
                    for (int Idx = StartPage; Idx <= EndPage; Idx++)
                    {
                        if (Idx == StartPage)
                        {
                            if (strOutput == "PDF")
                            {
                                MyLine = new ceTe.DynamicPDF.PageElements.Line(X1, GetPageRowPosition(Y1), X2, Output_Height - 36, GetPDFLineColor(LineColor));
                                MyLine.Width = LWidth;
                                MyPage[Idx].Elements.Add(MyLine);
                                MyCircle = new ceTe.DynamicPDF.PageElements.Circle(X1, Output_Height - 26, 8);
                                MyPage[Idx].Elements.Add(MyCircle);
                                MyLabel = new ceTe.DynamicPDF.PageElements.Label("M", X1 - 25, Output_Height - 31, 48, 8, ceTe.DynamicPDF.Font.Helvetica, 7, ceTe.DynamicPDF.TextAlign.Center);
                                MyPage[Idx].Elements.Add(MyLabel);
                            }
                            else if (strOutput == "PPT")
                            {
                                GraphicImg[Idx].DrawLine(blackPen, X1, GetPageRowPosition(Y1), X2, Output_Height - 36);
                                GraphicImg[Idx].DrawEllipse(blackPen, X1 - 8, Output_Height - 36, 16, 16);
                                GraphicImg[Idx].DrawString("M", drawFont, drawBrush, X1 - 6, Output_Height - 33);
                            }
                        }
                        else if (Idx == EndPage)
                        {
                            if (Connector.Substring(7, 1) == "0")
                            {
                                if (strOutput == "PDF")
                                {
                                    MyCircle = new ceTe.DynamicPDF.PageElements.Circle(X1, 25, 8);
                                    MyPage[Idx].Elements.Add(MyCircle);
                                    MyLabel = new ceTe.DynamicPDF.PageElements.Label("M", X1 - 25, 20, 48, 8, ceTe.DynamicPDF.Font.Helvetica, 7, ceTe.DynamicPDF.TextAlign.Center);
                                    MyPage[Idx].Elements.Add(MyLabel);
                                    MyLine = new ceTe.DynamicPDF.PageElements.Line(X1, 35, X2, GetPageRowPosition(Y2), GetPDFLineColor(LineColor));
                                    MyLine.Width = LWidth;
                                    MyPage[Idx].Elements.Add(MyLine);
                                }
                                else if (strOutput == "PPT")
                                {
                                    GraphicImg[Idx].DrawEllipse(blackPen, X1 - 8, 20, 16, 16);
                                    GraphicImg[Idx].DrawString("M", drawFont, drawBrush, X1 - 6, 22);
                                    GraphicImg[Idx].DrawLine(blackPen, X1, 35, X2, GetPageRowPosition(Y2));
                                }
                            }
                            else
                            {
                                if (strOutput == "PDF")
                                {
                                    MyCircle = new ceTe.DynamicPDF.PageElements.Circle(X1, 5, 8);
                                    MyPage[Idx].Elements.Add(MyCircle);
                                    MyLabel = new ceTe.DynamicPDF.PageElements.Label("M", X1 - 25, 0, 48, 8, ceTe.DynamicPDF.Font.Helvetica, 7, ceTe.DynamicPDF.TextAlign.Center);
                                    MyPage[Idx].Elements.Add(MyLabel);
                                }
                                else if (strOutput == "PPT")
                                {
                                    GraphicImg[Idx].DrawEllipse(blackPen, X1 - 8, 13, 16, 16);
                                    GraphicImg[Idx].DrawString("M", drawFont, drawBrush, X1 - 6, 0);
                                }
                            }
                        }
                        else
                        {
                            if (strOutput == "PDF")
                            {
                                MyCircle = new ceTe.DynamicPDF.PageElements.Circle(X1, 25, 8);
                                MyPage[Idx].Elements.Add(MyCircle);

                                MyLabel = new ceTe.DynamicPDF.PageElements.Label("M", X1 - 25, 20, 48, 8, ceTe.DynamicPDF.Font.Helvetica, 7, ceTe.DynamicPDF.TextAlign.Center);
                                MyPage[Idx].Elements.Add(MyLabel);
                                MyLine = new ceTe.DynamicPDF.PageElements.Line(X1, 35, X2, Output_Height - 21, GetPDFLineColor(LineColor));
                                MyLine.Width = LWidth;
                                MyPage[Idx].Elements.Add(MyLine);

                                MyCircle = new ceTe.DynamicPDF.PageElements.Circle(X1, Output_Height - 11, 8);
                                MyPage[Idx].Elements.Add(MyCircle);
                                MyLabel = new ceTe.DynamicPDF.PageElements.Label("M", X1 - 25, Output_Height - 16, 48, 8, ceTe.DynamicPDF.Font.Helvetica, 7, ceTe.DynamicPDF.TextAlign.Center);
                                MyPage[Idx].Elements.Add(MyLabel);
                            }
                            else if (strOutput == "PPT")
                            {
                                GraphicImg[Idx].DrawEllipse(blackPen, X1 - 8, 20, 16, 16);

                                GraphicImg[Idx].DrawString("M", drawFont, drawBrush, X1 - 6, 22);
                                GraphicImg[Idx].DrawLine(blackPen, X1, 35, X2, Output_Height - 21);

                                GraphicImg[Idx].DrawEllipse(blackPen, X1 - 8, Output_Height - 20, 16, 16);
                                GraphicImg[Idx].DrawString("M", drawFont, drawBrush, X1 - 6, Output_Height - 18);
                            }
                        }
                    }
                }

                return;
            }
            else
            {
                if (strOutput == "PDF")
                {
                    MyLine = new ceTe.DynamicPDF.PageElements.Line(X1, GetPageRowPosition(Y1) + 15, X2, GetPageRowPosition(Y2) + 15, GetPDFLineColor(LineColor));
                    MyLine.Width = LWidth;
                    MyPage[CurPage].Elements.Add(MyLine);
                }
                else if (strOutput == "PPT")
                {
                    int Y1Inf = GetPageRowPosition(Y1) + 15, Y2Inf = GetPageRowPosition(Y2) + 15;
                    GraphicImg[CurPage].DrawLine(blackPen, X1, Y1Inf, X2, Y2Inf);
                }
            }
        }

        // Gets the ancestor Page No. 
        private string GetAncestorPageNo(string sID)
        {
            for (int Idx = 0; Idx <= lstID.Count - 1; Idx++)
            {
                if (lstID[Idx] == sID) return lstPageNo[Idx].ToString();
            }

            return "-1";
        }

        private ObjectInf GetIdInfo(List<ObjectInf> theObjectInf, string Id)
        {
            foreach (ObjectInf Obj in theObjectInf)
            {
                if (Obj.Id == Id) return Obj;
            }
            return null;
        }

        // Check for level existence
        private string CheckLevelInfo(ObjectInf Obj)
        {
            if (Obj.NextLevelFlag == "Y") return Obj.Id;

            return "NO";
        }

        private int NoOfLevel(List<ObjectInf> theObjectInf, string LId)
        {

            int Idx = 0;
            foreach (ObjectInf Obj in theObjectInf)
            {
                if (Obj.Level == LId) Idx++;
            }

            return Idx;
        }

        private string GetDistinctLevel(string ParentId, string Level)
        {
            string[] PId = ParentId.Split(',');

            if (PId.Length == 0) return Level;
            for (int Idx = 0; Idx <= PId.Length - 1; Idx++)
            {
                if (PId[Idx] == Level) return "";
            }

            return Level;
        }

        private string RemoveCommasInLevel(string ParentId)
        {
            string[] PId = ParentId.Split(',');
            string sIds = "";

            if (PId.Length == 0) return "";
            for (int Idx = 0; Idx <= PId.Length - 1; Idx++)
            {
                if (PId[Idx].Length >= 1) sIds += "," + PId[Idx];
            }

            return sIds.Substring(1);
        }

        private string IdInPId(List<ObjectInf> theObjectInf, string Id)
        {

            string ParentId = "";
            foreach (ObjectInf Obj in theObjectInf)
            {
                if (Obj.PId == Id) ParentId += "," + Obj.Id;
            }
            if (ParentId == "") return "";

            return RemoveCommasInLevel(ParentId.Substring(1));
        }

        private string LevelInId(List<ObjectInf> theObjectInf, string LevelId, int Level)
        {
            string ParentId = "";
            foreach (ObjectInf Obj in theObjectInf)
            {
                if (Obj.Level == LevelId)
                {
                    ParentId += "," + Obj.Id;
                    switch (Level)
                    {
                        case 0:
                            lstObjLevel0.Add(Obj);
                            break;
                        case 1:
                            lstObjLevel1.Add(Obj);
                            break;
                        case 2:
                            lstObjLevel2.Add(Obj);
                            break;
                    }
                }
            }
            if (ParentId == "") return "";

            return RemoveCommasInLevel(ParentId.Substring(1));
        }

        private string GetAllLevels(List<ObjectInf> theObjectInf)
        {
            string ParentId = "";
            foreach (ObjectInf Obj in theObjectInf)
            {
                ParentId += "," + GetDistinctLevel(ParentId, Obj.Level);
            }
            if (ParentId == "") return "";

            return RemoveCommasInLevel(ParentId.Substring(1));
        }

        // Gets the Child height information
        private int GetChildHeight(List<ObjectInf> theObjectInf, string Id)
        {
            int Height = Original_Height_10;
            foreach (ObjectInf Obj in theObjectInf)
            {
                if (Obj.PId == Id) Height += Obj.Height;
            }

            return Height;
        }

        // DrawRectangle X1, Y1, W, H
        private void DrawRectangle(int X1, int Y1, int W, int H)
        {
            int Row = Y1 + H;
            if (Row >= Output_Height)
            {
                if (!(GetPageExistence(Row)))
                {
                    PageSizeNos.Add(Y1);
                    if (strOutput == "PDF")
                    {
                        //MyPage[PageSizeNos.Count - 1] = new ceTe.DynamicPDF.Page(PageSize.B4, PageOrientation.Landscape, 15.0F);
                        if (LevelCount == 4)
                            MyPage[PageSizeNos.Count - 1] = new ceTe.DynamicPDF.Page(1080F, 1050F, 15.3F);
                        else
                            MyPage[PageSizeNos.Count - 1] = new ceTe.DynamicPDF.Page(1600F, 1050F, 15.3F);

                        //MyPage[PageSizeNos.Count - 1] = new ceTe.DynamicPDF.Page(1050F, 768F, 15.3F);
                    }
                    else if (strOutput == "PPT")
                    {

                        if (LevelCount == 4)
                            ImageOut[PageSizeNos.Count - 1] = new Bitmap(1024, Output_Height + 30);
                        else
                            ImageOut[PageSizeNos.Count - 1] = new Bitmap(1600, Output_Height + 30);

                        // Set DPI of image (xDpi, yDpi)
                        ImageOut[PageSizeNos.Count - 1].SetResolution(256.0F, 256.0F);

                        GraphicImg[PageSizeNos.Count - 1] = Graphics.FromImage(ImageOut[PageSizeNos.Count - 1]);
                        GraphicImg[PageSizeNos.Count - 1].Clear(System.Drawing.Color.White);
                    }
                    MaxPage = PageSizeNos.Count;
                }
                if (strOutput == "PDF")
                {
                    MyRect = new ceTe.DynamicPDF.PageElements.Rectangle(X1, GetPageRowPosition(Y1) + 15, W, H);
                    MyPage[CurPage].Elements.Add(MyRect);
                }
                else if (strOutput == "PPT")
                {
                    int Y1Inf = GetPageRowPosition(Y1) + 15;
                    GraphicImg[CurPage].DrawRectangle(blackPen, X1, Y1Inf, W, H);
                }

                return;
            }
            CurPage = PDFPage;
            if (strOutput == "PDF")
            {
                MyRect = new ceTe.DynamicPDF.PageElements.Rectangle(X1, Y1 + 15, W, H);
                MyPage[CurPage].Elements.Add(MyRect);
            }
            else if (strOutput == "PPT")
            {
                int Y1Inf = GetPageRowPosition(Y1) + 15;
                GraphicImg[CurPage].DrawRectangle(blackPen, X1, Y1Inf, W, H);
            }
        }

        // Draws the Div PPT Image
        private void DrawDivPPTImage(string sFP, int iPage, int X, int Y, int W, int H, ObjectInf Obj)
        {
            Pen Pen = null;
            string[] sParam = sFP.Split('|');
            if (Obj.ShowFullBox == "N")
            {
                if (Obj.Level == "1")
                {
                    int H1 = H - Convert.ToInt16(sParam[2]);
                    H = Convert.ToInt16(sParam[2]);
                    if (LevelFlag == "2")
                        GraphicImg[CurPage].DrawLine(blackPen, X + 10, Y + H, X + 10, Y + H + H1 + 1);
                }
                else if ((Obj.Level == "2") || (Obj.Level == "0"))
                {
                    Y = Y + (H - Convert.ToInt16(sParam[2]));
                    H = Convert.ToInt16(sParam[2]);
                }
            }
            if (Obj.ColorFlag != "0") sParam[0] = Obj.ColorFlag;
            switch (sParam[0].ToLower())
            {
                case "#663300":
                    Pen = new Pen(System.Drawing.Color.Brown, Convert.ToInt16(sParam[4]));
                    break;
                case "#ffb266":
                    Pen = new Pen(System.Drawing.Color.Orange, Convert.ToInt16(sParam[4]));
                    break;
                case "#ffffff":
                    Pen = new Pen(System.Drawing.Color.White, Convert.ToInt16(sParam[4]));
                    break;
                case "#0000ff":
                    Pen = new Pen(System.Drawing.Color.Blue, Convert.ToInt16(sParam[4]));
                    break;
                case "#008000":
                    Pen = new Pen(System.Drawing.Color.Green, Convert.ToInt16(sParam[4]));
                    break;
                case "#911414":
                    Pen = new Pen(System.Drawing.Color.DarkRed, Convert.ToInt16(sParam[4]));
                    break;
                case "#ff0000":
                    Pen = new Pen(System.Drawing.Color.Red, Convert.ToInt16(sParam[4]));
                    break;
                case "#000000":
                    Pen = new Pen(System.Drawing.Color.Black, Convert.ToInt16(sParam[4]));
                    break;
            }
            if (Obj.BackColor.ToLower() == "#ffb266")
            {
                Brush brush = new SolidBrush(System.Drawing.Color.Orange);
                GraphicImg[iPage].FillRectangle(brush, X, Y, W, H);
            }
            else
            {
                if (Obj.GrayColourFlag == "Y")
                {
                    Brush brush = new SolidBrush(System.Drawing.Color.LightGray);
                    GraphicImg[iPage].FillRectangle(brush, X, Y, W, H);
                }
                if (Obj.DottedLineFlag == "Y")
                {
                    Pen.DashStyle = DashStyle.Dash;
                    GraphicImg[iPage].DrawRectangle(Pen, X, Y, W, H);
                }
                else
                {
                    Pen.DashStyle = DashStyle.Solid;
                    GraphicImg[iPage].DrawRectangle(Pen, X, Y, W, H);
                }
            }

            // Rectangle information
            RectPPT.Col = X;
            RectPPT.Row = Y;
            RectPPT.Width = W;
            RectPPT.Height = H;

            if (Obj.ShowFullBox == "Y")
            {
                int iCol = Convert.ToInt32(sParam[1]) + X;
                int iRow = Convert.ToInt32(sParam[2]) + Y;
                switch (sParam[3].ToLower())
                {
                    case "#663300":
                        Pen = new Pen(System.Drawing.Color.Brown, Convert.ToInt16(sParam[5]));
                        break;
                    case "#ffb266":
                        Pen = new Pen(System.Drawing.Color.Orange, Convert.ToInt16(sParam[5]));
                        break;
                    case "#ffffff":
                        Pen = new Pen(System.Drawing.Color.White, Convert.ToInt16(sParam[5]));
                        break;
                    case "#0000ff":
                        Pen = new Pen(System.Drawing.Color.Blue, Convert.ToInt16(sParam[5]));
                        break;
                    case "#008000":
                        Pen = new Pen(System.Drawing.Color.Green, Convert.ToInt16(sParam[5]));
                        break;
                    case "#911414":
                        Pen = new Pen(System.Drawing.Color.DarkRed, Convert.ToInt16(sParam[5]));
                        break;
                    case "#ff0000":
                        Pen = new Pen(System.Drawing.Color.Red, Convert.ToInt16(sParam[5]));
                        break;
                    case "#000000":
                        Pen = new Pen(System.Drawing.Color.Black, Convert.ToInt16(sParam[5]));
                        break;
                }
                Pen.DashStyle = DashStyle.Solid;
                if (sParam[6] == "Y") GraphicImg[iPage].DrawRectangle(Pen, iCol, iRow, W, 1);
            }
        }

        // Draws the Div PDF Image
        private void DrawDivPDFImage(string sFP, int iPage, int X, int Y, int W, int H, ObjectInf Obj)
        {
            ceTe.DynamicPDF.RgbColor Pen = null;
            string[] sParam = sFP.Split('|');
            if (Obj.ShowFullBox == "N")
            {
                if (Obj.Level == "1")
                {
                    int H1 = H - Convert.ToInt16(sParam[2]);
                    H = Convert.ToInt16(sParam[2]);
                    if (LevelFlag == "2")
                    {
                        MyLine = new ceTe.DynamicPDF.PageElements.Line(X + 10, Y + H, X + 10, Y + H + H1 + 1, GetPDFLineColor(LineColor));
                        MyLine.Width = Convert.ToInt16(LineWidth);
                        MyPage[iPage].Elements.Add(MyLine);
                    }
                }
                else if ((Obj.Level == "2") || (Obj.Level == "0"))
                {
                    Y = Y + (H - Convert.ToInt16(sParam[2]));
                    H = Convert.ToInt16(sParam[2]);
                }
            }
            if (Obj.ColorFlag != "0") sParam[0] = Obj.ColorFlag;
            switch (sParam[0].ToLower())
            {
                case "#663300":
                    Pen = ceTe.DynamicPDF.WebColor.Brown;
                    break;
                case "#ffb266":
                    Pen = ceTe.DynamicPDF.WebColor.Orange;
                    break;
                case "#ffffff":
                    Pen = ceTe.DynamicPDF.WebColor.White;
                    break;
                case "#0000ff":
                    Pen = ceTe.DynamicPDF.WebColor.Blue;
                    break;
                case "#008000":
                    Pen = ceTe.DynamicPDF.WebColor.Green;
                    break;
                case "#911414":
                    Pen = ceTe.DynamicPDF.WebColor.DarkRed;
                    break;
                case "#ff0000":
                    Pen = ceTe.DynamicPDF.WebColor.Red;
                    break;
                case "#000000":
                    Pen = ceTe.DynamicPDF.WebColor.Black;
                    break;
            }
            MyRect = new ceTe.DynamicPDF.PageElements.Rectangle(X, Y, W, H);
            MyRect.BorderColor = Pen;
            MyRect.BorderWidth = Convert.ToInt16(sParam[4]);
            if (Obj.GrayColourFlag == "Y") MyRect.FillColor = ceTe.DynamicPDF.WebColor.Gray;
            if (Obj.BackColor.ToLower() == "#ffb266") MyRect.FillColor = ceTe.DynamicPDF.WebColor.Orange;
            if (Obj.DottedLineFlag == "Y") MyRect.BorderStyle = LineStyle.Dash;
            MyPage[iPage].Elements.Add(MyRect);
            MyLabelRect = MyRect;
            if (Obj.ShowFullBox == "Y")
            {
                int iCol = Convert.ToInt32(sParam[1]) + X;
                int iRow = Convert.ToInt32(sParam[2]) + Y;
                switch (sParam[3].ToLower())
                {
                    case "#663300":
                        Pen = ceTe.DynamicPDF.WebColor.Brown;
                        break;
                    case "#ffb266":
                        Pen = ceTe.DynamicPDF.WebColor.Orange;
                        break;
                    case "#ffffff":
                        Pen = ceTe.DynamicPDF.WebColor.White;
                        break;
                    case "#0000ff":
                        Pen = ceTe.DynamicPDF.WebColor.Blue;
                        break;
                    case "#008000":
                        Pen = ceTe.DynamicPDF.WebColor.Green;
                        break;
                    case "#911414":
                        Pen = ceTe.DynamicPDF.WebColor.DarkRed;
                        break;
                    case "#ff0000":
                        Pen = ceTe.DynamicPDF.WebColor.Red;
                        break;
                    case "#000000":
                        Pen = ceTe.DynamicPDF.WebColor.Black;
                        break;
                }
                if (sParam[6] == "Y")
                {
                    MyLine = new ceTe.DynamicPDF.PageElements.Line(iCol, iRow, iCol + W, iRow);
                    MyLine.Color = Pen;
                    MyLine.Width = Convert.ToInt16(sParam[5]);
                    MyPage[iPage].Elements.Add(MyLine);
                }
            }
        }

        // DrawImage X1, Y1, W, H
        private void DrawImage(string sFilePath, int X1, int Y1, int W, int H, string Flag, ObjectInf Obj)
        {
            string[] sFP = sFilePath.Split('~');

            int Row = Y1 + H + 20;
            if (Row >= Output_Height)
            {
                if (!(GetPageExistence(Row)))
                {
                    if (PreviousObj != null)
                    {
                        if (PreviousObj.Level == "1")
                        {
                            if (!(GetPageExistence(PreviousObj.Row + PreviousObj.Oheight + 20)))
                                PageSizeNos.Add(PreviousObj.Row - 60);
                            else
                                PageSizeNos.Add(Y1 - 20);
                        }
                        else PageSizeNos.Add(Y1 - 20);
                    }
                    else PageSizeNos.Add(Y1 - 20);
                    if (strOutput == "PDF")
                    {
                        //MyPage[PageSizeNos.Count - 1] = new ceTe.DynamicPDF.Page(PageSize.B4, PageOrientation.Landscape, 15.0F);
                        if (LevelCount == 4)
                            MyPage[PageSizeNos.Count - 1] = new ceTe.DynamicPDF.Page(1080F, 1050F, 15.3F);
                        else
                            MyPage[PageSizeNos.Count - 1] = new ceTe.DynamicPDF.Page(1600F, 1050F, 15.3F);

                        //MyPage[PageSizeNos.Count - 1] = new ceTe.DynamicPDF.Page(1050F, 768F, 15.3F);
                        MaxPage = PageSizeNos.Count;
                    }
                    else if (strOutput == "PPT")
                    {
                        if (LevelCount == 4)
                            ImageOut[PageSizeNos.Count - 1] = new Bitmap(1024, Output_Height + 30);
                        else
                            ImageOut[PageSizeNos.Count - 1] = new Bitmap(1600, Output_Height + 30);

                        // Set DPI of image (xDpi, yDpi)
                        ImageOut[PageSizeNos.Count - 1].SetResolution(256.0F, 256.0F);

                        GraphicImg[PageSizeNos.Count - 1] = Graphics.FromImage(ImageOut[PageSizeNos.Count - 1]);
                        GraphicImg[PageSizeNos.Count - 1].Clear(System.Drawing.Color.White);
                        MaxPage = PageSizeNos.Count;
                    }
                }
                if (strOutput == "PDF")
                {
                    if (sFP[0] == "BRD")
                    {
                        int Y1Inf = GetPageRowPosition(Y1) + 15;
                        DrawDivPDFImage(sFP[1], CurPage, X1, Y1Inf, W - 1, H - 1, Obj);

                        if (Flag == "Y")
                        {
                            thePageObjectInf.Add(new PageObjectInf(Obj.Id,
                                                                   Convert.ToInt32(MyLabelRect.X + (MyLabelRect.Width / 2) - 21),
                                                                   Convert.ToInt32(MyLabelRect.Y + 2 + MyLabelRect.Height),
                                                                   CurPage + iTotalPage,
                                                                   0));

                            MyImage = new ceTe.DynamicPDF.PageElements.Image(System.Web.HttpContext.Current.Server.MapPath("~/Content/images/downarrow.jpg").ToString(),
                                                                             MyLabelRect.X + (MyLabelRect.Width / 2) - 21,
                                                                             MyLabelRect.Y + 2 + MyLabelRect.Height);
                            MyImage.Height = 14;
                            MyImage.Width = 42;
                            MyPage[CurPage].Elements.Add(MyImage);
                        }
                        else if (Flag == "U")
                        {
                            MyImage = new ceTe.DynamicPDF.PageElements.Image(System.Web.HttpContext.Current.Server.MapPath("~/Content/images/uparrow.jpg").ToString(),
                                                                             MyLabelRect.X + (MyLabelRect.Width / 2) - 21,
                                                                             MyLabelRect.Y - 19);
                            MyImage.Height = 14;
                            MyImage.Width = 42;
                            MyPage[CurPage].Elements.Add(MyImage);
                        }

                    }
                    else
                    {
                        MyImage = new ceTe.DynamicPDF.PageElements.Image(System.Web.HttpContext.Current.Server.MapPath(sFP[1]).ToString(), X1, GetPageRowPosition(Y1) + 15);
                        MyImage.Height = H - 1;
                        MyImage.Width = W - 1;
                        MyPage[CurPage].Elements.Add(MyImage);

                        if (Flag == "Y")
                        {
                            int YInf = GetPageRowPosition(Y1) + 14 + H;
                            thePageObjectInf.Add(new PageObjectInf(Obj.Id,
                                                                   Convert.ToInt32(X1 + (W / 2) - 21),
                                                                   Convert.ToInt32(YInf),
                                                                   CurPage + iTotalPage,
                                                                   0));

                            MyImage = new ceTe.DynamicPDF.PageElements.Image(System.Web.HttpContext.Current.Server.MapPath("~/Content/images/downarrow.jpg").ToString(), X1 + (W / 2) - 21, YInf);
                            MyImage.Height = 14;
                            MyImage.Width = 42;
                            MyPage[CurPage].Elements.Add(MyImage);
                        }
                        else if (Flag == "U")
                        {
                            MyImage = new ceTe.DynamicPDF.PageElements.Image(System.Web.HttpContext.Current.Server.MapPath("~/Content/images/uparrow.jpg").ToString(), X1 + (W / 2) - 21, Y1 - 3);
                            MyImage.Height = 14;
                            MyImage.Width = 42;
                            MyPage[CurPage].Elements.Add(MyImage);
                        }
                    }
                }
                else if (strOutput == "PPT")
                {
                    int Y1Inf = GetPageRowPosition(Y1) + 15;
                    if (sFP[0] == "BRD")
                    {
                        DrawDivPPTImage(sFP[1], CurPage, X1, Y1Inf, W - 1, H - 1, Obj);

                        if (Flag == "Y")
                        {
                            thePageObjectInf.Add(new PageObjectInf(Obj.Id,
                                                                   Convert.ToInt32(RectPPT.Col + (RectPPT.Width / 2) - 21),
                                                                   Convert.ToInt32(RectPPT.Row + RectPPT.Height),
                                                                   CurPage + iTotalPage,
                                                                   0));

                            GraphicImg[CurPage].DrawImage(BmpDnArrow, RectPPT.Col + (RectPPT.Width / 2) - 21, RectPPT.Row + RectPPT.Height, 42, 14);
                        }
                        else if (Flag == "U")
                        {
                            GraphicImg[CurPage].DrawImage(BmpUpArrow, RectPPT.Col + (RectPPT.Width / 2) - 21, RectPPT.Row - 19, 42, 14);
                        }
                    }
                    else
                    {
                        GraphicImg[CurPage].DrawImage(BmpURL, X1, Y1Inf, W - 1, H - 1);
                        if (Flag == "Y")
                        {
                            thePageObjectInf.Add(new PageObjectInf(Obj.Id,
                                                                   Convert.ToInt32(X1 + (W / 2) - 21),
                                                                   Convert.ToInt32(Y1Inf + H),
                                                                   CurPage + iTotalPage,
                                                                   0));

                            GraphicImg[CurPage].DrawImage(BmpDnArrow, X1 + (W / 2) - 21, Y1Inf + H, 42, 14);
                        }
                        else if (Flag == "U")
                        {
                            GraphicImg[CurPage].DrawImage(BmpUpArrow, X1 + (W / 2) - 21, Y1Inf - 3, 42, 14);
                        }
                    }
                }

                return;
            }
            CurPage = PDFPage;
            if (strOutput == "PDF")
            {
                if (sFP[0] == "BRD")
                {
                    DrawDivPDFImage(sFP[1], CurPage, X1, Y1 + 15, W - 1, H - 1, Obj);
                    if (Flag == "Y")
                    {
                        thePageObjectInf.Add(new PageObjectInf(Obj.Id,
                                                               Convert.ToInt32(MyLabelRect.X + (MyLabelRect.Width / 2) - 21),
                                                               Convert.ToInt32(MyLabelRect.Y + 2 + MyLabelRect.Height),
                                                               CurPage + iTotalPage,
                                                               0));

                        MyImage = new ceTe.DynamicPDF.PageElements.Image(System.Web.HttpContext.Current.Server.MapPath("~/Content/images/downarrow.jpg").ToString(),
                                                                         MyLabelRect.X + (MyLabelRect.Width / 2) - 21,
                                                                         MyLabelRect.Y + 2 + MyLabelRect.Height);
                        MyImage.Height = 14;
                        MyImage.Width = 42;
                        MyPage[CurPage].Elements.Add(MyImage);
                    }
                    else if (Flag == "U")
                    {
                        MyImage = new ceTe.DynamicPDF.PageElements.Image(System.Web.HttpContext.Current.Server.MapPath("~/Content/images/uparrow.jpg").ToString(),
                                                                         MyLabelRect.X + (MyLabelRect.Width / 2) - 21,
                                                                         MyLabelRect.Y - 19);
                        MyImage.Height = 14;
                        MyImage.Width = 42;
                        MyPage[CurPage].Elements.Add(MyImage);
                    }
                }
                else
                {
                    MyImage = new ceTe.DynamicPDF.PageElements.Image(System.Web.HttpContext.Current.Server.MapPath(sFP[1]).ToString(), X1, Y1 + 15);
                    MyImage.Height = H - 1;
                    MyImage.Width = W - 1;
                    MyPage[CurPage].Elements.Add(MyImage);

                    if (Flag == "Y")
                    {
                        MyImage = new ceTe.DynamicPDF.PageElements.Image(System.Web.HttpContext.Current.Server.MapPath("~/Content/images/downarrow.jpg").ToString(), X1 + (W / 2) - 21, Y1 + 16 + H);
                        MyImage.Height = 14;
                        MyImage.Width = 42;
                        MyPage[CurPage].Elements.Add(MyImage);

                        thePageObjectInf.Add(new PageObjectInf(Obj.Id,
                                                               Convert.ToInt32(X1 + (W / 2) - 21),
                                                               Convert.ToInt32(Y1 + 16 + H),
                                                               CurPage + iTotalPage,
                                                               0));
                    }
                    else if (Flag == "U")
                    {
                        MyImage = new ceTe.DynamicPDF.PageElements.Image(System.Web.HttpContext.Current.Server.MapPath("~/Content/images/uparrow.jpg").ToString(), X1 + (W / 2) - 21, Y1 - 3);
                        MyImage.Height = 14;
                        MyImage.Width = 42;
                        MyPage[CurPage].Elements.Add(MyImage);
                    }
                }

            }
            else if (strOutput == "PPT")
            {
                if (sFP[0] == "BRD")
                {
                    DrawDivPPTImage(sFP[1], CurPage, X1, Y1 + 15, W - 1, H - 1, Obj);
                    if (Flag == "Y")
                    {
                        thePageObjectInf.Add(new PageObjectInf(Obj.Id,
                                                               Convert.ToInt32(RectPPT.Col + (RectPPT.Width / 2) - 21),
                                                               Convert.ToInt32(RectPPT.Row + RectPPT.Height + 2),
                                                               CurPage + iTotalPage,
                                                               0));
                        GraphicImg[CurPage].DrawImage(BmpDnArrow, RectPPT.Col + (RectPPT.Width / 2) - 21, RectPPT.Row + RectPPT.Height + 2, 42, 14);
                    }
                    else if (Flag == "U")
                        GraphicImg[CurPage].DrawImage(BmpUpArrow, RectPPT.Col + (RectPPT.Width / 2) - 21, RectPPT.Row - 19, 42, 14);
                }
                else
                {
                    GraphicImg[CurPage].DrawImage(BmpURL, X1, Y1 + 15, W - 1, H - 1);
                    if (Flag == "Y")
                    {
                        thePageObjectInf.Add(new PageObjectInf(Obj.Id,
                                                               Convert.ToInt32(X1 + (W / 2) - 21),
                                                               Convert.ToInt32(Y1 + 15 + H),
                                                               CurPage + iTotalPage,
                                                               0));
                        GraphicImg[CurPage].DrawImage(BmpDnArrow, X1 + (W / 2) - 21, Y1 + 15 + H, 42, 14);
                    }
                    else if (Flag == "U")
                        GraphicImg[CurPage].DrawImage(BmpUpArrow, X1 + (W / 2) - 21, Y1 - 3, 42, 14);
                }
            }
        }

        // Get Font Name for display
        private ceTe.DynamicPDF.Font GetPDFFontName(int Idx)
        {
            switch ((dtFieldInformation.Rows[Idx]["FONT_NAME"].ToString() + dtFieldInformation.Rows[Idx]["FONT_STYLE"].ToString()).ToUpper())
            {
                case "ARIALNORMAL":
                    return ceTe.DynamicPDF.Font.Helvetica;
                case "ARIALITALLIC":
                    return ceTe.DynamicPDF.Font.HelveticaBoldOblique;
                case "ARIALBOLD":
                    return ceTe.DynamicPDF.Font.HelveticaBold;
                case "TIMESNORMAL":
                    return ceTe.DynamicPDF.Font.TimesRoman;
                case "TIMESITALLIC":
                    return ceTe.DynamicPDF.Font.TimesItalic;
                case "TIMESBOLD":
                    return ceTe.DynamicPDF.Font.TimesBold;
                case "COURIERNORMAL":
                    return ceTe.DynamicPDF.Font.Courier;
                case "COURIERITALLIC":
                    return ceTe.DynamicPDF.Font.CourierOblique;
                case "COURIERBOLD":
                    return ceTe.DynamicPDF.Font.CourierBold;
            }
            return ceTe.DynamicPDF.Font.Helvetica;
        }

        // Get Font Size for display
        private int GetPDFFontSize(int Idx)
        {
            return Convert.ToInt16(dtFieldInformation.Rows[Idx]["FONT_SIZE"]);
        }

        // Get Font Size for display
        private ceTe.DynamicPDF.TextAlign GetPDFFontFloat(string FontStyle)
        {
            if (FontStyle.ToUpper() == "RIGHT")
                return ceTe.DynamicPDF.TextAlign.Right;
            else if (FontStyle.ToUpper() == "LEFT")
                return ceTe.DynamicPDF.TextAlign.Left;
            else if (FontStyle.ToUpper() == "CENTER")
                return ceTe.DynamicPDF.TextAlign.Center;

            return ceTe.DynamicPDF.TextAlign.Left;
        }

        // Get Font Size for display
        private ceTe.DynamicPDF.RgbColor GetPDFFontColor(int Idx)
        {
            switch (dtFieldInformation.Rows[Idx]["FONT_COLOR"].ToString().ToLower())
            {
                case "#663300":
                    return ceTe.DynamicPDF.WebColor.Brown;
                    break;
                case "#ffb266":
                    return ceTe.DynamicPDF.WebColor.Orange;
                    break;
                case "#ffffff":
                    return ceTe.DynamicPDF.WebColor.White;
                    break;
                case "#ff0000":
                    return ceTe.DynamicPDF.WebColor.Red;
                    break;
                case "#911414":
                    return ceTe.DynamicPDF.WebColor.DarkRed;
                    break;
                case "#000000":
                    return ceTe.DynamicPDF.WebColor.Black;
                    break;

            }
            return ceTe.DynamicPDF.WebColor.Black;
        }

        //Function to get Font style
        public System.Drawing.FontStyle GetFontStyle(string FontStyle)
        {
            switch (FontStyle)
            {
                case "underline":
                    return System.Drawing.FontStyle.Underline;
                    break;
                case "bold-ul":
                    return System.Drawing.FontStyle.Underline;
                    break;
                case "bold":
                    return System.Drawing.FontStyle.Bold;
                    break;
                case "itallic":
                    return System.Drawing.FontStyle.Italic;
                    break;
                case "strikethru":
                    return System.Drawing.FontStyle.Strikeout;
                    break;
            }

            return System.Drawing.FontStyle.Regular;
        }

        // Place information in PDF
        private void PlaceInfoPDF(string Info, int Col, int Row, int Width, int Height, string Level, string ShowFullBox)
        {
            string[] LabelInfo = Info.Replace("&amp;", "&").Split(';');
            string FontName = "", FontSize = "", FontColor = "", FontStyle = "", FontFloat = "", FontWidth = "", Adjustment = "";
            string LGL_SHOW_FIELD = WebConfigurationManager.AppSettings["LGL_SHOW_FIELD"];
            DataTable dtFieldInf = null;
            if (sAllPDF == "N")
                dtFieldInf = dtFieldInformation;
            else
                dtFieldInf = dtFieldActive;
            if (LabelInfo.Length >= 1)
            {
                int Idx = 0, LeftPos = 0;
                FieldCount = dtFieldInf.Rows.Count;
                for (int Idy = 0; Idy <= FieldCount - 1; Idy++)
                {
                    if (dtFieldInf.Rows[Idx]["FIELD_NAME"].ToString().ToUpper() == "FULLNAME")
                    {
                        string sName = "";
                    }

                    try
                    {
                        int iWidth = 255;
                        //if (LevelCount == 6) iWidth = 168;

                        string[] LabelText = LabelInfo[Idx].Split('|');
                        if (dtFieldInf.Rows[Idx]["ACTIVE_IND"].ToString() == "Y")
                        {
                            FontName = dtFieldInf.Rows[Idx]["FONT_NAME"].ToString();
                            FontSize = dtFieldInf.Rows[Idx]["FONT_SIZE"].ToString();
                            FontColor = dtFieldInf.Rows[Idx]["FONT_COLOR"].ToString();
                            FontStyle = dtFieldInf.Rows[Idx]["FONT_STYLE"].ToString();
                            FontFloat = dtFieldInf.Rows[Idx]["FONT_FLOAT"].ToString();
                            FontWidth = dtFieldInf.Rows[Idx]["FIELD_WIDTH"].ToString();
                            Adjustment = dtFieldInf.Rows[Idx]["ADJUSTMENT"].ToString();

                            LeftPos = 0;
                            if ((Adjustment == "Y") && (FontFloat == "right"))
                            {
                                LeftPos = iWidth - 255;
                                if (Level == "1") LeftPos += -10;
                                if (Level == "2") LeftPos += -20;
                            }
                            else if ((Adjustment == "Y") && (FontFloat == "center"))
                            {
                                if (Convert.ToInt16(FontWidth) >= iWidth)
                                {
                                    if (Level == "0") FontWidth = iWidth.ToString();
                                    if (Level == "1") FontWidth = (iWidth - 10).ToString();
                                    if (Level == "2") FontWidth = (iWidth - 20).ToString();
                                }
                            }
                            if (strOutput == "PDF")
                            {
                                if (ShowFullBox != "N")
                                {
                                    if (dtFieldInf.Rows[Idx]["FIELD_NAME"].ToString() == "NOR_COUNT")
                                    {
                                        if (LabelText[0].ToString() != "0")
                                        {
                                            MyLabel = new ceTe.DynamicPDF.PageElements.Label("SoC",
                                                                                             Col + Convert.ToInt32(LabelText[2]) + LeftPos,
                                                                                             Row + Convert.ToInt32(LabelText[1]),
                                                                                             Convert.ToInt16(FontWidth),
                                                                                             Height,
                                                                                             GetPDFFontName(Idx),
                                                                                             GetPDFFontSize(Idx),
                                                                                             GetPDFFontFloat(FontFloat),
                                                                                             GetPDFFontColor(Idx));
                                            if ((FontStyle == "bold-ul") || (FontStyle == "underline")) MyLabel.Underline = true;
                                            MyPage[CurPage].Elements.Add(MyLabel);
                                            MyLabel = new ceTe.DynamicPDF.PageElements.Label(Convert.ToInt32(LabelText[0]).ToString("#,##0"),
                                                                                         Col + Convert.ToInt32(LabelText[2]) + LeftPos + 50,
                                                                                         Row + Convert.ToInt32(LabelText[1]),
                                                                                         Convert.ToInt16(FontWidth),
                                                                                         Height,
                                                                                         GetPDFFontName(Idx),
                                                                                         GetPDFFontSize(Idx),
                                                                                         GetPDFFontFloat(FontFloat),
                                                                                         GetPDFFontColor(Idx));
                                            if ((FontStyle == "bold-ul") || (FontStyle == "underline")) MyLabel.Underline = true;
                                            MyPage[CurPage].Elements.Add(MyLabel);
                                        }
                                    }
                                    else if (dtFieldInf.Rows[Idx]["FIELD_NAME"].ToString() == "SOC_COUNT")
                                    {
                                        if (LabelText[0].ToString() != "0")
                                        {
                                            MyLabel = new ceTe.DynamicPDF.PageElements.Label("NoR",
                                                                                             Col + Convert.ToInt32(LabelText[2]) + LeftPos,
                                                                                             Row + Convert.ToInt32(LabelText[1]),
                                                                                             Convert.ToInt16(FontWidth),
                                                                                             Height,
                                                                                             GetPDFFontName(Idx),
                                                                                             GetPDFFontSize(Idx),
                                                                                             GetPDFFontFloat(FontFloat),
                                                                                             GetPDFFontColor(Idx));
                                            if ((FontStyle == "bold-ul") || (FontStyle == "underline")) MyLabel.Underline = true;
                                            MyPage[CurPage].Elements.Add(MyLabel);
                                            MyLabel = new ceTe.DynamicPDF.PageElements.Label(Convert.ToInt32(LabelText[0]).ToString("#,##0"),
                                                                                         Col + Convert.ToInt32(LabelText[2]) + LeftPos + 50,
                                                                                         Row + Convert.ToInt32(LabelText[1]),
                                                                                         Convert.ToInt16(FontWidth),
                                                                                         Height,
                                                                                         GetPDFFontName(Idx),
                                                                                         GetPDFFontSize(Idx),
                                                                                         GetPDFFontFloat(FontFloat),
                                                                                         GetPDFFontColor(Idx));
                                            if ((FontStyle == "bold-ul") || (FontStyle == "underline")) MyLabel.Underline = true;
                                            MyPage[CurPage].Elements.Add(MyLabel);

                                        }
                                    }
                                    else
                                    {
                                        MyLabel = new ceTe.DynamicPDF.PageElements.Label(LabelText[0],
                                                                                         Col + Convert.ToInt32(LabelText[2]) + LeftPos,
                                                                                         Row + Convert.ToInt32(LabelText[1]),
                                                                                         Convert.ToInt16(FontWidth),
                                                                                         Height,
                                                                                         GetPDFFontName(Idx),
                                                                                         GetPDFFontSize(Idx),
                                                                                         GetPDFFontFloat(FontFloat),
                                                                                         GetPDFFontColor(Idx));
                                        if ((FontStyle == "bold-ul") || (FontStyle == "underline"))
                                        {
                                            MyLabel.Underline = true;
                                        }
                                        MyPage[CurPage].Elements.Add(MyLabel);
                                    }
                                }
                                else
                                {
                                    if (dtFieldInf.Rows[Idx]["FIELD_NAME"].ToString().ToUpper() == LGL_SHOW_FIELD.ToUpper())
                                    {
                                        MyLabel = new ceTe.DynamicPDF.PageElements.Label(LabelText[0],
                                                                                         MyLabelRect.X,
                                                                                         MyLabelRect.Y,
                                                                                         MyLabelRect.Width,
                                                                                         MyLabelRect.Height,
                                                                                         GetPDFFontName(Idx),
                                                                                         14,
                                                                                         ceTe.DynamicPDF.TextAlign.Center,
                                                                                         GetPDFFontColor(Idx));
                                        if ((FontStyle == "bold-ul") || (FontStyle == "underline")) MyLabel.Underline = true;
                                        MyLabel.VAlign = ceTe.DynamicPDF.VAlign.Center;
                                        MyPage[CurPage].Elements.Add(MyLabel);
                                    }
                                }

                            }
                            else if (strOutput == "PPT")
                            {
                                if (ShowFullBox != "N")
                                {
                                    System.Drawing.Font drawFontText = new System.Drawing.Font(dtFieldInf.Rows[Idx]["FONT_NAME"].ToString(),
                                                                                               Convert.ToSingle(dtFieldInf.Rows[Idx]["FONT_SIZE"]),
                                                                                               GetFontStyle(dtFieldInf.Rows[Idx]["FONT_STYLE"].ToString()),
                                                                                               GraphicsUnit.Pixel);
                                    SolidBrush drawBrushText = new SolidBrush(System.Drawing.Color.Black);
                                    switch (FontColor.ToLower())
                                    {
                                        case "#663300":
                                            drawBrushText = new SolidBrush(System.Drawing.Color.Brown);
                                            break;
                                        case "#ffb266":
                                            drawBrushText = new SolidBrush(System.Drawing.Color.Orange);
                                            break;
                                        case "#ffffff":
                                            drawBrushText = new SolidBrush(System.Drawing.Color.White);
                                            break;
                                        case "#0000ff":
                                            drawBrushText = new SolidBrush(System.Drawing.Color.Blue);
                                            break;
                                        case "#008000":
                                            drawBrushText = new SolidBrush(System.Drawing.Color.Green);
                                            break;
                                        case "#911414":
                                            drawBrushText = new SolidBrush(System.Drawing.Color.DarkRed);
                                            break;
                                        case "#ff0000":
                                            drawBrushText = new SolidBrush(System.Drawing.Color.Red);
                                            break;
                                        case "#000000":
                                            drawBrushText = new SolidBrush(System.Drawing.Color.Black);
                                            break;
                                    }


                                    StringFormat stringFormat = new StringFormat();
                                    if (FontFloat.ToUpper() == "LEFT")
                                        stringFormat.Alignment = StringAlignment.Near;
                                    else if (FontFloat.ToUpper() == "RIGHT")
                                        stringFormat.Alignment = StringAlignment.Far;
                                    else if (FontFloat.ToUpper() == "CENTER")
                                        stringFormat.Alignment = StringAlignment.Center;
                                    if (dtFieldInf.Rows[Idx]["FIELD_NAME"].ToString() == "NOR_COUNT")
                                    {
                                        if (LabelText[0].ToString() != "0")
                                        {
                                            System.Drawing.Rectangle rectLabel = new System.Drawing.Rectangle(Col + Convert.ToInt32(LabelText[2]) + LeftPos - 50, Row + Convert.ToInt32(LabelText[1]), Convert.ToInt16(FontWidth), Height);
                                            GraphicImg[CurPage].DrawString("SoC", drawFontText, drawBrushText, rectLabel, stringFormat);
                                            rectLabel = new System.Drawing.Rectangle(Col + Convert.ToInt32(LabelText[2]) + LeftPos, Row + Convert.ToInt32(LabelText[1]), Convert.ToInt16(FontWidth), Height);
                                            GraphicImg[CurPage].DrawString(Convert.ToInt32(LabelText[0]).ToString("#,##0"), drawFontText, drawBrushText, rectLabel, stringFormat);
                                        }
                                    }
                                    else if (dtFieldInf.Rows[Idx]["FIELD_NAME"].ToString() == "SOC_COUNT")
                                    {
                                        if (LabelText[0].ToString() != "0")
                                        {
                                            System.Drawing.Rectangle rectLabel = new System.Drawing.Rectangle(Col + Convert.ToInt32(LabelText[2]) + LeftPos - 50, Row + Convert.ToInt32(LabelText[1]), Convert.ToInt16(FontWidth), Height);
                                            GraphicImg[CurPage].DrawString("NoR", drawFontText, drawBrushText, rectLabel, stringFormat);
                                            rectLabel = new System.Drawing.Rectangle(Col + Convert.ToInt32(LabelText[2]) + LeftPos, Row + Convert.ToInt32(LabelText[1]), Convert.ToInt16(FontWidth), Height);
                                            GraphicImg[CurPage].DrawString(Convert.ToInt32(LabelText[0]).ToString("#,##0"), drawFontText, drawBrushText, rectLabel, stringFormat);
                                        }
                                    }
                                    else
                                    {
                                        System.Drawing.Rectangle rectLabel = new System.Drawing.Rectangle(Col + Convert.ToInt32(LabelText[2]) + LeftPos, Row + Convert.ToInt32(LabelText[1]), Convert.ToInt16(FontWidth), Height);
                                        GraphicImg[CurPage].DrawString(LabelText[0], drawFontText, drawBrushText, rectLabel, stringFormat);
                                    }
                                }
                                else
                                {
                                    if (dtFieldInf.Rows[Idx]["FIELD_NAME"].ToString().ToUpper() == LGL_SHOW_FIELD.ToUpper())
                                    {

                                        System.Drawing.Font drawFontText = new System.Drawing.Font(dtFieldInf.Rows[Idx]["FONT_NAME"].ToString(),
                                                                                                   14,
                                                                                                   GetFontStyle("bold"),
                                                                                                   GraphicsUnit.Pixel);
                                        SolidBrush drawBrushText = new SolidBrush(System.Drawing.Color.Black);
                                        switch (FontColor.ToLower())
                                        {
                                            case "#663300":
                                                drawBrushText = new SolidBrush(System.Drawing.Color.Brown);
                                                break;
                                            case "#ffb266":
                                                drawBrushText = new SolidBrush(System.Drawing.Color.Orange);
                                                break;
                                            case "#ffffff":
                                                drawBrushText = new SolidBrush(System.Drawing.Color.White);
                                                break;
                                            case "#0000ff":
                                                drawBrushText = new SolidBrush(System.Drawing.Color.Blue);
                                                break;
                                            case "#008000":
                                                drawBrushText = new SolidBrush(System.Drawing.Color.Green);
                                                break;
                                            case "#911414":
                                                drawBrushText = new SolidBrush(System.Drawing.Color.DarkRed);
                                                break;
                                            case "#ff0000":
                                                drawBrushText = new SolidBrush(System.Drawing.Color.Red);
                                                break;
                                            case "#000000":
                                                drawBrushText = new SolidBrush(System.Drawing.Color.Black);
                                                break;
                                        }


                                        StringFormat stringFormat = new StringFormat();
                                        stringFormat.Alignment = StringAlignment.Center;
                                        stringFormat.LineAlignment = StringAlignment.Center;
                                        System.Drawing.Rectangle rectLabel = new System.Drawing.Rectangle(RectPPT.Col, RectPPT.Row, RectPPT.Width, RectPPT.Height);
                                        GraphicImg[CurPage].DrawString(LabelText[0], drawFontText, drawBrushText, rectLabel, stringFormat);
                                    }
                                }
                            }
                        }
                        Idx++;
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
        }

        // DrawLabel Obj, X1, Y1
        private void DrawLabel(ObjectInf Obj, int X1, int Y1)
        {
            if (Y1 + Obj.Oheight + 20 >= Output_Height)
            {
                if (!(GetPageExistence(Y1 + Obj.Oheight)))
                {
                    PageSizeNos.Add(Y1);
                    //MyPage[PageSizeNos.Count - 1] = new ceTe.DynamicPDF.Page(PageSize.B4, PageOrientation.Landscape, 15.0F);
                    if (LevelCount == 4)
                        MyPage[PageSizeNos.Count - 1] = new ceTe.DynamicPDF.Page(1080F, 1050F, 15.3F);
                    else
                        MyPage[PageSizeNos.Count - 1] = new ceTe.DynamicPDF.Page(1600F, 1050F, 15.3F);

                    // MyPage[PageSizeNos.Count - 1] = new ceTe.DynamicPDF.Page(1050F, 768F, 15.3F);
                    MaxPage = PageSizeNos.Count;
                }
                PlaceInfoPDF(Obj.Title, X1, GetPageRowPosition(Y1) + 15, Obj.Width - 30, Obj.Oheight - 2, Obj.Level, Obj.ShowFullBox);

                return;
            }
            CurPage = PDFPage;
            PlaceInfoPDF(Obj.Title, X1, Y1 + 15, Obj.Width - 30, Obj.Oheight - 2, Obj.Level, Obj.ShowFullBox);
        }

        // Draws the photo with respect to Position Id
        private string DrawPhoto(ObjectInf Obj, int col, int row)
        {
            string strElement = "";
            string PhotoFlag = "Y";
            if (dtconf.Rows[0]["PHOTO_FLAG"]==null) PhotoFlag = "N";
            else if (dtconf.Rows[0]["PHOTO_FLAG"].ToString() != "Y") PhotoFlag = "N";
            if (PhotoFlag == "Y")
            {
                string sPhotoDIR = HttpContext.Current.Server.MapPath("~/Content/images/photos/") + Obj.Id + ".jpg", sPhotoURL = "~/Content/images/photos/" + Obj.Id + ".jpg";
                if (strOutput == "PDF")
                {
                    if (File.Exists(sPhotoDIR))
                    {
                        if (Obj.Level == "0")
                        {
                            MyImage = new ceTe.DynamicPDF.PageElements.Image(sPhotoDIR, Obj.Col + col, Obj.Row + row + 17);
                            MyImage.Height = 70;
                            MyImage.Width = 70;

                            MyPage[CurPage].Elements.Add(MyImage);
                        }
                        else
                        {
                            MyImage = new ceTe.DynamicPDF.PageElements.Image(sPhotoDIR, Obj.Col, Obj.Row + row + 17);
                            MyImage.Height = 70;
                            MyImage.Width = 70;

                            MyPage[CurPage].Elements.Add(MyImage);
                        }
                    }
                }
                else if (strOutput == "PPT")
                {
                    if (Obj.Level == "0")
                    {
                        Bitmap BmpPhoto = null;
                        if (File.Exists(sPhotoDIR))
                        {
                            BmpPhoto = new Bitmap(sPhotoDIR);
                            GraphicImg[CurPage].DrawImage(BmpPhoto, Obj.Col + col, Obj.Row + row + 17, 70, 70);
                        }
                    }
                    else
                    {
                        Bitmap BmpPhoto = null;
                        if (File.Exists(sPhotoDIR))
                        {
                            BmpPhoto = new Bitmap(sPhotoDIR);
                            GraphicImg[CurPage].DrawImage(BmpPhoto, Obj.Col, Obj.Row + row + 17, 70, 70);
                        }
                    }
                }
                else if (strOutput == "HTML")
                {
                    if (File.Exists(sPhotoDIR))
                        strElement = "<img src=\"" + sPhotoURL + "\" alt=\"Photo\" style=\"height:75px;position:absolute;left:" + (Obj.Col + col + 2).ToString() + "px;top:" + (Obj.Row + row + 2).ToString() + "px\"/>";
                }
            }

            return strElement;
        }


        // Places the Level 0 Information
        private void Level_0_Information(List<ObjectInf> theObjectInf, string sArrow, string sTPage, string sCPage)
        {
            //lstID.Add(HttpContext.Current.Session["ID"].ToString());
            string sUpperArrow = "U";
            ObjectInf Obj = GetIdInfo(theObjectInf, LevelIds[0].ToString());

            if (Obj.Id != HttpContext.Current.Session["ID"].ToString())
            {
                if (sArrow == "I")
                {
                    if (strOutput == "PDF")
                    {
                        string AP = GetAncestorPageNo(Obj.Id);
                        XYDestination dest = new XYDestination(Convert.ToInt32(AP), 1, 1);
                        MyLink = new ceTe.DynamicPDF.PageElements.Link(Obj.Col + 288, 2, 255, 10, dest);
                        MyPage[CurPage].Elements.Add(MyLink);

                        MyLabel = new ceTe.DynamicPDF.PageElements.Label("( #" + sCPage + " / " + sTPage + ")", Obj.Col + 448, 12, 255, 10, ceTe.DynamicPDF.Font.HelveticaBold, 10, ceTe.DynamicPDF.TextAlign.Left);
                        MyPage[CurPage].Elements.Add(MyLabel);
                        MyLabel = new ceTe.DynamicPDF.PageElements.Label("( # " + AP + ")", Obj.Col + 288, 2, 255, 10, ceTe.DynamicPDF.Font.HelveticaBold, 10, ceTe.DynamicPDF.TextAlign.Center);
                        MyPage[CurPage].Elements.Add(MyLabel);
                    }
                    else if (strOutput == "PPT")
                    {
                        System.Drawing.Font drawFontText = new System.Drawing.Font("Arail", 10, FontStyle.Bold, GraphicsUnit.Pixel);
                        SolidBrush drawBrushText = new SolidBrush(System.Drawing.Color.Black);
                        StringFormat stringFormat = new StringFormat();
                        stringFormat.Alignment = StringAlignment.Near;

                        System.Drawing.Rectangle rectLabel = new System.Drawing.Rectangle(Obj.Col + 448, 12, 255, 10);
                        GraphicImg[CurPage].DrawString("( #" + sCPage + " / " + sTPage + ")", drawFontText, drawBrushText, rectLabel, stringFormat);

                        stringFormat = new StringFormat();
                        stringFormat.Alignment = StringAlignment.Center;
                        rectLabel = new System.Drawing.Rectangle(Obj.Col + 288, 2, 255, 10);
                        GraphicImg[CurPage].DrawString("( # " + GetAncestorPageNo(Obj.Id) + " )", drawFontText, drawBrushText, rectLabel, stringFormat);
                    }
                }
            }
            else sArrow = "C";

            // Add label to MyPage
            PreviousObj = null;
            if (LevelCount == 4)
            {
                if (sArrow == "C") sUpperArrow = ((Obj.PId == "-1") || (Obj.PId == "10000000")) ? "N" : "U";
                DrawImage(TemplateURL, Obj.Col + 33, Obj.Row, Obj.Width, Obj.Oheight, sUpperArrow, Obj);
                if (IdInPId(theObjectInf, Obj.Id).Length != 0)
                    DrawLine(Obj.Id, Obj.Col + Convert.ToInt16(Obj.Width / 2) + 33, Obj.Row + Obj.Oheight, Obj.Col + Convert.ToInt16(Obj.Width / 2) + 33, Obj.Row + Obj.Oheight + 20);
                DrawLabel(Obj, Obj.Col + 36, Obj.Row);
                DrawPhoto(Obj, 33, 0);
            }
            else if (LevelCount == 6)
            {
                if (sArrow == "C") sUpperArrow = ((Obj.PId == "-1") || (Obj.PId == "10000000")) ? "N" : "U";
                DrawImage(TemplateURL, Obj.Col + 288, Obj.Row, Obj.Width, Obj.Oheight, sUpperArrow, Obj);
                if (IdInPId(theObjectInf, Obj.Id).Length != 0)
                    DrawLine(Obj.Id, Obj.Col + Convert.ToInt16(Obj.Width / 2) + 288, Obj.Row + Obj.Oheight, Obj.Col + Convert.ToInt16(Obj.Width / 2) + 288, Obj.Row + Obj.Oheight + 20);
                DrawLabel(Obj, Obj.Col + 291, Obj.Row);
                DrawPhoto(Obj, 290, 0);
            }
        }

        // Places the Level 1 Information
        private void Level_1_Information(List<ObjectInf> theObjectInf)
        {
            var listObj = from p in theObjectInf
                          where p.Level == "1"
                          orderby p.Row, p.Col
                          select p;

            PreviousObj = null;
            foreach (ObjectInf Obj in listObj)
            {
                if (Obj.Level == "1")               // Gets the Start & End column, row information
                {
                    // Add label to MyPage
                    string NextLevel = CheckLevelInfo(Obj);
                    DrawImage(TemplateURL, Obj.Col, Obj.Row, Obj.Width - 10, Obj.Oheight, NextLevel == "NO" ? "N" : "Y", Obj);
                    DrawLabel(Obj, Obj.Col + 2, Obj.Row);
                    DrawPhoto(Obj, 33, 0);
                }
            }
        }

        // Places the Level 1 Information
        private void Level_1_Line_Information(List<ObjectInf> theObjectInf)
        {
            int TotalCol = 0, TCol = 0, MaxHeight = 0, Idx = 0;
            ObjectInf ObjStart = null, ObjCur = null;

            var listObj = from p in theObjectInf
                          where p.Level == "1"
                          orderby p.Row, p.Col
                          select p;

            PreviousObj = null;
            foreach (ObjectInf Obj in listObj)
            {
                if (Obj.Level == "1")               // Gets the Start & End column, row information
                {
                    // Places the Line above Level 1
                    if (((Idx % LevelCount) == 0) && (Idx != 0))
                    {
                        // Places next level line
                        DrawLine(ObjCur.Id, ObjStart.Col + Convert.ToInt16(ObjStart.Width / 2), ObjStart.Row - 20, ObjCur.Col + Convert.ToInt16(ObjCur.Width / 2), ObjCur.Row - 20);

                        // Places center line
                        if ((listObj.Count() - (listObj.Count() % LevelCount)) == Idx)
                        {
                            if (listObj.Count() == Idx + 1)
                            {
                                if ((GetPageRowStartPosition(Obj.Row) + 20) == Obj.Row)
                                    DrawLine("Middle:1", Convert.ToInt16(TotalCol / 2), ObjStart.Row - 20, Convert.ToInt16(TotalCol / 2), ObjCur.Row + MaxHeight);
                                else
                                {
                                    if ((ObjCur.Row + MaxHeight) <= 640)
                                        DrawLine("Middle:0", Convert.ToInt16(TotalCol / 2), ObjStart.Row - 20, Convert.ToInt16(TotalCol / 2), ObjCur.Row + MaxHeight - 20);
                                    else
                                    {
                                        if (LevelFlag == "1")
                                            DrawLine("Middle:1", Convert.ToInt16(TotalCol / 2), ObjStart.Row - 20, Convert.ToInt16(TotalCol / 2), ObjCur.Row + MaxHeight - 20);
                                        else
                                            DrawLine("Middle:0", Convert.ToInt16(TotalCol / 2), ObjStart.Row - 20, Convert.ToInt16(TotalCol / 2), ObjCur.Row + MaxHeight - 5);
                                    }
                                }
                            }
                            else
                            {
                                //MaxHeight = MaxHeight - 20;
                                if (LevelFlag == "1")
                                    DrawLine("Middle:1", Convert.ToInt16(TotalCol / 2), ObjStart.Row - 20, Convert.ToInt16(TotalCol / 2), ObjCur.Row + MaxHeight - 20);
                                else
                                    DrawLine("Middle:0", Convert.ToInt16(TotalCol / 2), ObjStart.Row - 20, Convert.ToInt16(TotalCol / 2), ObjCur.Row + MaxHeight - 5);
                            }
                        }
                        else
                        {
                            if (LevelFlag == "1")
                                DrawLine("Middle:1", Convert.ToInt16(TotalCol / 2), ObjStart.Row - 20, Convert.ToInt16(TotalCol / 2), ObjCur.Row + MaxHeight - 20);
                            else
                                DrawLine("Middle:0", Convert.ToInt16(TotalCol / 2), ObjStart.Row - 20, Convert.ToInt16(TotalCol / 2), ObjCur.Row + MaxHeight - 5);
                        }

                        // Initialise total column points
                        TCol = TotalCol;
                        TotalCol = ObjStart.Col;
                    }

                    ObjCur = Obj;
                    if ((Idx % LevelCount) == 0)
                    {
                        ObjStart = Obj;
                        MaxHeight = 0;
                    }
                    if (Idx == 0) TotalCol = ObjStart.Col;

                    TotalCol += Obj.Width;
                    if (MaxHeight <= Obj.Height) MaxHeight = Obj.Height;

                    // Draws the Lines for Level 1
                    if (IdInPId(theObjectInf, Obj.Id).Length != 0)
                    {
                        int Minus_Height = 0;
                        if (LevelCount == 4)
                        {
                            if (Original_Height == 80) Minus_Height = -7;
                            if (Original_Height == 90) Minus_Height = -6;
                            if (Original_Height == 100) Minus_Height = 0;
                            if (Original_Height == 110) Minus_Height = 4;
                            if (Original_Height == 120) Minus_Height = 5;
                        }
                        else if (LevelCount == 6)
                        {
                            if (Original_Height == 80) Minus_Height = -11;
                            if (Original_Height == 90) Minus_Height = -11;
                            if (Original_Height == 100) Minus_Height = -4;
                            if (Original_Height == 110) Minus_Height = -2;
                            if (Original_Height == 120) Minus_Height = -2;
                        }

                        DrawLine(Obj.Id, Obj.Col + 10, (Obj.Row + Obj.Oheight), Obj.Col + 10, (Obj.Row + ((Obj.Height + Adjustment_Height) - Obj.Oheight + Minus_Height)));
                    }
                    DrawLine(Obj.Id, Obj.Col + Convert.ToInt16(Obj.Width / 2), Obj.Row - 20, Obj.Col + Convert.ToInt16(Obj.Width / 2), Obj.Row);

                    Idx++;
                }
            }

            // Places the last line 
            if (LevelCount == 4)
            {
                if ((((Idx % LevelCount) <= 2) && ((Idx % LevelCount) != 0)))
                    DrawLine(ObjCur.Id, ObjStart.Col + Convert.ToInt16(ObjStart.Width / 2), ObjStart.Row - 20, Convert.ToInt16(785 / 2) + 120, ObjCur.Row - 20);
                else
                    DrawLine(ObjCur.Id, ObjStart.Col + Convert.ToInt16(ObjStart.Width / 2), ObjStart.Row - 20, ObjCur.Col + Convert.ToInt16(ObjCur.Width / 2), ObjCur.Row - 20);
            }
            else if (LevelCount == 6)
            {
                if ((((Idx % LevelCount) <= 3) && ((Idx % LevelCount) != 0)))
                    DrawLine(ObjCur.Id, ObjStart.Col + Convert.ToInt16(ObjStart.Width / 2), ObjStart.Row - 20, Convert.ToInt16(1300 / 2) + 117, ObjCur.Row - 20);
                else
                    DrawLine(ObjCur.Id, ObjStart.Col + Convert.ToInt16(ObjStart.Width / 2), ObjStart.Row - 20, ObjCur.Col + Convert.ToInt16(ObjCur.Width / 2), ObjCur.Row - 20);
            }
        }

        // Places the Level without child Information
        private void Level_Information(List<ObjectInf> theObjectInf)
        {
            var listObj = from p in theObjectInf
                          orderby p.Row, p.Col
                          select p;

            PreviousObj = null;
            foreach (ObjectInf Obj in listObj)
            {
                if ((Obj.Level != "0") && (Obj.Level != "1"))
                {
                    string NextLevel = CheckLevelInfo(Obj);
                    DrawImage(TemplateURL, Obj.Col + 5, Obj.Row, Obj.Width - 30, Obj.Oheight, NextLevel == "NO" ? "N" : "Y", Obj);
                    DrawLabel(Obj, Obj.Col + 2, Obj.Row);
                    DrawLine(Obj.Id, Obj.Col - 5, Obj.Row + Convert.ToInt16(Obj.Oheight / 2), Obj.Col + 5, Obj.Row + Convert.ToInt16(Obj.Oheight / 2));
                }
                PreviousObj = Obj;
            }
        }

        private string Level2Check(string Level1Inf, string Id)
        {
            string[] Level1 = Level1Inf.Split(',');
            for (int Idx = 0; Idx <= Level1.Length - 1; Idx++)
            {
                if (Level1[Idx] == Id) return "N";
            }

            return "Y";
        }

        private ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }

        string sData1 = "", sData2 = "", sData3 = "", sData4 = "", sQData = "";
        DataTable dtDR = null;
        private int SetPDFlevelInfo(DataRow[] dtlbl, string ShowLevel, string TemplateName, string DownloadType)
        {
            int intIdx = 0, intIdy = 0, intIdz = 0, SortNo=0;
            string InfoPos = "", SetFlag = "N";
            foreach (DataRow dr in dtlbl)
            {
                if (intIdx == 30)
                {
                    List<ObjectInfPDF> ObjectInf = lstObjectInf.OrderBy(o => o.Level).ToList();
                    string json = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(ObjectInf);
                    sData1 = ""; sData2 = ""; sData3 = ""; sData4 = "";
                    sData1 = json.Substring(0, (json.Length - 0) >= 7000 ? 7000 : json.Length);
                    if (json.Length >= 7001) sData2 = json.Substring(7000, (json.Length - 7000) >= 7000 ? 7000 : (json.Length - 7000));
                    if (json.Length >= 14001) sData3 = json.Substring(14000, (json.Length - 14000) >= 14000 ? 7000 : (json.Length - 14000));
                    if (json.Length >= 21001) sData4 = json.Substring(21000, (json.Length - 21000) >= 21000 ? 7000 : (json.Length - 21000));
                    string keyDate = LevelDate.Substring(6, 4) + "-" + LevelDate.Substring(3, 2) + "-" + LevelDate.Substring(0, 2);
                    if (View == "OV")
                    {
                        sQData += "INSERT INTO "+ dtHD_DIRECT_REPORT_DATA + " (DRIndex, PositionID, DrType, Data1, Data2, Data3, Data4, UpdatedDate, TemplateName, DownloadType) " +
                                                           " VALUES('" + intIdz.ToString() + "','" + ShowLevel + "','OV','" + 
                                                                         sData1 + "','" + sData2 + "','" + sData3 + "','" + sData4 + "','" + 
                                                                         keyDate + "','" + TemplateName + "','" + DownloadType + "');";
                    }
                    else
                    {
                        sQData += "INSERT INTO "+ dtHD_DIRECT_REPORT_DATA_LV + " (DRIndex, PositionID, DrType, Data1, Data2, Data3, Data4, UpdatedDate, TemplateName, DownloadType) " +
                                                           " VALUES('" + intIdz.ToString() + "','" + ShowLevel + "','LV','" + 
                                                                         sData1 + "','" + sData2 + "','" + sData3 + "','" + sData4 + "','" + 
                                                                         keyDate + "','" + TemplateName + "','" + DownloadType + "');";
                    }

                    for (intIdy = lstObjectInf.Count() - 1; intIdy >= 1; intIdy--)
                        lstObjectInf.RemoveAt(intIdy);
                    intIdx = 0; intIdz++;
                }

                InfoPos = "";
                foreach (DataRow drconf in dtconf.Rows)
                {
                    try
                    {
                        string LEVEL_ID = OPR_LEVEL_ID;
                        if (View == "LV") LEVEL_ID = LGL_PARENT_ID;
                        if (dr["POSITIONFLAG"].ToString() == dr[LEVEL_ID].ToString())
                        {
                            if ((drconf["FIELD_NAME"].ToString() == "FIRSTNAME") || (drconf["FIELD_NAME"].ToString() == "PositionTitle"))
                            {
                                InfoPos += ";    |" +
                                                    drconf["FIELD_ROW"].ToString() + "|" +
                                                    drconf["FIELD_COL"].ToString();
                            }
                            else
                            {
                                if (drconf["FIELD_NAME"].ToString() == PositionTotalCostField ||
                                    drconf["FIELD_NAME"].ToString() == PositionCostField)
                                {
                                    string Value = Convert.ToDecimal(dr[drconf["FIELD_NAME"].ToString()].ToString()).ToString("#,##0.00");
                                    InfoPos += ";" + Value + "|" +
                                                     drconf["FIELD_ROW"].ToString() + "|" +
                                                     drconf["FIELD_COL"].ToString();
                                }
                                else
                                {
                                    if (drconf["FIELD_NAME"].ToString().LastIndexOf("~") >= 1)
                                    {
                                        string Value = "";
                                        string[] FieldInf = drconf["FIELD_NAME"].ToString().Split('~');
                                        foreach(string FN in FieldInf)
                                        {
                                            string[] FN_PIPE = FN.Trim().Split('|');
                                            if (FN_PIPE.Length == 2)
                                            {
                                                Value += FN_PIPE[0].Trim();
                                                Value += dr[FN_PIPE[1].Trim()].ToString();
                                            }
                                            else if (FN.Trim() != "(" && FN.Trim() != ")")
                                            {
                                                Value += dr[FN.Trim()].ToString();
                                            }
                                            else Value += FN;
                                        }
                                        InfoPos += ";" + Value + "|" +
                                                         drconf["FIELD_ROW"].ToString() + "|" +
                                                         drconf["FIELD_COL"].ToString();

                                    }
                                    else
                                    {
                                        string Value = "";
                                        string[] FN_PIPE = drconf["FIELD_NAME"].ToString().Trim().Split('|');
                                        if (FN_PIPE.Length == 2)
                                        {
                                            Value += FN_PIPE[0].Trim();
                                            Value += dr[FN_PIPE[1].Trim()].ToString();
                                        }
                                        else Value = dr[drconf["FIELD_NAME"].ToString()].ToString();
                                        InfoPos += ";" + Value + "|" +
                                                         drconf["FIELD_ROW"].ToString() + "|" +
                                                         drconf["FIELD_COL"].ToString();
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (drconf["FIELD_NAME"].ToString() == PositionTotalCostField ||
                                drconf["FIELD_NAME"].ToString() == PositionCostField)
                            {
                                string Value = Convert.ToDecimal(dr[drconf["FIELD_NAME"].ToString()].ToString()).ToString("#,##0.00");
                                InfoPos += ";" + Value + "|" +
                                                 drconf["FIELD_ROW"].ToString() + "|" +
                                                 drconf["FIELD_COL"].ToString();
                            }
                            else
                            {
                                if (drconf["FIELD_NAME"].ToString().LastIndexOf("~") >= 1)
                                {
                                    string Value = "";
                                    string[] FieldInf = drconf["FIELD_NAME"].ToString().Split('~');
                                    foreach (string FN in FieldInf)
                                    {
                                        string[] FN_PIPE = FN.Trim().Split('|');
                                        if (FN_PIPE.Length == 2)
                                        {
                                            Value += FN_PIPE[0].Trim();
                                            Value += dr[FN_PIPE[1].Trim()].ToString();
                                        }
                                        else if (FN.Trim() != "(" && FN.Trim() != ")")
                                        {
                                            Value += dr[FN.Trim()].ToString();
                                        }
                                        else Value += FN;
                                    }
                                    InfoPos += ";" + Value + "|" +
                                                     drconf["FIELD_ROW"].ToString() + "|" +
                                                     drconf["FIELD_COL"].ToString();
                                }
                                else
                                {
                                    string Value = "";
                                    string[] FN_PIPE = drconf["FIELD_NAME"].ToString().Trim().Split('|');
                                    if (FN_PIPE.Length == 2)
                                    {
                                        Value += FN_PIPE[0].Trim();
                                        Value += dr[FN_PIPE[1].Trim()].ToString();
                                    }
                                    else Value = dr[drconf["FIELD_NAME"].ToString()].ToString();
                                    InfoPos += ";" + Value + "|" +
                                                     drconf["FIELD_ROW"].ToString() + "|" +
                                                     drconf["FIELD_COL"].ToString();
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }

                string sColor = "0", sBackColor = "0", ParentID = "", OrangeLevel = "";
                sBackColor = "0";
                if (View == "OV")
                {
                    OrangeLevel = "0"; ParentID = "-1";
                    if (dr[OPR_LEVEL_ID].ToString() != ShowLevel)
                    {
                        OrangeLevel = "1";
                        ParentID = dr[OPR_PARENT_ID].ToString();
                    }
                    if (dr["NOR_COUNT"].ToString() != "0")
                    {
                        AddPositionID(dr[OPR_LEVEL_ID].ToString());
                        SetFlag = "Y";
                    }
                    else SetFlag = "N";
                    intIdx++;
                    lstObjectInf.Add(new ObjectInfPDF(dr[OPR_LEVEL_ID].ToString(),
                                                        InfoPos.Substring(1),
                                                        ParentID,
                                                        OrangeLevel,
                                                        0, 0, 0, 0, 0, 0, 175, Original_Height,
                                                        SetFlag,
                                                        dr["GRAY_COLORED_FLAG"].ToString(),
                                                        dr["DOTTED_LINE_FLAG"].ToString(),
                                                        dr["SHOW_FULL_BOX"].ToString(),
                                                        dr["LANGUAGE_SELECTED"].ToString(),
                                                        //(++SortNo).ToString(),
                                                        (dr["SORTNO"].ToString()=="0")?(++SortNo).ToString(): dr["SORTNO"].ToString(),
                                                        dr["SOC_COUNT"].ToString(),
                                                        dr["NOR_COUNT"].ToString(),
                                                        dr["POSITIONFLAG"].ToString(),
                                                        sColor,
                                                        sBackColor,
                                                        dr["FLAG"].ToString(),
                                                        dr["BREAD_GRAM"].ToString(),
                                                        dr["BREAD_GRAM_NAME"].ToString(),
                                                        Convert.ToInt32(dr["LEVEL_NO"].ToString()), 
                                                        0, 0, 0, 0, 0));
                }
                else if (View == "LV")
                {
                    OrangeLevel = "0"; ParentID = "-1";
                    if (dr[LGL_LEVEL_ID].ToString() != ShowLevel)
                    {
                        OrangeLevel = "1";
                        ParentID = dr[LGL_PARENT_ID].ToString();
                    }
                    if (Convert.ToInt32(dr["NOR_COUNT"].ToString().Trim()) != 0)
                    {
                        AddPositionID(dr[LGL_LEVEL_ID].ToString());
                        SetFlag = "Y";
                    }
                    else SetFlag = "N";
                    intIdx++;
                    lstObjectInf.Add(new ObjectInfPDF(dr[LGL_LEVEL_ID].ToString(),
                                                        InfoPos.Substring(1),
                                                        ParentID,
                                                        OrangeLevel,
                                                        0, 0, 0, 0, 0, 0, 175, Original_Height,
                                                        SetFlag,
                                                        dr["GRAY_COLORED_FLAG"].ToString(),
                                                        dr["DOTTED_LINE_FLAG"].ToString(),
                                                        dr["SHOW_FULL_BOX"].ToString(),
                                                        dr["LANGUAGE_SELECTED"].ToString(),
                                                        //(++SortNo).ToString(),
                                                        (dr["SORTNO"].ToString() == "0") ? (++SortNo).ToString() : dr["SORTNO"].ToString(),
                                                        dr["SOC_COUNT"].ToString(),
                                                        dr["NOR_COUNT"].ToString(),
                                                        dr["POSITIONFLAG"].ToString(),
                                                        sColor,
                                                        sBackColor,
                                                        dr["FLAG"].ToString(),
                                                        dr["BREAD_GRAM"].ToString(),
                                                        dr["BREAD_GRAM_NAME"].ToString(),
                                                        Convert.ToInt32(dr["LEVEL_NO"].ToString()), 
                                                        0, 0, 0, 0, 0));
                }
            }

            return intIdz;
        }

        // Gets the Object Info from the table
        private string[] GetPDFLevelInfo(DataRow[] rowDR, string Level, string LevelNo, string TemplateName, string DownloadType)
        {
            string ShowLevel = Level;
            DataTable dtTable = rowDR.CopyToDataTable();
            DataRow[] dtlbl = null;
            int iIndex = 0, SOC_COUNT = 0;
            string[] Ret = { "0", "0", "", "" };

            lstObjectInf.Clear();
            if (View == "OV")
            {
                dtlbl = dtTable.Select("LEVEL_ID='" + Level + "'");
                SOC_COUNT = Convert.ToInt32(dtlbl[0]["NOR_COUNT"]);
                iIndex = SetPDFlevelInfo(dtlbl, Level, TemplateName, DownloadType);
                dtlbl = dtTable.Select("PARENT_LEVEL_ID='" + Level + "'");
                iIndex = SetPDFlevelInfo(dtlbl, Level, TemplateName, DownloadType);
            }
            else if (View == "LV")
            {
                dtlbl = dtTable.Select("LEVEL_ID='" + Level + "'");
                SOC_COUNT = Convert.ToInt32(dtlbl[0]["NOR_COUNT"]);
                iIndex = SetPDFlevelInfo(dtlbl, Level, TemplateName, DownloadType);
                dtlbl = dtTable.Select("PARENT_LEVEL_ID='" + Level + "'");
                iIndex = SetPDFlevelInfo(dtlbl, Level, TemplateName, DownloadType);
            }

            // Direct and Next Level Info
            foreach (DataRow dr in dtlbl)
            {
                Ret[3] += "," + dr["LEVEL_ID"].ToString();
                if (Convert.ToUInt32(dr["SOC_COUNT"]) >= 1) Ret[2] += "," + dr["LEVEL_ID"].ToString();
            }

            List<ObjectInfPDF> ObjectInf = lstObjectInf.OrderBy(o => o.Level).ToList();
            if (lstObjectInf.Count >= 2)
            {
                string json = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(ObjectInf);
                sData1 = ""; sData2 = ""; sData3 = ""; sData4 = "";
                sData1 = json.Substring(0, (json.Length - 0) >= 7000 ? 7000 : json.Length);
                if (json.Length >= 7001) sData2 = json.Substring(7000, (json.Length - 7000) >= 7000 ? 7000 : (json.Length - 7000));
                if (json.Length >= 14001) sData3 = json.Substring(14000, (json.Length - 14000) >= 14000 ? 7000 : (json.Length - 14000));
                if (json.Length >= 21001) sData4 = json.Substring(21000, (json.Length - 21000) >= 21000 ? 7000 : (json.Length - 21000));
                string keyDate = LevelDate.Substring(6, 4) + "-" + LevelDate.Substring(3, 2) + "-" + LevelDate.Substring(0, 2);
                if (View == "OV")
                {
                    sQData += "INSERT INTO " + dtHD_DIRECT_REPORT_DATA + " (DRIndex, PositionID, DrType, Data1, Data2, Data3, Data4, UpdatedDate, TemplateName, DownloadType) " +
                                                                         " VALUES('" + LevelNo + "','" + ShowLevel + "','" + View + "','" + 
                                                                                       sData1 + "','" + sData2 + "','" + sData3 + "','" + sData4 + "','" + 
                                                                                       keyDate + "','" + TemplateName + "','" + DownloadType + "');";
                }
                else
                {
                    sQData += "INSERT INTO DIRECT_REPORT_DATA_LV (DRIndex, PositionID, DrType, Data1, Data2, Data3, Data4, UpdatedDate, TemplateName, DownloadType) " +
                                                       " VALUES('" + LevelNo + "','" + ShowLevel + "','" + View + "','" + 
                                                                     sData1 + "','" + sData2 + "','" + sData3 + "','" + sData4 + "','" + 
                                                                     keyDate + "','" + TemplateName + "','" + DownloadType + "');";
                }
            }

            Ret[0] = iIndex.ToString();
            Ret[1] = SOC_COUNT.ToString();
            Ret[2] = ((Ret[2] == "") ? "" : Ret[2].Substring(1));
            Ret[3] = ((Ret[3] == "") ? "" : Ret[3].Substring(1));

            return Ret;
        }


        public void AddPositionID(string PostionID)
        {
            //sParentLevelNo = sParentLevelNo + "," + PostionID;
            for (int Idx = 0; Idx <= lstID.Count - 1; Idx++)
            {
                if (lstID[Idx] == PostionID) return;
            }
            lstID.Add(PostionID);
            lstLevel.Add(iLevel.ToString());
        }

        // Create the all level(Position) PDF
        public string CreateAllLevelPDFIntermediateResults(DataSet OrgDataSet, string ShowType, 
                                                           string VersionNo, string TemplateName, 
                                                           string DownloadType)
        {
            DataTable OrgDataTable = OrgDataSet.Tables[0];
            string sQuery = "", sID = "";
            LoginUsers UserData = LI.GetLoginUserInfo("");
            string dtORG_TABLE = UserData.CompanyName + "_" + FinalyzerVersion.ToString() + "_";

            if (ShowType == "OV")
            {
                dtORG_TABLE += "LevelInfos";
                string sSQL = "DELETE FROM " + dtHD_DIRECT_REPORT + " WHERE TemplateName='"+ TemplateName + "' AND DownloadType='"+ DownloadType + "';" +
                              "DELETE FROM " + dtHD_DIRECT_REPORT_DATA + " WHERE TemplateName='" + TemplateName + "' AND DownloadType='" + DownloadType + "';";
                SqlCommand SqlCmd = null;
                using (SqlConnection SqlDelCon = new SqlConnection(OrgConnection))
                {

                    SqlCmd = new SqlCommand(sSQL, SqlDelCon);
                    SqlDelCon.Open();
                    SqlCmd.CommandTimeout = 0;
                    SqlCmd.CommandType = CommandType.Text;
                    SqlCmd.ExecuteReader();
                    SqlCmd.Dispose();
                    SqlCmd = null;
                }

                using (SqlConnection SqlCon = new SqlConnection(OrgConnection))
                {
                    SqlCon.Open();

                    // Gets all information
                    string keyDate = LevelDate.Substring(6, 4) + "-" + LevelDate.Substring(3, 2) + "-" + LevelDate.Substring(0, 2);
                    string uName = UserData.UserName;

                    //dtDR = csobj.SQLReturnDataTable("SELECT * FROM " + dtORG_TABLE + " WHERE VERSION='" + VersionNo + "'");
                    dtDR = OrgDataTable;

                    int Idz = 0; iLevel = 0;
                    sQuery = ""; lstID.Add(Level);
                    lstLevel.Add(iLevel.ToString());
                    for (int Idy = 0; Idy <= lstID.Count - 1; Idy++)
                    {
                        try
                        {
                            DataRow[] rowDR = dtDR.Select("LEVEL_ID='" + lstID[Idy].ToString() + "' OR PARENT_LEVEL_ID='" + lstID[Idy].ToString() + "'");
                            if (rowDR != null && rowDR.Count() >= 1)
                            {
                                // Collects the Hierarchy data to show 
                                string[] Values = GetPDFLevelInfo(rowDR, lstID[Idy].ToString(), (Convert.ToInt16(lstLevel[Idy]) + 1).ToString(), TemplateName, DownloadType);

                                sQuery += "INSERT INTO " + dtHD_DIRECT_REPORT + " (PositionID, KeyDate, DirectReport, NextLevel, DrType, Country, LevelNo, SOC, MaxLevel, TemplateName, DownloadType) " +
                                                                " VALUES('" + lstID[Idy].ToString() + "','" + keyDate + "','" +
                                                                              Values[2] + "','" + Values[3] + "','OV','','" + (Convert.ToInt16(lstLevel[Idy]) + 1).ToString() + "','" +
                                                                              Values[1] + "','" + Values[0] + "','"+ TemplateName + "','"+ DownloadType + "');";

                                iLevel = Convert.ToInt16(lstLevel[Idy]);
                                iLevel++;

                                if (((Idz + 1) % 1000) == 0)
                                {
                                    csobj.ExecuteQuery(sQuery, SqlCon);
                                    csobj.ExecuteQuery(sQData, SqlCon);
                                    sQuery = ""; sQData = ""; Idz = 0;
                                }
                                else Idz++;

                                DataRow[] dtlbl = dtDR.Select("PARENT_LEVEL_ID='" + lstID[Idy].ToString() + "'");
                                for (int Idx = 0; Idx <= dtlbl.Length - 1; Idx++)
                                {
                                    if ((dtlbl[Idx]["SOC_COUNT"].ToString() != "0"))
                                    {
                                        AddPositionID(dtlbl[Idx]["LEVEL_ID"].ToString());
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            sID += "," + lstID[Idy].ToString();
                        }
                    }
                    if (sQuery != "") csobj.ExecuteQuery(sQuery, SqlCon);
                    if (sQData != "") csobj.ExecuteQuery(sQData, SqlCon);

                    dtDR.Rows.Clear();
                    dtDR.Columns.Clear();
                    dtDR = null;
                }
            }
            else if (ShowType == "LV")
            {
                dtORG_TABLE += "LegalInfos";
                string sSQL = "DELETE FROM " + dtHD_DIRECT_REPORT_LV + " WHERE TemplateName='" + TemplateName + "' AND DownloadType='" + DownloadType + "';" +
                              "DELETE FROM " + dtHD_DIRECT_REPORT_DATA_LV + " WHERE TemplateName='" + TemplateName + "' AND DownloadType='" + DownloadType + "';";
                SqlCommand SqlCmd = null;
                using (SqlConnection SqlDelCon = new SqlConnection(OrgConnection))
                {

                    SqlCmd = new SqlCommand(sSQL, SqlDelCon);
                    SqlDelCon.Open();
                    SqlCmd.CommandTimeout = 0;
                    SqlCmd.CommandType = CommandType.Text;
                    SqlCmd.ExecuteReader();
                    SqlCmd.Dispose();
                    SqlCmd = null;
                }

                int Idz = 0;
                sSQL = "SELECT * FROM LegalCountries";
                DataTable dtCOUNTRY = csobj.SQLReturnDataTable(sSQL);

                for (int IdCountry = 0; IdCountry <= dtCOUNTRY.Rows.Count - 1; IdCountry++)
                {

                    using (SqlConnection SqlCon = new SqlConnection(OrgConnection))
                    {
                        SqlCon.Open();

                        // Gets all information
                        string keyDate = LevelDate.Substring(6, 4) + "-" + LevelDate.Substring(3, 2) + "-" + LevelDate.Substring(0, 2);
                        string uName = UserData.UserName;

                        dtDR = csobj.SQLReturnDataTable("SELECT * FROM " + dtORG_TABLE + " WHERE VERSION='" + VersionNo + "' AND BREAD_GRAM like '%" + dtCOUNTRY.Rows[IdCountry]["OrgUnit"].ToString() + "%'");
                        if (dtDR != null)
                        {
                            if (dtDR.Rows.Count >= 1)
                            {
                                int iLevel = 0; Level = dtCOUNTRY.Rows[IdCountry]["OrgUnit"].ToString();
                                sQuery = ""; sQData = ""; lstID.Clear(); lstID.Add(Level);
                                lstLevel.Clear(); lstLevel.Add(iLevel.ToString());
                                for (int Idy = 0; Idy <= lstID.Count - 1; Idy++)
                                {
                                    try
                                    {
                                        DataRow[] rowDR = dtDR.Select("LEVEL_ID='" + lstID[Idy].ToString() + "' OR PARENT_LEVEL_ID='" + lstID[Idy].ToString() + "'");
                                        if (rowDR != null && rowDR.Count() >= 1)
                                        {
                                            // Collects the Hierarchy data to show 
                                            string[] Values = GetPDFLevelInfo(rowDR, lstID[Idy].ToString(), (Convert.ToInt16(lstLevel[Idy]) + 1).ToString(), TemplateName, DownloadType);

                                            sQuery += "INSERT INTO " + dtHD_DIRECT_REPORT_LV + " (PositionID, KeyDate, DirectReport, NextLevel, DrType, Country, LevelNo, SOC, MaxLevel, TemplateName, DownloadType) " +
                                                                            " VALUES('" + lstID[Idy].ToString() + "','" + keyDate + "','" +
                                                                                          Values[2] + "','" + Values[3] + "','OV','','" + (Convert.ToInt16(lstLevel[Idy]) + 1).ToString() + "','" +
                                                                                          Values[1] + "','" + Values[0] + "','" + TemplateName + "','" + DownloadType + "');";

                                            iLevel = Convert.ToInt16(lstLevel[Idy]);
                                            iLevel++;

                                            if (((Idz + 1) % 1000) == 0)
                                            {
                                                csobj.ExecuteQuery(sQuery, SqlCon);
                                                csobj.ExecuteQuery(sQData, SqlCon);
                                                sQuery = ""; sQData = ""; Idz = 0;
                                            }
                                            else Idz++;

                                            DataRow[] dtlbl = dtDR.Select("PARENT_LEVEL_ID='" + lstID[Idy].ToString() + "'");
                                            for (int Idx = 0; Idx <= dtlbl.Length - 1; Idx++)
                                            {
                                                if ((dtlbl[Idx]["SOC_COUNT"].ToString() != "0"))
                                                {
                                                    AddPositionID(dtlbl[Idx]["LEVEL_ID"].ToString());
                                                }
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        sID += "," + lstID[Idy].ToString();
                                    }
                                }
                                if (sQuery != "") csobj.ExecuteQuery(sQuery, SqlCon);
                                if (sQData != "") csobj.ExecuteQuery(sQData, SqlCon);

                                dtDR.Rows.Clear();
                                dtDR.Columns.Clear();
                                dtDR = null;
                            }
                        }
                    }
                }
            }

            return "";
        }

        // Create the all level PDF
        public string CreateAllLevelPDF(string ShowType, DateTime fixtureDate, string TemplateName, string DownloadType)
        {
            //DateTime fixtureDate = DateTime.Now;
            string KeyDate = fixtureDate.Year.ToString() + fixtureDate.Month.ToString("d2") + fixtureDate.Day.ToString("d2");
            string LastRefresh = "";
            string pptLayoutPath = HttpContext.Current.Server.MapPath("~/Content/PPTX/") + "OrgChartInfo.pptx";
            string pptFilePath = HttpContext.Current.Server.MapPath("~/Content/PPTX/") + HttpContext.Current.Session["ID"].ToString() + "_" + fixtureDate.ToString("yyyyMMddhhmmss") + "_OrgChart.pptx";

            string sID = "", sTPage = "", sCPage = "", strCVIEW = "";
            strCVIEW = "VIEW_DEFAULT";
            HttpContext.Current.Session["VIEW"] = strCVIEW;

            if (View == "OV")
            {
                dtFieldInformation = csobj.SQLReturnDataTable("SELECT * FROM LEVEL_CONFIG_INFO " +
                                                              " WHERE VIEW_ID='" + strCVIEW + "' AND TEMPLATE_NAME='" + TemplateName + "' AND " +
                                                              "       DOWNLOAD_TYPE='" + DownloadType + "' AND COMPANY_NAME='" + CompanyName + "'");
                dtFieldActive = csobj.SQLReturnDataTable("SELECT * FROM LEVEL_CONFIG_INFO " +
                                                         " WHERE VIEW_ID='" + strCVIEW + "' AND ACTIVE_IND='Y' AND TEMPLATE_NAME='" + TemplateName + "' AND " +
                                                         " DOWNLOAD_TYPE='" + DownloadType + "' AND COMPANY_NAME='" + CompanyName + "'");
            }
            else if (View == "LV")
            {
                dtFieldInformation = csobj.SQLReturnDataTable("SELECT * FROM LEGAL_CONFIG_INFO " +
                                                              " WHERE VIEW_ID='" + strCVIEW + "' AND TEMPLATE_NAME='" + TemplateName + "' AND " +
                                                              "       DOWNLOAD_TYPE='" + DownloadType + "' AND COMPANY_NAME='" + CompanyName + "'");
                dtFieldActive = csobj.SQLReturnDataTable("SELECT * FROM LEGAL_CONFIG_INFO " +
                                                         " WHERE VIEW_ID='" + strCVIEW + "' AND ACTIVE_IND='Y' AND TEMPLATE_NAME='" + TemplateName + "' AND " +
                                                         "       DOWNLOAD_TYPE='" + DownloadType + "' AND COMPANY_NAME='" + CompanyName + "'");
            }
            DataTable dtlevel = csobj.SQLReturnDataTable("SELECT * FROM ORG_CONFIG_INFO WHERE VIEW_ID='" + strCVIEW + "'");

            foreach (DataRow drlvl in dtlevel.Rows)
            {
                if (drlvl["FIELD_NAME"].ToString() == "LEVEL") LevelUpto = LevelFlag;
                if (drlvl["FIELD_NAME"].ToString() == "HEIGHT") Height = drlvl["FIELD_VALUE"].ToString();
                if (drlvl["FIELD_NAME"].ToString() == "LINECOLOR")
                {
                    LineColor = drlvl["FIELD_VALUE"].ToString();
                }
                if (drlvl["FIELD_NAME"].ToString() == "LINEWIDTH")
                {
                    LineWidth = drlvl["FIELD_VALUE"].ToString();
                    blackPen = new Pen(GetDrawingLineColor(LineColor), Convert.ToInt16(LineWidth));
                }
                if (drlvl["FIELD_NAME"].ToString() == "TEMPLATE")
                {
                    TemplateURL = drlvl["FIELD_VALUE"].ToString();
                    string[] TempURL = TemplateURL.Split('~');
                    if (TempURL[0] == "IMG")
                    {
                        BmpURL = new Bitmap(System.Web.HttpContext.Current.Server.MapPath(TempURL[1]));
                        BmpURL.SetResolution(1200f, 1200f);
                    }
                    BmpUpArrow = new Bitmap(System.Web.HttpContext.Current.Server.MapPath("~/Content/images/uparrow.jpg"));
                    BmpDnArrow = new Bitmap(System.Web.HttpContext.Current.Server.MapPath("~/Content/images/downarrow.jpg"));
                }
            }

            // Gets the Box Height
            Output_Height = 1020;
            Original_Height = Convert.ToInt32(Height);
            Original_Height_10 = Original_Height + 40;
            Adjustment_Height = ((((Original_Height - (Original_Height % 30)) / 30)) * LevelCount) - 1;
            HttpContext.Current.Session["BoxHeight"] = Original_Height;

            string sSQL = "";
            DataTable dtObj = null;
            if (View == "OV")
            {
                sSQL = "SELECT DISTINCT a.PositionID, a.KeyDate, a.DirectReport, a.NextLevel, a.DrType, a.Country, a.LevelNo, a.MaxLevel, a.SOC SOC_COUNT, a.SOC NOR_COUNT " +
                            " FROM " + dtHD_DIRECT_REPORT + " a " +
                            " WHERE a.DrType='OV' AND a.TemplateName='" + TemplateName + "' AND a.DownloadType='" + DownloadType + "'";
                dtObj = csobj.SQLReturnDataTable("SELECT * FROM " + dtHD_DIRECT_REPORT_DATA + " WHERE DrType='" + View + "' AND TemplateName='" + TemplateName + "' AND DownloadType='" + DownloadType + "'");
            }
            else if (View == "LV")
            {
                sSQL = "SELECT DISTINCT a.PositionID, a.KeyDate, a.DirectReport, a.NextLevel, a.DrType, a.Country, a.LevelNo, a.MaxLevel, a.SOC SOC_COUNT, a.SOC NOR_COUNT " +
                            " FROM " + dtHD_DIRECT_REPORT_LV + " a " +
                            " WHERE a.DrType='OV' AND a.TemplateName='" + TemplateName + "' AND a.DownloadType='" + DownloadType + "'";
                dtObj = csobj.SQLReturnDataTable("SELECT * FROM " + dtHD_DIRECT_REPORT_DATA_LV + " WHERE DrType='" + View + "' AND TemplateName='" + TemplateName + "' AND DownloadType='" + DownloadType + "'");
            }
            DataTable dtLevelInfo = csobj.SQLReturnDataTable(sSQL);
            if (dtLevelInfo.Rows.Count >= 1) LastRefresh = dtLevelInfo.Rows[0]["KeyDate"].ToString();

            lstID.Clear();
            lstLevel.Clear();
            lstID.Add(HttpContext.Current.Session["ID"].ToString());
            lstPageNo.Add("1");

            DataRow[] drRow = dtLevelInfo.Select("POSITIONID='" + lstID[0].ToString() + "'");
            DataRow[] drNOR = dtLevelInfo.Select("POSITIONID='" + lstID[0].ToString() + "'");
            if (drRow.Length >= 1)
            {
                if (Convert.ToInt32(drRow[0]["SOC_COUNT"]) <= 60000)
                {
                    MyDocument = new ceTe.DynamicPDF.Document();
                    MyDocument.Creator = "DynamicChart.aspx";
                    MyDocument.Author = "Raj";
                    MyDocument.Title = "All Organization Chart";

                    int iLevel = 2;
                    iTotalPage = 0;
                    lstLevel.Add(iLevel.ToString());
                    for (int Idc = 0; Idc <= lstID.Count - 1; Idc++)
                    {
                        try
                        {
                            drRow = dtLevelInfo.Select("POSITIONID='" + lstID[Idc].ToString() + "'");
                            if (drRow.Length >= 1)
                            {

                                SetPositionPageNo(lstID[Idc].ToString(), iTotalPage + 1);
                                if (drRow[0]["NextLevel"].ToString() != "")
                                {
                                    string[] LevelInf = drRow[0]["NextLevel"].ToString().Split(',');
                                    for (int Ida = 0; Ida <= LevelInf.Length - 1; Ida++)
                                    {
                                        if (dtLevelInfo.Select("POSITIONID='" + LevelInf[Ida].ToString() + "'").Count() >= 1)
                                        {
                                            lstID.Add(LevelInf[Ida].ToString());
                                            lstPageNo.Add((iTotalPage + 1).ToString());
                                        }
                                        iLevel = Convert.ToInt32(drRow[0]["LevelNo"].ToString());
                                    }

                                    DataRow[] drObj = dtObj.Select("POSITIONID='" + lstID[Idc].ToString() + "'");
                                    foreach (DataRow drInf in drObj)
                                    {
                                        sTPage = (Convert.ToInt32(drRow[0]["MaxLevel"].ToString()) + 1).ToString();
                                        sCPage = (Convert.ToInt32(drInf["DRIndex"].ToString()) + 1).ToString();
                                        string jsonString = drInf["Data1"].ToString();
                                        if (drInf["Data2"] != null) jsonString += drInf["Data2"].ToString();
                                        if (drInf["Data3"] != null) jsonString += drInf["Data3"].ToString();
                                        if (drInf["Data4"] != null) jsonString += drInf["Data4"].ToString();
                                        jsonString = jsonString.Substring(0, jsonString.IndexOf("}]") + 2);
                                        List<ObjectInf> theObjectInf = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<List<ObjectInf>>(jsonString);

                                        foreach (ObjectInf Obj in theObjectInf)
                                        {
                                            Obj.Oheight = Convert.ToInt32(Height);
                                        }

                                        LevelInf = GetAllLevels(theObjectInf).Split(',');
                                        LevelIds = new string[LevelInf.Length];
                                        if (LevelInf.Length >= 1)
                                        {
                                            // Get Level based Ids
                                            for (int Idx = 0; Idx <= LevelInf.Length - 1; Idx++)
                                            {
                                                LevelIds[Convert.ToInt16(LevelInf[Idx])] = LevelInId(theObjectInf, LevelInf[Idx], Idx);
                                            }

                                            // Calculates the Height for each Ids
                                            int iWidth = 255, FLC = 0;
                                            if (LevelCount == 6) iWidth = 255;

                                            ObjectInf Obj = null, CurObj = null, PrevObj = null;
                                            Obj = GetIdInfo(theObjectInf, LevelIds[0].ToString());
                                            Obj.Height = Original_Height_10;
                                            Obj.Width = iWidth;
                                            string sSkip = "N";
                                            FLC = LevelIds[1].Split(',').Length;
                                            for (int Idx = LevelIds.Length - 1; Idx >= 1; Idx--)
                                            {
                                                string[] Ids = LevelIds[Idx].Split(',');
                                                if (Idx == 1) FLC = Ids.Length;
                                                for (int Idy = 0; Idy <= Ids.Length - 1; Idy++)
                                                {
                                                    sSkip = "N";
                                                    if (Idx == 1) Obj = theObjectInf[Idy + 1]; else Obj = theObjectInf[Idy + FLC + 1];
                                                    if (Idx == 2) sSkip = Level2Check(LevelIds[1], Obj.PId);
                                                    if (sSkip == "N")
                                                    {
                                                        if (Obj.Level == "2")                // Last Level
                                                        {
                                                            Obj.Height = Original_Height_10 - 22;
                                                            Obj.Width = iWidth;
                                                        }
                                                        else
                                                        {
                                                            Obj.Height = GetChildHeight(theObjectInf, Ids[Idy].ToString());
                                                            Obj.Width = iWidth;
                                                        }
                                                    }
                                                }
                                            }

                                            // Calculates the Row and Column info for each Ids
                                            int Row = 20, Col = 352, AddHeight = 0, MaxHeight = 0;
                                            FLC = 0;
                                            Obj = GetIdInfo(theObjectInf, LevelIds[0].ToString());
                                            Obj.Row = Row;
                                            Obj.Col = Col;
                                            for (int Idx = 1; Idx <= LevelIds.Length - 1; Idx++)
                                            {
                                                string[] Ids = LevelIds[Idx].Split(',');
                                                if (Idx == 1) FLC = Ids.Length;
                                                for (int Idy = 0; Idy <= Ids.Length - 1; Idy++)
                                                {
                                                    sSkip = "N";
                                                    if (Idx == 1)
                                                    {
                                                        //CurObj = GetIdInfo(theObjectInf, Ids[Idy].ToString());
                                                        CurObj = theObjectInf[Idy + 1];
                                                    }
                                                    else
                                                        CurObj = theObjectInf[Idy + FLC + 1];

                                                    if (Idx == 2) sSkip = Level2Check(LevelIds[1], CurObj.PId);
                                                    if (sSkip == "N")
                                                    {
                                                        if ((Idx == 1) && ((Idy % LevelCount) == 0) && (Idy != 0))
                                                        {
                                                            AddHeight += MaxHeight;
                                                            MaxHeight = 0;
                                                        }

                                                        if (Idx == 1)
                                                        {
                                                            CurObj = theObjectInf[Idy + 1];
                                                            Obj = GetIdInfo(theObjectInf, CurObj.PId);             // To get the Row in which this object is displayed

                                                            Row = Obj.Row + Obj.Height + AddHeight;
                                                            Col = ((Idy % LevelCount) * iWidth) + 5;
                                                            if (MaxHeight <= CurObj.Height) MaxHeight = CurObj.Height;
                                                        }
                                                        else
                                                        {
                                                            CurObj = theObjectInf[Idy + FLC + 1];
                                                            Obj = GetIdInfo(theObjectInf, CurObj.PId);             // To get the Row in which this object is displayed

                                                            Row = Obj.Row;
                                                            for (int Idz = 0; Idz <= Ids.Length - 1; Idz++)
                                                            {
                                                                PrevObj = theObjectInf[Idz + FLC + 1];
                                                                if (PrevObj.PId == Obj.Id)
                                                                {
                                                                    Row += PrevObj.Height;
                                                                    Col = Obj.Col + 15;
                                                                    if (PrevObj.Id == Ids[Idy].ToString()) break;
                                                                }
                                                            }
                                                        }
                                                        CurObj.Row = Row;
                                                        CurObj.Col = Col;
                                                    }
                                                }
                                            }
                                        }

                                        if (ShowType == "PDF")
                                        {
                                            PageSizeNos.Clear();

                                            CurPage = 0; MaxPage = 1; strOutput = "PDF";
                                            PDFPage = CurPage; sID = "";
                                            PageSizeNos.Add(0);
                                            MyPage[CurPage] = new ceTe.DynamicPDF.Page(1600F, 1050F, 15.3F);

                                            // Places the Level Information in the PDF
                                            LevelUpto = "1";

                                            int LevelShowUpto = Convert.ToInt32(LevelUpto);
                                            if ((LevelIds.Length >= 3) && (LevelShowUpto >= 2)) Level_Information(theObjectInf);         // Level without child Information 
                                            if ((LevelIds.Length >= 2) && (LevelShowUpto >= 1)) Level_1_Information(theObjectInf);       // Level 1 Information 
                                            if ((LevelIds.Length >= 2) && (LevelShowUpto >= 1)) Level_1_Line_Information(theObjectInf);  // Level 1 Line Information 
                                            if ((LevelIds.Length >= 1) && (LevelShowUpto >= 0)) Level_0_Information(theObjectInf, "I", sTPage, sCPage);  // Level 0 Information

                                            for (int Idx = 0; Idx <= MaxPage - 1; Idx++)
                                            {
                                                MyAllPage[iTotalPage] = MyPage[Idx];
                                                pageLevel.Add((iLevel).ToString());
                                                iTotalPage++;
                                            }
                                        }
                                        else if (ShowType == "PPT")
                                        {
                                            // Creates the Document
                                            CurPage = 0; MaxPage = 1; LevelUpto = "1"; strOutput = "PPT";
                                            PageSizeNos.Clear();
                                            PageSizeNos.Add(0);
                                            ImageOut[CurPage] = new Bitmap(1600, Output_Height + 30);

                                            // Set DPI of image (xDpi, yDpi)
                                            ImageOut[CurPage].SetResolution(256.0F, 256.0F);

                                            GraphicImg[CurPage] = Graphics.FromImage(ImageOut[CurPage]);
                                            GraphicImg[CurPage].InterpolationMode = InterpolationMode.HighQualityBicubic;
                                            GraphicImg[CurPage].Clear(System.Drawing.Color.White);

                                            // Places the Level Information in the PDF
                                            int LevelShowUpto = Convert.ToInt32(LevelUpto);
                                            if ((LevelIds.Length >= 3) && (LevelShowUpto >= 2)) Level_Information(theObjectInf);         // Level without child Information 
                                            if ((LevelIds.Length >= 2) && (LevelShowUpto >= 1)) Level_1_Information(theObjectInf);       // Level 1 Information 
                                            if ((LevelIds.Length >= 2) && (LevelShowUpto >= 1)) Level_1_Line_Information(theObjectInf);  // Level 1 Line Information 
                                            if ((LevelIds.Length >= 1) && (LevelShowUpto >= 0)) Level_0_Information(theObjectInf, "I", sTPage, sCPage);       // Level 0 Information


                                            //Add MyImages to MyDocument
                                            for (int Idx = 0; Idx <= MaxPage - 1; Idx++)
                                            {
                                                System.Drawing.Font drawFontText = new System.Drawing.Font("Arail", 10, FontStyle.Bold, GraphicsUnit.Pixel);
                                                SolidBrush drawBrushText = new SolidBrush(System.Drawing.Color.Black);
                                                StringFormat stringFormat = new StringFormat();
                                                stringFormat.Alignment = StringAlignment.Near;

                                                System.Drawing.Rectangle rectLabel = new System.Drawing.Rectangle(5, 975, 200, 20);
                                                GraphicImg[Idx].DrawString("Level No : N - " + iLevel.ToString(), drawFontText, drawBrushText, rectLabel, stringFormat);
                                                rectLabel = new System.Drawing.Rectangle(1400, 975, 200, 20);
                                                GraphicImg[Idx].DrawString("Page No : " + (iTotalPage + 1).ToString(), drawFontText, drawBrushText, rectLabel, stringFormat);
                                                string sText = "This chart indicates operational and functional coordination relationships supporting the global business. The actual company relationships of the individuals on this chart are to executives within the companies that employ them. For business use only. (source: HR Core) Last updated on " + LastRefresh + " at 1am CET";
                                                rectLabel = new System.Drawing.Rectangle(5, 995, 1550, 20);
                                                GraphicImg[Idx].DrawString(sText, drawFontText, drawBrushText, rectLabel, stringFormat);

                                                string imgFileName100 = HttpContext.Current.Server.MapPath("~/Content/PPTX/") + "OrgChart_100_" + iTotalPage.ToString() + ".jpg";
                                                ImageOut[Idx].SetResolution(256.0F, 256.0F);

                                                // Create an Encoder object based on the GUID
                                                // for the Quality parameter category.
                                                System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;

                                                // Create an EncoderParameters object.
                                                // An EncoderParameters object has an array of EncoderParameter
                                                // objects. In this case, there is only one
                                                // EncoderParameter object in the array.
                                                ImageCodecInfo jgpEncoder = GetEncoder(ImageFormat.Jpeg);
                                                EncoderParameters myEncoderParameters = new EncoderParameters(1);
                                                EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 100L);
                                                myEncoderParameters.Param[0] = myEncoderParameter;
                                                ImageOut[Idx].Save(imgFileName100, jgpEncoder, myEncoderParameters);
                                                ImageOut[Idx].Dispose();

                                                iTotalPage++;
                                                pageLevel.Add((iLevel).ToString());
                                            }
                                        }

                                        theObjectInf.Clear();
                                        jsonString = null;
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            sID += "," + lstID[Idc].ToString();
                        }
                    }

                    try
                    {
                        if (ShowType == "PDF")
                        {
                            for (int Idx = 0; Idx <= iTotalPage - 1; Idx++)
                            {
                                MyLabel = new ceTe.DynamicPDF.PageElements.Label("Level No : N - " + pageLevel[Idx].ToString(), 5, 975, 200, 20, ceTe.DynamicPDF.Font.HelveticaBold, 15, ceTe.DynamicPDF.TextAlign.Left);
                                MyAllPage[Idx].Elements.Add(MyLabel);

                                MyLabel = new ceTe.DynamicPDF.PageElements.Label("Page No : " + (Idx + 1).ToString(), 1400, 975, 200, 20, ceTe.DynamicPDF.Font.HelveticaBold, 15, ceTe.DynamicPDF.TextAlign.Left);
                                MyAllPage[Idx].Elements.Add(MyLabel);

                                string LevelDate = HttpContext.Current.Session["LEVEL_DATE"].ToString();
                                string keyDate = LevelDate.Substring(6, 4) + "-" + LevelDate.Substring(3, 2) + "-" + LevelDate.Substring(0, 2);
                                string sText = "This chart indicates operational and functional coordination relationships supporting the global business. The actual company relationships of the individuals on this chart are to executives within the companies that employ them. For business use only. (source: HR Core) Last updated on " + LastRefresh + " at 1am CET";
                                sText = "";
                                MyLabel = new ceTe.DynamicPDF.PageElements.Label(sText, 5, 995, 1550, 20, ceTe.DynamicPDF.Font.HelveticaBold, 10, ceTe.DynamicPDF.TextAlign.Left);
                                MyAllPage[Idx].Elements.Add(MyLabel);

                                MyDocument.Pages.Add(MyAllPage[Idx]);
                            }

                            if (thePageObjectInf.Count >= 1)
                            {
                                foreach (PageObjectInf PObj in thePageObjectInf)
                                {
                                    try
                                    {
                                        string baseUrl = HttpContext.Current.Request.Url.Scheme + "://" +
                                                            HttpContext.Current.Request.Url.Authority +
                                                            HttpContext.Current.Request.ApplicationPath.TrimEnd('/') + "/";
                                        KeyDate = fixtureDate.Year.ToString() + fixtureDate.Month.ToString("d2") + fixtureDate.Day.ToString("d2");
                                        if (PObj.PageNo != 0)
                                        {
                                            XYDestination dest = new XYDestination(PObj.PageNo, 1, 1);
                                            MyLink = new ceTe.DynamicPDF.PageElements.Link(PObj.Col + 40, PObj.Row, 200, 20, dest);
                                            if (PObj.PageNo <= iTotalPage) MyAllPage[PObj.CurPageNo].Elements.Add(MyLink);

                                            MyLink = new ceTe.DynamicPDF.PageElements.Link(PObj.Col - 90, PObj.Row - 60, 200, 20, dest);
                                            if (PObj.PageNo <= iTotalPage) MyAllPage[PObj.CurPageNo].Elements.Add(MyLink);
                                        }
                                        MyLabel = new ceTe.DynamicPDF.PageElements.Label("( # " + PObj.PageNo.ToString() + " )", PObj.Col + 40, PObj.Row, 200, 20, ceTe.DynamicPDF.Font.HelveticaBold, 12, ceTe.DynamicPDF.TextAlign.Left);
                                        MyAllPage[PObj.CurPageNo].Elements.Add(MyLabel);
                                    }
                                    catch (Exception ex)
                                    {
                                    }
                                }
                            }

                            //Outputs the MyDocument to the current web MyPage
                            string sFP = HttpContext.Current.Server.MapPath("~/Content/PDF/" + lstID[0].ToString() + "_" + KeyDate + ".pdf");
                            MyDocument.Draw(sFP);

                            if (File.Exists(sFP))
                            {
                                // Clears the memory
                                dtLevelInfo.Rows.Clear();
                                dtLevelInfo.Columns.Clear();
                                dtLevelInfo = null;

                                dtObj.Rows.Clear();
                                dtObj.Columns.Clear();
                                dtObj = null;

                                lstID.Clear();
                                lstLevel.Clear();
                                lstPageNo.Clear();

                                GC.Collect();

                                FileInfo myDoc = new FileInfo(sFP);

                                HttpContext.Current.Response.Clear();
                                HttpContext.Current.Response.ContentType = "application/pdf";
                                HttpContext.Current.Response.AddHeader("content-disposition", "inline;filename=" + myDoc.Name);
                                HttpContext.Current.Response.AddHeader("Content-Length", myDoc.Length.ToString());
                                HttpContext.Current.Response.ContentType = "application/octet-stream";
                                HttpContext.Current.Response.WriteFile(myDoc.FullName);
                                HttpContext.Current.Response.End();

                                return "";
                            }
                        }
                        else if (ShowType == "PPT")
                        {
                            if (File.Exists(pptFilePath)) File.Delete(pptFilePath);
                            File.Copy(pptLayoutPath, pptFilePath);
                            using (PresentationDocument presentationDocument = PresentationDocument.Open(pptFilePath, true))
                            {
                                string filename = "", imageExt = "image/jpeg";
                                PresentationPart presentationPart = presentationDocument.PresentationPart;
                                for (int Idx = 0; Idx <= iTotalPage - 1; Idx++)
                                {
                                    filename = HttpContext.Current.Server.MapPath("~/Content/PPTX/") + "OrgChart_100_" + Idx.ToString() + ".jpg";
                                    Slide slide = new InsertImage().InsertSlide(presentationPart, "Blank");
                                    new InsertImage().InsertImageInLastSlide(slide, filename, imageExt);
                                    slide.Save();
                                    File.Delete(filename);
                                }
                                presentationDocument.PresentationPart.Presentation.Save();
                            }

                            // Deletes the first slide[Empty slide]
                            InsertImage dslide = new InsertImage();
                            dslide.DeleteSlide(pptFilePath, 0);
                            System.Threading.Thread.Sleep(20000);

                            // Clears the memory
                            dtLevelInfo.Rows.Clear();
                            dtLevelInfo.Columns.Clear();
                            dtLevelInfo = null;

                            dtObj.Rows.Clear();
                            dtObj.Columns.Clear();
                            dtObj = null;

                            lstID.Clear();
                            lstLevel.Clear();
                            lstPageNo.Clear();

                            GC.Collect();

                            HttpContext.Current.Response.Redirect("downloadFile.aspx?msg=" + pptFilePath, false);
                            HttpContext.Current.ApplicationInstance.CompleteRequest();
                        }
                    }
                    catch (Exception ex)
                    {
                        string sError = ex.Message;
                    }

                    // Clears the memory
                    dtLevelInfo.Rows.Clear();
                    dtLevelInfo.Columns.Clear();
                    dtLevelInfo = null;

                    dtObj.Rows.Clear();
                    dtObj.Columns.Clear();
                    dtObj = null;

                    lstID.Clear();
                    lstLevel.Clear();
                    lstPageNo.Clear();

                    GC.Collect();

                    return pptFilePath;
                }
                else return "Ignore";
            }
            else return "Nodata";
        }

        // Create the all level PPT
        public string CreateAllLevelTemplatePPT(string ShowType, DateTime fixtureDate, string TemplateName, string DownloadType)
        {
            string ImagePath = ConfigurationManager.AppSettings["ImagePath"].ToString();
            string KeyDate = fixtureDate.Year.ToString() + fixtureDate.Month.ToString("d2") + fixtureDate.Day.ToString("d2");
            string LastRefresh = "";
            string pptLayoutPath = HttpContext.Current.Server.MapPath("~/Content/PPTX/") + "OrgChartInfo.pptx";
            string pptFilePath = HttpContext.Current.Server.MapPath("~/Content/PPTX/") + HttpContext.Current.Session["ID"].ToString() + "_" + fixtureDate.ToString("yyyyMMddhhmmss") + "_OrgChart.pptx";

            int SlideIndex = 0;
            string sID = "", sTPage = "", sCPage = "", strCVIEW = "";
            strCVIEW = "VIEW_DEFAULT";
            HttpContext.Current.Session["VIEW"] = strCVIEW;

            if (View == "OV")
            {
                dtFieldInformation = csobj.SQLReturnDataTable("SELECT * FROM LEVEL_CONFIG_INFO " +
                                                              " WHERE VIEW_ID='" + strCVIEW + "' AND TEMPLATE_NAME='" + TemplateName + "' AND " +
                                                              " DOWNLOAD_TYPE='" + DownloadType + "' AND COMPANY_NAME='" + CompanyName + "'");
                dtFieldActive = csobj.SQLReturnDataTable("SELECT * FROM LEVEL_CONFIG_INFO " +
                                                         " WHERE VIEW_ID='" + strCVIEW + "' AND ACTIVE_IND='Y' AND TEMPLATE_NAME='" + TemplateName + "' AND " +
                                                         " DOWNLOAD_TYPE='" + DownloadType + "' AND COMPANY_NAME='" + CompanyName + "'");
            }
            else if (View == "LV")
            {
                dtFieldInformation = csobj.SQLReturnDataTable("SELECT * FROM LEGAL_CONFIG_INFO " +
                                                              " WHERE VIEW_ID='" + strCVIEW + "' AND TEMPLATE_NAME='" + TemplateName + "' AND " +
                                                              " DOWNLOAD_TYPE='" + DownloadType + "' AND COMPANY_NAME='" + CompanyName + "'");
                dtFieldActive = csobj.SQLReturnDataTable("SELECT * FROM LEGAL_CONFIG_INFO " +
                                                         " WHERE VIEW_ID='" + strCVIEW + "' AND ACTIVE_IND='Y' AND TEMPLATE_NAME='" + TemplateName + "' AND " +
                                                         " DOWNLOAD_TYPE='" + DownloadType + "' AND COMPANY_NAME='" + CompanyName + "'");
            }
            DataTable dtlevel = csobj.SQLReturnDataTable("SELECT * FROM ORG_CONFIG_INFO WHERE VIEW_ID='" + strCVIEW + "'");

            foreach (DataRow drlvl in dtlevel.Rows)
            {
                if (drlvl["FIELD_NAME"].ToString() == "LEVEL") LevelUpto = LevelFlag;
                if (drlvl["FIELD_NAME"].ToString() == "HEIGHT") Height = drlvl["FIELD_VALUE"].ToString();
                if (drlvl["FIELD_NAME"].ToString() == "LINECOLOR")
                {
                    LineColor = drlvl["FIELD_VALUE"].ToString();
                }
                if (drlvl["FIELD_NAME"].ToString() == "LINEWIDTH")
                {
                    LineWidth = drlvl["FIELD_VALUE"].ToString();
                    blackPen = new Pen(GetDrawingLineColor(LineColor), Convert.ToInt16(LineWidth));
                }
                if (drlvl["FIELD_NAME"].ToString() == "TEMPLATE")
                {
                    TemplateURL = drlvl["FIELD_VALUE"].ToString();
                    string[] TempURL = TemplateURL.Split('~');
                    if (TempURL[0] == "IMG")
                    {
                        BmpURL = new Bitmap(System.Web.HttpContext.Current.Server.MapPath(TempURL[1]));
                        BmpURL.SetResolution(1200f, 1200f);
                    }
                    BmpUpArrow = new Bitmap(System.Web.HttpContext.Current.Server.MapPath("~/Content/images/uparrow.jpg"));
                    BmpDnArrow = new Bitmap(System.Web.HttpContext.Current.Server.MapPath("~/Content/images/downarrow.jpg"));
                }
            }

            // Gets the Box Height
            Output_Height = 1020;
            Original_Height = Convert.ToInt32(Height);
            Original_Height_10 = Original_Height + 40;
            Adjustment_Height = ((((Original_Height - (Original_Height % 30)) / 30)) * LevelCount) - 1;
            HttpContext.Current.Session["BoxHeight"] = Original_Height;

            string sSQL = "";
            DataTable dtObj = null;
            if (View == "OV")
            {
                sSQL = "SELECT DISTINCT a.PositionID, a.KeyDate, a.DirectReport, a.NextLevel, a.DrType, a.Country, a.LevelNo, a.MaxLevel, a.SOC SOC_COUNT, a.SOC NOR_COUNT " +
                            " FROM " + dtHD_DIRECT_REPORT + " a " +
                            " WHERE a.DrType='OV' AND a.TemplateName='" + TemplateName + "' AND a.DownloadType='" + DownloadType + "'";
                dtObj = csobj.SQLReturnDataTable("SELECT * FROM " + dtHD_DIRECT_REPORT_DATA + " WHERE DrType='" + View + "' AND TemplateName='" + TemplateName + "' AND DownloadType='" + DownloadType + "'");
            }
            else if (View == "LV")
            {
                sSQL = "SELECT DISTINCT a.PositionID, a.KeyDate, a.DirectReport, a.NextLevel, a.DrType, a.Country, a.LevelNo, a.MaxLevel, a.SOC SOC_COUNT, a.SOC NOR_COUNT " +
                            " FROM " + dtHD_DIRECT_REPORT_LV + " a " +
                            " WHERE a.DrType='OV' AND a.TemplateName='" + TemplateName + "' AND a.DownloadType='" + DownloadType + "'";
                dtObj = csobj.SQLReturnDataTable("SELECT * FROM " + dtHD_DIRECT_REPORT_DATA_LV + " WHERE DrType='" + View + "' AND TemplateName='" + TemplateName + "' AND DownloadType='" + DownloadType + "'");
            }
            DataTable dtLevelInfo = csobj.SQLReturnDataTable(sSQL);
            if (dtLevelInfo.Rows.Count >= 1) LastRefresh = dtLevelInfo.Rows[0]["KeyDate"].ToString();

            // PPTx information
            string TemplatePPTX = "";
            int NoOfNodes = 0;
            DataTable dtPPTX = csobj.SQLReturnDataTable("SELECT * FROM PPTX_CONFIG_INFO WHERE TemplateName='" + TemplateName + "' AND CompanyName = '" + CompanyName + "'");
            if (dtPPTX!=null)
            {
                if (dtPPTX.Rows.Count >= 1)
                {
                    TemplatePPTX = dtPPTX.Rows[0]["FileName"].ToString();
                    NoOfNodes = Convert.ToInt32(dtPPTX.Rows[0]["NodeCount"].ToString());
                }
            }

            lstID.Clear();
            lstLevel.Clear();
            lstPageNo.Clear();
            lstID.Add(HttpContext.Current.Session["ID"].ToString());
            lstPageNo.Add("1");

            LoginUsers UserData = LI.GetLoginUserInfo("");

            string InitialFile = "Powerpoint_Templates.pptx";
            string InitialFilePath = HttpContext.Current.Server.MapPath("~/Content/PPTX/") + InitialFile;
            string SourceFile = TemplatePPTX;
            string SourceFilePath = HttpContext.Current.Server.MapPath("~/Content/PPTX/") + SourceFile;
            string DestinationFile = UserData.UserName.ToUpper() + "_" + DateTime.Now.ToString("yyyyMMdd") + "_" + DateTime.Now.ToString("hhmmss") + ".pptx";
            string DestinationFilePath = HttpContext.Current.Server.MapPath("~/Content/PPTX/") + DestinationFile;
            File.Copy(InitialFilePath, DestinationFilePath);

            // To insert that much slides at the last.
            DataRow[] drRow = dtLevelInfo.Select("POSITIONID='" + lstID[0].ToString() + "'");
            DataRow[] drNOR = dtLevelInfo.Select("POSITIONID='" + lstID[0].ToString() + "'");
            if (drRow.Length >= 1)
            {
                if (Convert.ToInt32(drRow[0]["SOC_COUNT"]) <= 60000)
                {
                    int iLevel = 2;
                    iTotalPage = 0; SlideIndex = 0;
                    lstLevel.Add(iLevel.ToString());
                    for (int Idc = 0; Idc <= lstID.Count - 1; Idc++)
                    {
                        try
                        {
                            drRow = dtLevelInfo.Select("POSITIONID='" + lstID[Idc].ToString() + "'");
                            if (drRow.Length >= 1)
                            {
                                SetPositionPageNo(lstID[Idc].ToString(), iTotalPage + 1);
                                if (drRow[0]["NextLevel"].ToString() != "")
                                {
                                    string[] LevelInf = drRow[0]["NextLevel"].ToString().Split(',');
                                    for (int Ida = 0; Ida <= LevelInf.Length - 1; Ida++)
                                    {
                                        if (dtLevelInfo.Select("POSITIONID='" + LevelInf[Ida].ToString() + "'").Count() >= 1)
                                        {
                                            lstID.Add(LevelInf[Ida].ToString());
                                            lstPageNo.Add((iTotalPage + 1).ToString());
                                        }
                                        iLevel = Convert.ToInt32(drRow[0]["LevelNo"].ToString());
                                    }

                                    int Idx = LevelInf.Length, Length=0;
                                    while (Idx >= 1) {
                                        if (Idx - NoOfNodes >= 1) Length = NoOfNodes; else Length = Idx;
                                        new InsertImage().MergeSlides(HttpContext.Current.Server.MapPath("~/Content/PPTX/"), SourceFile, DestinationFile, SlideIndex++, Length);
                                        Idx = Idx - Length;
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }
            }

            // Initialise the list array
            lstID.Clear();
            lstLevel.Clear();
            lstPageNo.Clear();
            lstID.Add(HttpContext.Current.Session["ID"].ToString());
            lstPageNo.Add("1");

            using (var presentationDocument = PresentationDocument.Open(DestinationFilePath, true))
            {
                PresentationPart presentationPart = presentationDocument.PresentationPart;
                drRow = dtLevelInfo.Select("POSITIONID='" + lstID[0].ToString() + "'");
                drNOR = dtLevelInfo.Select("POSITIONID='" + lstID[0].ToString() + "'");
                if (drRow.Length >= 1)
                {
                    if (Convert.ToInt32(drRow[0]["SOC_COUNT"]) <= 60000)
                    {
                        int iLevel = 2;
                        iTotalPage = 0; SlideIndex = 0;
                        lstLevel.Add(iLevel.ToString());
                        for (int Idc = 0; Idc <= lstID.Count - 1; Idc++)
                        {
                            try
                            {
                                drRow = dtLevelInfo.Select("POSITIONID='" + lstID[Idc].ToString() + "'");
                                if (drRow.Length >= 1)
                                {
                                    SetPositionPageNo(lstID[Idc].ToString(), iTotalPage + 1);
                                    if (drRow[0]["NextLevel"].ToString() != "")
                                    {
                                        string[] LevelInf = drRow[0]["NextLevel"].ToString().Split(',');
                                        for (int Ida = 0; Ida <= LevelInf.Length - 1; Ida++)
                                        {
                                            if (dtLevelInfo.Select("POSITIONID='" + LevelInf[Ida].ToString() + "'").Count() >= 1)
                                            {
                                                lstID.Add(LevelInf[Ida].ToString());
                                                lstPageNo.Add((iTotalPage + 1).ToString());
                                            }
                                            iLevel = Convert.ToInt32(drRow[0]["LevelNo"].ToString());
                                        }

                                        DataRow[] drObj = dtObj.Select("POSITIONID='" + lstID[Idc].ToString() + "'");
                                        foreach (DataRow drInf in drObj)
                                        {
                                            sTPage = (Convert.ToInt32(drRow[0]["MaxLevel"].ToString()) + 1).ToString();
                                            sCPage = (Convert.ToInt32(drInf["DRIndex"].ToString()) + 1).ToString();
                                            string jsonString = drInf["Data1"].ToString();
                                            if (drInf["Data2"] != null) jsonString += drInf["Data2"].ToString();
                                            if (drInf["Data3"] != null) jsonString += drInf["Data3"].ToString();
                                            if (drInf["Data4"] != null) jsonString += drInf["Data4"].ToString();
                                            jsonString = jsonString.Substring(0, jsonString.IndexOf("}]") + 2);
                                            List<ObjectInf> theObjectInf = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<List<ObjectInf>>(jsonString);

                                            if (ShowType == "PPT")
                                            {
                                                int NodeCount = 0;
                                                List<string> LstShowText = new List<string>();
                                                List<string> LstShowPicture = new List<string>();
                                                foreach (ObjectInf Obj in theObjectInf)
                                                {
                                                    string ShowText = "";
                                                    string[] LabelInfo = Obj.Title.Replace("&amp;", "&").Split(';');
                                                    if (LabelInfo.Length >= 1)
                                                    {
                                                        try
                                                        {
                                                            for (int Idl = 0; Idl <= LabelInfo.Length - 1; Idl++)
                                                            {
                                                                string[] LabelText = LabelInfo[Idl].Split('|');
                                                                ShowText += ";"+LabelText[0];
                                                            }
                                                        }
                                                        catch (Exception ex)
                                                        {

                                                        }
                                                    }
                                                    LstShowText.Add(ShowText == "" ? "" : ShowText.Substring(1));
                                                    LstShowPicture.Add(Obj.Id+".jpg");

                                                    if (NodeCount >= NoOfNodes)
                                                    {
                                                        List<PPTXTemplateFields> PPTXAllparams = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<List<PPTXTemplateFields>>(dtPPTX.Rows[0]["FieldsInfo"].ToString());
                                                        new InsertImage().ReplaceFirstImageMatchingAltText(presentationDocument, @"assign1.jpg", "Related image",
                                                                                                           ImagePath, LstShowPicture, LstShowText, SlideIndex++, dtFieldActive, PPTXAllparams);
                                                        if ((NodeCount + 1) != theObjectInf.Count)
                                                        {
                                                            NodeCount = 0;
                                                            LstShowText.Clear();
                                                            LstShowPicture.Clear();

                                                            ObjectInf ObjFirst = theObjectInf[0]; ShowText = "";
                                                            LabelInfo = ObjFirst.Title.Replace("&amp;", "&").Split(';');
                                                            if (LabelInfo.Length >= 1)
                                                            {
                                                                try
                                                                {
                                                                    for (int Idl = 0; Idl <= LabelInfo.Length - 1; Idl++)
                                                                    {
                                                                        string[] LabelText = LabelInfo[Idl].Split('|');
                                                                        ShowText += ";" + LabelText[0];
                                                                    }
                                                                }
                                                                catch (Exception ex)
                                                                {

                                                                }
                                                            }
                                                            LstShowText.Add(ShowText == "" ? "" : ShowText.Substring(1));
                                                            LstShowPicture.Add(Obj.Id + ".jpg");

                                                            NodeCount++;
                                                        }
                                                        else
                                                        {
                                                            NodeCount = 0;
                                                            LstShowText.Clear();
                                                            LstShowPicture.Clear();
                                                        }
                                                    }
                                                    else NodeCount++;
                                                }
                                                if (NodeCount >= 1)
                                                {
                                                    List<PPTXTemplateFields> PPTXAllparams = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<List<PPTXTemplateFields>>(dtPPTX.Rows[0]["FieldsInfo"].ToString());
                                                    new InsertImage().ReplaceFirstImageMatchingAltText(presentationDocument, @"assign1.jpg", "Related image",
                                                                                                       ImagePath, LstShowPicture, LstShowText, SlideIndex++, dtFieldActive, PPTXAllparams);
                                                }
                                            }
                                            theObjectInf.Clear();
                                            jsonString = null;
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                sID += "," + lstID[Idc].ToString();
                            }
                        }
                    }
                }
                presentationDocument.Close();

                FileInfo myDoc = new FileInfo(DestinationFilePath);

                HttpContext.Current.Response.Clear();
                HttpContext.Current.Response.ContentType = "application/ppt";
                HttpContext.Current.Response.AddHeader("content-disposition", "inline;filename=" + myDoc.Name);
                HttpContext.Current.Response.AddHeader("Content-Length", myDoc.Length.ToString());
                HttpContext.Current.Response.ContentType = "application/octet-stream";
                HttpContext.Current.Response.WriteFile(myDoc.FullName);
                HttpContext.Current.Response.End();
            }

            return "Nodata";
        }


        // All PPT download
        public void CreateAllLevelPPT(string ShowType, DateTime fixtureDate)
        {
            LoginUsers UserData = LI.GetLoginUserInfo("");
            string SourceFilePath = HttpContext.Current.Server.MapPath("~/Content/PPTX/") +"TemplatePPT.pptx";
            string DestinationFilePath = HttpContext.Current.Server.MapPath("~/Content/PPTX/") + UserData.UserName.ToUpper()+"ALL_LEVEL_ORG_PPTX_"+DateTime.Now.ToString("yyyyMMddhhmmss")+".pptx";
            File.Copy(SourceFilePath, DestinationFilePath);

            string hierarchicalStructure = @"
                       <node label=""Raj"">
                          <node label=""Subbu1"">
                                  <node label=""Vijay&#xA;He is on leave today""  highlight=""true"" />
                                  <node label=""Rakesh"" />
                                  <node label=""Jay"" />
                                  <node label=""Abuzer"" />
                          </node>
                          <node label=""Shyam"" />
                      </node>";

            using (var p = PresentationDocument.Open(DestinationFilePath, true))
            {
                foreach (var slide in p.PresentationPart.GetPartsOfType<SlidePart>().Where(sp => IsVisible(sp)))
                {
                    foreach (var diagramPart in slide.DiagramDataParts)
                    {
                        foreach (var text in diagramPart.RootElement.Descendants<Run>().Select(d => hierarchicalStructure))
                        {
                            XmlDocument xmlDoc = new XmlDocument();//load xml
                            xmlDoc.LoadXml(hierarchicalStructure);
                            AddNodesToDiagram("", xmlDoc.ChildNodes[0], diagramPart.DataModelRoot.PointList, diagramPart.DataModelRoot.ConnectionList, 0U, true);
                            diagramPart.DataModelRoot.Save();
                        }
                    }
                    p.Close();
                    System.Diagnostics.Process.Start(DestinationFilePath);
                }
            }
        }

        private string RemoveStringAtEnd(string searchStr, string targetStr)
        {
            if (targetStr.ToLower().EndsWith(searchStr.ToLower()))
            {
                string resultStr = targetStr.Substring(0, targetStr.Length - searchStr.Length);
                return resultStr;
            }
            return targetStr;
        }

        private bool IsVisible(SlidePart s)
        {
            return (s.Slide != null) &&
              ((s.Slide.Show == null) || (s.Slide.Show.HasValue &&
              s.Slide.Show.Value));
        }

        private void AddNodesToDiagram(string parentNodeGuid, XmlNode currentNode, 
                                       Dgm.PointList pointList, Dgm.ConnectionList connectionList, 
                                       UInt32 connectionSourcePosition, bool clearExisting)
        {
            //recursive function to add nodes to an existing diagram
            if (clearExisting)
            {
                //remove all connections
                connectionList.RemoveAllChildren();

                //remove all nodes except where type = 'doc' ... NOTE: Didn't test what happens if you also remove the doc type node, but it seemed important.
                List<Dgm.Point> pts = pointList.OfType<Dgm.Point>().Where(x => x.Type != "doc").ToList();

                for (int i = pts.Count - 1; i >= 0; i--) { pts[i].Remove(); }//remove in reverse order.
            }

            string currentNodeGuid = "{" + Guid.NewGuid().ToString() + "}";//generate new guid, not sure if curly brackets are required (probably not).

            //create the new point
            Dgm.Point newPoint = new Dgm.Point(new Dgm.PropertySet()) { ModelId = currentNodeGuid };
            Dgm.ShapeProperties newPointShapeProperties = new Dgm.ShapeProperties();

            if (currentNode.Attributes["highlight"] != null && currentNode.Attributes["highlight"].Value == "true")
            {
                //if we need to highlight this particular point, then add a solidfill to it
                newPointShapeProperties.Append(new SolidFill(new SchemeColor() { Val = SchemeColorValues.Accent5 }));
            }

            Dgm.TextBody newPointTextBody = new Dgm.TextBody(new BodyProperties(), new ListStyle(),
                new Paragraph(new Run(new RunProperties() { Language = "en-AU" }, new DocumentFormat.OpenXml.Drawing.Text(currentNode.Attributes["label"].Value))));

            newPoint.Append(newPointShapeProperties, newPointTextBody);//append to point

            //append the point to the point list
            pointList.Append(newPoint);

            if (!string.IsNullOrEmpty(parentNodeGuid))
            {
                //if parent specified, then create the connection where the parent is the source and the current node is the destination
                connectionList.Append(new Dgm.Connection() { ModelId = "{" + Guid.NewGuid().ToString() + "}", SourceId = parentNodeGuid, DestinationId = currentNodeGuid, SourcePosition = (UInt32Value)connectionSourcePosition, DestinationPosition = (UInt32Value)0U });
            }

            foreach (XmlNode childNode in currentNode.ChildNodes)
            {
                //call this method for every child
                AddNodesToDiagram(currentNodeGuid, childNode, pointList, connectionList, connectionSourcePosition++, false);
            }
        }

        private List<ObjectInf> GetLevelInfo()
        {
            var theObjectInf = new List<ObjectInf>();
            Common csobj = new Common();
            DataTable dtlevel = null;
            SqlCommand cmd = new SqlCommand();

            string InfoPos = "", ShowLevel = Level, ParentLevel="";
            string[] SearchIDs = { "123 ", "123" };

            HttpContext.Current.Session["LANGUAGE"] = Language;
            LevelDate = DateTime.Now.ToString("dd-MM-yyyy");
            if (LevelDate != "")
            {
                HttpContext.Current.Session["LEVEL_ID"] = ShowLevel;
                HttpContext.Current.Session["LEVEL_DATE"] = LevelDate;
            }
            else
            {
                HttpContext.Current.Session["LEVEL_ID"] = "";
                HttpContext.Current.Session["LEVEL_DATE"] = "";
            }


            csobj = new Common();
            dtlevel = csobj.SQLReturnDataTable("SELECT * FROM ORG_CONFIG_INFO WHERE VIEW_ID='VIEW_DEFAULT'");
            foreach (DataRow drlvl in dtlevel.Rows)
            {
                if (drlvl["FIELD_NAME"].ToString() == "LEVEL") LevelUpto = LevelFlag;
                if (drlvl["FIELD_NAME"].ToString() == "HEIGHT") Height = drlvl["FIELD_VALUE"].ToString();
                if (drlvl["FIELD_NAME"].ToString() == "LINECOLOR")
                {
                    LineColor = drlvl["FIELD_VALUE"].ToString();
                }
                if (drlvl["FIELD_NAME"].ToString() == "LINEWIDTH")
                {
                    LineWidth = drlvl["FIELD_VALUE"].ToString();
                    blackPen = new Pen(GetDrawingLineColor(LineColor), Convert.ToInt16(LineWidth));
                }
                if (drlvl["FIELD_NAME"].ToString() == "TEMPLATE")
                {
                    TemplateURL = drlvl["FIELD_VALUE"].ToString();
                    string[] TempURL = TemplateURL.Split('~');
                    if (TempURL[0] == "IMG")
                    {
                        BmpURL = new Bitmap(System.Web.HttpContext.Current.Server.MapPath(TempURL[1]));
                        BmpURL.SetResolution(1200f, 1200f);
                    }
                    BmpUpArrow = new Bitmap(System.Web.HttpContext.Current.Server.MapPath("~/Content/images/uparrow.jpg"));
                    BmpDnArrow = new Bitmap(System.Web.HttpContext.Current.Server.MapPath("~/Content/images/downarrow.jpg"));
                }
            }

            // Gets the Box Height
            Original_Height = Convert.ToInt32(Height);
            Original_Height_10 = Original_Height + 40;
            Adjustment_Height = ((((Original_Height - (Original_Height % 30)) / 30)) * LevelCount) - 1;
            HttpContext.Current.Session["BoxHeight"] = Original_Height;

            LevelInfo LI = new LevelInfo();
            MyLastAction myla = LI.GetUserCurrentAction("");
            DataSet dtSet = LI.GetOrgChartDataTable(myla.Role, myla.Country, ShowLevel, PreviousLevel, myla.Levels, myla.Oper, myla.Version, myla.ShowLevel, "No", myla.SelectedFunctionalManagerType);
            DataTable dtlbl = dtSet.Tables[0];
            foreach (DataRow dr in dtlbl.Rows)
            {
                InfoPos = "";
                foreach (DataRow drconf in dtconf.Rows)
                {
                    try
                    {
                        string LEVEL_ID = OPR_LEVEL_ID;
                        if (View == "LV") LEVEL_ID = LGL_PARENT_ID;
                        if (dr["POSITIONFLAG"].ToString() == dr[LEVEL_ID].ToString())
                        {
                            if ((drconf["FIELD_NAME"].ToString() == "FIRSTNAME") || (drconf["FIELD_NAME"].ToString() == "PositionTitle"))
                            {
                                InfoPos += ";    |" +
                                                    drconf["FIELD_ROW"].ToString() + "|" +
                                                    drconf["FIELD_COL"].ToString();
                            }
                            else
                            {
                                if (drconf["FIELD_NAME"].ToString() == PositionTotalCostField ||
                                    drconf["FIELD_NAME"].ToString() == PositionCostField)
                                {
                                    string Value = Convert.ToDecimal(dr[drconf["FIELD_NAME"].ToString()].ToString()).ToString("#,##0.00");
                                    InfoPos += ";" + Value + "|" +
                                                     drconf["FIELD_ROW"].ToString() + "|" +
                                                     drconf["FIELD_COL"].ToString();
                                }
                                else
                                {
                                    InfoPos += ";" + dr[drconf["FIELD_NAME"].ToString()].ToString() + "|" +
                                                     drconf["FIELD_ROW"].ToString() + "|" +
                                                     drconf["FIELD_COL"].ToString();
                                }
                            }
                        }
                        else
                        {
                            if (drconf["FIELD_NAME"].ToString() == PositionTotalCostField ||
                                drconf["FIELD_NAME"].ToString() == PositionCostField)
                            {
                                string Value = Convert.ToDecimal(dr[drconf["FIELD_NAME"].ToString()].ToString()).ToString("#,##0.00");
                                InfoPos += ";" + Value + "|" +
                                                 drconf["FIELD_ROW"].ToString() + "|" +
                                                 drconf["FIELD_COL"].ToString();
                            }
                            else
                            {
                                InfoPos += ";" + dr[drconf["FIELD_NAME"].ToString()].ToString() + "|" +
                                                    drconf["FIELD_ROW"].ToString() + "|" +
                                                    drconf["FIELD_COL"].ToString();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }

                string sColor = "0", sBackColor = "0", ParentID = "", OrangeLevel = "", SetFlag = "N";
                sBackColor = "0";
                if (View == "OV")
                {
                    OrangeLevel = (Convert.ToInt32(dr["LEVEL_NO"]) - 1).ToString(); ParentID = "-1";
                    if (dr[OPR_LEVEL_ID].ToString() != ShowLevel)
                    {
                        ParentID = dr[OPR_PARENT_ID].ToString();
                    }
                    if (dr["NOR_COUNT"].ToString() != "0")
                    {
                        AddPositionID(dr[OPR_LEVEL_ID].ToString());
                        SetFlag = "Y";
                    }
                    else SetFlag = "N";
                    theObjectInf.Add(new ObjectInf(dr[OPR_LEVEL_ID].ToString(),
                                                        InfoPos.Substring(1),
                                                        ParentID,
                                                        OrangeLevel,
                                                        0, 0, 0, 0, 175, Original_Height,
                                                        SetFlag,
                                                        dr["GRAY_COLORED_FLAG"].ToString(),
                                                        dr["DOTTED_LINE_FLAG"].ToString(),
                                                        dr["SHOW_FULL_BOX"].ToString(),
                                                        dr["LANGUAGE_SELECTED"].ToString(),
                                                        dr["SORTNO"].ToString(),
                                                        dr["POSITIONFLAG"].ToString(),
                                                        sColor,
                                                        sBackColor,
                                                        dr["FLAG"].ToString(),
                                                        iLevel.ToString(), ""));
                }
                else if (View == "LV")
                {
                    OrangeLevel = (Convert.ToInt32(dr["LEVEL_NO"]) - 1).ToString(); ParentID = "-1";
                    if (dr[LGL_LEVEL_ID].ToString() != ShowLevel)
                    {
                        OrangeLevel = "1";
                        ParentID = dr[LGL_PARENT_ID].ToString();
                    }
                    if (Convert.ToInt32(dr["NOR_COUNT"].ToString().Trim()) != 0)
                    {
                        AddPositionID(dr[LGL_LEVEL_ID].ToString());
                        SetFlag = "Y";
                    }
                    else SetFlag = "N";
                    theObjectInf.Add(new ObjectInf(dr[LGL_LEVEL_ID].ToString(),
                                                        InfoPos.Substring(1),
                                                        ParentID,
                                                        OrangeLevel,
                                                        0, 0, 0, 0, 175, Original_Height,
                                                        SetFlag,
                                                        dr["GRAY_COLORED_FLAG"].ToString(),
                                                        dr["DOTTED_LINE_FLAG"].ToString(),
                                                        dr["SHOW_FULL_BOX"].ToString(),
                                                        dr["LANGUAGE_SELECTED"].ToString(),
                                                        dr["SORTNO"].ToString(),
                                                        dr["POSITIONFLAG"].ToString(),
                                                        sColor,
                                                        sBackColor,
                                                        dr["FLAG"].ToString(),
                                                        iLevel.ToString(), ""));
                }

            }

            // Json object to show level information
            var javaScriptSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            JsonFieldInfo = javaScriptSerializer.Serialize(theObjectInf);

            return theObjectInf;
        }

        public List<ObjectInf> ConvertToPDF(string ShowID, string strCVIEW, string DownloadType, string CompanyName, string LevelUpto)
        {
            string LastRefresh = "";
            Common csobj = new Common();
            int iWidth = 255, FLC = 0; sAllPDF = "N";
            if (LevelCount == 6) iWidth = 255;

            if (View == "OV")
            {
                dtFieldInformation = csobj.SQLReturnDataTable("SELECT * FROM LEVEL_CONFIG_INFO " +
                                                              " WHERE VIEW_ID='" + strCVIEW + "' AND " +
                                                              "       DOWNLOAD_TYPE='" + DownloadType + "' AND COMPANY_NAME='" + CompanyName + "'");
                dtFieldActive = csobj.SQLReturnDataTable("SELECT * FROM LEVEL_CONFIG_INFO " +
                                                         " WHERE VIEW_ID='" + strCVIEW + "' AND ACTIVE_IND='Y' AND " +
                                                         " DOWNLOAD_TYPE='" + DownloadType + "' AND COMPANY_NAME='" + CompanyName + "'");
            }
            else if (View == "LV")
            {
                dtFieldInformation = csobj.SQLReturnDataTable("SELECT * FROM LEGAL_CONFIG_INFO " +
                                                              " WHERE VIEW_ID='" + strCVIEW + "' AND " +
                                                              "       DOWNLOAD_TYPE='" + DownloadType + "' AND COMPANY_NAME='" + CompanyName + "'");
                dtFieldActive = csobj.SQLReturnDataTable("SELECT * FROM LEGAL_CONFIG_INFO " +
                                                         " WHERE VIEW_ID='" + strCVIEW + "' AND ACTIVE_IND='Y' AND " +
                                                         "       DOWNLOAD_TYPE='" + DownloadType + "' AND COMPANY_NAME='" + CompanyName + "'");
            }

            // Collects the Hierarchy data to show 
            strOutput = "PDF";
            PDFPage = CurPage;
            Output_Height = 1020;
            List<ObjectInf> theObjectInf = null;

            theObjectInf = GetLevelInfo();
            string json = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(theObjectInf);
            theObjectInf = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<List<ObjectInf>>(json).OrderBy(o => Convert.ToInt32(o.Level)).ToList();
            csobj = new Common();
            DataTable dtlevel = csobj.SQLReturnDataTable("SELECT * FROM LEVEL_CONFIG_INFO WHERE VIEW_ID='VIEW_DEFAULT'");
            foreach (DataRow drlvl in dtlevel.Rows)
            {
                //if (drlvl["FIELD_NAME"].ToString() == "LEVEL") LevelUpto = drlvl["FIELD_VALUE"].ToString();
                if (drlvl["FIELD_NAME"].ToString() == "LEVEL") LevelUpto = LevelFlag;
                if (drlvl["FIELD_NAME"].ToString() == "HEIGHT") Height = drlvl["FIELD_VALUE"].ToString();
                if (drlvl["FIELD_NAME"].ToString() == "LINECOLOR")
                {
                    LineColor = drlvl["FIELD_VALUE"].ToString();
                }
                if (drlvl["FIELD_NAME"].ToString() == "LINEWIDTH")
                {
                    LineWidth = drlvl["FIELD_VALUE"].ToString();
                    blackPen = new Pen(GetDrawingLineColor(LineColor), Convert.ToInt16(LineWidth));
                }
                if (drlvl["FIELD_NAME"].ToString() == "TEMPLATE")
                {
                    TemplateURL = drlvl["FIELD_VALUE"].ToString();
                    string[] TempURL = TemplateURL.Split('~');
                    if (TempURL[0] == "IMG")
                    {
                        BmpURL = new Bitmap(System.Web.HttpContext.Current.Server.MapPath(TempURL[1]));
                        BmpURL.SetResolution(1200f, 1200f);
                    }
                    BmpUpArrow = new Bitmap(System.Web.HttpContext.Current.Server.MapPath("~/Content/images/uparrow.jpg"));
                    BmpDnArrow = new Bitmap(System.Web.HttpContext.Current.Server.MapPath("~/Content/images/downarrow.jpg"));
                }
            }

            // Gets the Box Height
            Original_Height = Convert.ToInt32(Height);
            Original_Height_10 = Original_Height + 40;
            Adjustment_Height = ((((Original_Height - (Original_Height % 30)) / 30)) * LevelCount) - 1;
            HttpContext.Current.Session["BoxHeight"] = Original_Height;

            // Gets the available levels
            string[] LevelInf = GetAllLevels(theObjectInf).Split(',');
            if (LevelInf.Count() <= 1) return theObjectInf;
            LevelIds = new string[LevelInf.Length];
            ObjectInf Obj = null, CurObj = null, PrevObj = null;
            if (LevelInf.Length >= 1)
            {
                // Get Level based Ids
                for (int Idx = 0; Idx <= LevelInf.Length - 1; Idx++)
                {
                    LevelIds[Convert.ToInt16(LevelInf[Idx])] = LevelInId(theObjectInf, LevelInf[Idx], Idx);
                }

                // Calculates the Height for each Ids
                Obj = GetIdInfo(theObjectInf, LevelIds[0].ToString());
                Obj.Height = Original_Height_10;
                Obj.Width = iWidth;
                string sSkip = "N";
                FLC = LevelIds[1].Split(',').Length;
                for (int Idx = LevelIds.Length - 1; Idx >= 1; Idx--)
                {
                    string[] Ids = LevelIds[Idx].Split(',');
                    if (Idx == 1) FLC = Ids.Length;
                    for (int Idy = 0; Idy <= Ids.Length - 1; Idy++)
                    {
                        sSkip = "N";

                        if (Idx == 1) Obj = theObjectInf[Idy + 1]; else Obj = theObjectInf[Idy + FLC + 1];
                        if (Idx == 2) sSkip = Level2Check(LevelIds[1], Obj.PId);
                        if (sSkip == "N")
                        {
                            if (Obj.Level == "2")                // Last Level
                            {
                                Obj.Height = Original_Height_10 - 22;
                                Obj.Width = iWidth;
                            }
                            else
                            {
                                Obj.Height = GetChildHeight(theObjectInf, Ids[Idy].ToString());
                                Obj.Width = iWidth;
                            }
                        }
                    }
                }

                // Calculates the Row and Column info for each Ids
                int Row = 2, Col = 352, AddHeight = 0, MaxHeight = 0;
                FLC = 0;
                Obj = GetIdInfo(theObjectInf, LevelIds[0].ToString());
                Obj.Row = Row;
                Obj.Col = Col;

                for (int Idx = 1; Idx <= LevelIds.Length - 1; Idx++)
                {
                    string[] Ids = LevelIds[Idx].Split(',');
                    if (Idx == 1) FLC = Ids.Length;
                    for (int Idy = 0; Idy <= Ids.Length - 1; Idy++)
                    {
                        sSkip = "N";
                        if (Idx == 1)
                        {
                            //CurObj = GetIdInfo(theObjectInf, Ids[Idy].ToString());
                            CurObj = theObjectInf[Idy + 1];
                        }
                        else
                            CurObj = theObjectInf[Idy + FLC + 1];

                        if (Idx == 2) sSkip = Level2Check(LevelIds[1], CurObj.PId);
                        if (sSkip == "N")
                        {
                            if ((Idx == 1) && ((Idy % LevelCount) == 0) && (Idy != 0))
                            {
                                AddHeight += MaxHeight;
                                MaxHeight = 0;
                            }
                            //changed by vignesh
                            if (Idx == 1)
                            {
                                try
                                {
                                    CurObj = theObjectInf[Idy + 1];
                                    Obj = GetIdInfo(theObjectInf, CurObj.PId);             // To get the Row in which this object is displayed

                                    Row = Obj.Row + Obj.Height + AddHeight;
                                    Col = ((Idy % LevelCount) * iWidth) + 5;
                                    if (MaxHeight <= CurObj.Height) MaxHeight = CurObj.Height;
                                }
                                catch (Exception Ex)
                                {
                                    string errMsg = Ex.ToString();
                                    Console.WriteLine("  Message: {0}", errMsg);
                                }
                            }
                            else
                            {
                                CurObj = theObjectInf[Idy + FLC + 1];
                                Obj = GetIdInfo(theObjectInf, CurObj.PId);             // To get the Row in which this object is displayed

                                Row = Obj.Row;
                                for (int Idz = 0; Idz <= Ids.Length - 1; Idz++)
                                {
                                    PrevObj = theObjectInf[Idz + FLC + 1];
                                    if (PrevObj.PId == Obj.Id)
                                    {
                                        Row += PrevObj.Height;
                                        Col = Obj.Col + 15;
                                        if (PrevObj.Id == Ids[Idy].ToString()) break;
                                    }
                                }
                            }
                            CurObj.Row = Row;
                            CurObj.Col = Col;
                        }
                    }
                }
            }

            // Creates the Document
            MyDocument = new ceTe.DynamicPDF.Document();
            MyDocument.Creator = "DynamicChart.aspx";
            MyDocument.Author = "Subramanian";
            MyDocument.Title = "Organization Chart";
            PageSizeNos.Add(0);

            //(PageSize.B4, PageOrientation.Landscape, 0.3F);
            if (LevelCount == 4)
                MyPage[CurPage] = new ceTe.DynamicPDF.Page(1080F, 1050F, 15.3F);
            else
                MyPage[CurPage] = new ceTe.DynamicPDF.Page(1600F, 1050F, 15.3F);
            MaxPage = 1;

            // Places the Level Information in the PDF
            int LevelShowUpto = Convert.ToInt32(LevelUpto);
            if ((LevelIds.Length >= 3) && (LevelShowUpto >= 2)) Level_Information(theObjectInf);         // Level without child Information 
            if ((LevelIds.Length >= 2) && (LevelShowUpto >= 1)) Level_1_Information(theObjectInf);       // Level 1 Information 
            if ((LevelIds.Length >= 2) && (LevelShowUpto >= 1)) Level_1_Line_Information(theObjectInf);  // Level 1 Line Information 
            if ((LevelIds.Length >= 1) && (LevelShowUpto >= 0)) Level_0_Information(theObjectInf, "C", "", "");       // Level 0 Information


            string objLevel = "-1";
            if (View == "OV")
            {
                csobj = new Common();
                DataTable dtLevel = csobj.SQLReturnDataTable("SELECT TOP 1 LEVELNO FROM [DIRECT_REPORT] WHERE PositionID= '" + Level + "'");
                if (dtLevel.Rows.Count >= 1)
                {
                    objLevel = dtLevel.Rows[0]["LEVELNO"].ToString();
                }
            }

            for (int Idx = 0; Idx <= MaxPage - 1; Idx++)
            {
                if (LevelCount == 6)
                {
                    if (objLevel != "-1")
                    {
                        MyLabel = new ceTe.DynamicPDF.PageElements.Label("Level No : N - " + objLevel, 5, 975, 200, 20, ceTe.DynamicPDF.Font.HelveticaBold, 15, ceTe.DynamicPDF.TextAlign.Left);
                        MyPage[Idx].Elements.Add(MyLabel);
                    }

                    MyLabel = new ceTe.DynamicPDF.PageElements.Label("Page No : " + (Idx + 1).ToString(), 1350, 975, 200, 20, ceTe.DynamicPDF.Font.HelveticaBold, 15, ceTe.DynamicPDF.TextAlign.Right);
                    MyPage[Idx].Elements.Add(MyLabel);

                    string LevelDate = HttpContext.Current.Session["LEVEL_DATE"].ToString();
                    string keyDate = LevelDate.Substring(6, 4) + "-" + LevelDate.Substring(3, 2) + "-" + LevelDate.Substring(0, 2);
                    string sText = "This org. chart shows the legal entity situation of Novartis. The primary structure is the country-based followed by the legal entities. The legal chart shows only the legal relationship between employee and manager, hence there are no dotted lines shown in this chart. Source HR Core and last refresh on current date(" + LastRefresh + ") at 1am CET.";
                    if (View == "OV")
                        sText = "This chart indicates operational and functional coordination relationships supporting the global business. The actual company relationships of the individuals on this chart are to executives within the companies that employ them. For business use only. (source: HR Core) Last updated on " + LastRefresh + " at 1am CET";
                    sText = "";
                    MyLabel = new ceTe.DynamicPDF.PageElements.Label(sText, 5, 995, 1550, 20, ceTe.DynamicPDF.Font.HelveticaBold, 10, ceTe.DynamicPDF.TextAlign.Left);
                    MyPage[Idx].Elements.Add(MyLabel);

                }
                else
                {
                    if (objLevel != "-1")
                    {
                        MyLabel = new ceTe.DynamicPDF.PageElements.Label("Level No : N - " + objLevel, 5, 975, 200, 20, ceTe.DynamicPDF.Font.HelveticaBold, 8, ceTe.DynamicPDF.TextAlign.Left);
                        MyPage[Idx].Elements.Add(MyLabel);
                    }

                    MyLabel = new ceTe.DynamicPDF.PageElements.Label("Page No : " + (Idx + 1).ToString(), 850, 975, 200, 20, ceTe.DynamicPDF.Font.HelveticaBold, 8, ceTe.DynamicPDF.TextAlign.Right);
                    MyPage[Idx].Elements.Add(MyLabel);

                    string LevelDate = HttpContext.Current.Session["LEVEL_DATE"].ToString();
                    string keyDate = LevelDate.Substring(6, 4) + "-" + LevelDate.Substring(3, 2) + "-" + LevelDate.Substring(0, 2);
                    string sText = "This org. chart shows the legal entity situation of Novartis. The primary structure is the country-based followed by the legal entities. The legal chart shows only the legal relationship between employee and manager, hence there are no dotted lines shown in this chart. Source HR Core and last refresh on current date(" + LastRefresh + ") at 1am CET.";
                    if (View == "OV")
                        sText = "This chart indicates operational and functional coordination relationships supporting the global business. The actual company relationships of the individuals on this chart are to executives within the companies that employ them. For business use only. (source: HR Core) Last updated on " + LastRefresh + " at 1am CET";
                    MyLabel = new ceTe.DynamicPDF.PageElements.Label(sText, 5, 995, 1040, 20, ceTe.DynamicPDF.Font.HelveticaBold, 8, ceTe.DynamicPDF.TextAlign.Left);
                    MyPage[Idx].Elements.Add(MyLabel);
                }
                MyDocument.Pages.Add(MyPage[Idx]);
            }

            //Outputs the MyDocument to the current web MyPage
            MyDocument.DrawToWeb("OrgChart.pdf");

            return theObjectInf;
        }

        public void CreateAllLevelmagePDF(string FP)
        {
            string ImagePath = HttpContext.Current.Session["CurrPDF"].ToString();

            // Creates the Document
            Document MyDocument = new ceTe.DynamicPDF.Document();
            MyDocument.Creator = "DynamicChart.aspx";
            MyDocument.Author = "Subramanian";
            MyDocument.Title = "Organization Chart";

            Page MyPage = new ceTe.DynamicPDF.Page(ceTe.DynamicPDF.PageSize.A3, PageOrientation.Landscape, 0.3F);
            System.Drawing.Image Img = System.Drawing.Image.FromFile(ImagePath);
            FormattedTextAreaStyle style = new FormattedTextAreaStyle(ceTe.DynamicPDF.FontFamily.Helvetica, 14, true);
            string text = "<p><font color='FF0000'>\u2022 <font color='#000000'>This text is </p>";
            FormattedTextArea area = new FormattedTextArea(text, 100, 100, 200, 100, style);
            MyPage.Elements.Add(area);
            MyDocument.Pages.Add(MyPage);

            //Outputs the MyDocument to the current web MyPage
            MyDocument.Draw(FP);
        }

        // Put All Level information in PDF
        private int PutFieldInfoPDF(ObjectInfPDF OI, string Info, int CurrentCol, int CurrentRow, int Width, int Height, int BottomHeight, Page MyPage, string ConnectorLineType)
        {
            int Idx = 0, Idy=0, StartCol=0, CurrentA0PageRow=CurrentRow+A0StartHeight;
            string[] LabelInfo = Info.Replace("&amp;", "&").Split(';');
            string FontName = "", FontSize = "", FontColor = "", FontStyle = "", FontFloat = "", FontWidth = "", Adjustment = "";
            DataTable dtFieldInf = dtFieldActive;

            if (LabelInfo.Length >= 1)
            {
                MyRect = new ceTe.DynamicPDF.PageElements.Rectangle(CurrentCol, CurrentA0PageRow, Width, Height);
                if (OI.GrayColourFlag=="Y")
                    MyRect.BorderColor = ShowPDFBoxColor("#000000");
                else if (OI.DottedLineFlag=="N")
                    MyRect.BorderColor = ShowPDFBoxColor(BoxColor);
                else if (OI.DottedLineFlag == "Y")
                    MyRect.BorderColor = ShowPDFBoxColor("#ffb266");
                MyRect.BorderWidth = 2;
                MyPage.Elements.Add(MyRect);

                OI.RealCol = CurrentCol;
                OI.RealRow = CurrentRow;
                OI.RealBoxWidth = Width;
                OI.RealBoxHeight = Height;

                if (ConnectorLineType == "V")
                {
                    StartCol = CurrentCol - 30;
                    MyLine = new ceTe.DynamicPDF.PageElements.Line(CurrentCol - 30, CurrentA0PageRow + 60, CurrentCol - 1, CurrentA0PageRow + 60, ShowPDFLineColor(LineColor));
                    MyLine.Width = 2;
                    MyPage.Elements.Add(MyLine);
                }
                else if (ConnectorLineType == "H")
                {
                    StartCol = CurrentCol + 100;
                    MyLine = new ceTe.DynamicPDF.PageElements.Line(CurrentCol+100, CurrentA0PageRow - 30, CurrentCol+100, CurrentA0PageRow, ShowPDFLineColor(LineColor));
                    MyLine.Width = 2;
                    MyPage.Elements.Add(MyLine);
                }
                else if (ConnectorLineType == "HB")
                {
                    StartCol = CurrentCol + 100;
                    MyLine = new ceTe.DynamicPDF.PageElements.Line(CurrentCol + 100, CurrentA0PageRow - 30, CurrentCol + 100, CurrentA0PageRow, ShowPDFLineColor(LineColor));
                    MyLine.Width = 2;
                    MyPage.Elements.Add(MyLine);
                    MyLine = new ceTe.DynamicPDF.PageElements.Line(CurrentCol + 100, CurrentA0PageRow + 80, CurrentCol + 100, CurrentA0PageRow + 100 + BottomHeight, ShowPDFLineColor(LineColor));
                    MyLine.Width = 2;
                    MyPage.Elements.Add(MyLine);
                }
                else if (ConnectorLineType == "B")
                {
                    StartCol = CurrentCol + 100;
                    MyLine = new ceTe.DynamicPDF.PageElements.Line(CurrentCol + 100, CurrentA0PageRow + 80, CurrentCol + 100, CurrentA0PageRow + 100 + BottomHeight, ShowPDFLineColor(LineColor));
                    MyLine.Width = 2;
                    MyPage.Elements.Add(MyLine);
                }
                foreach (DataRow drFI in dtFieldInf.Rows)
                {
                    try
                    {
                        string[] LabelText = LabelInfo[Idx++].Split('|');
                        if (drFI["ALL_LEVEL_FLAG"].ToString() == "Y")
                        {
                            Idy++;
                            FontName = drFI["FONT_NAME"].ToString();
                            FontSize = drFI["FONT_SIZE"].ToString();
                            FontColor = drFI["FONT_COLOR"].ToString();
                            FontStyle = drFI["FONT_STYLE"].ToString();
                            FontFloat = drFI["FONT_FLOAT"].ToString();
                            FontWidth = drFI["FIELD_WIDTH"].ToString();
                            Adjustment = drFI["ADJUSTMENT"].ToString();

                            MyLabel = new ceTe.DynamicPDF.PageElements.Label(LabelText[0].ToString(), CurrentCol, CurrentA0PageRow + ((Idy-1)*18)+10, Width, Height, 
                                                                             ceTe.DynamicPDF.Font.HelveticaBold, 8, 
                                                                             ceTe.DynamicPDF.TextAlign.Center);
                            MyPage.Elements.Add(MyLabel);

                        }
                    }
                    catch (Exception)
                    {

                    }
                }
            }

            return StartCol;
        }

        // Put All Level information(Assistance) in PDF
        private int[] PutAssistanceFieldInfoPDF(int LevelId, int CurrentCol, int CurrentRow, int Width, int Height, Page MyPage, string Type)
        {
            int Idx = 0, Idy=0, Index=0, AssistanceCol=CurrentCol+300, AssistanceRow=CurrentRow+A0StartHeight;
            int[] retValue = { 0, 0 };
            string FontName = "", FontSize = "", FontColor = "", FontStyle = "", FontFloat = "", FontWidth = "", Adjustment = "";
            string[] LabelInfo=null;
            DataTable dtFieldInf = dtFieldActive;

            DataRow[] drObj = dtLevel2.Select("POSITIONID='" + LevelId + "'");
            foreach (DataRow drInf in drObj)
            {
                string jsonString = drInf["Data1"].ToString();
                if (drInf["Data2"] != null) jsonString += drInf["Data2"].ToString();
                if (drInf["Data3"] != null) jsonString += drInf["Data3"].ToString();
                if (drInf["Data4"] != null) jsonString += drInf["Data4"].ToString();
                jsonString = jsonString.Substring(0, jsonString.IndexOf("}]") + 2);
                List<ObjectInfPDF> theObjectInf = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<List<ObjectInfPDF>>(jsonString);
                foreach (ObjectInfPDF OI in theObjectInf)
                {
                    if (OI.PId != "-1" && OI.Flag=="Y")
                    {
                        LabelInfo = OI.Title.Replace("&amp;", "&").Split(';');
                        if (LabelInfo != null)
                        {
                            MyRect = new ceTe.DynamicPDF.PageElements.Rectangle(AssistanceCol, AssistanceRow, Width, Height);
                            MyRect.BorderColor = ShowPDFBoxColor(BoxColor);
                            MyRect.BorderWidth = 2;
                            MyPage.Elements.Add(MyRect);
                            MyLine = new ceTe.DynamicPDF.PageElements.Line((Type=="V"?AssistanceCol - 280: AssistanceCol - 200), AssistanceRow + 10, AssistanceCol, AssistanceRow + 10, ShowPDFLineColor(LineColor));
                            MyLine.Width = 2;
                            MyPage.Elements.Add(MyLine);

                            OI.RealCol = AssistanceCol;
                            OI.RealRow = AssistanceRow - A0StartHeight;
                            OI.RealBoxWidth = Width;
                            OI.RealBoxHeight = Height;

                            Index++;
                            Idy = 0; Idx = 0;
                            foreach (DataRow drFI in dtFieldInf.Rows)
                            {
                                try
                                {
                                    string[] LabelText = LabelInfo[Idx++].Split('|');
                                    if (drFI["ALL_LEVEL_FLAG"].ToString() == "Y")
                                    {
                                        Idy++;
                                        FontName = drFI["FONT_NAME"].ToString();
                                        FontSize = drFI["FONT_SIZE"].ToString();
                                        FontColor = drFI["FONT_COLOR"].ToString();
                                        FontStyle = drFI["FONT_STYLE"].ToString();
                                        FontFloat = drFI["FONT_FLOAT"].ToString();
                                        FontWidth = drFI["FIELD_WIDTH"].ToString();
                                        Adjustment = drFI["ADJUSTMENT"].ToString();

                                        MyLabel = new ceTe.DynamicPDF.PageElements.Label(LabelText[0].ToString(), AssistanceCol, AssistanceRow + ((Idy - 1) * 18)+10, Width, Height,
                                                                                         ceTe.DynamicPDF.Font.HelveticaBold, 8,
                                                                                         ceTe.DynamicPDF.TextAlign.Center);
                                        MyPage.Elements.Add(MyLabel);

                                    }
                                }
                                catch (Exception)
                                {

                                }
                            }
                            AssistanceRow = AssistanceRow + 130;
                        }
                    }
                }
            }

            if (Idy >= 1)
            {
                retValue[0] = AssistanceCol + 300;
                retValue[1] = AssistanceRow - A0StartHeight;
            }

            return retValue;
        }

        private int GetObjectInfPDF(List<ObjectInfPDF> lstObjectInfPDF, string LevelId)
        {
            int Index = 0;
            foreach(ObjectInfPDF Obj in lstObjectInfPDF)
            {
                if (Obj.Id == LevelId) return Index;
                Index++;
            }

            return 999999;
        }

        // Recursive call(Vertical Positions)
        public int SelectLevelVertical(string LevelId, int CurrentCol, int MaxCol, FormattedTextAreaStyle style, Page MyPage)
        {
            int[] AssistanceInf = null;
            DataRow[] drObj = dtLevel2.Select("POSITIONID='" + LevelId + "'");
            foreach (DataRow drInf in drObj)
            {
                string jsonString = drInf["Data1"].ToString();
                if (drInf["Data2"] != null) jsonString += drInf["Data2"].ToString();
                if (drInf["Data3"] != null) jsonString += drInf["Data3"].ToString();
                if (drInf["Data4"] != null) jsonString += drInf["Data4"].ToString();
                jsonString = jsonString.Substring(0, jsonString.IndexOf("}]") + 2);
                List<ObjectInfPDF> theObjectInf = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<List<ObjectInfPDF>>(jsonString);
                foreach (ObjectInfPDF OI in theObjectInf)
                {
                    if (OI.PId != "-1" && OI.Flag=="N")
                    {
                        if (OI.Id.ToString() == "10017999")
                            OI.Title = OI.Title;
                        lstObjectPDF.Add(new ObjectInfPDF(OI.Id.ToString(),
                                                          OI.Title,
                                                          OI.PId,
                                                          OI.Level,
                                                          CurrentRow,
                                                          CurrentCol,
                                                          0,
                                                          0,
                                                          0,
                                                          0,
                                                          0,
                                                          0,
                                                          OI.NextLevelFlag,
                                                          OI.GrayColourFlag,
                                                          OI.DottedLineFlag,
                                                          OI.ShowFullBox,
                                                          OI.Language,
                                                          OI.SortNo,
                                                          OI.NOR.ToString(),
                                                          OI.SOC.ToString(),
                                                          OI.PositionFlag,
                                                          OI.ColorFlag,
                                                          OI.BackColor,
                                                          OI.Flag,
                                                          OI.BreadGram,
                                                          OI.BreadGramName,
                                                          OI.ActualLevelNo,
                                                          0, 0, 0, 0, 0));


                        // Puts the data in the required area.
                        PutFieldInfoPDF(OI, OI.Title, CurrentCol, CurrentRow, 200, 80, 0, MyPage, "V");

                        if (OI.NOR.ToString() != "0")
                        {
                            CurrentCol = CurrentCol + 50;
                            if (MaxCol <= CurrentCol) MaxCol = CurrentCol;
                            CurrentRow = CurrentRow + 100;

                            lstObjectPDF[lstObjectPDF.Count - 1].Width = CurrentCol - 30;
                            lstObjectPDF[lstObjectPDF.Count - 1].Height = CurrentRow - 20;

                            AssistanceInf = PutAssistanceFieldInfoPDF(Convert.ToInt32(OI.Id), CurrentCol, CurrentRow, 200, 80, MyPage, "V");
                            if (AssistanceInf != null)
                            {
                                if (AssistanceInf[1] >= CurrentRow) CurrentRow = AssistanceInf[1];
                            }
                            else
                            {
                                AssistanceInf = new int[2];
                                AssistanceInf[0] = 0; AssistanceInf[1] = 0;
                            }

                            SelectLevelVertical(OI.Id, CurrentCol, MaxCol, style, MyPage);
                            int Index = GetObjectInfPDF(lstObjectPDF, OI.Id);
                            if (Index != 999999)
                            {
                                lstObjectPDF[Index].Owidth = CurrentCol - 30;
                                lstObjectPDF[Index].Oheight = CurrentRow - 20;
                            }
                            CurrentCol = CurrentCol - 50;
                        }
                        else
                        {
                            CurrentRow = CurrentRow + 100;
                            int Index = GetObjectInfPDF(lstObjectPDF, OI.PId);
                            if (Index != 999999)
                            {
                                lstObjectPDF[Index].Owidth = CurrentCol - 30;
                                lstObjectPDF[Index].Oheight = CurrentRow - 20;
                            }
                        }
                    }
                }
            }
            CurrentCol = CurrentCol - 50;

            return MaxCol+250;
        }

        // Recursive call(Horizantal Positions)
        public int SelectLevelHorizantal(string LevelId, int CurrentCol, int MaxCol, FormattedTextAreaStyle style, Page MyPage, string TopNodeFlag)
        {
            ObjectInfPDF TopNode = null;
            int StartCol = CurrentCol, StartRow = CurrentRow;
            int[] AssistanceInf = null;
            DataRow[] drObj = dtLevel2.Select("POSITIONID='" + LevelId + "'");
            foreach (DataRow drInf in drObj)
            {
                string jsonString = drInf["Data1"].ToString();
                if (drInf["Data2"] != null) jsonString += drInf["Data2"].ToString();
                if (drInf["Data3"] != null) jsonString += drInf["Data3"].ToString();
                if (drInf["Data4"] != null) jsonString += drInf["Data4"].ToString();
                jsonString = jsonString.Substring(0, jsonString.IndexOf("}]") + 2);
                List<ObjectInfPDF> theObjectInf = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<List<ObjectInfPDF>>(jsonString);
                foreach (ObjectInfPDF OI in theObjectInf)
                {
                    if (OI.PId != "-1" && OI.Flag == "N")
                    {
                        lstObjectPDF.Add(new ObjectInfPDF(OI.Id.ToString(),
                                                          OI.Title,
                                                          OI.PId,
                                                          OI.Level,
                                                          CurrentRow,
                                                          CurrentCol,
                                                          0,
                                                          0,
                                                          0,
                                                          0,
                                                          0,
                                                          0,
                                                          OI.NextLevelFlag,
                                                          OI.GrayColourFlag,
                                                          OI.DottedLineFlag,
                                                          OI.ShowFullBox,
                                                          OI.Language,
                                                          OI.SortNo,
                                                          OI.NOR.ToString(),
                                                          OI.SOC.ToString(),
                                                          OI.PositionFlag,
                                                          OI.ColorFlag,
                                                          OI.BackColor,
                                                          OI.Flag,
                                                          OI.BreadGram,
                                                          OI.BreadGramName,
                                                          OI.ActualLevelNo,
                                                          0, 0, 0, 0, 0));


                        // Puts the data in the required area.
                        if (TopNodeFlag=="Y") CurrentRow = 200;
                        PutFieldInfoPDF(OI, OI.Title, CurrentCol, CurrentRow, 200, 80, 0, MyPage, "H");

                        if (OI.NOR.ToString() != "0")
                        {
                            CurrentCol = CurrentCol + 250;
                            if (MaxCol <= CurrentCol) MaxCol = CurrentCol;
                            CurrentRow = CurrentRow + 150;

                            lstObjectPDF[lstObjectPDF.Count - 1].Width = CurrentCol - 130;
                            lstObjectPDF[lstObjectPDF.Count - 1].Height = CurrentRow - 120;

                            AssistanceInf = PutAssistanceFieldInfoPDF(Convert.ToInt32(OI.Id), CurrentCol, CurrentRow, 200, 80, MyPage, "H");
                            if (AssistanceInf != null) {
                                if (AssistanceInf[1] >= CurrentRow) CurrentRow = AssistanceInf[1];
                            }
                            else
                            {
                                AssistanceInf= new int[2];
                                AssistanceInf[0] = 0; AssistanceInf[1] = 0;
                            }

                            MaxCol = SelectLevelHorizantal(OI.Id, CurrentCol, MaxCol, style, MyPage, "N");
                            if (AssistanceInf[0] >= MaxCol) MaxCol = AssistanceInf[0];
                            int Index = GetObjectInfPDF(lstObjectPDF, OI.Id);
                            if (Index != 999999)
                            {
                                lstObjectPDF[Index].Owidth = CurrentCol - 130;
                                lstObjectPDF[Index].Oheight = CurrentRow - 120;
                            }
                            CurrentCol = MaxCol;
                        }
                        else
                        {
                            lstObjectPDF[lstObjectPDF.Count - 1].Width = CurrentCol;
                            lstObjectPDF[lstObjectPDF.Count - 1].Height = CurrentRow;
                            lstObjectPDF[lstObjectPDF.Count - 1].Owidth = CurrentCol;
                            lstObjectPDF[lstObjectPDF.Count - 1].Oheight = CurrentRow + 80;

                            CurrentCol = CurrentCol + 250;
                            if (MaxCol <= CurrentCol) MaxCol = CurrentCol;
                        }
                    }
                    else TopNode = OI;
                }
            }
            CurrentRow = CurrentRow + 100;

            MyLine = new ceTe.DynamicPDF.PageElements.Line(StartCol + 100, StartRow - 30, MaxCol - 150, StartRow - 30, GetPDFLineColor(LineColor));
            MyLine.Width = 2;
            MyPage.Elements.Add(MyLine);

            if (TopNodeFlag=="Y")
            {
                // Puts the data in the required area.
                PutFieldInfoPDF(TopNode, TopNode.Title, (MaxCol - 200)/2, StartRow - 130, 200, 80, 0, MyPage, "B");
            }
            PageMaxWidth = MaxCol;

            return MaxCol;
        }

        private int GetNodeHorizantalHeight(string BreadGram)
        {
            int CurrentRow = 100, Index=0;
            string[] stringSeparators = new string[] { "-->" };
            string[] ArrayBreadGram = BreadGram.Split(stringSeparators, StringSplitOptions.None);

            string CheckBreadGram = "";
            foreach (string bg in ArrayBreadGram)
            {
                if (Index++ != ArrayBreadGram.Length - 1)
                {
                    if (CheckBreadGram == "")
                        CheckBreadGram += bg;
                    else
                        CheckBreadGram += "-->" + bg;

                    foreach (ObjectInfPDF NodePDF in lstObjectPDF)
                    {
                        if (NodePDF.BreadGram == CheckBreadGram)
                        {
                            CurrentRow += 130;
                            break;
                        }
                    }
                }
            }

            return CurrentRow;
        }

        private int[] GetHorizantalNodeRowColumn(List<ObjectInfPDF> theSelectedObjectInfRC, string SortNo, string PID)
        {
            int[] RetValue = { 40, 200 };
            int CurrentCol = 0, CurrentRow=0;

            if (CurrentCol == 0 || CurrentRow == 0)
            {
                foreach (ObjectInfPDF NodePDF in lstObjectPDF)
                {
                    if (NodePDF.Id == PID)
                    {
                        CurrentCol = NodePDF.Col;
                        CurrentRow = NodePDF.Row + 150;
                    }
                }
            }

            foreach (ObjectInfPDF NodePDF in theSelectedObjectInfRC)
            {
                if (ShowFitInAssociates(NodePDF.Id) == "Yes")
                {
                    if (NodePDF.Flag == "N")
                    {
                        if ((Convert.ToInt32(NodePDF.SortNo)) <= (Convert.ToInt32(SortNo) - 1))
                            CurrentCol += NodePDF.Owidth;
                        if (Convert.ToInt32(NodePDF.SortNo) == Convert.ToInt32(SortNo))
                            CurrentRow = GetNodeHorizantalHeight(NodePDF.BreadGram);
                        PID = NodePDF.PId;
                    }
                }
            }

            RetValue[0] = CurrentCol==0?40:CurrentCol;
            RetValue[1] = CurrentRow;

            return RetValue;
        }

        // Get All Level Assistance Information
        private int[] GetAllLevelAssistance(int LevelId)
        {
            int Idy = 0, AssistanceCol = 300, AssistanceRow = 50;
            int[] retValue = { 0, 0 };
            DataRow[] drObj = dtLevel2.Select("POSITIONID='" + LevelId + "'");
            foreach (DataRow drInf in drObj)
            {
                string jsonString = drInf["Data1"].ToString();
                if (drInf["Data2"] != null) jsonString += drInf["Data2"].ToString();
                if (drInf["Data3"] != null) jsonString += drInf["Data3"].ToString();
                if (drInf["Data4"] != null) jsonString += drInf["Data4"].ToString();
                jsonString = jsonString.Substring(0, jsonString.IndexOf("}]") + 2);
                List<ObjectInfPDF> theObjectInf = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<List<ObjectInfPDF>>(jsonString);
                foreach (ObjectInfPDF OI in theObjectInf)
                {
                    if (OI.PId != "-1" && OI.Flag == "Y")
                    {
                        AssistanceRow += 100;
                        Idy++;
                    }
                }
            }

            if (Idy >= 1)
            {
                retValue[0] = AssistanceCol + 300;
                retValue[1] = AssistanceRow;
            }

            return retValue;
        }

        public void SetParentChildRelationship(string LevelId, int LastLevel)
        {
            DataRow[] drObj = dtLevel2.Select("POSITIONID='" + LevelId + "'");
            foreach (DataRow drInf in drObj)
            {
                string jsonString = drInf["Data1"].ToString();
                if (drInf["Data2"] != null) jsonString += drInf["Data2"].ToString();
                if (drInf["Data3"] != null) jsonString += drInf["Data3"].ToString();
                if (drInf["Data4"] != null) jsonString += drInf["Data4"].ToString();
                jsonString = jsonString.Substring(0, jsonString.IndexOf("}]") + 2);
                List<ObjectInfPDF> theObjectInf = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<List<ObjectInfPDF>>(jsonString);
                foreach (ObjectInfPDF OI in theObjectInf)
                {
                    if (OI.PId != "-1" && OI.Flag == "N")
                    {
                        if (OI.ActualLevelNo == LastLevel && LastLevel != 6)
                        {
                            OI.NextLevelFlag = "N";
                            OI.SOC = 0;
                            OI.NOR = 0;
                        }
                        lstObjectPDF.Add(new ObjectInfPDF(OI.Id.ToString(),
                                                          OI.Title,
                                                          OI.PId,
                                                          OI.Level,
                                                          0,
                                                          0,
                                                          0,
                                                          0,
                                                          0,
                                                          0,
                                                          0,
                                                          0,
                                                          OI.NextLevelFlag,
                                                          OI.GrayColourFlag,
                                                          OI.DottedLineFlag,
                                                          OI.ShowFullBox,
                                                          OI.Language,
                                                          OI.SortNo,
                                                          OI.NOR.ToString(),
                                                          OI.SOC.ToString(),
                                                          OI.PositionFlag,
                                                          OI.ColorFlag,
                                                          OI.BackColor,
                                                          OI.Flag,
                                                          OI.BreadGram,
                                                          OI.BreadGramName,
                                                          OI.ActualLevelNo,
                                                          0, 0, 0, 0, 0));

                        if (OI.NOR.ToString() != "0" && OI.DottedLineFlag=="N")
                        {
                            SetParentChildRelationship(OI.Id, LastLevel);
                        }
                    }
                    else
                    {
                        StartPosition = Level;
                        if (OI.Id == StartPosition)
                        {
                            lstObjectPDF.Add(new ObjectInfPDF(OI.Id.ToString(),
                                                              OI.Title,
                                                              OI.PId,
                                                              OI.Level,
                                                              0,
                                                              0,
                                                              0,
                                                              0,
                                                              0,
                                                              0,
                                                              0,
                                                              0,
                                                              OI.NextLevelFlag,
                                                              OI.GrayColourFlag,
                                                              OI.DottedLineFlag,
                                                              OI.ShowFullBox,
                                                              OI.Language,
                                                              OI.SortNo,
                                                              OI.NOR.ToString(),
                                                              OI.SOC.ToString(),
                                                              OI.PositionFlag,
                                                              OI.ColorFlag,
                                                              OI.BackColor,
                                                              OI.Flag,
                                                              OI.BreadGram,
                                                              OI.BreadGramName,
                                                              OI.ActualLevelNo,
                                                              0, 0, 0, 0, 0));
                        }
                    }
                }
            }
        }

        DataTable NodeWH = null;
        DataRow NodeRow = null;
        List<ObjectIWPDF> lstObjectWHPDF = new List<ObjectIWPDF>();
        int NewA0PageRow = 0, NewA0PageNo=0;
        public int[] A0PageSizeBottomUpApproachWH(string LevelId, string ShowLevel, string UpdateFlag)
        {
            int CurrentCol = 0, CurrentRow = 0, TempNewA0PageRow = 0;
            int[] AssistanceInf = null, RetValue = { 0, 0, 0, 0 };
            List<ObjectInfPDF> theObjectInf = (from SO in lstObjectPDF where SO.PId == LevelId select SO).ToList();
            foreach (ObjectInfPDF OI in theObjectInf)
            {
                if (OI.Flag == "N")
                {
                    if (LevelId == "20000000")
                        LevelId = "20000000";

                    string DoOperation = "Y";
                    List<ObjectIWPDF> lstCheckWHPDF = (from SO in lstObjectWHPDF
                                                         where SO.PId == LevelId
                                                         select SO).ToList();
                    foreach (ObjectIWPDF OS in lstCheckWHPDF)
                    {
                        if (UpdateFlag == "N" && OS.Id == OI.Id)
                        {
                            DoOperation = "Y";
                            break;
                        }
                        else if (OS.UsedFlag == "Y" && UpdateFlag == "Y" && OS.Id == OI.Id)
                        {
                            DoOperation = "Y";
                            break;
                        }
                        else if (OS.UsedFlag == "N" && UpdateFlag == "U" && OS.Id == OI.Id)
                        {
                            DoOperation = "Y";
                            break;
                        }
                        DoOperation = "N";
                    }
                    if (DoOperation == "Y" || OI.NOR.ToString()=="0")
                    {
                        if (OI.NOR.ToString() != "0" && OI.DottedLineFlag == "N")
                        {
                            AssistanceInf = GetAllLevelAssistance(Convert.ToInt32(OI.Id));
                            OI.Width = AssistanceInf[0];
                            OI.Height = AssistanceInf[1];
                            OI.NewRow = NewA0PageRow;
                            OI.NewPage = NewA0PageNo;

                            if (OI.NOR == OI.SOC)
                            {
                                RetValue[0] = 350;
                                RetValue[1] = (OI.NOR + 1) * 130;

                                OI.Owidth = (OI.Width >= RetValue[0] ? OI.Width : RetValue[0]);
                                OI.Oheight = OI.Height + RetValue[1];
                                OI.Cwidth = (OI.Width >= RetValue[0] ? OI.Width : RetValue[0]);
                                OI.Cheight = OI.Height + 130;

                                RetValue = A0PageSizeBottomUpApproachWH(OI.Id, ShowLevel, UpdateFlag);

                                CurrentCol += OI.Owidth;
                                CurrentRow = (OI.Oheight >= CurrentRow) ? OI.Oheight : CurrentRow;
                            }
                            else
                            {
                                RetValue = A0PageSizeBottomUpApproachWH(OI.Id, ShowLevel, UpdateFlag);
                                OI.Owidth = (OI.Width >= RetValue[0] ? OI.Width : RetValue[0]);
                                OI.Oheight = RetValue[1] + 130;
                                OI.Cwidth = (OI.Width >= RetValue[0] ? OI.Width : RetValue[0]);
                                OI.Cheight = OI.Height + 130;

                                CurrentCol += OI.Owidth;
                                CurrentRow = (OI.Oheight >= CurrentRow) ? OI.Oheight : CurrentRow;
                            }
                        }
                        else
                        {
                            OI.Width = 0;
                            OI.Height = 0;
                            OI.Owidth = 250;
                            OI.Oheight = 130;
                            OI.Cwidth = 250;
                            OI.Cheight = 130;

                            CurrentCol += OI.Owidth;
                        }
                        OI.Cwidth = CurrentCol;
                        OI.Cheight = CurrentRow;
                    }
                }
            }

            if (LevelId == "20000000")
                LevelId = "20000000";

            // Summation of width
            int SWidth = 0;
            foreach (ObjectInfPDF OI in theObjectInf)
            {
                if (OI.Flag == "N")
                {
                    if (OI.NOR.ToString() != "0" && OI.DottedLineFlag == "N")
                    {
                        if (UpdateFlag == "N")
                        {
                            SWidth += OI.Owidth;
                            if (OI.NOR >= 1)
                            {
                                ObjectIWPDF IWPDF = (from SO in lstObjectWHPDF where SO.Id == OI.Id select SO).FirstOrDefault();
                                if (IWPDF == null) {
                                    lstObjectWHPDF.Add(new ObjectIWPDF(OI.Id.ToString(),
                                                                       OI.PId,
                                                                       OI.Owidth,
                                                                       OI.Oheight,
                                                                       "Y"));
                                }
                            }
                        }
                        else if (UpdateFlag == "Y")
                        {
                            List<ObjectIWPDF> lstSortingWHPDF = (from SO in lstObjectWHPDF
                                                                 where SO.Id == OI.Id
                                                                 select SO).ToList();
                            foreach (ObjectIWPDF OS in lstSortingWHPDF)
                            {
                                if (OS.UsedFlag == "Y") SWidth += OI.Owidth;
                            }
                        }
                        else if (UpdateFlag == "U")
                        {
                            List<ObjectIWPDF> lstSortingWHPDF = (from SO in lstObjectWHPDF
                                                                 where SO.Id == OI.Id
                                                                 select SO).ToList();
                            foreach (ObjectIWPDF OS in lstSortingWHPDF)
                            {
                                if (OS.UsedFlag == "N") SWidth += OI.Owidth;
                            }
                        }
                    }
                    else SWidth += 250;
                }
            }

            if (SWidth >= 14000)
            {
                List<ObjectIWPDF> lstSortingWHPDF = (from SO in lstObjectWHPDF
                                                     where SO.PId==LevelId
                                                     orderby SO.Width descending select SO).ToList();
                foreach (ObjectIWPDF OS in lstSortingWHPDF)
                {
                    if (SWidth >= 14001)
                    {
                        OS.UsedFlag = "N";
                        SWidth = SWidth - OS.Width;
                    }
                }

                CurrentCol = 0; CurrentRow = 0; TempNewA0PageRow = NewA0PageRow;
                foreach (ObjectIWPDF OS in lstSortingWHPDF)
                {
                    if (OS.UsedFlag == "N")
                    {
                        //NewA0PageRow++; CurrentCol = 0; CurrentRow = 0;
                        //A0PageSizeBottomUpApproachWH(OS.Id, OS.Id, "U");
                    }
                }

                NewA0PageRow = TempNewA0PageRow;
                RetValue = A0PageSizeBottomUpApproachWH(LevelId, ShowLevel, "Y");
                foreach (ObjectIWPDF OS in lstSortingWHPDF)
                {
                    if (OS.UsedFlag == "N") NewA0PageRow++;
                }
            }
            else
            {
                RetValue[0] = CurrentCol;
                RetValue[1] = CurrentRow;
            }

            return RetValue;
        }

        public int[] BottomUpApproachWH(string LevelId)
        {
            int CurrentCol = 0, CurrentRow = 0;
            int[] AssistanceInf = null, RetValue = { 0, 0, 0, 0 };
            List<ObjectInfPDF> theObjectInf = (from SO in lstObjectPDF where SO.PId == LevelId select SO).ToList();
            foreach (ObjectInfPDF OI in theObjectInf)
            {
                if (OI.Flag == "N")
                {
                    if (LevelId == "-1")
                    {
                        NodeWH = new DataTable();
                        NodeWH.Columns.Add("LevelId", typeof(string));
                        NodeWH.Columns.Add("ParentLevelId", typeof(string));
                        NodeWH.Columns.Add("Owidth", typeof(string));
                        NodeWH.Columns.Add("Oheight", typeof(string));
                        NodeWH.Columns.Add("CurrentCol", typeof(string));
                        NodeWH.Columns.Add("CurrentRow", typeof(string));
                        NodeWH.Columns.Add("BreadGram", typeof(string));
                    }
                    if (OI.NOR.ToString() != "0" && OI.DottedLineFlag=="N")
                    {
                        AssistanceInf = GetAllLevelAssistance(Convert.ToInt32(OI.Id));
                        OI.Width = AssistanceInf[0];
                        OI.Height = AssistanceInf[1];

                        if (OI.NOR == OI.SOC)
                        {
                            RetValue[0] = 350;
                            RetValue[1] = (OI.NOR + 1) * 130;

                            OI.Owidth = (OI.Width >= RetValue[0] ? OI.Width : RetValue[0]);
                            OI.Oheight = OI.Height + RetValue[1];
                            OI.Cwidth = (OI.Width >= RetValue[0] ? OI.Width : RetValue[0]);
                            OI.Cheight = OI.Height + 130;

                            RetValue = BottomUpApproachWH(OI.Id);

                            CurrentCol += OI.Owidth;
                            CurrentRow = (OI.Oheight >= CurrentRow) ? OI.Oheight : CurrentRow;
                        }
                        else
                        {
                            RetValue = BottomUpApproachWH(OI.Id);
                            OI.Owidth = (OI.Width >= RetValue[0] ? OI.Width : RetValue[0]);
                            OI.Oheight = RetValue[1] + 130;
                            OI.Cwidth = (OI.Width >= RetValue[0] ? OI.Width : RetValue[0]);
                            OI.Cheight = OI.Height + 130;

                            CurrentCol += OI.Owidth;
                            CurrentRow = (OI.Oheight >= CurrentRow) ? OI.Oheight : CurrentRow;
                        }
                    }
                    else
                    {
                        OI.Width = 0;
                        OI.Height = 0;
                        OI.Owidth = 250;
                        OI.Oheight = 130;
                        OI.Cwidth = 250;
                        OI.Cheight = 130;

                        CurrentCol += OI.Owidth;
                    }
                    OI.Cwidth = CurrentCol;
                    OI.Cheight = CurrentRow;

                    if (OI.BreadGram.Contains("10001339"))
                    {
                        NodeRow = NodeWH.NewRow();

                        NodeRow["LevelId"] = OI.Id;
                        NodeRow["ParentLevelId"] = OI.PId;
                        NodeRow["Owidth"] = OI.Owidth.ToString();
                        NodeRow["Oheight"] = OI.Oheight.ToString();
                        NodeRow["CurrentCol"] = CurrentCol.ToString();
                        NodeRow["CurrentRow"] = CurrentRow.ToString();
                        NodeRow["BreadGram"] = OI.BreadGram;

                        NodeWH.Rows.Add(NodeRow);
                    }
                }
            }

            RetValue[0] = CurrentCol;
            RetValue[1] = CurrentRow;

            return RetValue;
        }

        private int[] SelectTopLevelVertical(string LevelId, FormattedTextAreaStyle style, Page MyPage, string TopNodeFlag)
        {
            int StartCol = 0, StartRow = 0, CurrentCol = 0, CurrentRow = 0;
            int[] AssistanceInf = null, RetValue = { 0, 0 };
            List<ObjectInfPDF> theSelectedObjectInf = (from SO in lstObjectPDF where SO.PId == LevelId select SO).OrderBy(x => Convert.ToInt32(x.SortNo)).ToList();

            foreach (ObjectInfPDF NodePDF in lstObjectPDF)
            {
                if (NodePDF.Id == LevelId)
                {
                    StartCol = NodePDF.Col;
                    CurrentCol = NodePDF.Col;
                    StartRow = NodePDF.Row;
                    CurrentRow = NodePDF.Row;

                    break;
                }
            }

            if (LevelId == "10020620")
                LevelId = "10020620";

            // Check for the Assistant Node
            AssistanceInf = PutAssistanceFieldInfoPDF(Convert.ToInt32(LevelId), CurrentCol, CurrentRow+100, 200, 80, MyPage, "V");
            if (AssistanceInf[0] >= 1 && AssistanceInf[1] >= 1)
            {
                CurrentCol = AssistanceInf[0];
                CurrentRow = AssistanceInf[1];
            }
            foreach (ObjectInfPDF OI in theSelectedObjectInf)
            {
                if (OI.PId != "-1" && OI.Flag == "N")
                {
                    OI.Col = StartCol + 50;
                    OI.Row = CurrentRow + 100;

                    // Puts the data in the required area.
                    PutFieldInfoPDF(OI, OI.Title, StartCol + 50, CurrentRow+100, 200, 80, 130, MyPage, "V");
                    CurrentRow = CurrentRow + 100;
                }
            }
            MyLine = new ceTe.DynamicPDF.PageElements.Line(StartCol + 20, StartRow + A0StartHeight + 80, StartCol + 20, CurrentRow + A0StartHeight + 60, ShowPDFLineColor(LineColor));
            MyLine.Width = 2;
            MyPage.Elements.Add(MyLine);

            RetValue[0] = CurrentCol+50; RetValue[1] = CurrentRow;
            PageMaxWidth = (PageMaxWidth >= CurrentCol+50) ? PageMaxWidth : CurrentCol+50;
            if (PageMaxHeight <= CurrentRow) PageMaxHeight = CurrentRow;

            return RetValue;

        }

        private int[] CheckBoxInTheRange(int Row, int Col, ObjectInfPDF StartObjectInf, ObjectInfPDF EndObjectInf)
        {
            int[] RetValue = { 0, 0, 0 };
            if (EndObjectInf.RealCol <= Col && EndObjectInf.RealCol + EndObjectInf.RealBoxWidth >= Col &&
                EndObjectInf.RealRow <= Row && EndObjectInf.RealRow + EndObjectInf.RealBoxHeight+20 >= Row)
            {
                RetValue[0] = 5;
                RetValue[1] = EndObjectInf.RealCol;
                RetValue[2] = EndObjectInf.RealRow;

                return RetValue;
            }

            if (EndObjectInf.RealCol <= Col && EndObjectInf.RealCol + EndObjectInf.RealBoxWidth >= Col)
            {
                RetValue[0] = 4;
                RetValue[1] = EndObjectInf.RealCol;
                RetValue[2] = EndObjectInf.RealRow;

                return RetValue;
            }

            foreach (ObjectInfPDF OI in lstObjectPDF)
            {
                if (OI.DottedLineFlag != "Y")
                {
                   if (OI.RealCol <= Col && OI.RealCol + OI.RealBoxWidth >= Col &&
                       OI.RealRow <= Row && OI.RealRow + OI.RealBoxHeight >= Row &&
                       OI.Id != StartObjectInf.Id && OI.Id != EndObjectInf.Id)
                    {
                        RetValue[0] = 1;
                        RetValue[1] = OI.RealCol;
                        RetValue[2] = OI.RealRow;

                        return RetValue;
                    }                
                }
            }

            RetValue[0] = 2;
            RetValue[1] = Col;
            RetValue[2] = Row;

            return RetValue;
        }

        private int[] DrawLineHorizantalForDifferentParent(ObjectInfPDF StartObjectInf, ObjectInfPDF EndObjectInf, 
                                                          int LineWidth, int LineHeight, string ArrowFlag)
        {
            int[] ObjValue = { 0, 0, 0 };
            int ObjRow = StartObjectInf.RealRow + (StartObjectInf.RealBoxHeight/2), 
                ObjCol=StartObjectInf.RealCol + StartObjectInf.RealBoxWidth;
            bool MatchObject = true;
            lstObjStraightLine.Clear();
            while (MatchObject)
            {
                ObjValue = CheckBoxInTheRange(ObjRow, ObjCol + LineWidth, StartObjectInf, EndObjectInf);
                if (ObjValue[0] == 5)
                {
                    MatchObject = false;
                }
                else if (ObjValue[0] == 4)
                {
                    MatchObject = false;
                }
                else if (ObjValue[0] == 3)
                {
                    MatchObject = false;
                }
                else if (ObjValue[0] == 2)
                {
                    ObjectLine ObjLine = new ObjectLine();

                    ObjLine.NodeLineStartCol = ObjCol;
                    ObjLine.NodeLineStartRow = ObjRow;
                    ObjLine.NodeLineEndCol = ObjCol + LineWidth;
                    ObjLine.NodeLineEndRow = ObjRow;
                    ObjLine.ArrowFlag = "";

                    lstObjStraightLine.Add(ObjLine);
                    ObjCol=ObjCol + LineWidth;
                }
                else if (ObjValue[0] == 1)
                {
                    MatchObject = false;
                }
            }

            return ObjValue;
        }

        private void DrawLineHorizantalForSameParent(ObjectInfPDF StartObjectInf, ObjectInfPDF EndObjectInf, string ArrowFlag)
        {
            int ShowNodeIndex = Convert.ToInt32(StartObjectInf.NodeIndex) - Convert.ToInt32(EndObjectInf.NodeIndex);
            if (ShowNodeIndex == 1 || ShowNodeIndex == -1)
            {
                ObjectLine ObjLine = new ObjectLine();
                ObjLine.NodeLineStartCol = StartObjectInf.RealCol + StartObjectInf.RealBoxWidth;
                ObjLine.NodeLineStartRow = StartObjectInf.RealRow + (StartObjectInf.RealBoxHeight / 2);
                ObjLine.NodeLineEndCol = EndObjectInf.RealCol;
                ObjLine.NodeLineEndRow = EndObjectInf.RealRow + (EndObjectInf.RealBoxHeight / 2);
                ObjLine.ArrowFlag = "Left";
                lstObjLine.Add(ObjLine);
            }
            else
            {
                ObjectLine ObjLine = new ObjectLine();
                ObjLine.NodeLineStartCol = StartObjectInf.RealCol + StartObjectInf.RealBoxWidth;
                ObjLine.NodeLineStartRow = StartObjectInf.RealRow + (StartObjectInf.RealBoxHeight / 2);
                ObjLine.NodeLineEndCol = StartObjectInf.RealCol + StartObjectInf.RealBoxWidth + 20;
                ObjLine.NodeLineEndRow = StartObjectInf.RealRow + (StartObjectInf.RealBoxHeight / 2);
                if (ArrowFlag == "R") ObjLine.ArrowFlag = "Right"; else ObjLine.ArrowFlag = "";
                lstObjLine.Add(ObjLine);

                ObjLine = new ObjectLine();
                ObjLine.NodeLineStartCol = StartObjectInf.RealCol + StartObjectInf.RealBoxWidth + 20;
                ObjLine.NodeLineStartRow = StartObjectInf.RealRow + (StartObjectInf.RealBoxHeight / 2);
                ObjLine.NodeLineEndCol = StartObjectInf.RealCol + StartObjectInf.RealBoxWidth + 20;
                ObjLine.NodeLineEndRow = StartObjectInf.RealRow - 20;
                ObjLine.ArrowFlag = "";
                lstObjLine.Add(ObjLine);

                ObjLine = new ObjectLine();
                ObjLine.NodeLineStartCol = StartObjectInf.RealCol + StartObjectInf.RealBoxWidth + 20;
                ObjLine.NodeLineStartRow = StartObjectInf.RealRow - 20;
                ObjLine.NodeLineEndCol = EndObjectInf.RealCol - 20;
                ObjLine.NodeLineEndRow = StartObjectInf.RealRow - 20;
                ObjLine.ArrowFlag = "";
                lstObjLine.Add(ObjLine);

                ObjLine = new ObjectLine();
                ObjLine.NodeLineStartCol = EndObjectInf.RealCol - 20;
                ObjLine.NodeLineStartRow = StartObjectInf.RealRow - 20;
                ObjLine.NodeLineEndCol = EndObjectInf.RealCol - 20;
                ObjLine.NodeLineEndRow = EndObjectInf.RealRow + (EndObjectInf.RealBoxHeight / 2);
                ObjLine.ArrowFlag = "";
                lstObjLine.Add(ObjLine);

                ObjLine = new ObjectLine();
                ObjLine.NodeLineStartCol = EndObjectInf.RealCol - 20;
                ObjLine.NodeLineStartRow = EndObjectInf.RealRow + (EndObjectInf.RealBoxHeight / 2);
                ObjLine.NodeLineEndCol = EndObjectInf.RealCol;
                ObjLine.NodeLineEndRow = EndObjectInf.RealRow + (EndObjectInf.RealBoxHeight / 2);
                ObjLine.ArrowFlag = "Left";
                lstObjLine.Add(ObjLine);
            }
        }

        List<ObjectLine> lstObjLine = new List<ObjectLine>();
        List<ObjectLine> lstObjStraightLine = new List<ObjectLine>();
        private string DrawLineFunctionalManager(string StartObject, string EndObject, string PDFType)
        {
            ObjectInfPDF StartObjectInf = (from SO in lstObjectPDF where SO.Id == StartObject select SO).FirstOrDefault();
            ObjectInfPDF EndObjectInf = (from SO in lstObjectPDF where SO.Id == EndObject select SO).FirstOrDefault();

            if (StartObjectInf != null && EndObjectInf != null)
            {
                if (StartObjectInf.PId == EndObjectInf.PId)
                {
                    if (PDFType == "HORIZANTAL")
                    {
                        if (StartObjectInf.RealCol <= EndObjectInf.RealCol)
                            DrawLineHorizantalForSameParent(StartObjectInf, EndObjectInf, "N");
                        else
                            DrawLineHorizantalForSameParent(EndObjectInf, StartObjectInf, "R");
                    }
                    else if (PDFType == "VERTICAL")
                    {

                    }
                }
                else
                {
                    int[] ObjValue = { 0, 0, 0 };
                    int LineWidth = 100, LineHeight = 50;
                    if (PDFType == "HORIZANTAL")
                    {
                        if (StartObjectInf.RealCol <= EndObjectInf.RealCol)
                            ObjValue=DrawLineHorizantalForDifferentParent(StartObjectInf, EndObjectInf, LineWidth, LineHeight, "N");
                        else
                            ObjValue=DrawLineHorizantalForDifferentParent(EndObjectInf, StartObjectInf, LineWidth, LineHeight, "R");
                    }
                    else if (PDFType == "VERTICAL")
                    {
                    }

                    if (ObjValue[0] != 5 && ObjValue[0] != 4)
                    {
                        string[] ArrayStartObjectInf = StartObjectInf.BreadGram.Split(new[] { "-->" }, StringSplitOptions.None);
                        string[] ArrayEndObjectInf = EndObjectInf.BreadGram.Split(new[] { "-->" }, StringSplitOptions.None);
                        if (ArrayStartObjectInf.Length >= 2)
                        {
                            string ParentLevelId = ArrayStartObjectInf[ArrayStartObjectInf.Length - 2];
                            ObjectInfPDF ParentObjectInfo = (from OI in lstObjectPDF where OI.Id == ParentLevelId select OI).FirstOrDefault();
                            string StartLevelId = ArrayStartObjectInf[ArrayStartObjectInf.Length - 1];
                            ObjectInfPDF StartObjectInfo = (from OI in lstObjectPDF where OI.Id == StartLevelId select OI).FirstOrDefault();
                            string EndLevelId = ArrayEndObjectInf[ArrayEndObjectInf.Length - 1];
                            ObjectInfPDF EndObjectInfo = (from OI in lstObjectPDF where OI.Id == EndLevelId select OI).FirstOrDefault();

                            ObjectLine ObjLine = new ObjectLine();
                            ObjLine.NodeLineStartCol = StartObjectInfo.RealCol + (StartObjectInfo.RealBoxWidth / 2);
                            ObjLine.NodeLineStartRow = ParentObjectInfo.Row + (ParentObjectInfo.RealBoxHeight + 30);
                            ObjLine.NodeLineEndCol = StartObjectInfo.RealCol + (StartObjectInfo.RealBoxWidth / 2);
                            ObjLine.NodeLineEndRow = StartObjectInfo.RealRow;
                            ObjLine.ArrowFlag = "";
                            lstObjLine.Add(ObjLine);

                            ObjLine = new ObjectLine();
                            ObjLine.NodeLineStartCol = StartObjectInfo.RealCol + (StartObjectInfo.RealBoxWidth / 2);
                            ObjLine.NodeLineStartRow = ParentObjectInfo.Row + (ParentObjectInfo.RealBoxHeight + 30);
                            ObjLine.NodeLineEndCol = ParentObjectInfo.Col - 20;
                            ObjLine.NodeLineEndRow = ParentObjectInfo.Row + (ParentObjectInfo.RealBoxHeight + 30);
                            ObjLine.ArrowFlag = "";
                            lstObjLine.Add(ObjLine);

                            ObjLine = new ObjectLine();
                            ObjLine.NodeLineStartCol = ParentObjectInfo.Col - 20;
                            ObjLine.NodeLineStartRow = ParentObjectInfo.Row + (ParentObjectInfo.RealBoxHeight + 30);
                            ObjLine.NodeLineEndCol = ParentObjectInfo.Col - 20;
                            ObjLine.NodeLineEndRow = (int)PageMaxHeight + 400;
                            ObjLine.ArrowFlag = "";
                            lstObjLine.Add(ObjLine);

                            ObjLine = new ObjectLine();
                            ObjLine.NodeLineStartCol = EndObjectInfo.Col - 20;
                            ObjLine.NodeLineStartRow = (int)PageMaxHeight + 400;
                            ObjLine.NodeLineEndCol = ParentObjectInfo.Col - 20;
                            ObjLine.NodeLineEndRow = (int)PageMaxHeight + 400;
                            ObjLine.ArrowFlag = "";
                            lstObjLine.Add(ObjLine);

                            ObjLine = new ObjectLine();
                            ObjLine.NodeLineStartCol = EndObjectInfo.Col - 20;
                            ObjLine.NodeLineStartRow = EndObjectInfo.RealRow + (EndObjectInfo.RealBoxHeight / 2);
                            ObjLine.NodeLineEndCol = EndObjectInfo.Col - 20;
                            ObjLine.NodeLineEndRow = (int)PageMaxHeight + 400;
                            ObjLine.ArrowFlag = "";
                            lstObjLine.Add(ObjLine);

                            ObjLine = new ObjectLine();
                            ObjLine.NodeLineStartCol = EndObjectInfo.Col - 20;
                            ObjLine.NodeLineStartRow = EndObjectInfo.RealRow + (EndObjectInfo.RealBoxHeight / 2);
                            ObjLine.NodeLineEndCol = EndObjectInfo.RealCol;
                            ObjLine.NodeLineEndRow = EndObjectInfo.RealRow + (EndObjectInfo.RealBoxHeight / 2);
                            ObjLine.ArrowFlag = "";
                            lstObjLine.Add(ObjLine);
                        }
                    }
                    else
                    {
                        int ObjRow = 0, ObjCol = 0;
                        ObjectLine ObjLine = null;
                        foreach (ObjectLine OL in lstObjStraightLine)
                        {
                            ObjLine = new ObjectLine();
                            ObjLine.NodeLineStartCol = OL.NodeLineStartCol;
                            ObjLine.NodeLineStartRow = OL.NodeLineStartRow;
                            ObjLine.NodeLineEndCol = OL.NodeLineEndCol;
                            ObjLine.NodeLineEndRow = OL.NodeLineEndRow;
                            ObjLine.ArrowFlag = OL.ArrowFlag;
                            lstObjLine.Add(ObjLine);

                            ObjCol = OL.NodeLineEndCol;
                            ObjRow = OL.NodeLineEndRow;
                        }

                        if (StartObjectInf.RealCol >= EndObjectInf.RealCol)
                        {
                            ObjectInfPDF TempObjectInf = StartObjectInf;
                            StartObjectInf = EndObjectInf;
                            EndObjectInf = TempObjectInf;
                        }

                        if (ObjValue[0] == 4)
                        {
                            if (ObjCol >= EndObjectInf.RealCol)
                            {
                                ObjLine = new ObjectLine();
                                ObjLine.NodeLineStartCol = EndObjectInf.RealCol - 20;
                                ObjLine.NodeLineStartRow = EndObjectInf.RealRow + (EndObjectInf.RealBoxHeight / 2);
                                ObjLine.NodeLineEndCol = EndObjectInf.RealCol;
                                ObjLine.NodeLineEndRow = EndObjectInf.RealRow + (EndObjectInf.RealBoxHeight / 2); ;
                                ObjLine.ArrowFlag = "";
                                lstObjLine.Add(ObjLine);

                                ObjLine = new ObjectLine();
                                ObjLine.NodeLineStartCol = EndObjectInf.RealCol - 20;
                                ObjLine.NodeLineStartRow = EndObjectInf.RealRow + (EndObjectInf.RealBoxHeight/2);
                                ObjLine.NodeLineEndCol = EndObjectInf.RealCol - 20;
                                ObjLine.NodeLineEndRow = ObjRow;
                                ObjLine.ArrowFlag = "";
                                lstObjLine.Add(ObjLine);

                                ObjLine = new ObjectLine();
                                ObjLine.NodeLineStartCol = ObjCol;
                                ObjLine.NodeLineStartRow = ObjRow;
                                ObjLine.NodeLineEndCol = EndObjectInf.RealCol - 20;
                                ObjLine.NodeLineEndRow = ObjRow;
                                ObjLine.ArrowFlag = "";
                                lstObjLine.Add(ObjLine);
                            }
                            else
                            {
                                ObjLine = new ObjectLine();
                                ObjLine.NodeLineStartCol = EndObjectInf.RealCol - 20;
                                ObjLine.NodeLineStartRow = EndObjectInf.RealRow + (EndObjectInf.RealBoxHeight / 2);
                                ObjLine.NodeLineEndCol = EndObjectInf.RealCol;
                                ObjLine.NodeLineEndRow = EndObjectInf.RealRow + (EndObjectInf.RealBoxHeight / 2); ;
                                ObjLine.ArrowFlag = "";
                                lstObjLine.Add(ObjLine);

                                ObjLine = new ObjectLine();
                                ObjLine.NodeLineStartCol = EndObjectInf.RealCol - 20;
                                ObjLine.NodeLineStartRow = ObjRow;
                                ObjLine.NodeLineEndCol = EndObjectInf.RealCol - 20;
                                ObjLine.NodeLineEndRow = EndObjectInf.RealRow + (EndObjectInf.RealBoxHeight / 2);
                                ObjLine.ArrowFlag = "";
                                lstObjLine.Add(ObjLine);

                                ObjLine = new ObjectLine();
                                ObjLine.NodeLineStartCol = ObjCol;
                                ObjLine.NodeLineStartRow = ObjRow;
                                ObjLine.NodeLineEndCol = EndObjectInf.RealCol - 20;
                                ObjLine.NodeLineEndRow = ObjRow;
                                ObjLine.ArrowFlag = "";
                                lstObjLine.Add(ObjLine);
                            }
                        }
                        else if (ObjValue[1] == 5)
                        {
                            ObjLine = new ObjectLine();
                            ObjLine.NodeLineStartCol = ObjCol;
                            ObjLine.NodeLineStartRow = ObjRow;
                            ObjLine.NodeLineEndCol = EndObjectInf.RealCol;
                            ObjLine.NodeLineEndRow = ObjRow;
                            ObjLine.ArrowFlag = "";
                            lstObjLine.Add(ObjLine);
                        }
                    }
                }
            }

            return "Yes";
        }

        private string ShowFunctionalManager(DataTable OrgDataTableFM, string PDFType, Page MyPage)
        {
            if (lstObjLine.Count >= 1) lstObjLine.Clear();
            List<ObjectInfPDF> theSelectedObjectInf = (from SO in lstObjectPDF where SO.PId == Level select SO).OrderBy(x => Convert.ToInt32(x.SortNo)).ToList();
            foreach(DataRow dr in OrgDataTableFM.Rows)
            {
                lstObjLine.Clear();
                lstObjStraightLine.Clear();

                DrawLineFunctionalManager(dr["LEVEL_ID"].ToString(), dr["PARENT_LEVEL_ID"].ToString(), PDFType);
                foreach (ObjectLine OL in lstObjLine)
                {
                    MyLine = new ceTe.DynamicPDF.PageElements.Line(OL.NodeLineStartCol, OL.NodeLineStartRow, OL.NodeLineEndCol, OL.NodeLineEndRow);
                    MyLine.Width = 2;
                    MyLine.Style = LineStyle.Dash;
                    MyLine.Color = ShowPDFBoxColor(BoxColor);
                    MyPage.Elements.Add(MyLine);
                }
            }

            return "Yes";
        }

        // Populates the left assocaites
        Page[] MyListPage = new Page[100];
        int PageIndex = 0;
        public string OmmitExcessAssociates(Document MyDocument, FormattedTextAreaStyle style, string CurrentLevel, string ShowLevel)
        {
            List<ObjectIWPDF> lstSortingWHPDF = (from SO in lstObjectWHPDF
                                                 where SO.Id == CurrentLevel
                                                 orderby SO.Width descending
                                                 select SO).ToList();
            foreach (ObjectIWPDF OS in lstSortingWHPDF)
            {
                if (OS.UsedFlag == "N")
                {
                    PageDimensions MyPageDimensions = new PageDimensions(14300F, 9000F);
                    MyListPage[PageIndex] = new ceTe.DynamicPDF.Page(MyPageDimensions);

                    SelectTopLevelHorizantal(OS.Id, style, MyListPage[PageIndex], MyDocument, "Y", "Yes", ShowLevel, "Y");
                    PageIndex++;

                    return "No";
                }
            }

            return "Yes";
        }

        public string ShowFitInAssociates(string CurrentLevel)
        {
            List<ObjectIWPDF> lstSortingWHPDF = (from SO in lstObjectWHPDF
                                                 where SO.Id == CurrentLevel
                                                 orderby SO.Width descending
                                                 select SO).ToList();
            foreach (ObjectIWPDF OS in lstSortingWHPDF)
            {
                if (OS.UsedFlag == "N")
                {
                    return "No";
                }
            }

            return "Yes";
        }

        // Recursive call(Horizantal Positions)
        float PageMaxWidth = 0, PageMaxHeight = 0;
        int A0StartHeight=0;
        public int[] SelectTopLevelHorizantal(string LevelId, FormattedTextAreaStyle style, Page MyPage, Document MyDocument, 
                                              string TopNodeFlag, string LevelUp, string ShowLevel, string A0PageSizeFlag)
        {
            int NodeIndex = 1;
            ObjectInfPDF TopNode = null, RealTopNode = null, LastNode=null, OneLevelUpNode=null;
            int StartCol = 0, StartRow = 0, CurrentCol=0, CurrentRow=0, Column=0;
            int[] AssistanceInf = null, RetValue = { 0, 0 };
            List<ObjectInfPDF> theSelectedObjectInf = (from SO in lstObjectPDF where SO.PId == LevelId select SO).OrderBy(x => Convert.ToInt32(x.SortNo)).ToList();
            foreach (ObjectInfPDF OI in theSelectedObjectInf)
            {
                if (OI.PId != "-1" && OI.Flag == "N" && ShowFitInAssociates(OI.Id)=="Yes")
                {
                    if (OI.Id == "10027875")
                        OI.Flag = OI.Flag;

                    RetValue = GetHorizantalNodeRowColumn(theSelectedObjectInf, OI.SortNo, OI.PId);
                    CurrentCol = RetValue[0]; CurrentRow = RetValue[1];
                    OI.Col = CurrentCol;
                    OI.Row = CurrentRow;
                    OI.NodeIndex = NodeIndex++;
                    LastNode = OI;

                    if (StartCol==0 && StartRow==0)
                    {
                        StartRow = RetValue[1] + A0StartHeight;
                    }

                    // Puts the data in the required area.
                    if (OI.NOR != OI.SOC && OI.DottedLineFlag=="N")
                    {
                        if (OI.NOR.ToString() == "1") {
                            Column = PutFieldInfoPDF(OI, OI.Title, CurrentCol, CurrentRow, 200, 80, OI.Height, MyPage, "HB");
                            AssistanceInf = PutAssistanceFieldInfoPDF(Convert.ToInt32(OI.Id), CurrentCol, CurrentRow+100, 200, 80, MyPage, "H");
                        }
                        else {
                            Column = PutFieldInfoPDF(OI, OI.Title, CurrentCol + ((OI.Owidth / 2) - 100), CurrentRow, 200, 80, OI.Height, MyPage, "HB");
                            AssistanceInf = PutAssistanceFieldInfoPDF(Convert.ToInt32(OI.Id), CurrentCol + ((OI.Owidth / 2) - 100), CurrentRow+100, 200, 80, MyPage, "H");
                        }
                        RetValue = SelectTopLevelHorizantal(OI.Id, style, MyPage, MyDocument, "N", LevelUp, ShowLevel, A0PageSizeFlag);
                    }
                    else if (OI.NOR == OI.SOC || OI.DottedLineFlag == "Y")
                    {
                        Column = PutFieldInfoPDF(OI, OI.Title, CurrentCol, CurrentRow, 200, 80, OI.Height, MyPage, "H");
                        if (OI.NOR != 0 && OI.DottedLineFlag == "N") RetValue = SelectTopLevelVertical(OI.Id, style, MyPage, "N");
                    }
                    if (StartCol == 0) StartCol = Column;
                }
            }

            if (LevelId == "10027875")
                LevelId = LevelId;

            MyLine = new ceTe.DynamicPDF.PageElements.Line(StartCol, StartRow - 30, Column, StartRow - 30, ShowPDFLineColor(LineColor));
            MyLine.Width = 2;
            MyPage.Elements.Add(MyLine);

            // Puts the data in the required area
            StartPosition = Level;
            if (LevelId == StartPosition)
            {
                TopNode = (from SO in lstObjectPDF where SO.Id == LevelId select SO).FirstOrDefault();
                if (LevelUp == "Yes")
                {
                    RealTopNode = (from SO in lstObjectPDF where SO.Id == ShowLevel select SO).FirstOrDefault();
                    if (!(A0PageSizeFlag=="Y"))
                    {
                        TopNode.RealCol = RealTopNode.RealCol;
                        TopNode.RealRow = RealTopNode.RealRow - (TopNode.Height + 100);
                        TopNode.RealBoxWidth = RealTopNode.RealBoxWidth;
                        TopNode.RealBoxHeight = RealTopNode.RealBoxHeight;
                    }
                    else
                    {
                        TopNode.RealCol = (RealTopNode.Owidth / 2) - 200;
                        TopNode.RealRow =  StartRow - A0StartHeight-110;
                        TopNode.RealBoxWidth = RealTopNode.RealBoxWidth;
                        TopNode.RealBoxHeight = RealTopNode.RealBoxHeight;

                        MyLabel = new ceTe.DynamicPDF.PageElements.Label(TopNode.BreadGramName, 3, (TopNode.RealRow + A0StartHeight) - 175, 14000, 20,
                                                                                                     ceTe.DynamicPDF.Font.HelveticaBold, 20,
                                                                                                     ceTe.DynamicPDF.TextAlign.Left);
                        MyPage.Elements.Add(MyLabel);
                        MyLine = new ceTe.DynamicPDF.PageElements.Line(0, (TopNode.RealRow + A0StartHeight) - 150, 14000, (TopNode.RealRow + A0StartHeight) - 150, ShowPDFLineColor("#ffb266"));
                        MyLine.Width = 2;
                        MyPage.Elements.Add(MyLine);


                        if (TopNode.PId != "999999")
                        {
                            OneLevelUpNode = (from SO in lstObjectPDF where SO.Id == TopNode.PId select SO).FirstOrDefault();
                            PutFieldInfoPDF(OneLevelUpNode, OneLevelUpNode.Title, TopNode.RealCol, TopNode.RealRow - 120, 200, 80, TopNode.Height, MyPage, "B");
                        }
                    }

                    PutFieldInfoPDF(TopNode, TopNode.Title, TopNode.RealCol, TopNode.RealRow-20, 200, 80, TopNode.Height, MyPage, "B");
                    PutAssistanceFieldInfoPDF(Convert.ToInt32(TopNode.Id), (TopNode.RealCol + (TopNode.RealBoxWidth / 2) - 100), (TopNode.RealRow + TopNode.RealBoxHeight + 20), 200, 80, MyPage, "H");
                }
                else
                {
                    if (TopNode.NOR == 1)
                    {
                        string ID = LastNode.Id;
                        RealTopNode = (from SO in lstObjectPDF where SO.Id == ID select SO).FirstOrDefault();

                        TopNode.RealCol = RealTopNode.RealCol;
                        TopNode.RealRow = RealTopNode.RealRow - (TopNode.Height + 100);
                        TopNode.RealBoxWidth = RealTopNode.RealBoxWidth;
                        TopNode.RealBoxHeight = RealTopNode.RealBoxHeight;

                        PutFieldInfoPDF(TopNode, TopNode.Title, TopNode.RealCol, TopNode.RealRow - 20, 200, 80, TopNode.Height, MyPage, "B");
                        PutAssistanceFieldInfoPDF(Convert.ToInt32(TopNode.Id), (TopNode.RealCol + (TopNode.RealBoxWidth / 2) - 100), (TopNode.RealRow + TopNode.RealBoxHeight + 20), 200, 80, MyPage, "H");
                    }
                    else
                    {
                        PutFieldInfoPDF(TopNode, TopNode.Title, ((TopNode.Owidth-200) / 2), StartRow - (TopNode.Height + 130), 200, 80, TopNode.Height, MyPage, "B");
                        PutAssistanceFieldInfoPDF(Convert.ToInt32(TopNode.Id), (TopNode.RealCol + (TopNode.RealBoxWidth / 2) - 100), (TopNode.RealRow + TopNode.RealBoxHeight + 20), 200, 80, MyPage, "H");
                    }
                }
            }

            RetValue[0] = CurrentCol; RetValue[1] = CurrentRow;
            PageMaxWidth = (PageMaxWidth >= CurrentCol)?PageMaxWidth: CurrentCol;
            if (PageMaxHeight <= CurrentRow) PageMaxHeight = CurrentRow;

            return RetValue;
        }

        // Get Line Color for display
        private ceTe.DynamicPDF.RgbColor ShowPDFLineColor(string LineColor)
        {
            switch (LineColor)
            {
                case "#663300":
                    return ceTe.DynamicPDF.WebColor.Brown;
                    break;
                case "#ffb266":
                    return ceTe.DynamicPDF.WebColor.Orange;
                    break;
                case "#ffffff":
                    return ceTe.DynamicPDF.WebColor.White;
                    break;
                case "#ff0000":
                    return ceTe.DynamicPDF.WebColor.Red;
                    break;
                case "#911414":
                    return ceTe.DynamicPDF.WebColor.DarkRed;
                    break;
                case "#000000":
                    return ceTe.DynamicPDF.WebColor.Black;
                    break;

            }

            return ceTe.DynamicPDF.WebColor.DarkGray;
        }

        // Get Line Color for display
        private ceTe.DynamicPDF.RgbColor ShowPDFBoxColor(string LineColor)
        {
            switch (LineColor)
            {
                case "#663300":
                    return ceTe.DynamicPDF.WebColor.Brown;
                    break;
                case "#ffb266":
                    return ceTe.DynamicPDF.WebColor.Orange;
                    break;
                case "#ffffff":
                    return ceTe.DynamicPDF.WebColor.White;
                    break;
                case "#ff0000":
                    return ceTe.DynamicPDF.WebColor.Red;
                    break;
                case "#911414":
                    return ceTe.DynamicPDF.WebColor.DarkRed;
                    break;
                case "#000000":
                    return ceTe.DynamicPDF.WebColor.Black;
                    break;

            }

            return ceTe.DynamicPDF.WebColor.DarkRed;
        }

        // Get Font Color for display
        private ceTe.DynamicPDF.RgbColor ShowPDFFontColor(string FontColor)
        {
            switch (FontColor)
            {
                case "#663300":
                    return ceTe.DynamicPDF.WebColor.Brown;
                    break;
                case "#ffb266":
                    return ceTe.DynamicPDF.WebColor.Orange;
                    break;
                case "#ffffff":
                    return ceTe.DynamicPDF.WebColor.White;
                    break;
                case "#ff0000":
                    return ceTe.DynamicPDF.WebColor.Red;
                    break;
                case "#911414":
                    return ceTe.DynamicPDF.WebColor.DarkRed;
                    break;
                case "#000000":
                    return ceTe.DynamicPDF.WebColor.Black;
                    break;

            }

            return ceTe.DynamicPDF.WebColor.Black;
        }

        // Put All Level information in Vertical PDF
        private void PutFieldInfoVerticalPDF(string Info, int CurrentCol, int CurrentRow, int Width, int Height, int VerticalLineHeight, Page MyPage, string ConnectorLineType)
        {
            int Idx = 0, Idy = 0;
            string[] LabelInfo = Info.Replace("&amp;", "&").Split(';');
            string FontName = "", FontSize = "", FontColor = "", FontStyle = "", FontFloat = "", FontWidth = "", Adjustment = "";
            DataTable dtFieldInf = dtFieldActive;

            if (LabelInfo.Length >= 1)
            {
                MyRect = new ceTe.DynamicPDF.PageElements.Rectangle(CurrentCol, CurrentRow, Width, Height);
                MyPage.Elements.Add(MyRect);
                if (ConnectorLineType == "V")
                {
                    MyLine = new ceTe.DynamicPDF.PageElements.Line(CurrentCol - 30, CurrentRow + 60, CurrentCol - 1, CurrentRow + 60, GetPDFLineColor(LineColor));
                    MyLine.Width = 2;
                    MyPage.Elements.Add(MyLine);
                }
                else if (ConnectorLineType == "H")
                {
                    MyLine = new ceTe.DynamicPDF.PageElements.Line(CurrentCol + 100, CurrentRow - 30, CurrentCol + 100, CurrentRow, GetPDFLineColor(LineColor));
                    MyLine.Width = 2;
                    MyPage.Elements.Add(MyLine);
                }
                else if (ConnectorLineType == "B")
                {
                    MyLine = new ceTe.DynamicPDF.PageElements.Line(CurrentCol + 100, CurrentRow + 80, CurrentCol + 100, CurrentRow + 100 + VerticalLineHeight, GetPDFLineColor(LineColor));
                    MyLine.Width = 2;
                    MyPage.Elements.Add(MyLine);
                }
                foreach (DataRow drFI in dtFieldInf.Rows)
                {
                    try
                    {
                        string[] LabelText = LabelInfo[Idx++].Split('|');
                        if (drFI["ALL_LEVEL_FLAG"].ToString() == "Y")
                        {
                            Idy++;
                            FontName = drFI["FONT_NAME"].ToString();
                            FontSize = drFI["FONT_SIZE"].ToString();
                            FontColor = drFI["FONT_COLOR"].ToString();
                            FontStyle = drFI["FONT_STYLE"].ToString();
                            FontFloat = drFI["FONT_FLOAT"].ToString();
                            FontWidth = drFI["FIELD_WIDTH"].ToString();
                            Adjustment = drFI["ADJUSTMENT"].ToString();

                            MyLabel = new ceTe.DynamicPDF.PageElements.Label(LabelText[0].ToString(), CurrentCol, CurrentRow + ((Idy - 1) * 18)+5, Width, Height,
                                                                             ceTe.DynamicPDF.Font.HelveticaBold, 8,
                                                                             ceTe.DynamicPDF.TextAlign.Center);
                            MyPage.Elements.Add(MyLabel);

                        }
                    }
                    catch (Exception)
                    {

                    }
                }
            }
        }

        // Put All Level information(Assistance) in Vertical PDF
        private int[] PutAssistanceFieldInfoVerticalPDF(int LevelId, int CurrentCol, int CurrentRow, int Width, int Height, Page MyPage)
        {
            int Idx = 0, Idy = 0, Index = 0, AssistanceCol = CurrentCol + 200, AssistanceRow = CurrentRow - 40;
            int[] retValue = { 0, 0 };
            string FontName = "", FontSize = "", FontColor = "", FontStyle = "", FontFloat = "", FontWidth = "", Adjustment = "";
            string[] LabelInfo = null;
            DataTable dtFieldInf = dtFieldActive;

            DataRow[] drObj = dtLevel2.Select("POSITIONID='" + LevelId + "'");
            foreach (DataRow drInf in drObj)
            {
                string jsonString = drInf["Data1"].ToString();
                if (drInf["Data2"] != null) jsonString += drInf["Data2"].ToString();
                if (drInf["Data3"] != null) jsonString += drInf["Data3"].ToString();
                if (drInf["Data4"] != null) jsonString += drInf["Data4"].ToString();
                jsonString = jsonString.Substring(0, jsonString.IndexOf("}]") + 2);
                List<ObjectInfPDF> theObjectInf = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<List<ObjectInfPDF>>(jsonString);
                foreach (ObjectInfPDF OI in theObjectInf)
                {
                    if (OI.PId != "-1" && OI.Flag == "Y")
                    {
                        LabelInfo = OI.Title.Replace("&amp;", "&").Split(';');
                        if (LabelInfo != null)
                        {
                            MyRect = new ceTe.DynamicPDF.PageElements.Rectangle(AssistanceCol, AssistanceRow, Width, Height);
                            MyPage.Elements.Add(MyRect);
                            MyLine = new ceTe.DynamicPDF.PageElements.Line(CurrentCol - 30, CurrentRow + (Index * 100) + 10, CurrentCol + 200, CurrentRow + (Index * 100) + 10, GetPDFLineColor(LineColor));
                            MyLine.Width = 2;
                            MyPage.Elements.Add(MyLine);

                            Idy = 0; Idx = 0;
                            foreach (DataRow drFI in dtFieldInf.Rows)
                            {
                                try
                                {
                                    string[] LabelText = LabelInfo[Idx++].Split('|');
                                    if (drFI["ALL_LEVEL_FLAG"].ToString() == "Y")
                                    {
                                        Idy++;
                                        FontName = drFI["FONT_NAME"].ToString();
                                        FontSize = drFI["FONT_SIZE"].ToString();
                                        FontColor = drFI["FONT_COLOR"].ToString();
                                        FontStyle = drFI["FONT_STYLE"].ToString();
                                        FontFloat = drFI["FONT_FLOAT"].ToString();
                                        FontWidth = drFI["FIELD_WIDTH"].ToString();
                                        Adjustment = drFI["ADJUSTMENT"].ToString();

                                        MyLabel = new ceTe.DynamicPDF.PageElements.Label(LabelText[0].ToString(), AssistanceCol, AssistanceRow + ((Idy - 1) * 18)+5, Width, Height,
                                                                                         ceTe.DynamicPDF.Font.HelveticaBold, 8,
                                                                                         ceTe.DynamicPDF.TextAlign.Center);
                                        MyPage.Elements.Add(MyLabel);

                                    }
                                }
                                catch (Exception)
                                {

                                }
                            }
                            AssistanceRow += 100; Index++;
                        }
                    }
                }
            }

            if (Idy >= 1)
            {
                retValue[0] = AssistanceCol;
                retValue[1] = AssistanceRow;
            }

            return retValue;
        }

        private int GetObjectInfVerticalPDF(List<ObjectInfPDF> lstObjectInfPDF, string LevelId)
        {
            int Index = 0;
            foreach (ObjectInfPDF Obj in lstObjectInfPDF)
            {
                if (Obj.Id == LevelId && Obj.DottedLineFlag=="N") return Index;
                Index++;
            }

            return 999999;
        }

        // Recursive call(Vertical Positions)
        public int SelectLevelVerticalPDF(string LevelId, int CurrentCol, int MaxCol, FormattedTextAreaStyle style, Page MyPage)
        {
            int[] AssistanceInf = null;
            DataRow[] drObj = dtLevel2.Select("POSITIONID='" + LevelId + "'");
            foreach (DataRow drInf in drObj)
            {
                string jsonString = drInf["Data1"].ToString();
                if (drInf["Data2"] != null) jsonString += drInf["Data2"].ToString();
                if (drInf["Data3"] != null) jsonString += drInf["Data3"].ToString();
                if (drInf["Data4"] != null) jsonString += drInf["Data4"].ToString();
                jsonString = jsonString.Substring(0, jsonString.IndexOf("}]") + 2);
                List<ObjectInfPDF> theObjectInf = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<List<ObjectInfPDF>>(jsonString);
                foreach (ObjectInfPDF OI in theObjectInf)
                {
                    if (OI.PId != "-1" && OI.Flag == "N")
                    {
                        if (OI.Id.ToString() == "10017999")
                            OI.Title = OI.Title;
                        lstObjectPDF.Add(new ObjectInfPDF(OI.Id.ToString(),
                                                          OI.Title,
                                                          OI.PId,
                                                          OI.Level,
                                                          0,
                                                          0,
                                                          0,
                                                          0,
                                                          0,
                                                          0,
                                                          0,
                                                          0,
                                                          OI.NextLevelFlag,
                                                          OI.GrayColourFlag,
                                                          OI.DottedLineFlag,
                                                          OI.ShowFullBox,
                                                          OI.Language,
                                                          OI.SortNo,
                                                          OI.NOR.ToString(),
                                                          OI.SOC.ToString(),
                                                          OI.PositionFlag,
                                                          OI.ColorFlag,
                                                          OI.BackColor,
                                                          OI.Flag,
                                                          OI.BreadGram,
                                                          OI.BreadGramName,
                                                          OI.ActualLevelNo,
                                                          0, 0, 0, 0, 0));

                        // Puts the data in the required area.
                        PutFieldInfoVerticalPDF(OI.Title, CurrentCol, CurrentRow, 200, 80, 0, MyPage, "V");

                        if (OI.NOR.ToString() != "0" && OI.DottedLineFlag == "N")
                        {
                            CurrentCol = CurrentCol + 50;
                            if (MaxCol <= CurrentCol) MaxCol = CurrentCol;
                            CurrentRow = CurrentRow + 100;

                            lstObjectPDF[lstObjectPDF.Count - 1].Width = CurrentCol - 30;
                            lstObjectPDF[lstObjectPDF.Count - 1].Height = CurrentRow - 20;

                            AssistanceInf = PutAssistanceFieldInfoVerticalPDF(Convert.ToInt32(OI.Id), CurrentCol, CurrentRow, 200, 80, MyPage);
                            if (AssistanceInf != null)
                            {
                                if (AssistanceInf[1] >= CurrentRow) CurrentRow = AssistanceInf[1];
                                if (AssistanceInf[0] >= MaxCol) MaxCol = AssistanceInf[0];
                            }
                            else
                            {
                                AssistanceInf = new int[2];
                                AssistanceInf[0] = 0; AssistanceInf[1] = 0;
                            }

                            SelectLevelVerticalPDF(OI.Id, CurrentCol, MaxCol, style, MyPage);
                            int Index = GetObjectInfVerticalPDF(lstObjectPDF, OI.Id);
                            if (Index != 999999)
                            {
                                lstObjectPDF[Index].Owidth = CurrentCol - 30;
                                lstObjectPDF[Index].Oheight = CurrentRow - 20;
                            }
                            CurrentCol = CurrentCol - 50;
                        }
                        else
                        {
                            CurrentRow = CurrentRow + 100;
                            int Index = GetObjectInfVerticalPDF(lstObjectPDF, OI.PId);
                            if (Index != 999999)
                            {
                                lstObjectPDF[Index].Owidth = CurrentCol - 30;
                                lstObjectPDF[Index].Oheight = CurrentRow - 20;
                            }
                        }
                    }
                }
            }
            CurrentCol = CurrentCol - 50;
            if (PageMaxHeight <= CurrentRow) PageMaxHeight = CurrentRow;

            return MaxCol + 250;
        }

        // Recursive call(Horizantal Positions)
        public int SelectLevelHorizantalPDF(string LevelId, int CurrentCol, int MaxCol, FormattedTextAreaStyle style, Page MyPage, string TopNodeFlag)
        {
            ObjectInfPDF TopNode = null;
            int StartCol = CurrentCol, StartRow = CurrentRow, LineMaxCol=0, LineStartRow=0;
            int[] AssistanceInf = null;
            DataRow[] drObj = dtLevel2.Select("POSITIONID='" + LevelId + "'");
            foreach (DataRow drInf in drObj)
            {
                string jsonString = drInf["Data1"].ToString();
                if (drInf["Data2"] != null) jsonString += drInf["Data2"].ToString();
                if (drInf["Data3"] != null) jsonString += drInf["Data3"].ToString();
                if (drInf["Data4"] != null) jsonString += drInf["Data4"].ToString();
                jsonString = jsonString.Substring(0, jsonString.IndexOf("}]") + 2);
                List<ObjectInfPDF> theObjectInf = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<List<ObjectInfPDF>>(jsonString);
                foreach (ObjectInfPDF OI in theObjectInf)
                {
                    if (OI.PId != "-1" && OI.Flag == "N")
                    {
                        lstObjectPDF.Add(new ObjectInfPDF(OI.Id.ToString(),
                                                          OI.Title,
                                                          OI.PId,
                                                          OI.Level,
                                                          0,
                                                          0,
                                                          0,
                                                          0,
                                                          0,
                                                          0,
                                                          0,
                                                          0,
                                                          OI.NextLevelFlag,
                                                          OI.GrayColourFlag,
                                                          OI.DottedLineFlag,
                                                          OI.ShowFullBox,
                                                          OI.Language,
                                                          OI.SortNo,
                                                          OI.NOR.ToString(),
                                                          OI.SOC.ToString(),
                                                          OI.PositionFlag,
                                                          OI.ColorFlag,
                                                          OI.BackColor,
                                                          OI.Flag,
                                                          OI.BreadGram,
                                                          OI.BreadGramName,
                                                          OI.ActualLevelNo,
                                                          0, 0, 0, 0, 0));

                        // Puts the data in the required area.
                        CurrentRow = LineStartRow;
                        PutFieldInfoVerticalPDF(OI.Title, CurrentCol, CurrentRow, 200, 80, 0, MyPage, "H");

                        if (OI.NOR.ToString() != "0" && OI.DottedLineFlag == "N")
                        {
                            CurrentCol = CurrentCol + 50;
                            if (MaxCol <= CurrentCol) MaxCol = CurrentCol;
                            CurrentRow = CurrentRow + 100;

                            lstObjectPDF[lstObjectPDF.Count - 1].Width = CurrentCol - 30;
                            lstObjectPDF[lstObjectPDF.Count - 1].Height = CurrentRow - 20;

                            AssistanceInf = PutAssistanceFieldInfoVerticalPDF(Convert.ToInt32(OI.Id), CurrentCol, CurrentRow, 200, 80, MyPage);
                            if (AssistanceInf != null)
                            {
                                if (AssistanceInf[1] >= CurrentRow) CurrentRow = AssistanceInf[1];
                                if (AssistanceInf[0] >= MaxCol) MaxCol = AssistanceInf[0];
                            }
                            else
                            {
                                AssistanceInf = new int[2];
                                AssistanceInf[0] = 0; AssistanceInf[1] = 0;
                            }

                            MaxCol = SelectLevelVerticalPDF(OI.Id, CurrentCol, MaxCol, style, MyPage);
                            if (AssistanceInf[0] >= MaxCol) MaxCol = AssistanceInf[0];
                            int Index = GetObjectInfVerticalPDF(lstObjectPDF, OI.Id);
                            if (Index != 999999)
                            {
                                lstObjectPDF[Index].Owidth = CurrentCol - 30;
                                lstObjectPDF[Index].Oheight = CurrentRow - 20;
                            }
                            CurrentCol = MaxCol;
                            LineMaxCol = 250;
                        }
                        else
                        {
                            lstObjectPDF[lstObjectPDF.Count - 1].Width = CurrentCol;
                            lstObjectPDF[lstObjectPDF.Count - 1].Height = CurrentRow;
                            lstObjectPDF[lstObjectPDF.Count - 1].Owidth = CurrentCol;
                            lstObjectPDF[lstObjectPDF.Count - 1].Oheight = CurrentRow + 80;

                            CurrentCol = CurrentCol + 250;
                            if (MaxCol <= CurrentCol) MaxCol = CurrentCol;
                            LineMaxCol = 150;
                        }
                    }
                    else if (OI.Id == Level)
                    {
                        lstObjectPDF.Add(new ObjectInfPDF(OI.Id.ToString(),
                                  OI.Title,
                                  OI.PId,
                                  OI.Level,
                                  0,
                                  0,
                                  0,
                                  0,
                                  0,
                                  0,
                                  0,
                                  0,
                                  OI.NextLevelFlag,
                                  OI.GrayColourFlag,
                                  OI.DottedLineFlag,
                                  OI.ShowFullBox,
                                  OI.Language,
                                  OI.SortNo,
                                  OI.NOR.ToString(),
                                  OI.SOC.ToString(),
                                  OI.PositionFlag,
                                  OI.ColorFlag,
                                  OI.BackColor,
                                  OI.Flag,
                                  OI.BreadGram,
                                  OI.BreadGramName,
                                  OI.ActualLevelNo,
                                  0, 0, 0, 0, 0));

                        CurrentRow = 200;
                        DataRow[] drFirstObj = dtLevel2.Select("POSITIONID='" + LevelId + "'");
                        foreach (DataRow drFirstInf in drFirstObj)
                        {
                            jsonString = drFirstInf["Data1"].ToString();
                            if (drInf["Data2"] != null) jsonString += drInf["Data2"].ToString();
                            if (drInf["Data3"] != null) jsonString += drInf["Data3"].ToString();
                            if (drInf["Data4"] != null) jsonString += drInf["Data4"].ToString();
                            jsonString = jsonString.Substring(0, jsonString.IndexOf("}]") + 2);
                            List<ObjectInfPDF> theFirstObjectInf = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<List<ObjectInfPDF>>(jsonString);
                            foreach (ObjectInfPDF OI_FIRST in theFirstObjectInf)
                            {
                                if (OI_FIRST.PId != "-1" && OI_FIRST.Flag == "Y")
                                {
                                    string[] LabelInfo = OI_FIRST.Title.Replace("&amp;", "&").Split(';');
                                    if (LabelInfo != null)
                                    {
                                        CurrentRow += 100;
                                    }
                                }
                            }
                        }
                        LineStartRow = CurrentRow;
                    }
                }
            }
            CurrentRow = CurrentRow + 100;

            MyLine = new ceTe.DynamicPDF.PageElements.Line(StartCol + 100, StartRow + (LineStartRow - 200) - 30, MaxCol - LineMaxCol, StartRow + (LineStartRow - 200) - 30, GetPDFLineColor(LineColor));
            MyLine.Width = 2;
            MyPage.Elements.Add(MyLine);

            StartPosition = Level;
            if (LevelId == StartPosition)
            {
                TopNode = (from SO in lstObjectPDF where SO.Id == LevelId select SO).FirstOrDefault();
                PutFieldInfoVerticalPDF(TopNode.Title, (MaxCol - 200) / 2, StartRow - 130, 200, 80, (LineStartRow - 200), MyPage, "B");
                PutAssistanceFieldInfoVerticalPDF(Convert.ToInt32(TopNode.Id), ((MaxCol - 200) / 2)+130, 200, 200, 80, MyPage);
            }

            PageMaxWidth = MaxCol;

            return CurrentRow;
        }

        private int GetObjectHeightVerticalPDF(List<ObjectInfPDF> lstObjectInfPDF, string LevelId)
        {
            int Height = 0;
            ObjectInfPDF ObjInfPDF = null;
            List<ObjectInfPDF> lstObjInfPDF= (from OBJ in lstObjectInfPDF where OBJ.PId==LevelId select OBJ).ToList();
            foreach (ObjectInfPDF Obj in lstObjInfPDF)
            {
                if (Obj.DottedLineFlag == "N")
                {
                    ObjInfPDF = Obj;
                    Height = Obj.Height;
                }
            }
            if (ObjInfPDF != null) return ObjInfPDF.Oheight - ObjInfPDF.Height;

            return 0;
        }

        // All Level PDF in single 
        int CurrentRow = 100;
        string A0PageSizeFlag = "Y";
        DataTable dtLevel1 = null, dtLevel2 = null;
        public void CreateAllLevelPDF(DataSet OrgDataSet, string DownloadType, string CompanyName, string View, string ViewFlag, string FP, string LevelUp, string ShowLevel)
        {
            if (ViewFlag == "Horizontal(A0 Page Size)") A0PageSizeFlag = "Y"; else A0PageSizeFlag = "N";
            DataTable OrgDataTable = OrgDataSet.Tables[0];
            DataTable OrgDataTableFM = OrgDataSet.Tables[1];

            dtHD_DIRECT_REPORT = CompanyName.ToUpper() + "_" + FinalyzerVersion.ToString() + "_DIRECT_REPORT";
            dtHD_DIRECT_REPORT_DATA = CompanyName.ToUpper() + "_" + FinalyzerVersion.ToString() + "_DIRECT_REPORT_DATA";
            dtHD_DIRECT_REPORT_LV = CompanyName.ToUpper() + "_" + FinalyzerVersion.ToString() + "_DIRECT_REPORT_LV";
            dtHD_DIRECT_REPORT_DATA_LV = CompanyName.ToUpper() + "_" + FinalyzerVersion.ToString() + "_DIRECT_REPORT_DATA_LV";

            // Creates the Document
            Document MyDocument = new ceTe.DynamicPDF.Document();
            MyDocument.Creator = "DynamicChart.aspx";
            MyDocument.Author = "Subramanian";
            MyDocument.Title = "Organization Chart";

            //PageDimensions MyPageDimensions = new PageDimensions(PageSize.A3,  PageOrientation.Landscape);
            PageDimensions MyPageDimensions = new PageDimensions(14300F, 9000F);
            Page MyPage = new ceTe.DynamicPDF.Page(MyPageDimensions);
            FormattedTextAreaStyle style = new FormattedTextAreaStyle(ceTe.DynamicPDF.FontFamily.Helvetica, 14, true);

            string sSQL = "", strCVIEW = "VIEW_DEFAULT";
            if (View == "OV")
            {
                dtFieldInformation = csobj.SQLReturnDataTable("SELECT * FROM LEVEL_CONFIG_INFO " +
                                                              " WHERE VIEW_ID='" + strCVIEW + "' AND " +
                                                              "       DOWNLOAD_TYPE='" + DownloadType + "' AND COMPANY_NAME='" + CompanyName + "'");
                dtFieldActive = csobj.SQLReturnDataTable("SELECT * FROM LEVEL_CONFIG_INFO " +
                                                         " WHERE VIEW_ID='" + strCVIEW + "' AND ALL_LEVEL_FLAG='Y' AND " +
                                                         " DOWNLOAD_TYPE='" + DownloadType + "' AND COMPANY_NAME='" + CompanyName + "'");
            }
            else if (View == "LV")
            {
                dtFieldInformation = csobj.SQLReturnDataTable("SELECT * FROM LEGAL_CONFIG_INFO " +
                                                              " WHERE VIEW_ID='" + strCVIEW + "' AND " +
                                                              "       DOWNLOAD_TYPE='" + DownloadType + "' AND COMPANY_NAME='" + CompanyName + "'");
                dtFieldActive = csobj.SQLReturnDataTable("SELECT * FROM LEGAL_CONFIG_INFO " +
                                                         " WHERE VIEW_ID='" + strCVIEW + "' AND ALL_LEVEL_FLAG='Y' AND " +
                                                         "       DOWNLOAD_TYPE='" + DownloadType + "' AND COMPANY_NAME='" + CompanyName + "'");
            }
            if (View == "OV")
            {
                sSQL = "SELECT DISTINCT a.PositionID, a.KeyDate, a.DirectReport, a.NextLevel, a.DrType, a.Country, a.LevelNo, a.MaxLevel, a.SOC SOC_COUNT, a.SOC NOR_COUNT " +
                            " FROM " + dtHD_DIRECT_REPORT + " a " +
                            " WHERE a.DrType='OV' AND a.DownloadType='" + DownloadType + "'";
                dtLevel2 = csobj.SQLReturnDataTable("SELECT * FROM " + dtHD_DIRECT_REPORT_DATA + " WHERE DrType='" + View + "' AND DownloadType='" + DownloadType + "'");
            }
            else if (View == "LV")
            {
                sSQL = "SELECT DISTINCT a.PositionID, a.KeyDate, a.DirectReport, a.NextLevel, a.DrType, a.Country, a.LevelNo, a.MaxLevel, a.SOC SOC_COUNT, a.SOC NOR_COUNT " +
                            " FROM " + dtHD_DIRECT_REPORT_LV + " a " +
                            " WHERE a.DrType='OV' AND a.DownloadType='" + DownloadType + "'";
                dtLevel2 = csobj.SQLReturnDataTable("SELECT * FROM " + dtHD_DIRECT_REPORT_DATA_LV + " WHERE DrType='" + View + "' AND DownloadType='" + DownloadType + "'");
            }
            dtLevel1 = csobj.SQLReturnDataTable(sSQL);

            DataRow[] OrgFirstRow = OrgDataTable.Select("LEVEL_ID='" + Level + "'");
            if (OrgFirstRow.Length >= 1)
            {
                int[] RetValue1 = { 0, 0 };
                lstObjectPDF.Clear();
                lstObjectWHPDF.Clear();
                if (ViewFlag == "Horizontal(A0 Page Size)")
                {
                    string LastLevel = OrgFirstRow[0]["LEVEL_NO"].ToString();
                    switch (LevelUpto)
                    {
                        case "One":
                            LastLevel = (Convert.ToInt32(OrgFirstRow[0]["LEVEL_NO"].ToString()) + 1).ToString();
                            break;
                        case "Two":
                            LastLevel = (Convert.ToInt32(OrgFirstRow[0]["LEVEL_NO"].ToString()) + 2).ToString();
                            break;
                        case "Three":
                            LastLevel = (Convert.ToInt32(OrgFirstRow[0]["LEVEL_NO"].ToString()) + 3).ToString();
                            break;
                        case "Four":
                            LastLevel = (Convert.ToInt32(OrgFirstRow[0]["LEVEL_NO"].ToString()) + 4).ToString();
                            break;
                        case "Five":
                            LastLevel = (Convert.ToInt32(OrgFirstRow[0]["LEVEL_NO"].ToString()) + 5).ToString();
                            break;
                        case "All":
                            LastLevel = "All";
                            break;
                    }

                    PageMaxWidth = 0; PageMaxHeight = 0;
                    DataRow[] OrgDataRow = OrgDataTable.Select("LEVEL_NO='" + OrgFirstRow[0]["LEVEL_NO"].ToString() + "'");
                    foreach (DataRow drLevel2 in OrgDataRow)
                    {
                        if (LastLevel == "All")
                            SetParentChildRelationship(drLevel2["LEVEL_ID"].ToString(), 6);
                        else
                            SetParentChildRelationship(drLevel2["LEVEL_ID"].ToString(), Convert.ToInt32(LastLevel));
                    }
                    string SortOrder = "PARENT_LEVEL_ID ASC";
                    string Filter = "LEVEL_NO='" + OrgFirstRow[0]["LEVEL_NO"].ToString() + "'";
                    OrgDataRow = OrgDataTable.Select(Filter, SortOrder);
                    foreach (DataRow drLevel2 in OrgDataRow)
                    {
                        RetValue1 = A0PageSizeBottomUpApproachWH("-1", drLevel2["LEVEL_ID"].ToString(), "N");
                    }
                    
                    OrgDataRow = OrgDataTable.Select("LEVEL_NO='" + OrgFirstRow[0]["LEVEL_NO"].ToString() + "'");
                    foreach (DataRow drLevel2 in OrgDataRow)
                    {
                        SelectTopLevelHorizantal(drLevel2["LEVEL_ID"].ToString(), style, MyPage, MyDocument, "Y", LevelUp, ShowLevel, "N");
                    }

                    // Left associates
                    int PageHeight = 0;
                    A0StartHeight = (int)PageMaxHeight+200;
                    List<ObjectIWPDF> lstMissedWHPDF = (from SO in lstObjectWHPDF
                                                         where SO.UsedFlag=="N"
                                                         orderby SO.Width descending
                                                         select SO).ToList();
                    foreach(ObjectIWPDF IWPDF in lstMissedWHPDF)
                    {
                        PageHeight += IWPDF.Height;
                        if (PageHeight >= 8500)
                        {
                            MyDocument.Pages.Add(MyPage);
                            MyPage = new ceTe.DynamicPDF.Page(MyPageDimensions);
                            A0StartHeight = 0;
                            PageHeight = IWPDF.Height;
                        }
                        PageHeight += 200;
                        PageMaxWidth = 0; PageMaxHeight = 0;
                        Level = IWPDF.Id;
                        SelectTopLevelHorizantal(IWPDF.Id, style, MyPage, MyDocument, "Y", "Yes", IWPDF.Id, A0PageSizeFlag);
                        IWPDF.TotalWidth = PageMaxWidth;
                        IWPDF.TotalHeight = PageMaxHeight;
                        A0StartHeight += IWPDF.Height + 200;
                    }
                    MyDocument.Pages.Add(MyPage);
                    MyDocument.Draw(FP);

                    List<ObjectIWPDF> lstAddedWHPDF = (from SO in lstObjectWHPDF
                                                        where SO.UsedFlag == "Y"
                                                        orderby SO.Width descending
                                                        select SO).ToList();
                }
                else if (ViewFlag == "Horizontal")
                {
                    string LastLevel = OrgFirstRow[0]["LEVEL_NO"].ToString();
                    switch (LevelUpto)
                    {
                        case "One":
                            LastLevel = (Convert.ToInt32(OrgFirstRow[0]["LEVEL_NO"].ToString()) + 1).ToString();
                            break;
                        case "Two":
                            LastLevel = (Convert.ToInt32(OrgFirstRow[0]["LEVEL_NO"].ToString()) + 2).ToString();
                            break;
                        case "Three":
                            LastLevel = (Convert.ToInt32(OrgFirstRow[0]["LEVEL_NO"].ToString()) + 3).ToString();
                            break;
                        case "Four":
                            LastLevel = (Convert.ToInt32(OrgFirstRow[0]["LEVEL_NO"].ToString()) + 4).ToString();
                            break;
                        case "Five":
                            LastLevel = (Convert.ToInt32(OrgFirstRow[0]["LEVEL_NO"].ToString()) + 5).ToString();
                            break;
                        case "All":
                            LastLevel = "All";
                            break;
                    }

                    PageMaxWidth = 0; PageMaxHeight = 0;
                    DataRow[] OrgDataRow = OrgDataTable.Select("LEVEL_NO='"+ OrgFirstRow[0]["LEVEL_NO"].ToString() + "'");
                    foreach (DataRow drLevel2 in OrgDataRow)
                    {
                        if (LastLevel=="All")
                            SetParentChildRelationship(drLevel2["LEVEL_ID"].ToString(), 6);
                        else
                            SetParentChildRelationship(drLevel2["LEVEL_ID"].ToString(), Convert.ToInt32(LastLevel));
                    }
                    string SortOrder = "PARENT_LEVEL_ID ASC";
                    string Filter = "LEVEL_NO='" + OrgFirstRow[0]["LEVEL_NO"].ToString() + "'";
                    OrgDataRow = OrgDataTable.Select(Filter, SortOrder);
                    foreach (DataRow drLevel2 in OrgDataRow)
                    {
                        RetValue1 = BottomUpApproachWH("-1");
                    }
                    Common ComClass = new Common();
                    ComClass.ExecuteQuery("DELETE FROM [dbo].[NodeInfo]");
                    ComClass.BulkCopySQL(NodeWH, "NodeInfo");
                    HttpContext.Current.Response.Write("Bulk data stored successfully");

                    OrgDataRow = OrgDataTable.Select("LEVEL_NO='" + OrgFirstRow[0]["LEVEL_NO"].ToString() + "'");
                    foreach (DataRow drLevel2 in OrgDataRow)
                    {
                        SelectTopLevelHorizantal(drLevel2["LEVEL_ID"].ToString(), style, MyPage, MyDocument, "Y", LevelUp, ShowLevel, A0PageSizeFlag);
                    }

                    // Functional Manager Line
                    ShowFunctionalManager(OrgDataTableFM, "HORIZANTAL", MyPage);

                    //Outputs the MyDocument to the current web MyPage
                    MyPage.Dimensions.Width = PageMaxWidth + 300;
                    MyPage.Dimensions.Height = PageMaxHeight + 500;
                    MyDocument.Pages.Add(MyPage);
                    MyDocument.Draw(FP);
                }
                else if (ViewFlag == "Vertical")
                {
                    int CurrentCol = 0, MaxCol = 0;

                    lstObjectPDF.Clear();
                    PageMaxWidth = 0; PageMaxHeight = 0;
                    string SortOrder = "SortNo ASC";
                    DataRow[] OrgDataRow = OrgDataTable.Select("LEVEL_NO='" + OrgFirstRow[0]["LEVEL_NO"].ToString() + "'", SortOrder);
                    foreach (DataRow drLevel2 in OrgDataRow)
                    {
                        CurrentRow = 200;
                        CurrentRow = SelectLevelHorizantalPDF(drLevel2["LEVEL_ID"].ToString(), CurrentCol, MaxCol, style, MyPage, "Y");
                        CurrentCol = MaxCol;
                    }

                    foreach (ObjectInfPDF Obj in lstObjectPDF)
                    {
                        if (Obj.Id == "10017628")
                            Obj.Flag = Obj.Flag;
                        if (Convert.ToInt32(Obj.NOR) >= 1 && Obj.DottedLineFlag=="N")
                        {
                            if (Convert.ToInt32(Obj.NOR) == 1)
                            {
                                MyLine = new ceTe.DynamicPDF.PageElements.Line(Obj.Width, Obj.Height, Obj.Width, Obj.Height + (Convert.ToInt32(Obj.NOR) * 80), GetPDFLineColor(LineColor));
                                MyLine.Width = 2;
                                MyPage.Elements.Add(MyLine);
                            }
                            else if (Convert.ToInt32(Obj.NOR) >= 2)
                            {
                                int MinusHeight = GetObjectHeightVerticalPDF(lstObjectPDF, Obj.Id);
                                MyLine = new ceTe.DynamicPDF.PageElements.Line(Obj.Width, Obj.Height, Obj.Owidth, Obj.Oheight - MinusHeight - 20, GetPDFLineColor(LineColor));
                                MyLine.Width = 2;
                                MyPage.Elements.Add(MyLine);
                            }
                        }
                    }

                    //Outputs the MyDocument to the current web MyPage
                    MyPage.Dimensions.Width = PageMaxWidth + 300;
                    MyPage.Dimensions.Height = PageMaxHeight + 200;
                    MyDocument.Pages.Add(MyPage);
                    MyDocument.Draw(FP);
                }

                string LogPDFFlag = ConfigurationManager.AppSettings["LogPDFFlag"].ToString();
                if (LogPDFFlag == "Yes")
                {
                    
                    using (SqlConnection SqlDelCon = new SqlConnection(OrgConnection))
                    {
                        SqlDelCon.Open();
                        csobj.ExecuteQuery("DELETE FROM ObjectInfPDFs", SqlDelCon);
                    }

                    string InsertSQL = "";
                    int Index = 0;
                    List<ObjectInfPDF> theSelectedObjectInf = (from SO in lstObjectPDF select SO).ToList();
                    foreach (ObjectInfPDF OI in theSelectedObjectInf)
                    {
                        try
                        {
                            if (Index % 100 == 0 && Index!=0)
                            {
                                using (SqlConnection SqlCon = new SqlConnection(OrgConnection))
                                {
                                    SqlCon.Open();
                                    csobj.ExecuteQuery(InsertSQL, SqlCon);
                                    InsertSQL = "";
                                }
                            }
                            InsertSQL += "INSERT INTO ObjectInfPDFs VALUES(\'" + OI.Id.ToString() + "\',";
                            InsertSQL += "\'" + OI.Title.Replace("'", "''") + "\',";
                            InsertSQL += "\'" + OI.PId + "\',";
                            InsertSQL += "\'" + OI.Level + "\',";
                            InsertSQL += "\'" + OI.Row.ToString() + "\',";
                            InsertSQL += "\'" + OI.Col.ToString() + "\',";
                            InsertSQL += "\'" + OI.Height.ToString() + "\',";
                            InsertSQL += "\'" + OI.Width.ToString() + "\',";
                            InsertSQL += "\'" + OI.Cwidth.ToString() + "\',";
                            InsertSQL += "\'" + OI.Cheight.ToString() + "\',";
                            InsertSQL += "\'" + OI.Owidth.ToString() + "\',";
                            InsertSQL += "\'" + OI.Oheight.ToString() + "\',";
                            InsertSQL += "\'" + OI.NextLevelFlag + "\',";
                            InsertSQL += "\'" + OI.GrayColourFlag + "\',";
                            InsertSQL += "\'" + OI.DottedLineFlag + "\',";
                            InsertSQL += "\'" + OI.ShowFullBox + "\',";
                            InsertSQL += "\'" + OI.Language + "\',";
                            InsertSQL += "\'" + OI.SortNo + "\',";
                            InsertSQL += "\'" + OI.NOR.ToString() + "\',";
                            InsertSQL += "\'" + OI.SOC.ToString() + "\',";
                            InsertSQL += "\'" + OI.PositionFlag + "\',";
                            InsertSQL += "\'" + OI.ColorFlag + "\',";
                            InsertSQL += "\'" + OI.BackColor + "\',";
                            InsertSQL += "\'" + OI.Flag + "\',";
                            InsertSQL += "\'" + OI.BreadGram + "\',";
                            InsertSQL += "\'" + OI.RealRow.ToString() + "\',";
                            InsertSQL += "\'" + OI.RealCol.ToString() + "\',";
                            InsertSQL += "\'" + OI.RealBoxHeight.ToString() + "\',";
                            InsertSQL += "\'" + OI.RealBoxWidth.ToString() + "\');";

                            Index++;
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                }

                FileInfo myDoc = new FileInfo(FP);

                HttpContext.Current.Response.Clear();
                HttpContext.Current.Response.ContentType = "application/pdf";
                HttpContext.Current.Response.AddHeader("content-disposition", "inline;filename=" + myDoc.Name);
                HttpContext.Current.Response.AddHeader("Content-Length", myDoc.Length.ToString());
                HttpContext.Current.Response.ContentType = "application/octet-stream";
                HttpContext.Current.Response.WriteFile(myDoc.FullName);
                HttpContext.Current.Response.End();

            }
        }
    }
}
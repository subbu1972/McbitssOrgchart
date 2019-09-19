using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.Entity;

namespace REORGCHART.Models
{
    public class DBContext : DbContext
    {
        // You can add custom code to this file. Changes will not be overwritten.
        // 
        // If you want Entity Framework to drop and regenerate your database
        // automatically whenever you change your model schema, please use data migrations.
        // For more information refer to the documentation:
        // http://msdn.microsoft.com/en-us/data/jj591621.aspx

        public DBContext() : base("name=DBContextOrgchart")
        {
        }

        public System.Data.Entity.DbSet<REORGCHART.Models.InitializeTables> InitializeTables { get; set; }
        public System.Data.Entity.DbSet<REORGCHART.Models.UserLastActions> UserLastActions { get; set; }
        public System.Data.Entity.DbSet<REORGCHART.Models.LegalCountries> LegalCountries { get; set; }
        public System.Data.Entity.DbSet<REORGCHART.Models.AspNetUsers> AspNetUsers { get; set; }
        public System.Data.Entity.DbSet<REORGCHART.Models.CompanyDetails> Company { get; set; }
        public System.Data.Entity.DbSet<REORGCHART.Models.UserListDetails> UserListDetails { get; set; }
        public System.Data.Entity.DbSet<REORGCHART.Models.UserVersions> UserVersions { get; set; }
        public System.Data.Entity.DbSet<REORGCHART.Models.UserRoles> UserRoles { get; set; }
        public System.Data.Entity.DbSet<REORGCHART.Models.UserViews> UserViews { get; set; }
        public System.Data.Entity.DbSet<REORGCHART.Models.UploadFilesHeaders> UploadFilesHeaders { get; set; }
        public System.Data.Entity.DbSet<REORGCHART.Models.DeleteUploadFilesHeaders> DeleteUploadFilesHeaders { get; set; }
        public System.Data.Entity.DbSet<REORGCHART.Models.UploadFilesDetails> UploadFilesDetails { get; set; }
        public System.Data.Entity.DbSet<REORGCHART.Models.DeleteUploadFilesDetails> DeleteUploadFilesDetails { get; set; }
        public System.Data.Entity.DbSet<REORGCHART.Models.VersionDetails> VersionDetails { get; set; }
        public System.Data.Entity.DbSet<REORGCHART.Models.DeleteVersionDetails> DeleteVersionDetails { get; set; }
        public System.Data.Entity.DbSet<REORGCHART.Models.ObjectInfPDFs> ObjectInfPDFs { get; set; }
        public System.Data.Entity.DbSet<REORGCHART.Models.PPTX_CONFIG_INFO> PPTX_CONFIG_INFO { get; set; }
        public System.Data.Entity.DbSet<REORGCHART.Models.LEVEL_CONFIG_INFO> LEVEL_CONFIG_INFO { get; set; }
        public System.Data.Entity.DbSet<REORGCHART.Models.LEGAL_CONFIG_INFO> LEGAL_CONFIG_INFO { get; set; }
    }

    public class InitializeTables
    {
        [Key]
        public string CompanyName { get; set; }
        public string OprLevelId { get; set; }
        public string LglLevelId { get; set; }
        public string Authentication { get; set; }
        public string BaseCompany { get; set; }
        public string CompanyRoles { get; set; }
        public string FinalyzerCanChange { get; set; }
        public string FuntionalManagerDottedLines { get; set; }
    }

    public class PPTXTemplateFields
    {
        public string SelectedField { get; set; }
        public string MappedFields { get; set; }
        public string FieldDataType { get; set; }
        public string FieldDataFormat { get; set; }
    }

    public class LoginUsers
    {
        public string UserName { get; set; }
        public string CompanyName { get; set; }
        public string Email { get; set; }
        public string Roles { get; set; }
        public string ValidUser { get; set; }
    }

    public class AspNetUsers
    {
        [Key]
        public string Id { get; set; }
        public string Email { get; set; }
        public Boolean EmailConfirmed { get; set; }
        public string PasswordHash { get; set; }
        public string SecurityStamp { get; set; }
        public string PhoneNumber { get; set; }
        public Boolean PhoneNumberConfirmed { get; set; }
        public Boolean TwoFactorEnabled { get; set; }
        public string LockoutEndDateUtc { get; set; }
        public Boolean LockoutEnabled { get; set; }
        public int? AccessFailedCount { get; set; }
        public string UserName { get; set; }
        public int UID { get; set; }
        public string CompanyName { get; set; }
    }

    public class LegalCountries
    {
        [Key]
        public int Id { get; set; }
        public string CountryCode { get; set; }
        public string CountryName { get; set; }
        public string OrgUnit { get; set; }
        public int? DisplaySeq { get; set; }
    }

    public class UserLastActions
    {
        public string Role { get; set; }
        public string CopyPaste { get; set; }
        public string SelectedInitiative { get; set; }
        public string SelectedPopulation { get; set; }
        public string SelectedUser { get; set; }
        public string SelectedVersion { get; set; }
        public string SelectedShape { get; set; }
        public string SelectedSkin { get; set; }
        public string SelectedShowPicture { get; set; }
        public string SelectedSplitScreen { get; set; }
        public string SelectedSplitScreenDirection { get; set; }
        public string SelectedTextColor { get; set; }
        public string SelectedBorderColor { get; set; }
        public string SelectedBorderWidth { get; set; }
        public string SelectedLineColor { get; set; }
        public string SelectedBoxWidth { get; set; }
        public string SelectedPortraitModeMultipleLevel { get; set; }
        public string SelectedFunctionalManagerType { get; set; }
        public string OrgChartType { get; set; }
        public string ShowLevel { get; set; }
        public string ParentLevel { get; set; }
        public string PartitionShowLevel { get; set; }
        public string PartitionParentLevel { get; set; }
        public string Levels { get; set; }
        public string Version { get; set; }
        public string Oper { get; set; }
        public string UsedView { get; set; }
        public string Country { get; set; }
        public string KeyDate { get; set; }
        public string TemplatePPTX { get; set; }
        [Key]
        [Column(Order = 1)]
        public string Company { get; set; }
        [Key]
        [Column(Order = 2)]
        public string UserId { get; set; }
    }

    public class ObjectLine
    {
        public int NodeLineStartCol { get; set; }
        public int NodeLineStartRow { get; set; }
        public int NodeLineEndCol { get; set; }
        public int NodeLineEndRow { get; set; }
        public string ArrowFlag { get; set; }
    }

    public class ObjectInfPDFs
    {
        [Key]
        [Column(Order = 1)]
        public string Id { get; set; }
        public string Title { get; set; }
        [Key]
        [Column(Order = 2)]
        public string PId { get; set; }
        public string Level { get; set; }
        public int Row { get; set; }
        public int Col { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int Cwidth { get; set; }
        public int Cheight { get; set; }
        public int Owidth { get; set; }
        public int Oheight { get; set; }
        public string NextLevelFlag { get; set; }
        public string GrayColourFlag { get; set; }
        public string DottedLineFlag { get; set; }
        public string ShowFullBox { get; set; }
        public string Language { get; set; }
        public string SortNo { get; set; }
        public int NOR { get; set; }
        public int SOC { get; set; }
        public string PositionFlag { get; set; }
        public string ColorFlag { get; set; }
        public string BackColor { get; set; }
        public string Flag { get; set; }
        public string BreadGram { get; set; }
        public int? RealRow { get; set; }
        public int? RealCol { get; set; }
        public int? RealBoxWidth { get; set; }
        public int? RealBoxHeight { get; set; }
    }

    public class UserVersions
    {
        [Key]
        [Column(Order = 1)]
        public int UserId { get; set; }
        [Key]
        [Column(Order = 2)]
        public string Company { get; set; }
        public int VersionId { get; set; }
        public string UploadedFileName { get; set; }
        public string VersionFileName { get; set; }
        [Key]
        [Column(Order = 3)]
        public DateTime CreatedDate { get; set; }
    }

    public class UserRoles
    {
        [Key]
        [Column(Order = 1)]
        public string UserId { get; set; }
        public string CompanyName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
    }

    public class UserViews
    {
        [Key]
        [Column(Order = 1)]
        public string UserId { get; set; }
        [Key]
        [Column(Order = 2)]
        public string CompanyName { get; set; }
        [Key]
        [Column(Order = 3)]
        public string ViewName { get; set; }
        public string DisplayName { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Template { get; set; }
        public string URL { get; set; }
        public string JSMethod { get; set; }
        public string Parameter { get; set; }
        public string ImageURL { get; set; }
        public int DisplaySeq { get; set; }
    }

    public class ExcelDownLoadField
    {
        public string FieldCaption { get; set; }
        public string SearchField { get; set; }
        public string SearchFlag { get; set; }
        public string PositionFlag { get; set; }
    }

    public class SearchField
    {
        public string FieldName { get; set; }
        public string FieldValue { get; set; }
    }

    public class PPTX_CONFIG_INFO
    {
        [Key]
        public int ID { get; set; }
        public string TemplateName { get; set; }
        public string Description { get; set; }
        public string FieldsInfo { get; set; }
        public string FileName { get; set; }
        public string CompanyName { get; set; }
        public int NodeCount { get; set; }
        public string ACTIVE_IND { get; set; }

    }

    public class UploadFilesHeaders
    {
        public string UploadFileName { get; set; }
        public string KeyField { get; set; }
        public string ParentField { get; set; }
        [Key]
        [Column(Order = 4)]
        public DateTime CreatedDate { get; set; }
        public string SerialNoFlag { get; set; }
        public string FirstPosition { get; set; }
        public string FirstPositionField { get; set; }
        public string FileType { get; set; }
        public string UseFields { get; set; }
        public string ExcelDownLoadFields { get; set; }
        public string FullNameFields { get; set; }
        public string PositionCostField { get; set; }
        public string ShowFieldType { get; set; }
        public string PositionCostType { get; set; }
        public int CurrentVersionNo { get; set; }
        public int VersionNo { get; set; }
        public string VersionName { get; set; }
        public string VersionDesc { get; set; }
        [Key]
        [Column(Order = 3)]
        public string CompanyName { get; set; }
        [Key]
        [Column(Order = 2)]
        public string Role { get; set; }
        [Key]
        [Column(Order = 1)]
        public string UserId { get; set; }
    }

    public class DeleteUploadFilesHeaders
    {
        public string UploadFileName { get; set; }
        public string KeyField { get; set; }
        public string ParentField { get; set; }
        [Key]
        [Column(Order = 4)]
        public DateTime CreatedDate { get; set; }
        public string SerialNoFlag { get; set; }
        public string FirstPosition { get; set; }
        public string FirstPositionField { get; set; }
        public string FileType { get; set; }
        public string UseFields { get; set; }
        public string ExcelDownLoadFields { get; set; }
        public string FullNameFields { get; set; }
        public string PositionCostField { get; set; }
        public string ShowFieldType { get; set; }
        public string PositionCostType { get; set; }
        public int CurrentVersionNo { get; set; }
        public int VersionNo { get; set; }
        public string VersionName { get; set; }
        public string VersionDesc { get; set; }
        [Key]
        [Column(Order = 3)]
        public string CompanyName { get; set; }
        [Key]
        [Column(Order = 2)]
        public string Role { get; set; }
        [Key]
        [Column(Order = 1)]
        public string UserId { get; set; }
    }

    public class UploadFilesDetails
    {
        public string UploadFileName { get; set; }
        public DateTime KeyDate { get; set; }
        public string VersionStatus { get; set; }
        public string BackUpFile { get; set; }
        public string VersionName { get; set; }
        public string VersionDesc { get; set; }
        public string Country { get; set; }
        public string ShowLevel { get; set; }
        public string OperType { get; set; }
        [Key]
        [Column(Order = 4)]
        public int VersionNo { get; set; }
        [Key]
        [Column(Order = 3)]
        public string CompanyName { get; set; }
        [Key]
        [Column(Order = 2)]
        public string Role { get; set; }
        [Key]
        [Column(Order = 1)]
        public string UserId { get; set; }
    }

    public class DeleteUploadFilesDetails
    {
        public string UploadFileName { get; set; }
        public DateTime KeyDate { get; set; }
        public string VersionStatus { get; set; }
        public string BackUpFile { get; set; }
        public string VersionName { get; set; }
        public string VersionDesc { get; set; }
        public string Country { get; set; }
        public string ShowLevel { get; set; }
        public string OperType { get; set; }
        [Key]
        [Column(Order = 4)]
        public int VersionNo { get; set; }
        [Key]
        [Column(Order = 3)]
        public string CompanyName { get; set; }
        [Key]
        [Column(Order = 2)]
        public string Role { get; set; }
        [Key]
        [Column(Order = 1)]
        public string UserId { get; set; }
    }

    public class VersionDetails
    {
        [Key]
        public int Id { get; set; }
        public string UserRole { get; set; }
        public string Country { get; set; }
        public string Initiative { get; set; }
        public string IDescription { get; set; }
        public string Population { get; set; }
        public string PDescription { get; set; }
        public string UserName { get; set; }
        public string CompanyName { get; set; }
        public string ShowLevel { get; set; }
        public string OperType { get; set; }
        public int? VersionNo { get; set; }
        public int? FinalyzeVersionNo { get; set; }
        public string Version { get; set; }
        public string VDescription { get; set; }
        public string ActiveVersion { get; set; }
    }

    public class DeleteVersionDetails
    {
        [Key]
        public int Id { get; set; }
        public string UserRole { get; set; }
        public string Country { get; set; }
        public string Initiative { get; set; }
        public string IDescription { get; set; }
        public string Population { get; set; }
        public string PDescription { get; set; }
        public string UserName { get; set; }
        public string CompanyName { get; set; }
        public string ShowLevel { get; set; }
        public string OperType { get; set; }
        public int? VersionNo { get; set; }
        public int? FinalyzeVersionNo { get; set; }
        public string Version { get; set; }
        public string VDescription { get; set; }
        public DateTime DeletionDate { get; set; }
        public string ActiveVersion { get; set; }
    }

    public class LEVEL_CONFIG_INFO
    {
        [Key]
        public int ID { get; set; }
        public string VIEW_ID { get; set; }
        public string FIELD_WIDTH { get; set; }
        public string FIELD_CAPTION { get; set; }
        public string FIELD_NAME { get; set; }
        public int FIELD_ROW { get; set; }
        public string FIELD_ROW_TYPE { get; set; }
        public int FIELD_COL { get; set; }
        public string FIELD_COL_TYPE { get; set; }
        public string WRAP { get; set; }
        public string FONT_NAME { get; set; }
        public string FONT_SIZE { get; set; }
        public string FONT_COLOR { get; set; }
        public string FONT_STYLE { get; set; }
        public string FONT_FLOAT { get; set; }
        public string ACTIVE_IND { get; set; }
        public string TABLE_IND { get; set; }
        public string ADJUSTMENT { get; set; }
        public string SAMPLE_DATA { get; set; }
        public string PHOTO_FLAG { get; set; }
        public string TEMPLATE_NAME { get; set; }
        public string DOWNLOAD_TYPE { get; set; }
        public string COMPANY_NAME { get; set; }
    }

    public class LEGAL_CONFIG_INFO
    {
        [Key]
        public int ID { get; set; }
        public string VIEW_ID { get; set; }
        public string FIELD_WIDTH { get; set; }
        public string FIELD_CAPTION { get; set; }
        public string FIELD_NAME { get; set; }
        public int FIELD_ROW { get; set; }
        public string FIELD_ROW_TYPE { get; set; }
        public int FIELD_COL { get; set; }
        public string FIELD_COL_TYPE { get; set; }
        public string WRAP { get; set; }
        public string FONT_NAME { get; set; }
        public string FONT_SIZE { get; set; }
        public string FONT_COLOR { get; set; }
        public string FONT_STYLE { get; set; }
        public string FONT_FLOAT { get; set; }
        public string ACTIVE_IND { get; set; }
        public string TABLE_IND { get; set; }
        public string ADJUSTMENT { get; set; }
        public string SAMPLE_DATA { get; set; }
        public string PHOTO_FLAG { get; set; }
        public string TEMPLATE_NAME { get; set; }
        public string DOWNLOAD_TYPE { get; set; }
        public string COMPANY_NAME { get; set; }
    }

    public class CompanyDetails
    {
        [Key]
        public int CompanyID { get; set; }
        public string CompanyName { get; set; }
        public string UserName { get; set; }
        public string EmailID { get; set; }
        public string Logo { get; set; }
    }


    public class UserListDetails
    {
        [Key]
        public string UserName { get; set; }
        public string CompanyName { get; set; }
        public string Role { get; set; }
        public string Id { get; set; }
    }

    public class MyModel
    {
        public DateTime UseDate { get; set; }
        public string CompanyName { get; set; }
        public string KeyDate { get; set; }
        public string Country { get; set; }
        public string Countries { get; set; }
        public string SelectedInitiative { get; set; }
        public string SelectedPopulation { get; set; }
        public string SelectedUser { get; set; }
        public string SelectedVersion { get; set; }
        public string SelectedShape { get; set; }
        public string SelectedSkin { get; set; }
        public string SelectedShowPicture { get; set; }
        public string SelectedSplitScreen { get; set; }
        public string SelectedSplitScreenDirection { get; set; }
        public string SelectedTextColor { get; set; }
        public string SelectedBorderColor { get; set; }
        public string SelectedBorderWidth { get; set; }
        public string SelectedBoxWidth { get; set; }
        public string SelectedLineColor { get; set; }
        public string SelectedPortraitModeMultipleLevel { get; set; }
        public string SelectedFunctionalManagerType { get; set; }
        public string OrgChartType { get; set; }
        public string SerialNoFlag { get; set; }
        public string CopyPaste { get; set; }
        public string ShowLevel { get; set; }
        public string ParentLevel { get; set; }
        public string Levels { get; set; }
        public string Oper { get; set; }
        public string View { get; set; }
        public string Version { get; set; }
        public string ChartData { get; set; }
        public string ChartHRCoreData { get; set; }
        public string Role { get; set; }
        public string AssignedRole { get; set; }
        public string HRCoreVersion { get; set; }
        public string UserId { get; set; }
        public string Menu { get; set; }
        public string DDL { get; set; }
        public string OVDDL { get; set; }
        public string LVDDL { get; set; }
        public string SelectFields { get; set; }
        public string SearchFields { get; set; }
        public string InitialValues { get; set; }
        public string FinalyzerVerion { get; set; }
        public string FinalyzerVersionFlag { get; set; }
        public DataTable GridDataTable { get; set; }
        public string ValidateSecurity { get; set; }
    }

    public class MyLastAction
    {
        public string KeyDate { get; set; }
        public string Country { get; set; }
        public string SelectedInitiative { get; set; }
        public string SelectedPopulation { get; set; }
        public string SelectedUser { get; set; }
        public string SelectedVersion { get; set; }
        public string SelectedShape { get; set; }
        public string SelectedSkin { get; set; }
        public string SelectedShowPicture { get; set; }
        public string SelectedSplitScreen { get; set; }
        public string SelectedSplitScreenDirection { get; set; }
        public string SelectedTextColor { get; set; }
        public string SelectedBorderColor { get; set; }
        public string SelectedBorderWidth { get; set; }
        public string SelectedLineColor { get; set; }
        public string SelectedBoxWidth { get; set; }
        public string SelectedPortraitModeMultipleLevel { get; set; }
        public string SelectedFunctionalManagerType { get; set; }
        public string OrgChartType { get; set; }
        public string CopyPaste { get; set; }
        public string ShowLevel { get; set; }
        public string ParentLevel { get; set; }
        public string PartitionShowLevel { get; set; }
        public string PartitionParentLevel { get; set; }
        public string Levels { get; set; }
        public string Oper { get; set; }
        public string View { get; set; }
        public string Version { get; set; }
        public string Role { get; set; }
    }

    public class OrgChartData
    {
        public string key { get; set; }
        public string parent { get; set; }
        public string FULL_NAME { get; set; }
        public string GENDER { get; set; }
        public string DEPTID { get; set; }
        public string LOCATION { get; set; }
    }

    public class ChartDataInf
    {
        public string className { get; set; }
        public List<OrgChartData> nodeDataArray { get; set; }
    }

    public class MyData
    {
        public string LEVEL_ID { get; set; }
        public string PARENT_LEVEL_ID { get; set; }
        public string DATE_UPDATED { get; set; }
        public string NAME_PREFIX { get; set; }
        public string FULL_NAME { get; set; }
        public string LAST_NAME { get; set; }
        public string MIDDLE_NAME { get; set; }
        public string FIRST_NAME { get; set; }
        public string LOCATION { get; set; }
        public string LOCATION_DESCR { get; set; }
        public string EMAILID { get; set; }
        public string GENDER { get; set; }
        public string DEPTID { get; set; }
        public string DIVISION { get; set; }
        public string POSITION_TITLE { get; set; }
        public string JOB_TITLE { get; set; }
        public string FUNC_GP_L1_DESCR { get; set; }
        public string POSITION_NBR { get; set; }
        public string FUNC_GP_L2_DESCR { get; set; }
        public string FUNC_GP_L3_DESCR { get; set; }
        public string POSITION_GRADE { get; set; }
        public string FUNDING { get; set; }
        public string EMPLOYEE_TYPE { get; set; }
        public string EMPLOYEE_STATUS { get; set; }
        public string TYPE_OF_CONTRACT { get; set; }
        public string REGIONNAME { get; set; }
        public string SUBREGIONNAME { get; set; }
        public string OPERATIONNAME { get; set; }
        public string FOCUS_OFFICENAME { get; set; }
        public string OFFICEMSRPCODE { get; set; }
        public string AD_DISPLAY_NAME { get; set; }
        public string AD_TITLE { get; set; }
        public string AD_DEPARTMENT { get; set; }
        public string IPPHONE { get; set; }
        public string MSRP_COUNTRY { get; set; }
        public string POSITION_COST { get; set; }
        public string SUP_DISPLAY_NAME { get; set; }
    }
}

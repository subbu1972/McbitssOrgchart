USE [ORG_CHART_DEV]
GO
/****** Object:  StoredProcedure [dbo].[PROC_GET_DETAILS_VERSION_LIST]    Script Date: 26/10/2018 08:26:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Subramanian.C>
-- =============================================
ALTER PROCEDURE [dbo].[PROC_GET_DETAILS_VERSION_LIST] 
	@USERID AS VARCHAR(500),
	@ROLE AS VARCHAR(50)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @SQLQUERY AS VARCHAR(MAX);
	DECLARE @VERSIONNO AS VARCHAR(50);
	DECLARE @VERSION_INFO AS VARCHAR(500);
	
	SET @VERSION_INFO='USERID='''+@USERID+''' AND ';
	IF (@ROLE <> 'Player') SET @VERSION_INFO='';

	SELECT @VERSIONNO=VERSIONNO FROM UPLOADFILESHEADERS WHERE ROLE='Finalyzer'
	SET @VERSION_INFO = @VERSION_INFO + 'VERSIONNO > '+@VERSIONNO+' AND '; 

	SET @SQLQUERY = 'SELECT [VersionNo] 
                           ,[VersionName]
                           ,[VersionDesc]
                           ,[UploadFileName]
                           ,[KeyDate]
                           ,[VersionStatus]
                           ,[BackUpFile]
                           ,[Role]
                           ,[CompanyName]
                           ,[UserId] 
					FROM UPLOADFILESDETAILS 
					WHERE '+@VERSION_INFO+' VERSIONNAME IS NOT NULL AND ROLE=''Player''';
	EXEC(@SQLQUERY);

END


--EXEC PROC_GET_DETAILS_VERSION_LIST 'SUBBUCITTIBABU'
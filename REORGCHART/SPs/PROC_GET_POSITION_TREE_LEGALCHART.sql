USE [ORG_CHART_DEV]
GO
/****** Object:  StoredProcedure [dbo].[PROC_GET_POSITION_TREE_LEGALCHART]    Script Date: 26/10/2018 08:24:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Subramanian.C>
-- =============================================
ALTER PROCEDURE [dbo].[PROC_GET_POSITION_TREE_LEGALCHART] 
	@STARTPOSITION AS VARCHAR(50),
	@COUNTRY AS VARCHAR(100),
	@DEPTH AS VARCHAR(50),
	@VERSION AS VARCHAR(50),
	@USERTYPE AS VARCHAR(50),
	@COMPANYNAME AS VARCHAR(500),
	@USERID AS VARCHAR(50)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @SQLQUERY AS VARCHAR(MAX);
	DECLARE @COMPANYTB AS VARCHAR(500);
	SET @COMPANYTB = @COMPANYNAME + '_LEVELINFOS'

	IF (@USERTYPE='PLAYER') SET @COMPANYNAME=@USERID;
	IF (@DEPTH='ONE')
	BEGIN
		SET @SQLQUERY = 'SELECT LEVEL_ID [key], PARENT_LEVEL_ID [parent], * 
			                FROM '+@COMPANYTB+' WHERE USER_ID='''+@COMPANYNAME+''' AND VERSION='''+@VERSION+''' AND LEVEL_ID IN
			                    (SELECT '''+@STARTPOSITION+'''
				                    UNION 
			                        SELECT LEVEL_ID FROM '+@COMPANYTB+' WHERE USER_ID='''+@COMPANYNAME+''' AND 
			                                                                VERSION='''+@VERSION+''' AND 
																		    PARENT_LEVEL_ID='''+@STARTPOSITION+''')
							ORDER BY PARENT_LEVEL_ID DESC';
	END
	IF (@DEPTH='TWO')
	BEGIN
		SET @SQLQUERY = 'SELECT LEVEL_ID [key], PARENT_LEVEL_ID [parent], * 
			                FROM '+@COMPANYTB+' WHERE USER_ID='''+@COMPANYNAME+''' AND VERSION='''+@VERSION+''' AND LEVEL_ID IN
								(SELECT '''+@STARTPOSITION+'''
									UNION 
								SELECT LEVEL_ID FROM '+@COMPANYTB+' WHERE USER_ID='''+@COMPANYNAME+''' AND 
			                                                                VERSION='''+@VERSION+''' AND 
																		    PARENT_LEVEL_ID='''+@STARTPOSITION+'''
									UNION
								SELECT LEVEL_ID FROM '+@COMPANYTB+' WHERE USER_ID='''+@COMPANYNAME+''' AND 
			                                                                VERSION='''+@VERSION+''' AND 
																		    PARENT_LEVEL_ID IN (SELECT LEVEL_ID 
																						FROM '+@COMPANYTB+'  
																						WHERE USER_ID='''+@COMPANYNAME+''' AND 
			                                                                                    VERSION='''+@VERSION+''' AND 
																		                        PARENT_LEVEL_ID='''+@STARTPOSITION+'''))';
 	END
	IF (@DEPTH='ALL')
	BEGIN
		SET @SQLQUERY = 'SELECT LEVEL_ID [key], PARENT_LEVEL_ID [parent], * 
			                FROM '+@COMPANYTB+' WHERE USER_ID='''+@COMPANYNAME+''' AND VERSION='''+@VERSION+''' AND LEVEL_ID IN
			                    (SELECT '''+@STARTPOSITION+'''
				                    UNION 
			                        SELECT LEVEL_ID FROM '+@COMPANYTB+' WHERE USER_ID='''+@COMPANYNAME+''' AND 
			                                                                  VERSION='''+@VERSION+''' AND 
																		      BREAD_GRAM LIKE ''%'+@STARTPOSITION+'%'')
							ORDER BY PARENT_LEVEL_ID DESC';
	END
	PRINT @SQLQUERY
	EXEC(@SQLQUERY);
END


--EXEC PROC_GET_POSITION_TREE_LEGALCHART '101212', '', 'ALL', '69', 'Finalyzer', 'Mcbitss', 'SubbuCittibabu'
USE [ORG_CHART_DEV]
GO
/****** Object:  StoredProcedure [dbo].[PROC_GET_POSITION_TREE_OPERATIONALCHART]    Script Date: 26/10/2018 08:23:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Subramanian.C>
-- =============================================
ALTER PROCEDURE [dbo].[PROC_GET_POSITION_TREE_OPERATIONALCHART] 
	@STARTPOSITION AS VARCHAR(50),
	@DEPTH AS VARCHAR(50),
	@VERSION AS VARCHAR(50),
	@USERTYPE AS VARCHAR(50),
	@COMPANYNAME AS VARCHAR(500),
	@USERID AS VARCHAR(50),
	@OPER AS VARCHAR(50)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @SQLQUERY AS VARCHAR(MAX);
	DECLARE @COMPANYTB AS VARCHAR(500);
	SET @COMPANYTB = @COMPANYNAME + '_LEVELINFOS'
	IF (@OPER='LV') SET @COMPANYTB = @COMPANYNAME + '_LEGALINFOS'

	IF (@USERTYPE='PLAYER' OR @USERTYPE='FINALYZER') SET @COMPANYNAME=@USERID;
	IF (@DEPTH='ONE')
	BEGIN
		SET @SQLQUERY = 'SELECT LEVEL_ID [key], PARENT_LEVEL_ID [parent], * 
			                FROM '+@COMPANYTB+' WHERE VERSION='''+@VERSION+''' AND LEVEL_ID IN
			                    (SELECT '''+@STARTPOSITION+'''
				                    UNION 
			                        SELECT LEVEL_ID FROM '+@COMPANYTB+' WHERE VERSION='''+@VERSION+''' AND 
																		      PARENT_LEVEL_ID='''+@STARTPOSITION+''')
							ORDER BY PARENT_LEVEL_ID DESC';
	END
	IF (@DEPTH='TWO')
	BEGIN
		SET @SQLQUERY = 'SELECT LEVEL_ID [key], PARENT_LEVEL_ID [parent], * 
			                FROM '+@COMPANYTB+' WHERE VERSION='''+@VERSION+''' AND LEVEL_ID IN
								(SELECT '''+@STARTPOSITION+'''
									UNION 
								SELECT LEVEL_ID FROM '+@COMPANYTB+' WHERE VERSION='''+@VERSION+''' AND 
																		  PARENT_LEVEL_ID='''+@STARTPOSITION+'''
									UNION
								SELECT LEVEL_ID FROM '+@COMPANYTB+' WHERE VERSION='''+@VERSION+''' AND 
																		  PARENT_LEVEL_ID IN (SELECT LEVEL_ID 
																						FROM '+@COMPANYTB+'  
																						WHERE VERSION='''+@VERSION+''' AND 
																		                      PARENT_LEVEL_ID='''+@STARTPOSITION+'''))';
 	END
	IF (@DEPTH='THREE')
	BEGIN
		SET @SQLQUERY = 'SELECT LEVEL_ID [key], PARENT_LEVEL_ID [parent], * 
			                FROM '+@COMPANYTB+' WHERE VERSION='''+@VERSION+''' AND LEVEL_ID IN
								(SELECT '''+@STARTPOSITION+'''
									UNION 
								SELECT LEVEL_ID FROM '+@COMPANYTB+' WHERE VERSION='''+@VERSION+''' AND 
																		  PARENT_LEVEL_ID='''+@STARTPOSITION+'''
									UNION
								SELECT LEVEL_ID FROM '+@COMPANYTB+' WHERE VERSION='''+@VERSION+''' AND 
																		  PARENT_LEVEL_ID IN (SELECT LEVEL_ID 
																						FROM '+@COMPANYTB+'  
																						WHERE VERSION='''+@VERSION+''' AND 
																		                      PARENT_LEVEL_ID='''+@STARTPOSITION+'''))';
 	END
	IF (@DEPTH='ALL')
	BEGIN
		SET @SQLQUERY = 'SELECT LEVEL_ID [key], PARENT_LEVEL_ID [parent], * 
			                FROM '+@COMPANYTB+' WHERE VERSION='''+@VERSION+''' AND LEVEL_ID IN
			                    (SELECT '''+@STARTPOSITION+'''
				                    UNION 
			                        SELECT LEVEL_ID FROM '+@COMPANYTB+' WHERE VERSION='''+@VERSION+''' AND 
																		      BREAD_GRAM LIKE ''%'+@STARTPOSITION+'%'')
							ORDER BY PARENT_LEVEL_ID DESC';
	END
	EXEC(@SQLQUERY);
END


--EXEC PROC_GET_POSITION_TREE_OPERATIONALCHART '101212', 'All', '1', 'Player', 'Mcbitss', 'SubbuCittibabu'
﻿USE [ORG_CHART_DEV]
GO
/****** Object:  StoredProcedure [dbo].[PROC_GET_POSITION_TREEVIEW]    Script Date: 26/10/2018 08:20:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Subramanian.C>
-- =============================================
ALTER PROCEDURE [dbo].[PROC_GET_POSITION_TREEVIEW] 
	@VERSION AS VARCHAR(50),
	@COMPANYNAME AS VARCHAR(500),
	@OPER AS VARCHAR(50)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @SQLQUERY AS VARCHAR(MAX);
	DECLARE @COMPANYTB AS VARCHAR(500);
	IF (@OPER='OV')
	BEGIN
		SET @COMPANYTB = @COMPANYNAME + '_LEVELINFOS'

		SET @SQLQUERY = 'SELECT LEVEL_ID [key], CAST((CASE WHEN PARENT_LEVEL_ID=999999 THEN 0 ELSE PARENT_LEVEL_ID END) AS VARCHAR(50)) [parent], FULL_NAME [name], LEVEL_NO [level] 
							FROM '+@COMPANYTB+' WHERE VERSION='''+@VERSION+'''
							ORDER BY PARENT_LEVEL_ID DESC';
		EXEC(@SQLQUERY);
	END
	ELSE
	BEGIN
		IF (@OPER='LV')
		BEGIN
			SET @COMPANYTB = @COMPANYNAME + '_LEGALINFOS'

			SET @SQLQUERY = 'SELECT LEVEL_ID [key], CAST((CASE WHEN PARENT_LEVEL_ID=999999 THEN 0 ELSE PARENT_LEVEL_ID END) AS VARCHAR(50)) [parent], FULL_NAME [name], LEVEL_NO [level] 
								FROM '+@COMPANYTB+' WHERE VERSION='''+@VERSION+'''
								ORDER BY PARENT_LEVEL_ID DESC';
			EXEC(@SQLQUERY);
		END
	END
END


--EXEC PROC_GET_POSITION_TREEVIEW '1', 'MCBITSS', 'LV'
USE [ORG_CHART_DEV]
GO
/****** Object:  StoredProcedure [dbo].[PROC_GET_USER_MENU_LIST]    Script Date: 26/10/2018 08:17:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Subramanian.C>
-- =============================================
ALTER PROCEDURE [dbo].[PROC_GET_USER_MENU_LIST] 
     @USER_ID AS VARCHAR(100),
	 @COMPANY_NAME AS VARCHAR(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @ROLE AS VARCHAR(50)='User'
	SELECT @ROLE=ROLE FROM USERROLES WHERE USERID=@USER_ID

	IF (@ROLE='User')
	BEGIN
		SELECT * FROM UserMenus WHERE ROLE='USER' ORDER BY DISPLAYSEQ
	END
	IF (@ROLE='Player')
	BEGIN
		SELECT * FROM UserMenus WHERE ROLE IN ('USER', 'Player') ORDER BY DISPLAYSEQ
	END
	IF (@ROLE='Finalyzer')
	BEGIN
		SELECT * FROM UserMenus WHERE ROLE IN ('USER', 'Player', 'Finalyzer') ORDER BY DISPLAYSEQ
	END
	SELECT * FROM UserViews WHERE COMPANYNAME=@COMPANY_NAME ORDER BY DISPLAYSEQ
END


--EXEC PROC_GET_USER_MENU_LIST 'Finalyzer'
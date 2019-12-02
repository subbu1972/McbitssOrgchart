﻿USE [ORG_CHART_DEV]
GO

DELETE FROM MENAOrgChartData
INSERT INTO MENAOrgChartData(
       [MSRP_EMPLID]
      ,[MSRP_NAME_PREFIX]
      ,[MSRP_NAME]
      ,[MSRP_LAST_NAME]
      ,[MSRP_MIDDLE_NAME]
      ,[MSRP_FIRST_NAME]
      ,[MSRP_UN_INDEX_NBR]
      ,[MSRP_LOCATION]
      ,[MSRP_LOCATION_DESCR]
      ,[MSRP_COUNTRY_CODE]
      ,[MSRP_COUNTRY]
      ,[MSRP_PERSONAL_GRADE]
      ,[MSRP_EMAILID]
      ,[MSRP_GENDER]
      ,[MSRP_DEPTID]
      ,[MSRP_DIVISION]
      ,[MSRP_POSITION_TITLE]
      ,[MSRP_JOB_TITLE]
      ,[MSRP_FUNC_GP_L1_DESCR]
      ,[MSRP_POSITION_NBR]
      ,[MSRP_FUNC_GP_L2_DESCR]
      ,[MSRP_FUNC_GP_L3_DESCR]
      ,[MSRP_POSITION_GRADE]
      ,[MSRP_FUNDING]
      ,[MSRP_EMPLOYEE_TYPE]
      ,[MSRP_SUP_NAME])
SELECT [MSRP_EMPLID]
      ,[MSRP_NAME_PREFIX]
      ,[MSRP_NAME]
      ,[MSRP_LAST_NAME]
      ,[MSRP_MIDDLE_NAME]
      ,[MSRP_FIRST_NAME]
      ,[MSRP_UN_INDEX_NBR]
      ,[MSRP_LOCATION]
      ,[MSRP_LOCATION_DESCR]
      ,[MSRP_COUNTRY_CODE]
      ,[MSRP_COUNTRY]
      ,[MSRP_PERSONAL_GRADE]
      ,[MSRP_EMAILID]
      ,[MSRP_GENDER]
      ,[MSRP_DEPTID]
      ,[MSRP_DIVISION]
      ,[MSRP_POSITION_TITLE]
      ,[MSRP_JOB_TITLE]
      ,[MSRP_FUNC_GP_L1_DESCR]
      ,[MSRP_POSITION_NBR]
      ,[MSRP_FUNC_GP_L2_DESCR]
      ,[MSRP_FUNC_GP_L3_DESCR]
      ,[MSRP_POSITION_GRADE]
      ,[MSRP_FUNDING]
      ,[MSRP_EMPLOYEE_TYPE]
      ,[MSRP_SUP_NAME]
  FROM [MENAOrgChartInitialData]

DECLARE @LEVEL_ID AS INT = 10001339;
DECLARE @INITIAL_LEVEL_ID AS INT = 10001339;
DECLARE @PARENT_LEVEL_ID AS INT = 999999;
DECLARE @PARENT_COUNTRY_LEVEL_ID AS INT = 999999;
DECLARE @EXISTING_LEVEL_ID AS VARCHAR(100)='';
DECLARE @MSRP_LEVEL_ID AS VARCHAR(100)='';
DECLARE @MSRP_PARENT_LEVEL_ID AS VARCHAR(100)='';
DECLARE @MSRP_NAME AS VARCHAR(100)='';
DECLARE @MSRP_SUP_NAME AS VARCHAR(100)='';
DECLARE @MSRP_COUNTRY AS VARCHAR(100)='UNHCR';
DECLARE @MSRP_REGION AS VARCHAR(100)='';
DECLARE @MSRP_FUNC_GP_L1_DESCR AS VARCHAR(500)='';

DECLARE @REMOVE_COUNTRY TABLE(COUNTRY VARCHAR(100));
INSERT INTO @REMOVE_COUNTRY VALUES('POLAND');
INSERT INTO @REMOVE_COUNTRY VALUES('FRANCE');
INSERT INTO @REMOVE_COUNTRY VALUES('BELGIUM');

UPDATE MENAOrgChartData SET MSRP_COUNTRY='EGYPT REGIONAL OFFICE' WHERE TRIM(MSRP_COUNTRY)='EGYPT'
UPDATE MENAOrgChartData SET MSRP_NAME='EGYPT REGIONAL OFFICE' WHERE TRIM(MSRP_NAME)='EGYPT'

DECLARE @COMPANY_HEAD TABLE(
    LEVEL_ID VARCHAR(50),
	NAME VARCHAR(50),
    COUNTRY VARCHAR(100),
	REGION VARCHAR(100)
);

INSERT INTO @COMPANY_HEAD VALUES('30070266', 'LAU,France', 'TUNISIA', 'NORTH AFRICA');
INSERT INTO @COMPANY_HEAD VALUES('30070275', 'ITO,Ayaki', 'SYRIAN ARAB REPUBLIC', 'MIDDLE EAST');
INSERT INTO @COMPANY_HEAD VALUES('30070276', 'GIRARD,Mireille', 'LEBANON', 'MIDDLE EAST');
INSERT INTO @COMPANY_HEAD VALUES('30070277', 'GHARAIBEH,Ayman Y.', 'IRAQ', 'MIDDLE EAST');
INSERT INTO @COMPANY_HEAD VALUES('30067679', 'ENNIS,Carolyn', 'JORDAN', 'MIDDLE EAST');
INSERT INTO @COMPANY_HEAD VALUES('', '', 'SWITZERLAND', 'NONE');
INSERT INTO @COMPANY_HEAD VALUES('30070278', 'STAVROPOULOU,Maria', 'MAURITANIA', 'NORTH AFRICA');
INSERT INTO @COMPANY_HEAD VALUES('30070271', 'ATASSI,Karim', 'EGYPT REGIONAL OFFICE', 'NORTH AFRICA');
INSERT INTO @COMPANY_HEAD VALUES('30070274', 'MULAS,Agostino', 'ALGERIA', 'NORTH AFRICA');
INSERT INTO @COMPANY_HEAD VALUES('30070264', 'CAVALIERI,Jean-Paul', 'LIBYA', 'NORTH AFRICA');
INSERT INTO @COMPANY_HEAD VALUES('', '', 'SAUDI ARABIA', 'MIDDLE EAST');
INSERT INTO @COMPANY_HEAD VALUES('', '', 'KUWAIT', 'NONE');
INSERT INTO @COMPANY_HEAD VALUES('', '', 'UNITED ARAB EMIRATES', 'MIDDLE EAST');
INSERT INTO @COMPANY_HEAD VALUES('30069331', 'MANTEAW,Martin', 'YEMEN', 'MIDDLE EAST');
INSERT INTO @COMPANY_HEAD VALUES('30069333', 'DESSALEGNE,Damtew', 'ISRAEL', 'MIDDLE EAST');
INSERT INTO @COMPANY_HEAD VALUES('30069329', 'PAPAS,Filipos', 'WESTERN SAHARA', 'NORTH AFRICA');
INSERT INTO @COMPANY_HEAD VALUES('30070192', 'GAMBERT,Elisabeth Marguerite Andrée', 'MOROCCO', 'NORTH AFRICA');
INSERT INTO @COMPANY_HEAD VALUES('', '', 'QUATAR', 'NONE');
INSERT INTO @COMPANY_HEAD VALUES('', '', 'SOUTH AFRICA', 'NONE');
INSERT INTO @COMPANY_HEAD VALUES('', '', 'HUNGARY', 'NONE');

DELETE FROM MENAOrgchartData WHERE MSRP_NAME=MSRP_SUP_NAME
DELETE FROM MENAOrgchartData WHERE MSRP_COUNTRY NOT IN (SELECT COUNTRY FROM @COMPANY_HEAD)

-- Top level node
UPDATE MENAOrgChartData SET MSRP_PARENT_LEVEL_ID=@PARENT_LEVEL_ID, MSRP_LEVEL_ID=@LEVEL_ID, MSRP_LEVEL_NO='0' WHERE MSRP_POSITION_NBR=@LEVEL_ID
--INSERT INTO MENAOrgChartData (
--     [MSRP_NAME]
--	,[MSRP_COUNTRY]
--	,[MSRP_PERSONAL_GRADE]
--	,[MSRP_FUNC_GP_L1_DESCR]
--	,[MSRP_POSITION_NBR]
--	,[MSRP_LEVEL_NO]
--	,[MSRP_LEVEL_ID]
--	,[MSRP_PARENT_LEVEL_ID]
--)
--VALUES (
--	'AWAD,Mohamed Amin',
--	'RRC Syria Situation',
--	'D2',
--	'Director',
--	@LEVEL_ID,
--	'0',
--	@LEVEL_ID,
--	@PARENT_LEVEL_ID
--)

SET @PARENT_LEVEL_ID=@LEVEL_ID;
SET @LEVEL_ID=20000000;

-- Country Top level node
INSERT INTO MENAOrgChartData (
     [MSRP_NAME]
	,[MSRP_COUNTRY]
	,[MSRP_PERSONAL_GRADE]
	,[MSRP_FUNC_GP_L1_DESCR]
	,[MSRP_POSITION_NBR]
	,[MSRP_SUP_NAME]
	,[MSRP_LEVEL_NO]
	,[MSRP_LEVEL_ID]
	,[MSRP_PARENT_LEVEL_ID]
)
VALUES (
	'Country Officers',
	'RRC Syria Situation',
	'',
	'Country Officers',
	@LEVEL_ID,
	'AWAD,Mohamed Amin',
	'1',
	@LEVEL_ID,
	@PARENT_LEVEL_ID
)

SET @PARENT_LEVEL_ID=@LEVEL_ID;
SET @PARENT_COUNTRY_LEVEL_ID=@LEVEL_ID;
SET @LEVEL_ID=@LEVEL_ID + 1;

UPDATE MENAOrgChartData SET MSRP_PARENT_LEVEL_ID=@INITIAL_LEVEL_ID, 
                            MSRP_LEVEL_ID=MSRP_SERIELNO+30000000   
		WHERE MSRP_SUP_NAME = 'AWAD,Mohamed Amin' AND MSRP_LEVEL_ID IS NULL


DECLARE MENAORGCHART_COUNTRY_CURSOR CURSOR LOCAL FORWARD_ONLY FOR SELECT DISTINCT MSRP_COUNTRY 
                                                                    FROM MENAOrgChartData  
																	WHERE MSRP_COUNTRY IN (SELECT COUNTRY FROM @COMPANY_HEAD)
OPEN MENAORGCHART_COUNTRY_CURSOR  
FETCH NEXT FROM MENAORGCHART_COUNTRY_CURSOR INTO  @MSRP_COUNTRY
WHILE @@FETCH_STATUS = 0  
BEGIN  
	INSERT INTO MENAOrgChartData (
         [MSRP_NAME]
		,[MSRP_COUNTRY]
		,[MSRP_PERSONAL_GRADE]
		,[MSRP_FUNC_GP_L1_DESCR]
		,[MSRP_POSITION_NBR]
		,[MSRP_LEVEL_NO]
		,[MSRP_LEVEL_ID]
		,[MSRP_PARENT_LEVEL_ID]
	)
	VALUES (
	    @MSRP_COUNTRY,
		@MSRP_COUNTRY,
		'',
		@MSRP_COUNTRY,
		@LEVEL_ID,
		'1',
		@LEVEL_ID,
		@PARENT_COUNTRY_LEVEL_ID
	)

	SET @PARENT_LEVEL_ID=@LEVEL_ID;
	SET @LEVEL_ID=@LEVEL_ID + 1;	

	DECLARE MENAORGCHART_DEPARTMENT_CURSOR CURSOR LOCAL 
	       FORWARD_ONLY FOR SELECT DISTINCT MSRP_FUNC_GP_L1_DESCR 
		                       FROM MENAOrgChartData 
							   WHERE MSRP_COUNTRY=@MSRP_COUNTRY AND (MSRP_SUP_NAME='NULL' OR MSRP_SUP_NAME IS NULL) AND 
							         MSRP_PARENT_LEVEL_ID IS NULL
	OPEN MENAORGCHART_DEPARTMENT_CURSOR  
	FETCH NEXT FROM MENAORGCHART_DEPARTMENT_CURSOR INTO @MSRP_FUNC_GP_L1_DESCR
	WHILE @@FETCH_STATUS = 0  
	BEGIN  
		INSERT INTO MENAOrgChartData (
           [MSRP_NAME]
		  ,[MSRP_COUNTRY]
		  ,[MSRP_PERSONAL_GRADE]
		  ,[MSRP_FUNC_GP_L1_DESCR]
		  ,[MSRP_POSITION_NBR]
		  ,[MSRP_LEVEL_NO]
		  ,[MSRP_LEVEL_ID]
		  ,[MSRP_PARENT_LEVEL_ID]
		)
	    VALUES (
			@MSRP_FUNC_GP_L1_DESCR+'( '+@MSRP_COUNTRY +' )',
			@MSRP_COUNTRY,
			'',
			@MSRP_FUNC_GP_L1_DESCR,
			@LEVEL_ID,
			'2',
			@LEVEL_ID,
			@PARENT_LEVEL_ID
	    )
			
		--PRINT @MSRP_FUNC_GP_L1_DESCR	
		UPDATE MENAOrgChartData
		   SET MSRP_SUP_NAME=@MSRP_FUNC_GP_L1_DESCR+'( '+@MSRP_COUNTRY +' )', 
		       MSRP_LEVEL_ID=MSRP_SERIELNO+30000000, 
			   MSRP_PARENT_LEVEL_ID=@LEVEL_ID
		   WHERE MSRP_FUNC_GP_L1_DESCR = @MSRP_FUNC_GP_L1_DESCR AND 
		         MSRP_COUNTRY = @MSRP_COUNTRY AND 
				 MSRP_NAME <> @MSRP_COUNTRY AND 
				 MSRP_NAME <> MSRP_SUP_NAME AND
				 (MSRP_SUP_NAME='NULL' OR MSRP_SUP_NAME IS NULL)

		SET @LEVEL_ID=@LEVEL_ID + 1;

		FETCH NEXT FROM MENAORGCHART_DEPARTMENT_CURSOR INTO @MSRP_FUNC_GP_L1_DESCR
	END  
	CLOSE MENAORGCHART_DEPARTMENT_CURSOR  
	DEALLOCATE MENAORGCHART_DEPARTMENT_CURSOR  

	UPDATE MENAOrgChartData 
	  SET MSRP_LEVEL_ID=MSRP_SERIELNO+30000000 
	  WHERE MSRP_LEVEL_ID IS NULL AND MSRP_COUNTRY=@MSRP_COUNTRY

	DECLARE MENAORGCHART_SUP_NAME_CURSOR CURSOR LOCAL 
	       FORWARD_ONLY FOR SELECT DISTINCT MSRP_SUP_NAME 
		                       FROM MENAOrgChartData 
							   WHERE MSRP_COUNTRY=@MSRP_COUNTRY AND MSRP_PARENT_LEVEL_ID IS NULL
	OPEN MENAORGCHART_SUP_NAME_CURSOR  
	FETCH NEXT FROM MENAORGCHART_SUP_NAME_CURSOR INTO  @MSRP_SUP_NAME
	WHILE @@FETCH_STATUS = 0  
	BEGIN  		 
		SET @MSRP_PARENT_LEVEL_ID=NULL;
		SELECT @MSRP_PARENT_LEVEL_ID=CAST((MSRP_SERIELNO+30000000) AS VARCHAR(100)) 
		   FROM MENAOrgChartData 
		   WHERE TRIM(MSRP_NAME)=TRIM(@MSRP_SUP_NAME)
		IF (@MSRP_PARENT_LEVEL_ID IS NULL) SET @MSRP_PARENT_LEVEL_ID='99999999';

		--PRINT @MSRP_SUP_NAME	
		UPDATE MENAOrgChartData
		   SET MSRP_PARENT_LEVEL_ID=@MSRP_PARENT_LEVEL_ID
		   WHERE MSRP_SUP_NAME=@MSRP_SUP_NAME AND MSRP_COUNTRY=@MSRP_COUNTRY AND MSRP_NAME <> @MSRP_COUNTRY

		FETCH NEXT FROM MENAORGCHART_SUP_NAME_CURSOR INTO @MSRP_SUP_NAME
	END  
	CLOSE MENAORGCHART_SUP_NAME_CURSOR  
	DEALLOCATE MENAORGCHART_SUP_NAME_CURSOR  

	DECLARE MENAORGCHART_PARENT_ID_CURSOR CURSOR LOCAL 
	       FORWARD_ONLY FOR SELECT DISTINCT MSRP_COUNTRY, MSRP_FUNC_GP_L1_DESCR  
		                       FROM MENAOrgChartData 
							   WHERE MSRP_COUNTRY=@MSRP_COUNTRY AND MSRP_PARENT_LEVEL_ID = '99999999'
	OPEN MENAORGCHART_PARENT_ID_CURSOR  
	FETCH NEXT FROM MENAORGCHART_PARENT_ID_CURSOR INTO  @MSRP_COUNTRY, @MSRP_FUNC_GP_L1_DESCR
	WHILE @@FETCH_STATUS = 0  
	BEGIN  		 

	    SELECT @EXISTING_LEVEL_ID='99999999';
	    SELECT @EXISTING_LEVEL_ID=MSRP_LEVEL_ID
			FROM MENAOrgChartData 
			WHERE TRIM(MSRP_SUP_NAME)=TRIM(@MSRP_FUNC_GP_L1_DESCR+'( '+@MSRP_COUNTRY +' )')

		IF (@EXISTING_LEVEL_ID='99999999')
		BEGIN
			SELECT @PARENT_LEVEL_ID='10001339';
			SELECT @PARENT_LEVEL_ID=MSRP_LEVEL_ID
				FROM MENAOrgChartData 
				WHERE TRIM(MSRP_COUNTRY)=TRIM(@MSRP_COUNTRY) AND MSRP_PARENT_LEVEL_ID='20000000'

			INSERT INTO MENAOrgChartData (
			   [MSRP_NAME]
			  ,[MSRP_COUNTRY]
			  ,[MSRP_PERSONAL_GRADE]
			  ,[MSRP_FUNC_GP_L1_DESCR]
			  ,[MSRP_POSITION_NBR]
			  ,[MSRP_LEVEL_NO]
			  ,[MSRP_LEVEL_ID]
			  ,[MSRP_PARENT_LEVEL_ID]
			)
			VALUES (
				@MSRP_FUNC_GP_L1_DESCR+'( '+@MSRP_COUNTRY +' )',
				@MSRP_COUNTRY,
				'',
				@MSRP_FUNC_GP_L1_DESCR,
				@LEVEL_ID,
				'2',
				@LEVEL_ID,
				@PARENT_LEVEL_ID
			)
			
			PRINT 'PRINT' + @MSRP_COUNTRY+' '+@MSRP_FUNC_GP_L1_DESCR	
			UPDATE MENAOrgChartData
			   SET MSRP_PARENT_LEVEL_ID=@LEVEL_ID, MSRP_SUP_NAME=@MSRP_FUNC_GP_L1_DESCR+'( '+@MSRP_COUNTRY +' )'
			   WHERE MSRP_COUNTRY=@MSRP_COUNTRY AND MSRP_FUNC_GP_L1_DESCR = @MSRP_FUNC_GP_L1_DESCR AND MSRP_PARENT_LEVEL_ID='99999999';

			SET @LEVEL_ID=@LEVEL_ID + 1;
		END
		ELSE
		BEGIN
			--PRINT @MSRP_COUNTRY+' '+@MSRP_FUNC_GP_L1_DESCR	
			UPDATE MENAOrgChartData
			   SET MSRP_PARENT_LEVEL_ID=@EXISTING_LEVEL_ID, MSRP_SUP_NAME=@MSRP_FUNC_GP_L1_DESCR+'( '+@MSRP_COUNTRY +' )'
			   WHERE MSRP_COUNTRY=@MSRP_COUNTRY AND MSRP_FUNC_GP_L1_DESCR = @MSRP_FUNC_GP_L1_DESCR AND MSRP_PARENT_LEVEL_ID='99999999';
		END

		FETCH NEXT FROM MENAORGCHART_PARENT_ID_CURSOR INTO  @MSRP_COUNTRY, @MSRP_FUNC_GP_L1_DESCR
	END  
	CLOSE MENAORGCHART_PARENT_ID_CURSOR  
	DEALLOCATE MENAORGCHART_PARENT_ID_CURSOR  

	FETCH NEXT FROM MENAORGCHART_COUNTRY_CURSOR INTO  @MSRP_COUNTRY
END  
CLOSE MENAORGCHART_COUNTRY_CURSOR  
DEALLOCATE MENAORGCHART_COUNTRY_CURSOR 

DROP TABLE MENAOrgChartDataTemp
SELECT * INTO MENAOrgChartDataTemp FROM MENAOrgChartData

DECLARE MENAORGCHART_COUNTRYHEAD_CURSOR CURSOR LOCAL FORWARD_ONLY FOR SELECT * FROM @COMPANY_HEAD
OPEN MENAORGCHART_COUNTRYHEAD_CURSOR  
FETCH NEXT FROM MENAORGCHART_COUNTRYHEAD_CURSOR INTO  @MSRP_LEVEL_ID, @MSRP_NAME, @MSRP_COUNTRY, @MSRP_REGION
WHILE @@FETCH_STATUS = 0  
BEGIN  
	IF EXISTS ( SELECT 1 FROM MENAOrgChartData WHERE MSRP_COUNTRY=@MSRP_COUNTRY AND MSRP_PARENT_LEVEL_ID=@PARENT_COUNTRY_LEVEL_ID )
	BEGIN
	    SELECT @MSRP_LEVEL_ID=MSRP_LEVEL_ID 
		   FROM MENAOrgChartData 
		   WHERE MSRP_COUNTRY=@MSRP_COUNTRY AND MSRP_PARENT_LEVEL_ID=@PARENT_COUNTRY_LEVEL_ID

		SET @MSRP_PARENT_LEVEL_ID=''
	    SELECT @MSRP_PARENT_LEVEL_ID=MSRP_LEVEL_ID 
		   FROM MENAOrgChartData 
		   WHERE MSRP_COUNTRY=@MSRP_COUNTRY AND MSRP_NAME=@MSRP_NAME
		IF @MSRP_PARENT_LEVEL_ID <> ''
		BEGIN
			PRINT @MSRP_LEVEL_ID+' '+@MSRP_COUNTRY
			DELETE 
			   FROM MENAOrgChartData 
			   WHERE MSRP_COUNTRY=@MSRP_COUNTRY AND MSRP_PARENT_LEVEL_ID=@PARENT_COUNTRY_LEVEL_ID

			UPDATE MENAOrgChartData 
				 SET MSRP_PARENT_LEVEL_ID=@PARENT_COUNTRY_LEVEL_ID, MSRP_FUNC_GP_L1_DESCR = @MSRP_COUNTRY+'( '+ @MSRP_FUNC_GP_L1_DESCR +' )'
			   WHERE MSRP_COUNTRY=@MSRP_COUNTRY AND MSRP_NAME=@MSRP_NAME

			UPDATE MENAOrgChartData 
				 SET MSRP_PARENT_LEVEL_ID=@MSRP_PARENT_LEVEL_ID
			   WHERE MSRP_COUNTRY=@MSRP_COUNTRY AND MSRP_PARENT_LEVEL_ID=@MSRP_LEVEL_ID
        END
	END
	ELSE
	BEGIN
		UPDATE MENAOrgChartData SET MSRP_NAME=@MSRP_NAME
		   WHERE MSRP_COUNTRY=@MSRP_COUNTRY AND MSRP_PARENT_LEVEL_ID=@PARENT_COUNTRY_LEVEL_ID
	END

    FETCH NEXT FROM MENAORGCHART_COUNTRYHEAD_CURSOR INTO  @MSRP_LEVEL_ID, @MSRP_NAME, @MSRP_COUNTRY, @MSRP_REGION
END
CLOSE MENAORGCHART_COUNTRYHEAD_CURSOR  
DEALLOCATE MENAORGCHART_COUNTRYHEAD_CURSOR 

DECLARE MENAORGCHART_REGIONWISE_CURSOR CURSOR LOCAL FORWARD_ONLY FOR SELECT DISTINCT REGION FROM @COMPANY_HEAD WHERE REGION <> 'NONE'
OPEN MENAORGCHART_REGIONWISE_CURSOR  
FETCH NEXT FROM MENAORGCHART_REGIONWISE_CURSOR INTO @MSRP_REGION
WHILE @@FETCH_STATUS = 0  
BEGIN  
	-- Country Top level node
	INSERT INTO MENAOrgChartData (
		 [MSRP_NAME]
		,[MSRP_COUNTRY]
		,[MSRP_PERSONAL_GRADE]
		,[MSRP_FUNC_GP_L1_DESCR]
		,[MSRP_POSITION_NBR]
		,[MSRP_SUP_NAME]
		,[MSRP_LEVEL_NO]
		,[MSRP_LEVEL_ID]
		,[MSRP_PARENT_LEVEL_ID]
	)
	VALUES (
		@MSRP_REGION,
		@MSRP_REGION,
		'',
		@MSRP_REGION,
		@LEVEL_ID,
		'AWAD,Mohamed Amin',
		'2',
		@LEVEL_ID,
		@PARENT_COUNTRY_LEVEL_ID
	)
	SET @PARENT_LEVEL_ID=@LEVEL_ID
	SET @LEVEL_ID = @LEVEL_ID + 1	

	DECLARE MENAORGCHART_REGIONCOUNTRY_CURSOR CURSOR LOCAL FORWARD_ONLY FOR SELECT DISTINCT COUNTRY FROM @COMPANY_HEAD WHERE REGION = @MSRP_REGION
	OPEN MENAORGCHART_REGIONCOUNTRY_CURSOR  
	FETCH NEXT FROM MENAORGCHART_REGIONCOUNTRY_CURSOR INTO @MSRP_COUNTRY
	WHILE @@FETCH_STATUS = 0  
	BEGIN  
		UPDATE MENAOrgChartData 
				SET MSRP_PARENT_LEVEL_ID=@PARENT_LEVEL_ID
			WHERE MSRP_COUNTRY=@MSRP_COUNTRY AND MSRP_PARENT_LEVEL_ID=@PARENT_COUNTRY_LEVEL_ID

		FETCH NEXT FROM MENAORGCHART_REGIONCOUNTRY_CURSOR INTO  @MSRP_COUNTRY
	END
	CLOSE MENAORGCHART_REGIONCOUNTRY_CURSOR  
    DEALLOCATE MENAORGCHART_REGIONCOUNTRY_CURSOR

    FETCH NEXT FROM MENAORGCHART_REGIONWISE_CURSOR INTO  @MSRP_REGION
END
CLOSE MENAORGCHART_REGIONWISE_CURSOR  
DEALLOCATE MENAORGCHART_REGIONWISE_CURSOR 



SELECT *
   FROM MENAOrgChartData 
   WHERE MSRP_LEVEL_ID IS NOT NULL
   ORDER BY CAST(MSRP_PARENT_LEVEL_ID AS INT) , CAST(MSRP_LEVEL_ID as INT)

--SELECT MSRP_LEVEL_ID, MSRP_PARENT_LEVEL_ID, MSRP_LEVEL_NO, MSRP_NAME, MSRP_SUP_NAME, MSRP_COUNTRY, MSRP_FUNC_GP_L1_DESCR 
--   FROM MENAOrgChartData 
--   WHERE MSRP_LEVEL_ID IS NOT NULL
--   ORDER BY CAST(MSRP_PARENT_LEVEL_ID AS INT) , CAST(MSRP_LEVEL_ID as INT)

--DELETE FROM ORG_CHART_DEV.dbo.VersionDetails WHERE CompanyName='UNHCR'
--DELETE FROM ORG_CHART_DEV.dbo.UploadFilesHeaders WHERE CompanyName='UNHCR'
--DELETE FROM ORG_CHART_DEV.dbo.UploadFilesDetails WHERE CompanyName='UNHCR'
--SELECT DISTINCT MSRP_COUNTRY FROM MENAOrgChartData 



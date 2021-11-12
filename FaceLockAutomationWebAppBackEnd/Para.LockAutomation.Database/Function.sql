IF OBJECT_ID('fn_CountTotalFaceLogInGroup', 'FN') IS NOT NULL
	DROP FUNCTION fn_CountTotalFaceLogInGroup
GO

CREATE FUNCTION dbo.fn_CountTotalFaceLogInGroup
(
	@PersonGroupId INT = 1,
	@FromDate DateTime = NULL,
	@ToDate DateTime = NULL
)
RETURNS INT
AS
BEGIN
	DECLARE 
		@Count INT
	SELECT @Count = count(*) FROM faceLog
	WHERE PersonGroupId = @PersonGroupId 
		and (@FromDate IS NULL or @ToDate IS NULL or CreatedDate between @FromDate and @ToDate)
	RETURN @Count
END
GO


-----------------------------------------------------------------------------------------------------------------------


IF OBJECT_ID('fn_CountTotalKnownFaceLog', 'FN') IS NOT NULL
	DROP FUNCTION fn_CountTotalKnownFaceLog
GO

CREATE FUNCTION dbo.fn_CountTotalKnownFaceLog
(
	@PersonGroupId INT = 1,
	@FromDate DATETIME = NULL,
	@ToDate DATETIME = NULL
)
RETURNS INT
AS
BEGIN
	DECLARE 
		@Count INT
	SELECT @Count = count(*) FROM faceLog
	WHERE 
		PersonGroupId = @PersonGroupId and 
		(select count(*) from OPENJSON(Persons, '$')) > 0 and
		(select count(*) from OPENJSON(Persons, '$') where value <> '00000000-0000-0000-0000-000000000000') > 0 and
		(@FromDate IS NULL or @ToDate IS NULL or CreatedDate BETWEEN @FromDate AND @ToDate)
	RETURN @Count
END
GO

--------------------------------------------------------------------------------------------------------------------------

IF OBJECT_ID('fn_CountTotalUnknownFaceLog', 'FN') IS NOT NULL
	DROP FUNCTION fn_CountTotalUnknownFaceLog
GO

CREATE FUNCTION dbo.fn_CountTotalUnknownFaceLog
(
	@PersonGroupId INT = 1,
	@FromDate DATETIME = NULL,
	@ToDate DATETIME = NULL
)
RETURNS INT
AS
BEGIN
	DECLARE 
		@Count INT
	SELECT @Count = count(*) FROM faceLog
	WHERE
		PersonGroupId = @PersonGroupId and 
		(select count(*) from OPENJSON(Persons, '$')) > 0 and
		'00000000-0000-0000-0000-000000000000' in (select value from OPENJSON(Persons, '$')) and
		(@FromDate IS NULL or @ToDate IS NULL or CreatedDate BETWEEN @FromDate AND @ToDate)
	RETURN @Count
END
GO

---------------------------------------------------------------------------------------------------------------------------

IF OBJECT_ID('fn_CountTotalPersonFaceLog', 'FN') IS NOT NULL
	DROP FUNCTION fn_CountTotalPersonFaceLog
GO

CREATE FUNCTION dbo.fn_CountTotalPersonFaceLog
(
	@PersonGroupId INT = 1,
	@PersonId INT = 1,
	@FromDate DATETIME = NULL,
	@ToDate DATETIME = NULL
)
RETURNS INT
AS
BEGIN
	DECLARE 
		@PersonObjectId NVARCHAR(50),
		@Count INT

	SELECT @PersonObjectId = ObjectId FROM person WHERE Id = @PersonId;

	SELECT @Count = count(*) FROM faceLog
	WHERE
		PersonGroupId = @PersonGroupId and 
		@PersonObjectId in (select value from OPENJSON(Persons, '$')) and
		(@FromDate IS NULL or @ToDate IS NULL or CreatedDate BETWEEN @FromDate AND @ToDate)
	RETURN @Count
END
GO

--------------------------------------------------------------------------------------------------------------------------

IF OBJECT_ID('fn_CountTotalUndetectedFaceLog', 'FN') IS NOT NULL
	DROP FUNCTION fn_CountTotalUndetectedFaceLog
GO

CREATE FUNCTION dbo.fn_CountTotalUndetectedFaceLog
(
	@PersonGroupId INT = 1,
	@FromDate DATETIME = NULL,
	@ToDate DATETIME = NULL
)
RETURNS INT
AS
BEGIN
	DECLARE 
		@Count INT
	SELECT @Count = count(*) FROM faceLog
	WHERE
		PersonGroupId = @PersonGroupId and 
		(select count(*) from OPENJSON(Persons, '$')) = 0 and
		(@FromDate IS NULL or @ToDate IS NULL or CreatedDate BETWEEN @FromDate AND @ToDate)
	RETURN @Count
END
GO
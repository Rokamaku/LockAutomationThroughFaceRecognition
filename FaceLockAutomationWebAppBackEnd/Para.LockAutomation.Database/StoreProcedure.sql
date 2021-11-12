IF OBJECT_ID('sp_GetFaceLogInGroup', 'P') IS NOT NULL
	DROP PROC sp_GetFaceLogInGroup
GO

CREATE PROC dbo.sp_GetFaceLogInGroup
(
	@PersonGroupId INT = 1,
	@SortType NVARCHAR(4) = 'ASC',
	@FromDate DateTime = NULL,
	@ToDate DateTime = NULL,
	@PageNumber INT = 1,
	@PageSize INT = 10
)
AS
BEGIN
	SELECT * FROM faceLog
	WHERE PersonGroupId = @PersonGroupId 
		and (@FromDate IS NULL or @ToDate IS NULL or CreatedDate between @FromDate and @ToDate)
	ORDER BY 
		CASE WHEN @SortType = 'asc' THEN CreatedDate END ASC,
		CASE WHEN @SortType = 'desc' THEN CreatedDate END DESC
	OFFSET (@PageNumber - 1) * @PageSize ROWS
	FETCH NEXT @PageSize ROWS ONLY
END
GO


---------------------------------------------------------------------------------------------------------------


IF OBJECT_ID('sp_GetKnownFaceLog', 'P') IS NOT NULL
	DROP PROC sp_GetKnownFaceLog
GO

CREATE PROC dbo.sp_GetKnownFaceLog
(
	@PersonGroupId INT = 1,
	@SortType VARCHAR(4) = 'asc',
	@FromDate DATETIME = NULL,
	@ToDate DATETIME = NULL,
	@PageNumber INT = 1,
	@PageSize INT = 10
)
AS
BEGIN
	SELECT * FROM faceLog
	WHERE 
		PersonGroupId = @PersonGroupId and 
		(select count(*) from OPENJSON(Persons, '$')) > 0 and
		(select count(*) from OPENJSON(Persons, '$') where value <> '00000000-0000-0000-0000-000000000000') > 0 and
		(@FromDate IS NULL or @ToDate IS NULL or CreatedDate BETWEEN @FromDate AND @ToDate)
	ORDER BY 
		CASE WHEN @SortType = 'asc' THEN CreatedDate END ASC,
		CASE WHEN @SortType = 'desc' THEN CreatedDate END DESC
	OFFSET (@PageNumber - 1) * @PageSize ROWS
	FETCH NEXT @PageSize ROWS ONLY

END
GO

----------------------------------------------------------------------------------------------------------------------------------------

IF OBJECT_ID('sp_GetUnknownFaceLog', 'P') IS NOT NULL
	DROP PROC sp_GetUnknownFaceLog
GO

CREATE PROC dbo.sp_GetUnknownFaceLog
(
	@PersonGroupId INT = 1,
	@SortType NVARCHAR(4) = 'asc',
	@FromDate DATETIME = NULL,
	@ToDate DATETIME = NULL,
	@PageNumber INT = 1,
	@PageSize INT = 10
)
AS
BEGIN
	SELECT * FROM faceLog
	WHERE
		PersonGroupId = @PersonGroupId and 
		(select count(*) from OPENJSON(Persons, '$')) > 0 and
		'00000000-0000-0000-0000-000000000000' in (select value from OPENJSON(Persons, '$')) and
		(@FromDate IS NULL or @ToDate IS NULL or CreatedDate BETWEEN @FromDate AND @ToDate)
	ORDER BY 
		CASE WHEN @SortType = 'asc' THEN CreatedDate END ASC,
		CASE WHEN @SortType = 'desc' THEN CreatedDate END DESC
	OFFSET (@PageNumber - 1) * @PageSize ROWS
	FETCH NEXT @PageSize ROWS ONLY
END
GO

--------------------------------------------------------------------------------------------------------------------------------------

IF OBJECT_ID('sp_GetPersonFaceLog', 'P') IS NOT NULL
	DROP PROC sp_GetPersonFaceLog
GO

CREATE PROC dbo.sp_GetPersonFaceLog
(
	@PersonGroupId INT = 1,
	@PersonId INT = 1,
	@SortType NVARCHAR(4) = 'asc',
	@FromDate DATETIME = NULL,
	@ToDate DATETIME = NULL,
	@PageNumber INT = 1,
	@PageSize INT = 10
)
AS
BEGIN
	DECLARE 
		@PersonObjectId NVARCHAR(50)

	SELECT @PersonObjectId = ObjectId FROM person WHERE Id = @PersonId;

	SELECT * FROM faceLog
	WHERE
		PersonGroupId = @PersonGroupId and 
		@PersonObjectId in (select value from OPENJSON(Persons, '$')) and
		(@FromDate IS NULL or @ToDate IS NULL or CreatedDate BETWEEN @FromDate AND @ToDate)
	ORDER BY 
		CASE WHEN @SortType = 'asc' THEN CreatedDate END ASC,
		CASE WHEN @SortType = 'desc' THEN CreatedDate END DESC
	OFFSET (@PageNumber - 1) * @PageSize ROWS
	FETCH NEXT @PageSize ROWS ONLY
END
GO

------------------------------------------------------------------------------------------------------------------------

IF OBJECT_ID('sp_GetUndetectedFaceLog', 'P') IS NOT NULL
	DROP PROC sp_GetUndetectedFaceLog
GO

CREATE PROC dbo.sp_GetUndetectedFaceLog
(
	@PersonGroupId INT = 1,
	@SortType NVARCHAR(4) = 'asc',
	@FromDate DATETIME = NULL,
	@ToDate DATETIME = NULL,
	@PageNumber INT = 1,
	@PageSize INT = 10
)
AS
BEGIN
	SELECT * FROM faceLog
	WHERE
		PersonGroupId = @PersonGroupId and 
		(select count(*) from OPENJSON(Persons, '$')) = 0 and
		(@FromDate IS NULL or @ToDate IS NULL or CreatedDate BETWEEN @FromDate AND @ToDate)
	ORDER BY 
		CASE WHEN @SortType = 'asc' THEN CreatedDate END ASC,
		CASE WHEN @SortType = 'desc' THEN CreatedDate END DESC
	OFFSET (@PageNumber - 1) * @PageSize ROWS
	FETCH NEXT @PageSize ROWS ONLY
END
GO
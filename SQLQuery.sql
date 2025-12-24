-- Create Database
CREATE DATABASE DynamicFormBuilderDB;
GO

USE DynamicFormBuilderDB;
GO

-- Table to store Form information
CREATE TABLE Forms (
    FormId INT PRIMARY KEY IDENTITY(1,1),
    FormTitle NVARCHAR(500) NOT NULL,
    CreatedDate DATETIME DEFAULT GETDATE()
);
GO

-- Table to store Form Fields (dropdowns)
CREATE TABLE FormFields (
    FieldId INT PRIMARY KEY IDENTITY(1,1),
    FormId INT NOT NULL,
    FieldLabel NVARCHAR(200) NOT NULL,
    SelectedOption NVARCHAR(200) NOT NULL,
    IsRequired BIT NOT NULL,
    FieldOrder INT NOT NULL,
    FOREIGN KEY (FormId) REFERENCES Forms(FormId) ON DELETE CASCADE
);
GO

-- Stored Procedure to Insert Form
CREATE PROCEDURE sp_InsertForm
    @FormTitle NVARCHAR(500),
    @FormId INT OUTPUT
AS
BEGIN
    INSERT INTO Forms (FormTitle) VALUES (@FormTitle);
    SET @FormId = SCOPE_IDENTITY();
END
GO

-- Stored Procedure to Insert Form Field
CREATE PROCEDURE sp_InsertFormField
    @FormId INT,
    @FieldLabel NVARCHAR(200),
    @SelectedOption NVARCHAR(200),
    @IsRequired BIT,
    @FieldOrder INT
AS
BEGIN
    INSERT INTO FormFields (FormId, FieldLabel, SelectedOption, IsRequired, FieldOrder)
    VALUES (@FormId, @FieldLabel, @SelectedOption, @IsRequired, @FieldOrder);
END
GO

-- Stored Procedure to Get All Forms (for grid)
CREATE PROCEDURE sp_GetAllForms
    @PageNumber INT = 1,
    @PageSize INT = 10,
    @SearchValue NVARCHAR(200) = ''
AS
BEGIN
    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;
    
    SELECT 
        FormId,
        FormTitle,
        CreatedDate,
        (SELECT COUNT(*) FROM Forms WHERE FormTitle LIKE '%' + @SearchValue + '%') AS TotalRecords
    FROM Forms
    WHERE FormTitle LIKE '%' + @SearchValue + '%'
    ORDER BY CreatedDate DESC
    OFFSET @Offset ROWS
    FETCH NEXT @PageSize ROWS ONLY;
END
GO

-- Stored Procedure to Get Form by ID
CREATE PROCEDURE sp_GetFormById
    @FormId INT
AS
BEGIN
    SELECT FormId, FormTitle, CreatedDate FROM Forms WHERE FormId = @FormId;
    
    SELECT FieldId, FormId, FieldLabel, SelectedOption, IsRequired, FieldOrder 
    FROM FormFields 
    WHERE FormId = @FormId
    ORDER BY FieldOrder;
END
GO
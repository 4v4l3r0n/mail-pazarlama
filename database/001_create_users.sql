-- Users Tablosu Oluşturma Scripti
USE [MailPazarlamaDb];
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Users' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE dbo.Users
    (
        Id int IDENTITY(1, 1) PRIMARY KEY,
        FirstName nvarchar(100) NOT NULL,
        LastName nvarchar(100) NOT NULL,
        Email nvarchar(254) NOT NULL UNIQUE,
        EncryptedPassword nvarchar(max) NOT NULL,
        IsActive bit NOT NULL DEFAULT 1,
        CreatedAt datetime NOT NULL DEFAULT GETDATE()
    );
END
GO


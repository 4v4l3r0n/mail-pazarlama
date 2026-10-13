-- Subscribers Tablosu Oluşturma Scripti
USE [MailPazarlamaDb];
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Subscribers' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE dbo.Subscribers
    (
        Id int IDENTITY(1, 1) PRIMARY KEY,
        Email nvarchar(254) NOT NULL UNIQUE,
        CreatedAt datetime NOT NULL DEFAULT GETDATE()
    );
END
GO

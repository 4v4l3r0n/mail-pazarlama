-- MailTemplates Tablosu Oluşturma Scripti
USE [MailPazarlamaDb];
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'MailTemplates' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE dbo.MailTemplates
    (
        Id int IDENTITY(1, 1) PRIMARY KEY,
        Title nvarchar(200) NOT NULL,
        ContentHtml nvarchar(max) NOT NULL,
        IsActive bit NOT NULL DEFAULT 1,
        CreatedAt datetime NOT NULL DEFAULT GETDATE(),
        UserId int NOT NULL,
        CONSTRAINT FK_MailTemplates_Users FOREIGN KEY (UserId) REFERENCES dbo.Users(Id)
    );
END
GO

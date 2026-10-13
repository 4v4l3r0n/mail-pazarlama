-- MailSettings Tablosu Oluşturma Scripti
USE [MailPazarlamaDb];
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'MailSettings' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE dbo.MailSettings
    (
        Id int IDENTITY(1, 1) PRIMARY KEY,
        SmtpServer nvarchar(255) NOT NULL,
        SmtpPort int NOT NULL,
        EnableSsl bit NOT NULL DEFAULT 1,
        Email nvarchar(254) NOT NULL,
        EncryptedPassword nvarchar(max) NOT NULL,
        UpdatedAt datetime NOT NULL DEFAULT GETDATE()
    );
END
GO

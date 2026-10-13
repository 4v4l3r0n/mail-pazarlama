-- MailSendLogs Tablosu Oluşturma Scripti
USE [MailPazarlamaDb];
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'MailSendLogs' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE dbo.MailSendLogs
    (
        Id int IDENTITY(1, 1) PRIMARY KEY,
        SubscriberId int NOT NULL,
        TemplateId int NOT NULL,
        UserId int NOT NULL,
        SentAt datetime NOT NULL DEFAULT GETDATE(),
        Status nvarchar(20) NOT NULL,
        ErrorMessage nvarchar(max) NULL,
        CONSTRAINT FK_MailSendLogs_Subscribers FOREIGN KEY (SubscriberId) REFERENCES dbo.Subscribers(Id),
        CONSTRAINT FK_MailSendLogs_MailTemplates FOREIGN KEY (TemplateId) REFERENCES dbo.MailTemplates(Id),
        CONSTRAINT FK_MailSendLogs_Users FOREIGN KEY (UserId) REFERENCES dbo.Users(Id)
    );
END
GO

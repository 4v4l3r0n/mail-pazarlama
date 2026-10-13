-- MailPazarlamaDb Veritabanı Oluşturma Scripti
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'MailPazarlamaDb')
BEGIN
    CREATE DATABASE [MailPazarlamaDb] COLLATE Turkish_CI_AS;
END
GO

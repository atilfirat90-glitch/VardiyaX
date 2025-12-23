-- ShiftCraft Seed Data
-- Azure SQL Compatible
-- Version: 1.0.0

-- =============================================
-- SEED: DayTypes
-- =============================================
IF NOT EXISTS (SELECT 1 FROM [dbo].[DayTypes] WHERE [Code] = 'Weekday')
BEGIN
    INSERT INTO [dbo].[DayTypes] ([Code]) VALUES ('Weekday');
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[DayTypes] WHERE [Code] = 'Weekend')
BEGIN
    INSERT INTO [dbo].[DayTypes] ([Code]) VALUES ('Weekend');
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[DayTypes] WHERE [Code] = 'Holiday')
BEGIN
    INSERT INTO [dbo].[DayTypes] ([Code]) VALUES ('Holiday');
END
GO

-- =============================================
-- SEED: Roles
-- =============================================
IF NOT EXISTS (SELECT 1 FROM [dbo].[Roles] WHERE [Name] = 'Manager')
BEGIN
    INSERT INTO [dbo].[Roles] ([Name]) VALUES ('Manager');
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[Roles] WHERE [Name] = 'Supervisor')
BEGIN
    INSERT INTO [dbo].[Roles] ([Name]) VALUES ('Supervisor');
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[Roles] WHERE [Name] = 'Worker')
BEGIN
    INSERT INTO [dbo].[Roles] ([Name]) VALUES ('Worker');
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[Roles] WHERE [Name] = 'Trainee')
BEGIN
    INSERT INTO [dbo].[Roles] ([Name]) VALUES ('Trainee');
END
GO

PRINT 'ShiftCraft seed data inserted successfully.';
GO

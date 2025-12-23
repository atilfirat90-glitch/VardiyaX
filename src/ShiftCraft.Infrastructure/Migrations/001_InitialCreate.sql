-- ShiftCraft Initial Database Schema
-- Azure SQL Compatible Migration Script
-- Version: 1.0.0
-- Date: 2024

-- =============================================
-- CREATE TABLES
-- =============================================

-- Businesses
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Businesses')
BEGIN
    CREATE TABLE [dbo].[Businesses] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [Name] NVARCHAR(255) NOT NULL,
        [Timezone] NVARCHAR(100) NOT NULL,
        CONSTRAINT [PK_Businesses] PRIMARY KEY CLUSTERED ([Id])
    );
END
GO

-- DayTypes
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'DayTypes')
BEGIN
    CREATE TABLE [dbo].[DayTypes] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [Code] NVARCHAR(50) NOT NULL,
        CONSTRAINT [PK_DayTypes] PRIMARY KEY CLUSTERED ([Id])
    );
END
GO

-- Roles
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Roles')
BEGIN
    CREATE TABLE [dbo].[Roles] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [Name] NVARCHAR(100) NOT NULL,
        CONSTRAINT [PK_Roles] PRIMARY KEY CLUSTERED ([Id])
    );
END
GO

-- Employees
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Employees')
BEGIN
    CREATE TABLE [dbo].[Employees] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [BusinessId] INT NOT NULL,
        [Name] NVARCHAR(255) NOT NULL,
        [IsCoreStaff] BIT NOT NULL DEFAULT 0,
        [WeeklyMaxMinutes] INT NOT NULL DEFAULT 2400,
        [IsActive] BIT NOT NULL DEFAULT 1,
        CONSTRAINT [PK_Employees] PRIMARY KEY CLUSTERED ([Id]),
        CONSTRAINT [FK_Employees_Businesses] FOREIGN KEY ([BusinessId]) 
            REFERENCES [dbo].[Businesses]([Id]) ON DELETE NO ACTION
    );
END
GO

-- EmployeeRoles (Composite PK)
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'EmployeeRoles')
BEGIN
    CREATE TABLE [dbo].[EmployeeRoles] (
        [EmployeeId] INT NOT NULL,
        [RoleId] INT NOT NULL,
        CONSTRAINT [PK_EmployeeRoles] PRIMARY KEY CLUSTERED ([EmployeeId], [RoleId]),
        CONSTRAINT [FK_EmployeeRoles_Employees] FOREIGN KEY ([EmployeeId]) 
            REFERENCES [dbo].[Employees]([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_EmployeeRoles_Roles] FOREIGN KEY ([RoleId]) 
            REFERENCES [dbo].[Roles]([Id]) ON DELETE NO ACTION
    );
END
GO

-- ShiftTemplates
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ShiftTemplates')
BEGIN
    CREATE TABLE [dbo].[ShiftTemplates] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [BusinessId] INT NOT NULL,
        [Name] NVARCHAR(100) NOT NULL,
        [StartTime] TIME NOT NULL,
        [EndTime] TIME NOT NULL,
        [DurationMinutes] INT NOT NULL,
        CONSTRAINT [PK_ShiftTemplates] PRIMARY KEY CLUSTERED ([Id]),
        CONSTRAINT [FK_ShiftTemplates_Businesses] FOREIGN KEY ([BusinessId]) 
            REFERENCES [dbo].[Businesses]([Id]) ON DELETE NO ACTION
    );
END
GO

-- WeeklySchedules
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'WeeklySchedules')
BEGIN
    CREATE TABLE [dbo].[WeeklySchedules] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [BusinessId] INT NOT NULL,
        [WeekStartDate] DATE NOT NULL,
        [Status] INT NOT NULL DEFAULT 0,
        CONSTRAINT [PK_WeeklySchedules] PRIMARY KEY CLUSTERED ([Id]),
        CONSTRAINT [FK_WeeklySchedules_Businesses] FOREIGN KEY ([BusinessId]) 
            REFERENCES [dbo].[Businesses]([Id]) ON DELETE NO ACTION
    );
END
GO

-- ScheduleDays
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ScheduleDays')
BEGIN
    CREATE TABLE [dbo].[ScheduleDays] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [WeeklyScheduleId] INT NOT NULL,
        [Date] DATE NOT NULL,
        [DayTypeId] INT NOT NULL,
        CONSTRAINT [PK_ScheduleDays] PRIMARY KEY CLUSTERED ([Id]),
        CONSTRAINT [FK_ScheduleDays_WeeklySchedules] FOREIGN KEY ([WeeklyScheduleId]) 
            REFERENCES [dbo].[WeeklySchedules]([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_ScheduleDays_DayTypes] FOREIGN KEY ([DayTypeId]) 
            REFERENCES [dbo].[DayTypes]([Id]) ON DELETE NO ACTION
    );
END
GO

-- ShiftAssignments
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ShiftAssignments')
BEGIN
    CREATE TABLE [dbo].[ShiftAssignments] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [ScheduleDayId] INT NOT NULL,
        [EmployeeId] INT NOT NULL,
        [RoleId] INT NOT NULL,
        [ShiftTemplateId] INT NOT NULL,
        [Source] INT NOT NULL DEFAULT 0,
        CONSTRAINT [PK_ShiftAssignments] PRIMARY KEY CLUSTERED ([Id]),
        CONSTRAINT [FK_ShiftAssignments_ScheduleDays] FOREIGN KEY ([ScheduleDayId]) 
            REFERENCES [dbo].[ScheduleDays]([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_ShiftAssignments_Employees] FOREIGN KEY ([EmployeeId]) 
            REFERENCES [dbo].[Employees]([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_ShiftAssignments_Roles] FOREIGN KEY ([RoleId]) 
            REFERENCES [dbo].[Roles]([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_ShiftAssignments_ShiftTemplates] FOREIGN KEY ([ShiftTemplateId]) 
            REFERENCES [dbo].[ShiftTemplates]([Id]) ON DELETE NO ACTION
    );
END
GO

-- ShiftRequirements
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ShiftRequirements')
BEGIN
    CREATE TABLE [dbo].[ShiftRequirements] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [BusinessId] INT NOT NULL,
        [DayTypeId] INT NOT NULL,
        [RoleId] INT NOT NULL,
        [ShiftTemplateId] INT NOT NULL,
        [RequiredCount] INT NOT NULL DEFAULT 1,
        CONSTRAINT [PK_ShiftRequirements] PRIMARY KEY CLUSTERED ([Id]),
        CONSTRAINT [FK_ShiftRequirements_Businesses] FOREIGN KEY ([BusinessId]) 
            REFERENCES [dbo].[Businesses]([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_ShiftRequirements_DayTypes] FOREIGN KEY ([DayTypeId]) 
            REFERENCES [dbo].[DayTypes]([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_ShiftRequirements_Roles] FOREIGN KEY ([RoleId]) 
            REFERENCES [dbo].[Roles]([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_ShiftRequirements_ShiftTemplates] FOREIGN KEY ([ShiftTemplateId]) 
            REFERENCES [dbo].[ShiftTemplates]([Id]) ON DELETE NO ACTION
    );
END
GO

-- WorkRules
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'WorkRules')
BEGIN
    CREATE TABLE [dbo].[WorkRules] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [BusinessId] INT NOT NULL,
        [IsSevenDaysOpen] BIT NOT NULL DEFAULT 0,
        [MaxDailyMinutes] INT NOT NULL DEFAULT 480,
        [MinWeeklyOffDays] INT NOT NULL DEFAULT 1,
        CONSTRAINT [PK_WorkRules] PRIMARY KEY CLUSTERED ([Id]),
        CONSTRAINT [FK_WorkRules_Businesses] FOREIGN KEY ([BusinessId]) 
            REFERENCES [dbo].[Businesses]([Id]) ON DELETE NO ACTION
    );
END
GO

-- CoreStaffRules
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'CoreStaffRules')
BEGIN
    CREATE TABLE [dbo].[CoreStaffRules] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [BusinessId] INT NOT NULL,
        [DayTypeId] INT NOT NULL,
        [MinCoreStaffCount] INT NOT NULL DEFAULT 1,
        CONSTRAINT [PK_CoreStaffRules] PRIMARY KEY CLUSTERED ([Id]),
        CONSTRAINT [FK_CoreStaffRules_Businesses] FOREIGN KEY ([BusinessId]) 
            REFERENCES [dbo].[Businesses]([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_CoreStaffRules_DayTypes] FOREIGN KEY ([DayTypeId]) 
            REFERENCES [dbo].[DayTypes]([Id]) ON DELETE NO ACTION
    );
END
GO

-- RuleViolations
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'RuleViolations')
BEGIN
    CREATE TABLE [dbo].[RuleViolations] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [WeeklyScheduleId] INT NOT NULL,
        [EmployeeId] INT NOT NULL,
        [ViolationDate] DATE NOT NULL,
        [RuleCode] INT NOT NULL,
        CONSTRAINT [PK_RuleViolations] PRIMARY KEY CLUSTERED ([Id]),
        CONSTRAINT [FK_RuleViolations_WeeklySchedules] FOREIGN KEY ([WeeklyScheduleId]) 
            REFERENCES [dbo].[WeeklySchedules]([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_RuleViolations_Employees] FOREIGN KEY ([EmployeeId]) 
            REFERENCES [dbo].[Employees]([Id]) ON DELETE NO ACTION
    );
END
GO

-- =============================================
-- CREATE INDEXES
-- =============================================

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Employees_BusinessId')
    CREATE INDEX [IX_Employees_BusinessId] ON [dbo].[Employees]([BusinessId]);
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_WeeklySchedules_BusinessId')
    CREATE INDEX [IX_WeeklySchedules_BusinessId] ON [dbo].[WeeklySchedules]([BusinessId]);
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ScheduleDays_WeeklyScheduleId')
    CREATE INDEX [IX_ScheduleDays_WeeklyScheduleId] ON [dbo].[ScheduleDays]([WeeklyScheduleId]);
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ShiftAssignments_ScheduleDayId')
    CREATE INDEX [IX_ShiftAssignments_ScheduleDayId] ON [dbo].[ShiftAssignments]([ScheduleDayId]);
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ShiftAssignments_EmployeeId')
    CREATE INDEX [IX_ShiftAssignments_EmployeeId] ON [dbo].[ShiftAssignments]([EmployeeId]);
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_RuleViolations_WeeklyScheduleId')
    CREATE INDEX [IX_RuleViolations_WeeklyScheduleId] ON [dbo].[RuleViolations]([WeeklyScheduleId]);
GO

PRINT 'ShiftCraft database schema created successfully.';
GO

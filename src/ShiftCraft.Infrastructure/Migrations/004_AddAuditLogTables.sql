-- Migration: 004_AddAuditLogTables
-- Description: Add LoginLogs and PublishLogs tables for audit logging
-- Date: 2024-12-24

-- Create LoginLogs table
CREATE TABLE [dbo].[LoginLogs] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [Username] NVARCHAR(100) NOT NULL,
    [Timestamp] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [Action] NVARCHAR(20) NOT NULL,
    [DeviceInfo] NVARCHAR(500) NULL,
    [FailureReason] NVARCHAR(500) NULL,
    [IpAddress] NVARCHAR(50) NULL,
    [BusinessId] INT NULL,
    CONSTRAINT [PK_LoginLogs] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_LoginLogs_Businesses] FOREIGN KEY ([BusinessId]) 
        REFERENCES [dbo].[Businesses] ([Id]) ON DELETE NO ACTION
);

CREATE NONCLUSTERED INDEX [IX_LoginLogs_Timestamp] 
    ON [dbo].[LoginLogs] ([Timestamp] DESC);

CREATE NONCLUSTERED INDEX [IX_LoginLogs_Username] 
    ON [dbo].[LoginLogs] ([Username] ASC);

CREATE NONCLUSTERED INDEX [IX_LoginLogs_BusinessId] 
    ON [dbo].[LoginLogs] ([BusinessId] ASC);

-- Create PublishLogs table
CREATE TABLE [dbo].[PublishLogs] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [WeeklyScheduleId] INT NOT NULL,
    [PublisherUsername] NVARCHAR(100) NOT NULL,
    [Timestamp] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [AffectedEmployeeCount] INT NOT NULL DEFAULT 0,
    [BusinessId] INT NULL,
    CONSTRAINT [PK_PublishLogs] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_PublishLogs_WeeklySchedules] FOREIGN KEY ([WeeklyScheduleId]) 
        REFERENCES [dbo].[WeeklySchedules] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_PublishLogs_Businesses] FOREIGN KEY ([BusinessId]) 
        REFERENCES [dbo].[Businesses] ([Id]) ON DELETE NO ACTION
);

CREATE NONCLUSTERED INDEX [IX_PublishLogs_Timestamp] 
    ON [dbo].[PublishLogs] ([Timestamp] DESC);

CREATE NONCLUSTERED INDEX [IX_PublishLogs_PublisherUsername] 
    ON [dbo].[PublishLogs] ([PublisherUsername] ASC);

CREATE NONCLUSTERED INDEX [IX_PublishLogs_BusinessId] 
    ON [dbo].[PublishLogs] ([BusinessId] ASC);

-- Add IsAcknowledged column to RuleViolations table
ALTER TABLE [dbo].[RuleViolations]
ADD [IsAcknowledged] BIT NOT NULL DEFAULT 0,
    [AcknowledgedAt] DATETIME2 NULL,
    [AcknowledgedBy] NVARCHAR(100) NULL;

GO

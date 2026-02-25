-- Migration: 007_AddSwapAvailabilityTimeOffMessages
-- Description: Add ShiftSwapRequests, EmployeeAvailabilities, TimeOffRequests, TeamMessages tables
-- Date: 2026-02-25

-- ShiftSwapRequests
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ShiftSwapRequests')
BEGIN
    CREATE TABLE [dbo].[ShiftSwapRequests] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [RequesterId] INT NOT NULL,
        [RequesterShiftId] INT NOT NULL,
        [TargetEmployeeId] INT NULL,
        [TargetShiftId] INT NULL,
        [Status] NVARCHAR(50) NOT NULL DEFAULT 'Pending',
        [Reason] NVARCHAR(500) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [ResolvedAt] DATETIME2 NULL,
        [ResolvedBy] NVARCHAR(100) NULL,
        [BusinessId] INT NOT NULL,
        CONSTRAINT [PK_ShiftSwapRequests] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_ShiftSwapRequests_Requester] FOREIGN KEY ([RequesterId])
            REFERENCES [dbo].[Employees] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_ShiftSwapRequests_RequesterShift] FOREIGN KEY ([RequesterShiftId])
            REFERENCES [dbo].[ShiftAssignments] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_ShiftSwapRequests_TargetEmployee] FOREIGN KEY ([TargetEmployeeId])
            REFERENCES [dbo].[Employees] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_ShiftSwapRequests_TargetShift] FOREIGN KEY ([TargetShiftId])
            REFERENCES [dbo].[ShiftAssignments] ([Id]) ON DELETE NO ACTION
    );

    CREATE NONCLUSTERED INDEX [IX_ShiftSwapRequests_BusinessId_Status]
        ON [dbo].[ShiftSwapRequests] ([BusinessId] ASC, [Status] ASC);

    CREATE NONCLUSTERED INDEX [IX_ShiftSwapRequests_RequesterId]
        ON [dbo].[ShiftSwapRequests] ([RequesterId] ASC);
END

GO

-- EmployeeAvailabilities
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'EmployeeAvailabilities')
BEGIN
    CREATE TABLE [dbo].[EmployeeAvailabilities] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [EmployeeId] INT NOT NULL,
        [DayOfWeek] INT NOT NULL,
        [AvailableFrom] TIME NULL,
        [AvailableTo] TIME NULL,
        [IsAvailable] BIT NOT NULL DEFAULT 1,
        CONSTRAINT [PK_EmployeeAvailabilities] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_EmployeeAvailabilities_Employee] FOREIGN KEY ([EmployeeId])
            REFERENCES [dbo].[Employees] ([Id]) ON DELETE CASCADE
    );

    CREATE NONCLUSTERED INDEX [IX_EmployeeAvailabilities_EmployeeId]
        ON [dbo].[EmployeeAvailabilities] ([EmployeeId] ASC);
END

GO

-- TimeOffRequests
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'TimeOffRequests')
BEGIN
    CREATE TABLE [dbo].[TimeOffRequests] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [EmployeeId] INT NOT NULL,
        [StartDate] DATETIME2 NOT NULL,
        [EndDate] DATETIME2 NOT NULL,
        [Type] NVARCHAR(50) NOT NULL DEFAULT 'PaidLeave',
        [Status] NVARCHAR(50) NOT NULL DEFAULT 'Pending',
        [Reason] NVARCHAR(500) NULL,
        [ResponseNote] NVARCHAR(500) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [ResolvedAt] DATETIME2 NULL,
        [ResolvedBy] NVARCHAR(100) NULL,
        [BusinessId] INT NOT NULL,
        CONSTRAINT [PK_TimeOffRequests] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_TimeOffRequests_Employee] FOREIGN KEY ([EmployeeId])
            REFERENCES [dbo].[Employees] ([Id]) ON DELETE NO ACTION
    );

    CREATE NONCLUSTERED INDEX [IX_TimeOffRequests_EmployeeId]
        ON [dbo].[TimeOffRequests] ([EmployeeId] ASC);

    CREATE NONCLUSTERED INDEX [IX_TimeOffRequests_BusinessId_Status]
        ON [dbo].[TimeOffRequests] ([BusinessId] ASC, [Status] ASC);
END

GO

-- TeamMessages
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'TeamMessages')
BEGIN
    CREATE TABLE [dbo].[TeamMessages] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [SenderUserId] INT NOT NULL,
        [SenderName] NVARCHAR(100) NOT NULL,
        [Content] NVARCHAR(2000) NOT NULL,
        [Channel] NVARCHAR(50) NOT NULL DEFAULT 'general',
        [IsAnnouncement] BIT NOT NULL DEFAULT 0,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [BusinessId] INT NOT NULL,
        CONSTRAINT [PK_TeamMessages] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_TeamMessages_Sender] FOREIGN KEY ([SenderUserId])
            REFERENCES [dbo].[Users] ([Id]) ON DELETE NO ACTION
    );

    CREATE NONCLUSTERED INDEX [IX_TeamMessages_Channel_BusinessId]
        ON [dbo].[TeamMessages] ([Channel] ASC, [BusinessId] ASC);

    CREATE NONCLUSTERED INDEX [IX_TeamMessages_CreatedAt]
        ON [dbo].[TeamMessages] ([CreatedAt] DESC);
END

GO

PRINT 'ShiftSwapRequests, EmployeeAvailabilities, TimeOffRequests, TeamMessages tables created successfully.';
GO

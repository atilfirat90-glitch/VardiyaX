-- Migration: 005_AddPushNotificationTables
-- Description: Add DeviceRegistrations and NotificationPreferences tables
-- Date: 2024-12-24

-- Create DeviceRegistrations table
CREATE TABLE [dbo].[DeviceRegistrations] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [UserId] INT NOT NULL,
    [DeviceToken] NVARCHAR(500) NOT NULL,
    [Platform] NVARCHAR(20) NOT NULL,
    [RegisteredAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [LastActiveAt] DATETIME2 NULL,
    [IsActive] BIT NOT NULL DEFAULT 1,
    CONSTRAINT [PK_DeviceRegistrations] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_DeviceRegistrations_Users] FOREIGN KEY ([UserId]) 
        REFERENCES [dbo].[Users] ([Id]) ON DELETE CASCADE
);

CREATE NONCLUSTERED INDEX [IX_DeviceRegistrations_UserId] 
    ON [dbo].[DeviceRegistrations] ([UserId] ASC);

CREATE NONCLUSTERED INDEX [IX_DeviceRegistrations_DeviceToken] 
    ON [dbo].[DeviceRegistrations] ([DeviceToken] ASC);

-- Create NotificationPreferences table
CREATE TABLE [dbo].[NotificationPreferences] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [UserId] INT NOT NULL,
    [ScheduleNotificationsEnabled] BIT NOT NULL DEFAULT 1,
    [ViolationNotificationsEnabled] BIT NOT NULL DEFAULT 1,
    [ShiftRemindersEnabled] BIT NOT NULL DEFAULT 1,
    [ReminderHoursBefore] INT NOT NULL DEFAULT 24,
    CONSTRAINT [PK_NotificationPreferences] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_NotificationPreferences_Users] FOREIGN KEY ([UserId]) 
        REFERENCES [dbo].[Users] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [UQ_NotificationPreferences_UserId] UNIQUE ([UserId])
);

GO

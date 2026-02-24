-- Migration: 006_AddNotificationsTable
-- Description: Add Notifications table for in-app notification center
-- Date: 2026-02-24

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Notifications')
BEGIN
    CREATE TABLE [dbo].[Notifications] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [UserId] INT NOT NULL,
        [Title] NVARCHAR(255) NOT NULL,
        [Body] NVARCHAR(1000) NOT NULL,
        [Type] NVARCHAR(50) NOT NULL,
        [Action] NVARCHAR(100) NOT NULL DEFAULT '',
        [DataJson] NVARCHAR(MAX) NULL,
        [IsRead] BIT NOT NULL DEFAULT 0,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [ReadAt] DATETIME2 NULL,
        [BusinessId] INT NULL,
        CONSTRAINT [PK_Notifications] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_Notifications_Users] FOREIGN KEY ([UserId]) 
            REFERENCES [dbo].[Users] ([Id]) ON DELETE NO ACTION
    );

    CREATE NONCLUSTERED INDEX [IX_Notifications_UserId_IsRead] 
        ON [dbo].[Notifications] ([UserId] ASC, [IsRead] ASC)
        INCLUDE ([CreatedAt]);

    CREATE NONCLUSTERED INDEX [IX_Notifications_CreatedAt] 
        ON [dbo].[Notifications] ([CreatedAt] DESC);

    CREATE NONCLUSTERED INDEX [IX_Notifications_BusinessId] 
        ON [dbo].[Notifications] ([BusinessId] ASC);
END

GO

PRINT 'Notifications table created successfully.';
GO

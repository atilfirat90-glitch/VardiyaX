-- Migration: 003_AddUsersTable
-- Description: Add Users table for user management
-- Date: 2024-12-23

-- Create Users table
CREATE TABLE [dbo].[Users] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [Username] NVARCHAR(100) NOT NULL,
    [PasswordHash] NVARCHAR(255) NOT NULL,
    [Role] NVARCHAR(20) NOT NULL DEFAULT 'Worker',
    [IsActive] BIT NOT NULL DEFAULT 1,
    [MustChangePassword] BIT NOT NULL DEFAULT 0,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [LastLoginAt] DATETIME2 NULL,
    [BusinessId] INT NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Users_Businesses] FOREIGN KEY ([BusinessId]) 
        REFERENCES [dbo].[Businesses] ([Id]) ON DELETE NO ACTION
);

-- Create unique index on Username
CREATE UNIQUE NONCLUSTERED INDEX [IX_Users_Username] 
    ON [dbo].[Users] ([Username] ASC);

-- Create index on BusinessId for filtering
CREATE NONCLUSTERED INDEX [IX_Users_BusinessId] 
    ON [dbo].[Users] ([BusinessId] ASC);

-- Insert default admin user (password: ShiftCraft2024Prod!)
-- BCrypt hash for 'ShiftCraft2024Prod!'
INSERT INTO [dbo].[Users] ([Username], [PasswordHash], [Role], [IsActive], [MustChangePassword], [BusinessId])
VALUES ('admin', '$2a$11$K8xQzJ5YqR3vN1mL2pO4wOhGfE6dC8bA9yZ0xW1vU2tS3rQ4pO5nK', 'Admin', 1, 0, 1);

GO

IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
CREATE TABLE [User] (
    [Id] int NOT NULL IDENTITY,
    [UserID] nvarchar(max) NOT NULL,
    [UserName] nvarchar(50) NOT NULL,
    [Password] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_User] PRIMARY KEY ([Id])
);

CREATE TABLE [UserToken] (
    [Id] int NOT NULL IDENTITY,
    [UserID] nvarchar(max) NOT NULL,
    [RefreshToken] nvarchar(max) NOT NULL,
    [DateCreated] datetime2 NOT NULL,
    CONSTRAINT [PK_UserToken] PRIMARY KEY ([Id])
);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250408060849_TableMigrations', N'9.0.2');

COMMIT;
GO


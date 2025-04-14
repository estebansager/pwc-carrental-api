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
GO

DROP TABLE IF EXISTS Rentals;
GO

DROP TABLE IF EXISTS Services;
GO

DROP TABLE IF EXISTS Customers;
GO

DROP TABLE IF EXISTS Cars;
GO

CREATE TABLE [Cars] (
    [Id] uniqueidentifier NOT NULL,
    [Type] nvarchar(50) NOT NULL,
    [Model] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Cars] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Customers] (
    [Id] uniqueidentifier NOT NULL,
    [PersonalIdNumber] int NOT NULL,
    [FullName] nvarchar(max) NOT NULL,
    [Address] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Customers] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Services] (
    [Id] uniqueidentifier NOT NULL,
    [StartDate] datetime2 NOT NULL,
    [CarId] uniqueidentifier NOT NULL,
    [DurationInDays] int NOT NULL,
    [IsCompleted] bit NOT NULL,
    CONSTRAINT [PK_Services] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Services_Cars_CarId] FOREIGN KEY ([CarId]) REFERENCES [Cars] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Rentals] (
    [Id] uniqueidentifier NOT NULL,
    [CustomerId] uniqueidentifier NOT NULL,
    [CarId] uniqueidentifier NOT NULL,
    [StartDate] datetime2 NOT NULL,
    [EndDate] datetime2 NOT NULL,
    CONSTRAINT [PK_Rentals] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Rentals_Cars_CarId] FOREIGN KEY ([CarId]) REFERENCES [Cars] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Rentals_Customers_CustomerId] FOREIGN KEY ([CustomerId]) REFERENCES [Customers] ([Id]) ON DELETE CASCADE
);
GO

CREATE INDEX [IX_Rentals_CarId] ON [Rentals] ([CarId]);
GO

CREATE INDEX [IX_Rentals_CustomerId] ON [Rentals] ([CustomerId]);
GO

CREATE INDEX [IX_Services_CarId] ON [Services] ([CarId]);
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Type', N'Model') AND [object_id] = OBJECT_ID(N'[Cars]'))
    SET IDENTITY_INSERT [Cars] ON;
INSERT INTO [Cars] ([Id], [Type], [Model])
VALUES ('00000000-0000-0000-0000-000000000000', N'Sedan', N'Toyota Corolla'),
('6F9619FF-8B86-D011-B42D-00CF4FC964FF', N'Sedan', N'Toyota Corolla'),
('3F2504E0-4F89-11D3-9A0C-0305E82C3301', N'Sedan', N'Toyota Corolla'),
('48284652-77ED-4872-996E-04BB0F0030AE', N'SUV', N'Honda CR-V'),
('C9BF9E57-1685-4C89-BAAF-07DA1C8D8F9E', N'SUV', N'Honda CR-V'),
('9A499940-92D4-4A3F-A6B1-6D192F56106D', N'SUV', N'Honda CR-V'),
('B8C26292-DED9-4031-BB3D-92436E72E152', N'SUV', N'Honda CR-V'),
('7AC5C6D2-CD71-4F2F-81B5-13701F6FE99F', N'Hatchback', N'Volkswagen Gol'),
('52F06FA4-4C87-4F5A-8D27-179BC83B98A6', N'Hatchback', N'Volkswagen Gol'),
('846BDF8F-C8B2-4A44-A7E1-9E258679F366', N'Hatchback', N'Volkswagen Gol'),
('B5A3F8DE-A57D-4A5B-9A3F-DEC43FDE2CF8', N'Hatchback', N'Volkswagen Gol'),
('9276F7CB-8A29-4E5E-8727-23FC93E24319', N'Pickup', N'Ford Ranger'),
('B47F2C19-3C44-4DC4-94D1-F1B5EFAB7B0A', N'Pickup', N'Ford Ranger'),
('268A1E25-E91F-4B4A-9C4B-59CFE7634764', N'Pickup', N'Volkswagen Amarok'),
('FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF', N'Pickup', N'Volkswagen Amarok'),
('10FC3540-6D9B-4968-B648-5DB508A2296A', N'Sedan', N'Volkswagen Vento'),
('7D444840-9DC0-11D1-B245-5FFDCE74FAD2', N'Sedan', N'Volkswagen Vento'),
('C111402F-ECF3-4FE6-8421-6AEB117A2939', N'Sedan', N'Volkswagen Vento');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Type', N'Model') AND [object_id] = OBJECT_ID(N'[Cars]'))
    SET IDENTITY_INSERT [Cars] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'StartDate', N'CarId', N'DurationInDays', N'IsCompleted') AND [object_id] = OBJECT_ID(N'[Services]'))
    SET IDENTITY_INSERT [Services] ON;
INSERT INTO [Services] ([Id], [StartDate], [CarId], [DurationInDays], [IsCompleted])
VALUES ('11111111-1111-1111-1111-111111111111', '2025-03-30', '00000000-0000-0000-0000-000000000000', 2, CAST(1 AS bit)),
('22222222-2222-2222-2222-222222222222', '2025-03-30', '6F9619FF-8B86-D011-B42D-00CF4FC964FF', 2, CAST(1 AS bit)),
('33333333-3333-3333-3333-333333333333', '2025-04-20', '3F2504E0-4F89-11D3-9A0C-0305E82C3301', 2, CAST(0 AS bit)),
('44444444-4444-4444-4444-444444444444', '2025-04-20', '48284652-77ED-4872-996E-04BB0F0030AE', 2, CAST(0 AS bit)),
('55555555-5555-5555-5555-555555555555', '2025-04-20', 'C9BF9E57-1685-4C89-BAAF-07DA1C8D8F9E', 2, CAST(0 AS bit)),
('aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa', '2025-04-20', '9A499940-92D4-4A3F-A6B1-6D192F56106D', 2, CAST(0 AS bit)),
('bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb', '2025-04-25', 'B8C26292-DED9-4031-BB3D-92436E72E152', 2, CAST(0 AS bit)),
('cccccccc-cccc-cccc-cccc-cccccccccccc', '2025-04-25', '7AC5C6D2-CD71-4F2F-81B5-13701F6FE99F', 2, CAST(0 AS bit)),
('dddddddd-dddd-dddd-dddd-dddddddddddd', '2025-04-30', '52F06FA4-4C87-4F5A-8D27-179BC83B98A6', 2, CAST(0 AS bit)),
('eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee', '2025-05-02', '846BDF8F-C8B2-4A44-A7E1-9E258679F366', 2, CAST(0 AS bit)),
('ffffffff-ffff-ffff-ffff-ffffffffffff', '2025-05-02', 'B5A3F8DE-A57D-4A5B-9A3F-DEC43FDE2CF8', 2, CAST(0 AS bit));
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'StartDate', N'CarId', N'DurationInDays', N'IsCompleted') AND [object_id] = OBJECT_ID(N'[Services]'))
    SET IDENTITY_INSERT [Services] OFF;
GO

COMMIT;
GO


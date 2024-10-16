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

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240819164003_AddAll')
BEGIN
    CREATE TABLE [ActiveIngredients] (
        [IngredientId] int NOT NULL IDENTITY,
        [Name] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_ActiveIngredients] PRIMARY KEY ([IngredientId])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240819164003_AddAll')
BEGIN
    CREATE TABLE [ActiveIngredientStrengths] (
        [StrengthId] int NOT NULL IDENTITY,
        [Strength] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_ActiveIngredientStrengths] PRIMARY KEY ([StrengthId])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240819164003_AddAll')
BEGIN
    CREATE TABLE [AspNetRoles] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(256) NULL,
        [NormalizedName] nvarchar(256) NULL,
        [ConcurrencyStamp] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240819164003_AddAll')
BEGIN
    CREATE TABLE [AspNetUsers] (
        [Id] int NOT NULL IDENTITY,
        [UserRole] nvarchar(max) NOT NULL,
        [RegisteredDate] datetime2 NOT NULL,
        [UserName] nvarchar(256) NULL,
        [NormalizedUserName] nvarchar(256) NULL,
        [Email] nvarchar(256) NULL,
        [NormalizedEmail] nvarchar(256) NULL,
        [EmailConfirmed] bit NOT NULL,
        [PasswordHash] nvarchar(max) NULL,
        [SecurityStamp] nvarchar(max) NULL,
        [ConcurrencyStamp] nvarchar(max) NULL,
        [PhoneNumber] nvarchar(max) NULL,
        [PhoneNumberConfirmed] bit NOT NULL,
        [TwoFactorEnabled] bit NOT NULL,
        [LockoutEnd] datetimeoffset NULL,
        [LockoutEnabled] bit NOT NULL,
        [AccessFailedCount] int NOT NULL,
        CONSTRAINT [PK_AspNetUsers] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240819164003_AddAll')
BEGIN
    CREATE TABLE [ConditionDiagnoses] (
        [ConditionId] int NOT NULL IDENTITY,
        [Icd10Code] nvarchar(max) NOT NULL,
        [Name] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_ConditionDiagnoses] PRIMARY KEY ([ConditionId])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240819164003_AddAll')
BEGIN
    CREATE TABLE [DosageForms] (
        [DosageId] int NOT NULL IDENTITY,
        [Name] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_DosageForms] PRIMARY KEY ([DosageId])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240819164003_AddAll')
BEGIN
    CREATE TABLE [Provinces] (
        [ProvinceId] int NOT NULL IDENTITY,
        [Name] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_Provinces] PRIMARY KEY ([ProvinceId])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240819164003_AddAll')
BEGIN
    CREATE TABLE [Treatments] (
        [TreatmentId] int NOT NULL IDENTITY,
        [Code] nvarchar(max) NOT NULL,
        [Description] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_Treatments] PRIMARY KEY ([TreatmentId])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240819164003_AddAll')
BEGIN
    CREATE TABLE [Vitals] (
        [VitalId] int NOT NULL IDENTITY,
        [Name] nvarchar(max) NOT NULL,
        [Minimum] float NULL,
        [Maximum] float NULL,
        CONSTRAINT [PK_Vitals] PRIMARY KEY ([VitalId])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240819164003_AddAll')
BEGIN
    CREATE TABLE [MedicationInteractions] (
        [InteractionId] int NOT NULL IDENTITY,
        [ActiveIngredient1Id] int NOT NULL,
        [ActiveIngredient2Id] int NOT NULL,
        [Description] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_MedicationInteractions] PRIMARY KEY ([InteractionId]),
        CONSTRAINT [FK_MedicationInteractions_ActiveIngredients_ActiveIngredient1Id] FOREIGN KEY ([ActiveIngredient1Id]) REFERENCES [ActiveIngredients] ([IngredientId]) ON DELETE CASCADE,
        CONSTRAINT [FK_MedicationInteractions_ActiveIngredients_ActiveIngredient2Id] FOREIGN KEY ([ActiveIngredient2Id]) REFERENCES [ActiveIngredients] ([IngredientId])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240819164003_AddAll')
BEGIN
    CREATE TABLE [AspNetRoleClaims] (
        [Id] int NOT NULL IDENTITY,
        [RoleId] int NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240819164003_AddAll')
BEGIN
    CREATE TABLE [Admins] (
        [AdminId] int NOT NULL IDENTITY,
        [UserName] nvarchar(max) NOT NULL,
        [UserId] int NOT NULL,
        CONSTRAINT [PK_Admins] PRIMARY KEY ([AdminId]),
        CONSTRAINT [FK_Admins_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240819164003_AddAll')
BEGIN
    CREATE TABLE [Anaesthesiologists] (
        [AnaesthesiologistId] int NOT NULL IDENTITY,
        [Name] nvarchar(max) NOT NULL,
        [Surname] nvarchar(max) NOT NULL,
        [FullName] nvarchar(max) NOT NULL,
        [ContactNumber] nvarchar(max) NOT NULL,
        [EmailAddress] nvarchar(max) NOT NULL,
        [RegistrationNumber] nvarchar(max) NOT NULL,
        [UserId] int NOT NULL,
        CONSTRAINT [PK_Anaesthesiologists] PRIMARY KEY ([AnaesthesiologistId]),
        CONSTRAINT [FK_Anaesthesiologists_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240819164003_AddAll')
BEGIN
    CREATE TABLE [AspNetUserClaims] (
        [Id] int NOT NULL IDENTITY,
        [UserId] int NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240819164003_AddAll')
BEGIN
    CREATE TABLE [AspNetUserLogins] (
        [LoginProvider] nvarchar(128) NOT NULL,
        [ProviderKey] nvarchar(128) NOT NULL,
        [ProviderDisplayName] nvarchar(max) NULL,
        [UserId] int NOT NULL,
        CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
        CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240819164003_AddAll')
BEGIN
    CREATE TABLE [AspNetUserRoles] (
        [UserId] int NOT NULL,
        [RoleId] int NOT NULL,
        CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
        CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240819164003_AddAll')
BEGIN
    CREATE TABLE [AspNetUserTokens] (
        [UserId] int NOT NULL,
        [LoginProvider] nvarchar(128) NOT NULL,
        [Name] nvarchar(128) NOT NULL,
        [Value] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
        CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240819164003_AddAll')
BEGIN
    CREATE TABLE [Nurses] (
        [NurseId] int NOT NULL IDENTITY,
        [Name] nvarchar(max) NOT NULL,
        [Surname] nvarchar(max) NOT NULL,
        [FullName] nvarchar(max) NOT NULL,
        [ContactNumber] nvarchar(max) NOT NULL,
        [EmailAddress] nvarchar(max) NOT NULL,
        [RegistrationNumber] nvarchar(max) NOT NULL,
        [UserId] int NOT NULL,
        CONSTRAINT [PK_Nurses] PRIMARY KEY ([NurseId]),
        CONSTRAINT [FK_Nurses_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240819164003_AddAll')
BEGIN
    CREATE TABLE [Pharmacists] (
        [PharmacistId] int NOT NULL IDENTITY,
        [Name] nvarchar(max) NOT NULL,
        [Surname] nvarchar(max) NOT NULL,
        [FullName] nvarchar(max) NOT NULL,
        [ContactNumber] nvarchar(max) NOT NULL,
        [EmailAddress] nvarchar(max) NOT NULL,
        [RegistrationNumber] nvarchar(max) NOT NULL,
        [UserId] int NOT NULL,
        CONSTRAINT [PK_Pharmacists] PRIMARY KEY ([PharmacistId]),
        CONSTRAINT [FK_Pharmacists_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240819164003_AddAll')
BEGIN
    CREATE TABLE [Surgeons] (
        [SurgeonId] int NOT NULL IDENTITY,
        [Name] nvarchar(max) NOT NULL,
        [Surname] nvarchar(max) NOT NULL,
        [FullName] nvarchar(max) NOT NULL,
        [ContactNumber] nvarchar(max) NOT NULL,
        [EmailAddress] nvarchar(max) NOT NULL,
        [RegistrationNumber] nvarchar(max) NOT NULL,
        [UserId] int NOT NULL,
        CONSTRAINT [PK_Surgeons] PRIMARY KEY ([SurgeonId]),
        CONSTRAINT [FK_Surgeons_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240819164003_AddAll')
BEGIN
    CREATE TABLE [ContraIndications] (
        [ContraIndicationId] int NOT NULL IDENTITY,
        [ActiveIngredientId] int NOT NULL,
        [ConditionDiagnosisId] int NOT NULL,
        CONSTRAINT [PK_ContraIndications] PRIMARY KEY ([ContraIndicationId]),
        CONSTRAINT [FK_ContraIndications_ActiveIngredients_ActiveIngredientId] FOREIGN KEY ([ActiveIngredientId]) REFERENCES [ActiveIngredients] ([IngredientId]) ON DELETE CASCADE,
        CONSTRAINT [FK_ContraIndications_ConditionDiagnoses_ConditionDiagnosisId] FOREIGN KEY ([ConditionDiagnosisId]) REFERENCES [ConditionDiagnoses] ([ConditionId]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240819164003_AddAll')
BEGIN
    CREATE TABLE [Medications] (
        [MedicationId] int NOT NULL IDENTITY,
        [Name] nvarchar(max) NOT NULL,
        [Schedule] int NOT NULL,
        [StockOnHand] int NULL,
        [ReOrderLevel] int NULL,
        [DosageFormId] int NOT NULL,
        CONSTRAINT [PK_Medications] PRIMARY KEY ([MedicationId]),
        CONSTRAINT [FK_Medications_DosageForms_DosageFormId] FOREIGN KEY ([DosageFormId]) REFERENCES [DosageForms] ([DosageId]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240819164003_AddAll')
BEGIN
    CREATE TABLE [Cities] (
        [CityId] int NOT NULL IDENTITY,
        [Name] nvarchar(50) NOT NULL,
        [ProvinceId] int NOT NULL,
        CONSTRAINT [PK_Cities] PRIMARY KEY ([CityId]),
        CONSTRAINT [FK_Cities_Provinces_ProvinceId] FOREIGN KEY ([ProvinceId]) REFERENCES [Provinces] ([ProvinceId]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240819164003_AddAll')
BEGIN
    CREATE TABLE [order2s] (
        [OrderId] int NOT NULL IDENTITY,
        [Date] datetime2 NOT NULL,
        [Status] nvarchar(max) NOT NULL,
        [PharmacistId] int NOT NULL,
        CONSTRAINT [PK_order2s] PRIMARY KEY ([OrderId]),
        CONSTRAINT [FK_order2s_Pharmacists_PharmacistId] FOREIGN KEY ([PharmacistId]) REFERENCES [Pharmacists] ([PharmacistId]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240819164003_AddAll')
BEGIN
    CREATE TABLE [MedicationCarts] (
        [MedicationCartId] int NOT NULL IDENTITY,
        [MedicationId] int NOT NULL,
        [CartId] int NOT NULL,
        [Quantity] int NOT NULL,
        CONSTRAINT [PK_MedicationCarts] PRIMARY KEY ([MedicationCartId]),
        CONSTRAINT [FK_MedicationCarts_Medications_MedicationId] FOREIGN KEY ([MedicationId]) REFERENCES [Medications] ([MedicationId]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240819164003_AddAll')
BEGIN
    CREATE TABLE [MedicationIngredients] (
        [MedicationIngredientId] int NOT NULL IDENTITY,
        [ActiveIngredientId] int NOT NULL,
        [MedicationId] int NOT NULL,
        [ActiveIngredientStrength] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_MedicationIngredients] PRIMARY KEY ([MedicationIngredientId]),
        CONSTRAINT [FK_MedicationIngredients_ActiveIngredients_ActiveIngredientId] FOREIGN KEY ([ActiveIngredientId]) REFERENCES [ActiveIngredients] ([IngredientId]) ON DELETE CASCADE,
        CONSTRAINT [FK_MedicationIngredients_Medications_MedicationId] FOREIGN KEY ([MedicationId]) REFERENCES [Medications] ([MedicationId]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240819164003_AddAll')
BEGIN
    CREATE TABLE [StockOrders] (
        [StockOrderId] int NOT NULL IDENTITY,
        [MedicationId] int NOT NULL,
        [Quantity] int NOT NULL,
        CONSTRAINT [PK_StockOrders] PRIMARY KEY ([StockOrderId]),
        CONSTRAINT [FK_StockOrders_Medications_MedicationId] FOREIGN KEY ([MedicationId]) REFERENCES [Medications] ([MedicationId]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240819164003_AddAll')
BEGIN
    CREATE TABLE [Suburbs] (
        [SuburbId] int NOT NULL IDENTITY,
        [Name] nvarchar(50) NOT NULL,
        [PostalCode] int NOT NULL,
        [CityId] int NOT NULL,
        CONSTRAINT [PK_Suburbs] PRIMARY KEY ([SuburbId]),
        CONSTRAINT [FK_Suburbs_Cities_CityId] FOREIGN KEY ([CityId]) REFERENCES [Cities] ([CityId]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240819164003_AddAll')
BEGIN
    CREATE TABLE [PharmacistOrders] (
        [PharmacistOrderId] int NOT NULL IDENTITY,
        [Quantity] int NOT NULL,
        [OrderId] int NOT NULL,
        [MedicationId] int NOT NULL,
        [StockOnHand] int NOT NULL,
        CONSTRAINT [PK_PharmacistOrders] PRIMARY KEY ([PharmacistOrderId]),
        CONSTRAINT [FK_PharmacistOrders_Medications_MedicationId] FOREIGN KEY ([MedicationId]) REFERENCES [Medications] ([MedicationId]) ON DELETE CASCADE,
        CONSTRAINT [FK_PharmacistOrders_order2s_OrderId] FOREIGN KEY ([OrderId]) REFERENCES [order2s] ([OrderId]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240819164003_AddAll')
BEGIN
    CREATE TABLE [Hospitals] (
        [HospitalId] int NOT NULL IDENTITY,
        [Name] nvarchar(max) NOT NULL,
        [ContactNumber] nvarchar(max) NOT NULL,
        [EmailAddress] nvarchar(max) NOT NULL,
        [AddressLine1] nvarchar(max) NOT NULL,
        [AddressLine2] nvarchar(max) NOT NULL,
        [HospitalNumber] nvarchar(max) NOT NULL,
        [SuburbId] int NOT NULL,
        CONSTRAINT [PK_Hospitals] PRIMARY KEY ([HospitalId]),
        CONSTRAINT [FK_Hospitals_Suburbs_SuburbId] FOREIGN KEY ([SuburbId]) REFERENCES [Suburbs] ([SuburbId]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240819164003_AddAll')
BEGIN
    CREATE TABLE [Patients] (
        [PatientId] int NOT NULL IDENTITY,
        [Name] nvarchar(max) NOT NULL,
        [Surname] nvarchar(max) NOT NULL,
        [FullName] nvarchar(max) NOT NULL,
        [IdNumber] nvarchar(13) NOT NULL,
        [ContactNumber] nvarchar(max) NULL,
        [EmailAddress] nvarchar(max) NOT NULL,
        [Gender] nvarchar(max) NOT NULL,
        [DateOfBirth] datetime2 NOT NULL,
        [AdmissionDate] datetime2 NULL,
        [DischargeDate] datetime2 NULL,
        [AddressLine1] nvarchar(max) NULL,
        [AddressLine2] nvarchar(max) NULL,
        [SuburbId] int NULL,
        [NurseId] int NULL,
        CONSTRAINT [PK_Patients] PRIMARY KEY ([PatientId]),
        CONSTRAINT [FK_Patients_Nurses_NurseId] FOREIGN KEY ([NurseId]) REFERENCES [Nurses] ([NurseId]),
        CONSTRAINT [FK_Patients_Suburbs_SuburbId] FOREIGN KEY ([SuburbId]) REFERENCES [Suburbs] ([SuburbId])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240819164003_AddAll')
BEGIN
    CREATE TABLE [Wards] (
        [WardId] int NOT NULL IDENTITY,
        [WardNumber] nvarchar(max) NOT NULL,
        [HospitalId] int NULL,
        CONSTRAINT [PK_Wards] PRIMARY KEY ([WardId]),
        CONSTRAINT [FK_Wards_Hospitals_HospitalId] FOREIGN KEY ([HospitalId]) REFERENCES [Hospitals] ([HospitalId])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240819164003_AddAll')
BEGIN
    CREATE TABLE [Orders] (
        [OrderId] int NOT NULL IDENTITY,
        [Date] datetime2 NOT NULL,
        [IsUrgent] nvarchar(max) NOT NULL,
        [Status] nvarchar(max) NOT NULL,
        [AnaesthesiologistId] int NOT NULL,
        [PatientId] int NULL,
        [PharmacistId] int NULL,
        CONSTRAINT [PK_Orders] PRIMARY KEY ([OrderId]),
        CONSTRAINT [FK_Orders_Anaesthesiologists_AnaesthesiologistId] FOREIGN KEY ([AnaesthesiologistId]) REFERENCES [Anaesthesiologists] ([AnaesthesiologistId]) ON DELETE CASCADE,
        CONSTRAINT [FK_Orders_Patients_PatientId] FOREIGN KEY ([PatientId]) REFERENCES [Patients] ([PatientId]),
        CONSTRAINT [FK_Orders_Pharmacists_PharmacistId] FOREIGN KEY ([PharmacistId]) REFERENCES [Pharmacists] ([PharmacistId])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240819164003_AddAll')
BEGIN
    CREATE TABLE [PatientAllergies] (
        [AllergyId] int NOT NULL IDENTITY,
        [PatientId] int NOT NULL,
        [ActiveIngredientId] int NOT NULL,
        CONSTRAINT [PK_PatientAllergies] PRIMARY KEY ([AllergyId]),
        CONSTRAINT [FK_PatientAllergies_ActiveIngredients_ActiveIngredientId] FOREIGN KEY ([ActiveIngredientId]) REFERENCES [ActiveIngredients] ([IngredientId]) ON DELETE CASCADE,
        CONSTRAINT [FK_PatientAllergies_Patients_PatientId] FOREIGN KEY ([PatientId]) REFERENCES [Patients] ([PatientId]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240819164003_AddAll')
BEGIN
    CREATE TABLE [PatientConditions] (
        [PatientConditionId] int NOT NULL IDENTITY,
        [PatientId] int NOT NULL,
        [ConditionId] int NOT NULL,
        CONSTRAINT [PK_PatientConditions] PRIMARY KEY ([PatientConditionId]),
        CONSTRAINT [FK_PatientConditions_ConditionDiagnoses_ConditionId] FOREIGN KEY ([ConditionId]) REFERENCES [ConditionDiagnoses] ([ConditionId]) ON DELETE CASCADE,
        CONSTRAINT [FK_PatientConditions_Patients_PatientId] FOREIGN KEY ([PatientId]) REFERENCES [Patients] ([PatientId]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240819164003_AddAll')
BEGIN
    CREATE TABLE [PatientMedications] (
        [PatientMedicationId] int NOT NULL IDENTITY,
        [PatientId] int NOT NULL,
        [MedicationId] int NOT NULL,
        CONSTRAINT [PK_PatientMedications] PRIMARY KEY ([PatientMedicationId]),
        CONSTRAINT [FK_PatientMedications_Medications_MedicationId] FOREIGN KEY ([MedicationId]) REFERENCES [Medications] ([MedicationId]) ON DELETE CASCADE,
        CONSTRAINT [FK_PatientMedications_Patients_PatientId] FOREIGN KEY ([PatientId]) REFERENCES [Patients] ([PatientId]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240819164003_AddAll')
BEGIN
    CREATE TABLE [PatientVitals] (
        [PatientVitalId] int NOT NULL IDENTITY,
        [PatientId] int NOT NULL,
        [VitalId] int NOT NULL,
        CONSTRAINT [PK_PatientVitals] PRIMARY KEY ([PatientVitalId]),
        CONSTRAINT [FK_PatientVitals_Patients_PatientId] FOREIGN KEY ([PatientId]) REFERENCES [Patients] ([PatientId]) ON DELETE CASCADE,
        CONSTRAINT [FK_PatientVitals_Vitals_VitalId] FOREIGN KEY ([VitalId]) REFERENCES [Vitals] ([VitalId]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240819164003_AddAll')
BEGIN
    CREATE TABLE [Prescriptions] (
        [PrescriptionId] int NOT NULL IDENTITY,
        [Date] datetime2 NOT NULL,
        [Status] nvarchar(max) NOT NULL,
        [Urgent] nvarchar(max) NOT NULL,
        [Note] nvarchar(max) NOT NULL,
        [SurgeonId] int NOT NULL,
        [PatientId] int NOT NULL,
        [PharmacistId] int NULL,
        [NurseId] int NULL,
        CONSTRAINT [PK_Prescriptions] PRIMARY KEY ([PrescriptionId]),
        CONSTRAINT [FK_Prescriptions_Nurses_NurseId] FOREIGN KEY ([NurseId]) REFERENCES [Nurses] ([NurseId]),
        CONSTRAINT [FK_Prescriptions_Patients_PatientId] FOREIGN KEY ([PatientId]) REFERENCES [Patients] ([PatientId]) ON DELETE CASCADE,
        CONSTRAINT [FK_Prescriptions_Pharmacists_PharmacistId] FOREIGN KEY ([PharmacistId]) REFERENCES [Pharmacists] ([PharmacistId]),
        CONSTRAINT [FK_Prescriptions_Surgeons_SurgeonId] FOREIGN KEY ([SurgeonId]) REFERENCES [Surgeons] ([SurgeonId]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240819164003_AddAll')
BEGIN
    CREATE TABLE [VitalRanges] (
        [VitalId] int NOT NULL IDENTITY,
        [Height] nvarchar(max) NULL,
        [Weight] nvarchar(max) NULL,
        [Temperature] nvarchar(max) NULL,
        [BloodPressure] nvarchar(max) NULL,
        [PulseRate] nvarchar(max) NULL,
        [RespiratoryRate] nvarchar(max) NULL,
        [BloodGlucoseLevel] nvarchar(max) NULL,
        [BloodOxegenLevel] nvarchar(max) NULL,
        [OxygenSaturation] nvarchar(max) NULL,
        [HeartRate] nvarchar(max) NULL,
        [Time] datetime2 NOT NULL,
        [PatientId] int NOT NULL,
        CONSTRAINT [PK_VitalRanges] PRIMARY KEY ([VitalId]),
        CONSTRAINT [FK_VitalRanges_Patients_PatientId] FOREIGN KEY ([PatientId]) REFERENCES [Patients] ([PatientId]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240819164003_AddAll')
BEGIN
    CREATE TABLE [Beds] (
        [BedId] int NOT NULL IDENTITY,
        [BedNumber] nvarchar(max) NOT NULL,
        [WardId] int NOT NULL,
        CONSTRAINT [PK_Beds] PRIMARY KEY ([BedId]),
        CONSTRAINT [FK_Beds_Wards_WardId] FOREIGN KEY ([WardId]) REFERENCES [Wards] ([WardId]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240819164003_AddAll')
BEGIN
    CREATE TABLE [Theatres] (
        [TheatreId] int NOT NULL IDENTITY,
        [Name] nvarchar(max) NOT NULL,
        [WardId] int NOT NULL,
        CONSTRAINT [PK_Theatres] PRIMARY KEY ([TheatreId]),
        CONSTRAINT [FK_Theatres_Wards_WardId] FOREIGN KEY ([WardId]) REFERENCES [Wards] ([WardId]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240819164003_AddAll')
BEGIN
    CREATE TABLE [MedicationOrders] (
        [MedicationOrderId] int NOT NULL IDENTITY,
        [Quantity] int NOT NULL,
        [OrderId] int NOT NULL,
        [MedicationId] int NOT NULL,
        [Note] nvarchar(max) NULL,
        CONSTRAINT [PK_MedicationOrders] PRIMARY KEY ([MedicationOrderId]),
        CONSTRAINT [FK_MedicationOrders_Medications_MedicationId] FOREIGN KEY ([MedicationId]) REFERENCES [Medications] ([MedicationId]) ON DELETE CASCADE,
        CONSTRAINT [FK_MedicationOrders_Orders_OrderId] FOREIGN KEY ([OrderId]) REFERENCES [Orders] ([OrderId]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240819164003_AddAll')
BEGIN
    CREATE TABLE [AdministeredMedications] (
        [AdministeredMedicationId] int NOT NULL IDENTITY,
        [MedicationId] int NOT NULL,
        [PrescriptionId] int NOT NULL,
        [Quantity] int NOT NULL,
        [Time] datetime2 NOT NULL,
        CONSTRAINT [PK_AdministeredMedications] PRIMARY KEY ([AdministeredMedicationId]),
        CONSTRAINT [FK_AdministeredMedications_Medications_MedicationId] FOREIGN KEY ([MedicationId]) REFERENCES [Medications] ([MedicationId]) ON DELETE CASCADE,
        CONSTRAINT [FK_AdministeredMedications_Prescriptions_PrescriptionId] FOREIGN KEY ([PrescriptionId]) REFERENCES [Prescriptions] ([PrescriptionId]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240819164003_AddAll')
BEGIN
    CREATE TABLE [MedicationPrescriptions] (
        [MedicationPrescriptionId] int NOT NULL IDENTITY,
        [MedicationId] int NOT NULL,
        [PrescriptionId] int NOT NULL,
        [Quantity] int NOT NULL,
        [Instructions] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_MedicationPrescriptions] PRIMARY KEY ([MedicationPrescriptionId]),
        CONSTRAINT [FK_MedicationPrescriptions_Medications_MedicationId] FOREIGN KEY ([MedicationId]) REFERENCES [Medications] ([MedicationId]) ON DELETE CASCADE,
        CONSTRAINT [FK_MedicationPrescriptions_Prescriptions_PrescriptionId] FOREIGN KEY ([PrescriptionId]) REFERENCES [Prescriptions] ([PrescriptionId]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240819164003_AddAll')
BEGIN
    CREATE TABLE [PrescribedMedications] (
        [PrescribedMedicationId] int NOT NULL IDENTITY,
        [MedicationId] int NOT NULL,
        [PrescriptionId] int NULL,
        [OrderId] int NULL,
        [Quantity] int NOT NULL,
        CONSTRAINT [PK_PrescribedMedications] PRIMARY KEY ([PrescribedMedicationId]),
        CONSTRAINT [FK_PrescribedMedications_Medications_MedicationId] FOREIGN KEY ([MedicationId]) REFERENCES [Medications] ([MedicationId]) ON DELETE CASCADE,
        CONSTRAINT [FK_PrescribedMedications_Orders_OrderId] FOREIGN KEY ([OrderId]) REFERENCES [Orders] ([OrderId]),
        CONSTRAINT [FK_PrescribedMedications_Prescriptions_PrescriptionId] FOREIGN KEY ([PrescriptionId]) REFERENCES [Prescriptions] ([PrescriptionId])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240819164003_AddAll')
BEGIN
    CREATE TABLE [RejectedPrescriptions] (
        [RejectedPrescriptionId] int NOT NULL IDENTITY,
        [PrescriptionId] int NOT NULL,
        [RejectedReason] nvarchar(max) NULL,
        CONSTRAINT [PK_RejectedPrescriptions] PRIMARY KEY ([RejectedPrescriptionId]),
        CONSTRAINT [FK_RejectedPrescriptions_Prescriptions_PrescriptionId] FOREIGN KEY ([PrescriptionId]) REFERENCES [Prescriptions] ([PrescriptionId]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240819164003_AddAll')
BEGIN
    CREATE TABLE [PatientBeds] (
        [PatientBedId] int NOT NULL IDENTITY,
        [BedId] int NOT NULL,
        [PatientId] int NOT NULL,
        CONSTRAINT [PK_PatientBeds] PRIMARY KEY ([PatientBedId]),
        CONSTRAINT [FK_PatientBeds_Beds_BedId] FOREIGN KEY ([BedId]) REFERENCES [Beds] ([BedId]) ON DELETE CASCADE,
        CONSTRAINT [FK_PatientBeds_Patients_PatientId] FOREIGN KEY ([PatientId]) REFERENCES [Patients] ([PatientId]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240819164003_AddAll')
BEGIN
    CREATE TABLE [Bookings] (
        [BookingId] int NOT NULL IDENTITY,
        [Date] datetime2 NOT NULL,
        [PatientId] int NOT NULL,
        [SurgeonId] int NOT NULL,
        [AnaesthesiologistId] int NOT NULL,
        [TheatreId] int NOT NULL,
        [Status] bit NOT NULL,
        [Session] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_Bookings] PRIMARY KEY ([BookingId]),
        CONSTRAINT [FK_Bookings_Anaesthesiologists_AnaesthesiologistId] FOREIGN KEY ([AnaesthesiologistId]) REFERENCES [Anaesthesiologists] ([AnaesthesiologistId]) ON DELETE CASCADE,
        CONSTRAINT [FK_Bookings_Patients_PatientId] FOREIGN KEY ([PatientId]) REFERENCES [Patients] ([PatientId]) ON DELETE CASCADE,
        CONSTRAINT [FK_Bookings_Surgeons_SurgeonId] FOREIGN KEY ([SurgeonId]) REFERENCES [Surgeons] ([SurgeonId]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Bookings_Theatres_TheatreId] FOREIGN KEY ([TheatreId]) REFERENCES [Theatres] ([TheatreId]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240819164003_AddAll')
BEGIN
    CREATE TABLE [PatientTreatments] (
        [PatientTreatmentId] int NOT NULL IDENTITY,
        [PatientId] int NOT NULL,
        [TreatmentId] int NOT NULL,
        [BookingId] int NOT NULL,
        CONSTRAINT [PK_PatientTreatments] PRIMARY KEY ([PatientTreatmentId]),
        CONSTRAINT [FK_PatientTreatments_Bookings_BookingId] FOREIGN KEY ([BookingId]) REFERENCES [Bookings] ([BookingId]) ON DELETE CASCADE,
        CONSTRAINT [FK_PatientTreatments_Patients_PatientId] FOREIGN KEY ([PatientId]) REFERENCES [Patients] ([PatientId]) ON DELETE NO ACTION,
        CONSTRAINT [FK_PatientTreatments_Treatments_TreatmentId] FOREIGN KEY ([TreatmentId]) REFERENCES [Treatments] ([TreatmentId]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240819164003_AddAll')
BEGIN
    CREATE INDEX [IX_AdministeredMedications_MedicationId] ON [AdministeredMedications] ([MedicationId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240819164003_AddAll')
BEGIN
    CREATE INDEX [IX_AdministeredMedications_PrescriptionId] ON [AdministeredMedications] ([PrescriptionId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240819164003_AddAll')
BEGIN
    CREATE INDEX [IX_Admins_UserId] ON [Admins] ([UserId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240819164003_AddAll')
BEGIN
    CREATE INDEX [IX_Anaesthesiologists_UserId] ON [Anaesthesiologists] ([UserId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240819164003_AddAll')
BEGIN
    CREATE INDEX [IX_AspNetRoleClaims_RoleId] ON [AspNetRoleClaims] ([RoleId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240819164003_AddAll')
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [RoleNameIndex] ON [AspNetRoles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL');
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240819164003_AddAll')
BEGIN
    CREATE INDEX [IX_AspNetUserClaims_UserId] ON [AspNetUserClaims] ([UserId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240819164003_AddAll')
BEGIN
    CREATE INDEX [IX_AspNetUserLogins_UserId] ON [AspNetUserLogins] ([UserId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240819164003_AddAll')
BEGIN
    CREATE INDEX [IX_AspNetUserRoles_RoleId] ON [AspNetUserRoles] ([RoleId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240819164003_AddAll')
BEGIN
    CREATE INDEX [EmailIndex] ON [AspNetUsers] ([NormalizedEmail]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240819164003_AddAll')
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [UserNameIndex] ON [AspNetUsers] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL');
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240819164003_AddAll')
BEGIN
    CREATE INDEX [IX_Beds_WardId] ON [Beds] ([WardId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240819164003_AddAll')
BEGIN
    CREATE INDEX [IX_Bookings_AnaesthesiologistId] ON [Bookings] ([AnaesthesiologistId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240819164003_AddAll')
BEGIN
    CREATE INDEX [IX_Bookings_PatientId] ON [Bookings] ([PatientId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240819164003_AddAll')
BEGIN
    CREATE INDEX [IX_Bookings_SurgeonId] ON [Bookings] ([SurgeonId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240819164003_AddAll')
BEGIN
    CREATE INDEX [IX_Bookings_TheatreId] ON [Bookings] ([TheatreId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240819164003_AddAll')
BEGIN
    CREATE INDEX [IX_Cities_ProvinceId] ON [Cities] ([ProvinceId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240819164003_AddAll')
BEGIN
    CREATE INDEX [IX_ContraIndications_ActiveIngredientId] ON [ContraIndications] ([ActiveIngredientId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240819164003_AddAll')
BEGIN
    CREATE INDEX [IX_ContraIndications_ConditionDiagnosisId] ON [ContraIndications] ([ConditionDiagnosisId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240819164003_AddAll')
BEGIN
    CREATE INDEX [IX_Hospitals_SuburbId] ON [Hospitals] ([SuburbId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240819164003_AddAll')
BEGIN
    CREATE INDEX [IX_MedicationCarts_MedicationId] ON [MedicationCarts] ([MedicationId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240819164003_AddAll')
BEGIN
    CREATE INDEX [IX_MedicationIngredients_ActiveIngredientId] ON [MedicationIngredients] ([ActiveIngredientId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240819164003_AddAll')
BEGIN
    CREATE INDEX [IX_MedicationIngredients_MedicationId] ON [MedicationIngredients] ([MedicationId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240819164003_AddAll')
BEGIN
    CREATE INDEX [IX_MedicationInteractions_ActiveIngredient1Id] ON [MedicationInteractions] ([ActiveIngredient1Id]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240819164003_AddAll')
BEGIN
    CREATE INDEX [IX_MedicationInteractions_ActiveIngredient2Id] ON [MedicationInteractions] ([ActiveIngredient2Id]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240819164003_AddAll')
BEGIN
    CREATE INDEX [IX_MedicationOrders_MedicationId] ON [MedicationOrders] ([MedicationId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240819164003_AddAll')
BEGIN
    CREATE INDEX [IX_MedicationOrders_OrderId] ON [MedicationOrders] ([OrderId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240819164003_AddAll')
BEGIN
    CREATE INDEX [IX_MedicationPrescriptions_MedicationId] ON [MedicationPrescriptions] ([MedicationId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240819164003_AddAll')
BEGIN
    CREATE INDEX [IX_MedicationPrescriptions_PrescriptionId] ON [MedicationPrescriptions] ([PrescriptionId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240819164003_AddAll')
BEGIN
    CREATE INDEX [IX_Medications_DosageFormId] ON [Medications] ([DosageFormId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240819164003_AddAll')
BEGIN
    CREATE INDEX [IX_Nurses_UserId] ON [Nurses] ([UserId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240819164003_AddAll')
BEGIN
    CREATE INDEX [IX_order2s_PharmacistId] ON [order2s] ([PharmacistId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240819164003_AddAll')
BEGIN
    CREATE INDEX [IX_Orders_AnaesthesiologistId] ON [Orders] ([AnaesthesiologistId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240819164003_AddAll')
BEGIN
    CREATE INDEX [IX_Orders_PatientId] ON [Orders] ([PatientId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240819164003_AddAll')
BEGIN
    CREATE INDEX [IX_Orders_PharmacistId] ON [Orders] ([PharmacistId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240819164003_AddAll')
BEGIN
    CREATE INDEX [IX_PatientAllergies_ActiveIngredientId] ON [PatientAllergies] ([ActiveIngredientId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240819164003_AddAll')
BEGIN
    CREATE INDEX [IX_PatientAllergies_PatientId] ON [PatientAllergies] ([PatientId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240819164003_AddAll')
BEGIN
    CREATE INDEX [IX_PatientBeds_BedId] ON [PatientBeds] ([BedId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240819164003_AddAll')
BEGIN
    CREATE INDEX [IX_PatientBeds_PatientId] ON [PatientBeds] ([PatientId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240819164003_AddAll')
BEGIN
    CREATE INDEX [IX_PatientConditions_ConditionId] ON [PatientConditions] ([ConditionId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240819164003_AddAll')
BEGIN
    CREATE INDEX [IX_PatientConditions_PatientId] ON [PatientConditions] ([PatientId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240819164003_AddAll')
BEGIN
    CREATE INDEX [IX_PatientMedications_MedicationId] ON [PatientMedications] ([MedicationId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240819164003_AddAll')
BEGIN
    CREATE INDEX [IX_PatientMedications_PatientId] ON [PatientMedications] ([PatientId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240819164003_AddAll')
BEGIN
    CREATE INDEX [IX_Patients_NurseId] ON [Patients] ([NurseId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240819164003_AddAll')
BEGIN
    CREATE INDEX [IX_Patients_SuburbId] ON [Patients] ([SuburbId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240819164003_AddAll')
BEGIN
    CREATE INDEX [IX_PatientTreatments_BookingId] ON [PatientTreatments] ([BookingId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240819164003_AddAll')
BEGIN
    CREATE INDEX [IX_PatientTreatments_PatientId] ON [PatientTreatments] ([PatientId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240819164003_AddAll')
BEGIN
    CREATE INDEX [IX_PatientTreatments_TreatmentId] ON [PatientTreatments] ([TreatmentId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240819164003_AddAll')
BEGIN
    CREATE INDEX [IX_PatientVitals_PatientId] ON [PatientVitals] ([PatientId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240819164003_AddAll')
BEGIN
    CREATE INDEX [IX_PatientVitals_VitalId] ON [PatientVitals] ([VitalId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240819164003_AddAll')
BEGIN
    CREATE INDEX [IX_PharmacistOrders_MedicationId] ON [PharmacistOrders] ([MedicationId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240819164003_AddAll')
BEGIN
    CREATE INDEX [IX_PharmacistOrders_OrderId] ON [PharmacistOrders] ([OrderId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240819164003_AddAll')
BEGIN
    CREATE INDEX [IX_Pharmacists_UserId] ON [Pharmacists] ([UserId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240819164003_AddAll')
BEGIN
    CREATE INDEX [IX_PrescribedMedications_MedicationId] ON [PrescribedMedications] ([MedicationId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240819164003_AddAll')
BEGIN
    CREATE INDEX [IX_PrescribedMedications_OrderId] ON [PrescribedMedications] ([OrderId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240819164003_AddAll')
BEGIN
    CREATE INDEX [IX_PrescribedMedications_PrescriptionId] ON [PrescribedMedications] ([PrescriptionId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240819164003_AddAll')
BEGIN
    CREATE INDEX [IX_Prescriptions_NurseId] ON [Prescriptions] ([NurseId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240819164003_AddAll')
BEGIN
    CREATE INDEX [IX_Prescriptions_PatientId] ON [Prescriptions] ([PatientId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240819164003_AddAll')
BEGIN
    CREATE INDEX [IX_Prescriptions_PharmacistId] ON [Prescriptions] ([PharmacistId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240819164003_AddAll')
BEGIN
    CREATE INDEX [IX_Prescriptions_SurgeonId] ON [Prescriptions] ([SurgeonId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240819164003_AddAll')
BEGIN
    CREATE INDEX [IX_RejectedPrescriptions_PrescriptionId] ON [RejectedPrescriptions] ([PrescriptionId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240819164003_AddAll')
BEGIN
    CREATE INDEX [IX_StockOrders_MedicationId] ON [StockOrders] ([MedicationId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240819164003_AddAll')
BEGIN
    CREATE INDEX [IX_Suburbs_CityId] ON [Suburbs] ([CityId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240819164003_AddAll')
BEGIN
    CREATE INDEX [IX_Surgeons_UserId] ON [Surgeons] ([UserId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240819164003_AddAll')
BEGIN
    CREATE INDEX [IX_Theatres_WardId] ON [Theatres] ([WardId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240819164003_AddAll')
BEGIN
    CREATE INDEX [IX_VitalRanges_PatientId] ON [VitalRanges] ([PatientId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240819164003_AddAll')
BEGIN
    CREATE INDEX [IX_Wards_HospitalId] ON [Wards] ([HospitalId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20240819164003_AddAll')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20240819164003_AddAll', N'7.0.12');
END;
GO

COMMIT;
GO


CREATE TABLE [fic].[Story] (
    [StoryID]       INT                IDENTITY (1, 1) NOT NULL,
    [StoryKey]      UNIQUEIDENTIFIER   DEFAULT (newid()) NOT NULL,
    [SubscriberID]  INT                NOT NULL,
    [PersonaID]     INT                NOT NULL,
    [Name]          NVARCHAR (500)     NOT NULL,
    [IsPublished]   BIT                DEFAULT ((0)) NOT NULL,
    [Published]     DATETIMEOFFSET (7) NULL,
    [IsEnabled]     BIT                DEFAULT ((1)) NOT NULL,
    [IsDeleted]     BIT                DEFAULT ((0)) NOT NULL,
    [IsHidden]      BIT                DEFAULT ((0)) NOT NULL,
    [IsLocked]      BIT                DEFAULT ((0)) NOT NULL,
    [Created]       DATETIMEOFFSET (7) DEFAULT (sysdatetimeoffset()) NOT NULL,
    [CreatedUserID] INT                NOT NULL,
    [Updated]       DATETIMEOFFSET (7) NULL,
    [UpdatedUserID] INT                NULL,
    PRIMARY KEY CLUSTERED ([StoryID] ASC),
    CONSTRAINT [fk_fic_Story_CreatedUserID] FOREIGN KEY ([CreatedUserID]) REFERENCES [sec].[User] ([UserID]),
    CONSTRAINT [fk_fic_Story_UpdatedUserID] FOREIGN KEY ([UpdatedUserID]) REFERENCES [sec].[User] ([UserID])
);



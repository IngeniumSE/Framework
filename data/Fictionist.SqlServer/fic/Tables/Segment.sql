CREATE TABLE [fic].[Segment] (
    [SegmentID]     INT                IDENTITY (1, 1) NOT NULL,
    [SegmentKey]    UNIQUEIDENTIFIER   DEFAULT (newid()) NOT NULL,
    [Sequence]      INT                DEFAULT ((0)) NOT NULL,
    [StoryID]       INT                NOT NULL,
    [SubscriberID]  INT                NOT NULL,
    [PersonaID]     INT                NOT NULL,
    [Content]       NVARCHAR (MAX)     NOT NULL,
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
    PRIMARY KEY CLUSTERED ([SegmentID] ASC),
    CONSTRAINT [fk_fic_Segment_CreatedUserID] FOREIGN KEY ([CreatedUserID]) REFERENCES [sec].[User] ([UserID]),
    CONSTRAINT [fk_fic_Segment_UpdatedUserID] FOREIGN KEY ([UpdatedUserID]) REFERENCES [sec].[User] ([UserID])
);



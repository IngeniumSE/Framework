CREATE TABLE [sub].[Subscriber] (
    [SubscriberID]   INT                IDENTITY (1, 1) NOT NULL,
    [SubscriptionID] INT                NOT NULL,
    [Email]          NVARCHAR (500)     NOT NULL,
    [Name]           NVARCHAR (500)     NOT NULL,
    [Personas]       INT                NOT NULL,
		[From]					 DATETIME2					NOT NULL,
		[To]						 DATETIME2					NOT NULL,
		[Price]					 MONEY							NOT NULL,
		[IsMonthly]      BIT								DEFAULT ((0)) NOT NULL,
		[AutoRenew]			 BIT								DEFAULT ((0)) NOT NULL,
		[Renews]				 DATETIME2					NOT NULL,
    [IsEnabled]      BIT                DEFAULT ((1)) NOT NULL,
    [IsDeleted]      BIT                DEFAULT ((0)) NOT NULL,
    [IsHidden]       BIT                DEFAULT ((0)) NOT NULL,
    [IsLocked]       BIT                DEFAULT ((0)) NOT NULL,
    [Created]        DATETIMEOFFSET (7) DEFAULT (sysdatetimeoffset()) NOT NULL,
    [CreatedUserID]  INT                NOT NULL,
    [Updated]        DATETIMEOFFSET (7) NULL,
    [UpdatedUserID]  INT                NULL,
    PRIMARY KEY CLUSTERED ([SubscriberID] ASC),
    CONSTRAINT [fk_sub_Subscriber_CreatedUserID] FOREIGN KEY ([CreatedUserID]) REFERENCES [sec].[User] ([UserID]),
    CONSTRAINT [fk_sub_Subscriber_UpdatedUserID] FOREIGN KEY ([UpdatedUserID]) REFERENCES [sec].[User] ([UserID])
);



CREATE TABLE [sub].[Subscription] (
    [SubscriptionID] INT                IDENTITY (1, 1) NOT NULL,
    [Name]           NVARCHAR (500)     NOT NULL,
    [Personas]       INT                NOT NULL,
		[From]					 DATETIME2					NOT NULL,
		[To]						 DATETIME2					NOT NULL,
		[Price]					 MONEY							NOT NULL,
		[AnnualDiscount] DECIMAL(4,2)				NOT NULL,
    [IsEnabled]      BIT                DEFAULT ((1)) NOT NULL,
    [IsDeleted]      BIT                DEFAULT ((0)) NOT NULL,
    [IsHidden]       BIT                DEFAULT ((0)) NOT NULL,
    [IsLocked]       BIT                DEFAULT ((0)) NOT NULL,
    [Created]        DATETIMEOFFSET (7) DEFAULT (sysdatetimeoffset()) NOT NULL,
    [CreatedUserID]  INT                NOT NULL,
    [Updated]        DATETIMEOFFSET (7) NULL,
    [UpdatedUserID]  INT                NULL,
    PRIMARY KEY CLUSTERED ([SubscriptionID] ASC),
    CONSTRAINT [fk_sub_Subscription_CreatedUserID] FOREIGN KEY ([CreatedUserID]) REFERENCES [sec].[User] ([UserID]),
    CONSTRAINT [fk_sub_Subscription_UpdatedUserID] FOREIGN KEY ([UpdatedUserID]) REFERENCES [sec].[User] ([UserID])
);



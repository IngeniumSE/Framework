create table sec.[User] (
	[UserID] int identity(1,1) not null primary key,
	[Email] nvarchar(256) not null,
	[Name] nvarchar(512) not null,
	[FormalName] nvarchar(512) not null,
	[Handle] nvarchar(510) null,

	-- Standard flags
	[IsEnabled] bit not null default 1,
	[IsDeleted] bit not null default 0,
	[IsHidden] bit not null default 0,
	[IsLocked] bit not null default 0,
	-- Standard audit
	[Created] datetimeoffset not null default sysdatetimeoffset(),
	[CreatedUserID] int not null,
	[Updated] datetimeoffset null,
	[UpdatedUserID] int null,

  constraint fk_sec_User_CreatedUserID foreign key ([UserID]) references sec.[User] ([UserID]),
	constraint fk_sec_User_UpdatedUserID foreign key ([UserID]) references sec.[User] ([UserID])
)
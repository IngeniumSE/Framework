create table sec.[Role] (
	[RoleID] int identity(1,1) not null primary key,
	[Name] nvarchar(500) not null,

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

	constraint fk_sec_Role_CreatedUserID foreign key ([RoleID]) references sec.[User] ([UserID]),
	constraint fk_sec_Role_UpdatedUserID foreign key ([RoleID]) references sec.[User] ([UserID])
)
if not exists (
	select top(1) 1
		from sec.[User]
			where [UserID]=0) begin

			set identity_insert sec.[User] on;

			insert sec.[User] ([UserID], [Email], [Name], [IsHidden], [IsLocked], [CreatedUserID])
				values (0, '', 'System', 1, 1, 0);

			set identity_insert sec.[User] off;
end
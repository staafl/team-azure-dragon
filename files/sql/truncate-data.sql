EXEC sp_MSForEachTable "ALTER TABLE ? NOCHECK CONSTRAINT all";
--EXEC sp_MSForEachTable "DELETE FROM ?";
EXEC sp_MSForEachTable "DROP TABLE ?";
--EXEC sp_MSForEachTable "ALTER TABLE ? WITH CHECK CHECK CONSTRAINT all";

INSERT [dbo].[AspNetRoles] ([Id], [Name]) VALUES (N'1', N'admin')
INSERT [dbo].[AspNetRoles] ([Id], [Name]) VALUES (N'2', N'banned')
INSERT [dbo].[AspNetUsers] ([Id], [UserName], [Email], [Facebook], [Discriminator], [IsConfirmed]) VALUES (N'6d708f4b-53c7-4ae8-975f-a8972cfd2e25', N'admin', N'velko.nikolov@gmail.com', N'http://www.facebook.com/staafl', N'ApplicationUser', 1)
INSERT [dbo].[AspNetUserRoles] ([RoleId], [UserId]) VALUES (N'1', N'6d708f4b-53c7-4ae8-975f-a8972cfd2e25')
INSERT [dbo].[AspNetUserLogins] ([LoginProvider], [ProviderKey], [UserId]) VALUES (N'Local', N'admin', N'6d708f4b-53c7-4ae8-975f-a8972cfd2e25')
INSERT [dbo].[AspNetUserSecrets] ([UserName], [Secret]) VALUES (N'admin', N'ACQbq83L/rsvlWq11Zor2jVtz2KAMcHNd6x1SN2EXHs7VuZPGaE8DhhnvtyO10Nf5Q==')
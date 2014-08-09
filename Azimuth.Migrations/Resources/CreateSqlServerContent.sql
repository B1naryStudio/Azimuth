USE [Azimuth]
GO
CREATE TABLE [dbo].[Albums](
	[AlbumId] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Description] [nvarchar](500) NULL,
	[ArtistId] [bigint] NOT NULL,
 CONSTRAINT [PK_Albums] PRIMARY KEY CLUSTERED 
(
	[AlbumId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Artists]    Script Date: 09.08.2014 18:37:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Artists](
	[ArtistId] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Site] [nvarchar](100) NULL,
	[Description] [nvarchar](500) NULL,
 CONSTRAINT [PK_Artists] PRIMARY KEY CLUSTERED 
(
	[ArtistId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Genres]    Script Date: 09.08.2014 18:37:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Genres](
	[GenreId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Genres] PRIMARY KEY CLUSTERED 
(
	[GenreId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[PlaylistListeners]    Script Date: 09.08.2014 18:37:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PlaylistListeners](
	[PlaylistId] [bigint] NOT NULL,
	[ListenerId] [bigint] NOT NULL,
 CONSTRAINT [PK_PlaylistListeners] PRIMARY KEY CLUSTERED 
(
	[PlaylistId] ASC,
	[ListenerId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Playlists]    Script Date: 09.08.2014 18:37:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Playlists](
	[PlaylistsId] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Accessibilty] [nvarchar](50) NOT NULL,
	[CreatorId] [bigint] NOT NULL,
 CONSTRAINT [PK_Playlists] PRIMARY KEY CLUSTERED 
(
	[PlaylistsId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[PlaylistTracks]    Script Date: 09.08.2014 18:37:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PlaylistTracks](
	[PlaylistId] [bigint] NOT NULL,
	[TrackId] [bigint] NOT NULL,
 CONSTRAINT [PK_PlaylistTracks] PRIMARY KEY CLUSTERED 
(
	[PlaylistId] ASC,
	[TrackId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[SocialNetworks]    Script Date: 09.08.2014 18:37:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SocialNetworks](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Tracks]    Script Date: 09.08.2014 18:37:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Tracks](
	[TrackId] [bigint] IDENTITY(1,1) NOT NULL,
	[Lyrics] [nvarchar](500) NULL,
	[Duration] [nvarchar](50) NULL,
	[AlbumId] [bigint] NOT NULL,
	[ArtistId] [bigint] NOT NULL,
	[GenreId] [int] NOT NULL,
 CONSTRAINT [PK_Tracks] PRIMARY KEY CLUSTERED 
(
	[TrackId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Users]    Script Date: 09.08.2014 18:37:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[FirstName] [nvarchar](255) NULL,
	[LastName] [nvarchar](255) NULL,
	[ScreenName] [nvarchar](255) NULL,
	[Gender] [nvarchar](255) NULL,
	[Birthday] [nvarchar](255) NULL,
	[Photo] [nvarchar](255) NULL,
	[Timezone] [int] NULL,
	[Country] [nvarchar](255) NULL,
	[City] [nvarchar](255) NULL,
	[Email] [nvarchar](255) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[UserSocialNetworks]    Script Date: 09.08.2014 18:37:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserSocialNetworks](
	[UserId] [bigint] NOT NULL,
	[SocialNetworkId] [bigint] NOT NULL,
	[ThirdPartId] [nvarchar](255) NULL,
	[AccessToken] [nvarchar](255) NULL,
	[TokenExpires] [nvarchar](255) NULL,
PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[SocialNetworkId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[Albums]  WITH CHECK ADD  CONSTRAINT [FK_Albums_Artists] FOREIGN KEY([ArtistId])
REFERENCES [dbo].[Artists] ([ArtistId])
GO
ALTER TABLE [dbo].[Albums] CHECK CONSTRAINT [FK_Albums_Artists]
GO
ALTER TABLE [dbo].[PlaylistListeners]  WITH CHECK ADD  CONSTRAINT [FK_PlaylistListeners_Playlists] FOREIGN KEY([PlaylistId])
REFERENCES [dbo].[Playlists] ([PlaylistsId])
GO
ALTER TABLE [dbo].[PlaylistListeners] CHECK CONSTRAINT [FK_PlaylistListeners_Playlists]
GO
ALTER TABLE [dbo].[PlaylistListeners]  WITH CHECK ADD  CONSTRAINT [FK_PlaylistListeners_Users] FOREIGN KEY([ListenerId])
REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[PlaylistListeners] CHECK CONSTRAINT [FK_PlaylistListeners_Users]
GO
ALTER TABLE [dbo].[Playlists]  WITH CHECK ADD  CONSTRAINT [FK_Playlists_Users] FOREIGN KEY([CreatorId])
REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[Playlists] CHECK CONSTRAINT [FK_Playlists_Users]
GO
ALTER TABLE [dbo].[PlaylistTracks]  WITH CHECK ADD  CONSTRAINT [FK_PlaylistTracks_Playlists] FOREIGN KEY([PlaylistId])
REFERENCES [dbo].[Playlists] ([PlaylistsId])
GO
ALTER TABLE [dbo].[PlaylistTracks] CHECK CONSTRAINT [FK_PlaylistTracks_Playlists]
GO
ALTER TABLE [dbo].[PlaylistTracks]  WITH CHECK ADD  CONSTRAINT [FK_PlaylistTracks_Tracks] FOREIGN KEY([TrackId])
REFERENCES [dbo].[Tracks] ([TrackId])
GO
ALTER TABLE [dbo].[PlaylistTracks] CHECK CONSTRAINT [FK_PlaylistTracks_Tracks]
GO
ALTER TABLE [dbo].[Tracks]  WITH CHECK ADD  CONSTRAINT [FK_Tracks_Albums] FOREIGN KEY([AlbumId])
REFERENCES [dbo].[Albums] ([AlbumId])
GO
ALTER TABLE [dbo].[Tracks] CHECK CONSTRAINT [FK_Tracks_Albums]
GO
ALTER TABLE [dbo].[Tracks]  WITH CHECK ADD  CONSTRAINT [FK_Tracks_Artists] FOREIGN KEY([ArtistId])
REFERENCES [dbo].[Artists] ([ArtistId])
GO
ALTER TABLE [dbo].[Tracks] CHECK CONSTRAINT [FK_Tracks_Artists]
GO
ALTER TABLE [dbo].[Tracks]  WITH CHECK ADD  CONSTRAINT [FK_Tracks_Genres] FOREIGN KEY([GenreId])
REFERENCES [dbo].[Genres] ([GenreId])
GO
ALTER TABLE [dbo].[Tracks] CHECK CONSTRAINT [FK_Tracks_Genres]
GO
ALTER TABLE [dbo].[UserSocialNetworks]  WITH CHECK ADD  CONSTRAINT [FK85200F723CF999E8] FOREIGN KEY([SocialNetworkId])
REFERENCES [dbo].[SocialNetworks] ([Id])
GO
ALTER TABLE [dbo].[UserSocialNetworks] CHECK CONSTRAINT [FK85200F723CF999E8]
GO
ALTER TABLE [dbo].[UserSocialNetworks]  WITH CHECK ADD  CONSTRAINT [FK85200F7256DF715E] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[UserSocialNetworks] CHECK CONSTRAINT [FK85200F7256DF715E]
GO
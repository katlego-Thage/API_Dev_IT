USE [master]
GO
/****** Object:  Database [BookingDb]    Script Date: 2026/04/07 15:38:16 ******/
CREATE DATABASE [BookingDb]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'BookingDb', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL17.SQLEXPRESS\MSSQL\DATA\BookingDb.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'BookingDb_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL17.SQLEXPRESS\MSSQL\DATA\BookingDb_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [BookingDb].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [BookingDb] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [BookingDb] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [BookingDb] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [BookingDb] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [BookingDb] SET ARITHABORT OFF 
GO
ALTER DATABASE [BookingDb] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [BookingDb] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [BookingDb] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [BookingDb] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [BookingDb] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [BookingDb] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [BookingDb] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [BookingDb] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [BookingDb] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [BookingDb] SET  DISABLE_BROKER 
GO
ALTER DATABASE [BookingDb] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [BookingDb] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [BookingDb] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [BookingDb] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [BookingDb] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [BookingDb] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [BookingDb] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [BookingDb] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [BookingDb] SET  MULTI_USER 
GO
ALTER DATABASE [BookingDb] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [BookingDb] SET DB_CHAINING OFF 
GO
ALTER DATABASE [BookingDb] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [BookingDb] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [BookingDb] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [BookingDb] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
ALTER DATABASE [BookingDb] SET QUERY_STORE = ON
GO
ALTER DATABASE [BookingDb] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 1000, QUERY_CAPTURE_MODE = AUTO, SIZE_BASED_CLEANUP_MODE = AUTO, MAX_PLANS_PER_QUERY = 200, WAIT_STATS_CAPTURE_MODE = ON)
GO
USE [BookingDb]
GO
/****** Object:  Table [dbo].[__EFMigrationsHistory]    Script Date: 2026/04/07 15:38:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[__EFMigrationsHistory](
	[MigrationId] [nvarchar](150) NOT NULL,
	[ProductVersion] [nvarchar](32) NOT NULL,
 CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY CLUSTERED 
(
	[MigrationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Booking]    Script Date: 2026/04/07 15:38:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Booking](
	[BookingID] [int] IDENTITY(1,1) NOT NULL,
	[TenantID] [int] NOT NULL,
	[RoomID] [int] NOT NULL,
	[BookingDate] [date] NOT NULL,
	[CheckInDate] [date] NOT NULL,
	[CheckOutDate] [date] NOT NULL,
	[Status] [varchar](20) NULL,
	[UserId] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[BookingID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Payment]    Script Date: 2026/04/07 15:38:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Payment](
	[PaymentID] [int] IDENTITY(1,1) NOT NULL,
	[BookingID] [int] NOT NULL,
	[Amount] [decimal](10, 2) NOT NULL,
	[PaymentDate] [datetime] NULL,
	[PaymentMethod] [varchar](50) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[PaymentID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Role]    Script Date: 2026/04/07 15:38:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Role](
	[RoleID] [int] IDENTITY(1,1) NOT NULL,
	[RoleName] [varchar](50) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[RoleID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Room]    Script Date: 2026/04/07 15:38:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Room](
	[RoomID] [int] IDENTITY(1,1) NOT NULL,
	[RoomNumber] [varchar](10) NOT NULL,
	[RoomType] [varchar](50) NOT NULL,
	[PricePerNight] [decimal](10, 2) NOT NULL,
	[IsAvailable] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[RoomID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Tenant]    Script Date: 2026/04/07 15:38:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Tenant](
	[TenantID] [int] IDENTITY(1,1) NOT NULL,
	[FullName] [varchar](100) NOT NULL,
	[Phone] [varchar](20) NULL,
	[Email] [varchar](100) NULL,
	[Address] [varchar](255) NULL,
PRIMARY KEY CLUSTERED 
(
	[TenantID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[User]    Script Date: 2026/04/07 15:38:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[User](
	[UserID] [int] IDENTITY(1,1) NOT NULL,
	[Username] [varchar](50) NOT NULL,
	[PasswordHash] [varchar](255) NOT NULL,
	[Email] [varchar](100) NOT NULL,
	[RoleID] [int] NOT NULL,
	[CreatedAt] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[UserID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[Booking] ON 
GO
INSERT [dbo].[Booking] ([BookingID], [TenantID], [RoomID], [BookingDate], [CheckInDate], [CheckOutDate], [Status], [UserId]) VALUES (1, 1, 1, CAST(N'2026-03-14' AS Date), CAST(N'2026-03-20' AS Date), CAST(N'2026-03-25' AS Date), N'Confirmed', 7)
GO
INSERT [dbo].[Booking] ([BookingID], [TenantID], [RoomID], [BookingDate], [CheckInDate], [CheckOutDate], [Status], [UserId]) VALUES (2, 2, 3, CAST(N'2026-03-14' AS Date), CAST(N'2026-04-01' AS Date), CAST(N'2026-04-05' AS Date), N'Pending', NULL)
GO
SET IDENTITY_INSERT [dbo].[Booking] OFF
GO
SET IDENTITY_INSERT [dbo].[Payment] ON 
GO
INSERT [dbo].[Payment] ([PaymentID], [BookingID], [Amount], [PaymentDate], [PaymentMethod]) VALUES (1, 1, CAST(2500.00 AS Decimal(10, 2)), CAST(N'2026-03-14T15:29:47.027' AS DateTime), N'Credit Card')
GO
INSERT [dbo].[Payment] ([PaymentID], [BookingID], [Amount], [PaymentDate], [PaymentMethod]) VALUES (2, 2, CAST(3000.00 AS Decimal(10, 2)), CAST(N'2026-03-14T15:29:47.027' AS DateTime), N'Cash')
GO
SET IDENTITY_INSERT [dbo].[Payment] OFF
GO
SET IDENTITY_INSERT [dbo].[Role] ON 
GO
INSERT [dbo].[Role] ([RoleID], [RoleName]) VALUES (1, N'Admin')
GO
INSERT [dbo].[Role] ([RoleID], [RoleName]) VALUES (4, N'Customer')
GO
INSERT [dbo].[Role] ([RoleID], [RoleName]) VALUES (2, N'Manager')
GO
INSERT [dbo].[Role] ([RoleID], [RoleName]) VALUES (3, N'Receptionist')
GO
SET IDENTITY_INSERT [dbo].[Role] OFF
GO
SET IDENTITY_INSERT [dbo].[Room] ON 
GO
INSERT [dbo].[Room] ([RoomID], [RoomNumber], [RoomType], [PricePerNight], [IsAvailable]) VALUES (1, N'101', N'Single', CAST(500.00 AS Decimal(10, 2)), 1)
GO
INSERT [dbo].[Room] ([RoomID], [RoomNumber], [RoomType], [PricePerNight], [IsAvailable]) VALUES (2, N'102', N'Double', CAST(750.00 AS Decimal(10, 2)), 1)
GO
INSERT [dbo].[Room] ([RoomID], [RoomNumber], [RoomType], [PricePerNight], [IsAvailable]) VALUES (3, N'201', N'Suite', CAST(1500.00 AS Decimal(10, 2)), 1)
GO
SET IDENTITY_INSERT [dbo].[Room] OFF
GO
SET IDENTITY_INSERT [dbo].[Tenant] ON 
GO
INSERT [dbo].[Tenant] ([TenantID], [FullName], [Phone], [Email], [Address]) VALUES (1, N'John Doe', N'1234567890', N'john@example.com', N'123 Main St')
GO
INSERT [dbo].[Tenant] ([TenantID], [FullName], [Phone], [Email], [Address]) VALUES (2, N'Jane Smith', N'0987654321', N'jane@example.com', N'456 Oak Ave')
GO
INSERT [dbo].[Tenant] ([TenantID], [FullName], [Phone], [Email], [Address]) VALUES (4, N'Queen Letty', N'0713891130', N'queen@mail.com', N'36060 EXT 8, pretoria')
GO
SET IDENTITY_INSERT [dbo].[Tenant] OFF
GO
SET IDENTITY_INSERT [dbo].[User] ON 
GO
INSERT [dbo].[User] ([UserID], [Username], [PasswordHash], [Email], [RoleID], [CreatedAt]) VALUES (1, N'admin', N'hashedpassword123', N'admin@example.com', 1, CAST(N'2026-03-14T15:29:30.503' AS DateTime))
GO
INSERT [dbo].[User] ([UserID], [Username], [PasswordHash], [Email], [RoleID], [CreatedAt]) VALUES (2, N'manager1', N'hashed_password_456', N'manager@example.com', 2, CAST(N'2026-03-14T15:29:30.503' AS DateTime))
GO
INSERT [dbo].[User] ([UserID], [Username], [PasswordHash], [Email], [RoleID], [CreatedAt]) VALUES (3, N'reception1', N'hashed_password_789', N'reception@example.com', 3, CAST(N'2026-03-14T15:29:30.503' AS DateTime))
GO
INSERT [dbo].[User] ([UserID], [Username], [PasswordHash], [Email], [RoleID], [CreatedAt]) VALUES (4, N'customer1', N'hashed_password_abc', N'customer@example.com', 4, CAST(N'2026-03-14T15:29:30.503' AS DateTime))
GO
INSERT [dbo].[User] ([UserID], [Username], [PasswordHash], [Email], [RoleID], [CreatedAt]) VALUES (6, N'Sam', N'System.Collections.Generic.HashSet`1[System.Char]', N'sam@example.com', 1, CAST(N'2026-03-14T20:33:53.840' AS DateTime))
GO
INSERT [dbo].[User] ([UserID], [Username], [PasswordHash], [Email], [RoleID], [CreatedAt]) VALUES (7, N'kevin jonnes', N'AQAAAAIAAYagAAAAECvVyMbjFdwsttw95Z2oGm7MKFb0MKaqNHGBfeG4fLDKQ/HVFjALOLq141PYb6g9yw==', N'kevekj@mail.co.za', 4, CAST(N'2026-03-15T21:52:40.980' AS DateTime))
GO
SET IDENTITY_INSERT [dbo].[User] OFF
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__Role__8A2B616071363C5A]    Script Date: 2026/04/07 15:38:16 ******/
ALTER TABLE [dbo].[Role] ADD UNIQUE NONCLUSTERED 
(
	[RoleName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__Room__AE10E07A8A63F653]    Script Date: 2026/04/07 15:38:16 ******/
ALTER TABLE [dbo].[Room] ADD UNIQUE NONCLUSTERED 
(
	[RoomNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__Tenant__A9D10534663C1969]    Script Date: 2026/04/07 15:38:16 ******/
ALTER TABLE [dbo].[Tenant] ADD UNIQUE NONCLUSTERED 
(
	[Email] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__User__536C85E46758D748]    Script Date: 2026/04/07 15:38:16 ******/
ALTER TABLE [dbo].[User] ADD UNIQUE NONCLUSTERED 
(
	[Username] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__User__A9D105342C76971E]    Script Date: 2026/04/07 15:38:16 ******/
ALTER TABLE [dbo].[User] ADD UNIQUE NONCLUSTERED 
(
	[Email] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Booking] ADD  DEFAULT ('Pending') FOR [Status]
GO
ALTER TABLE [dbo].[Payment] ADD  DEFAULT (getdate()) FOR [PaymentDate]
GO
ALTER TABLE [dbo].[Room] ADD  DEFAULT ((1)) FOR [IsAvailable]
GO
ALTER TABLE [dbo].[User] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Booking]  WITH CHECK ADD FOREIGN KEY([RoomID])
REFERENCES [dbo].[Room] ([RoomID])
GO
ALTER TABLE [dbo].[Booking]  WITH CHECK ADD FOREIGN KEY([TenantID])
REFERENCES [dbo].[Tenant] ([TenantID])
GO
ALTER TABLE [dbo].[Payment]  WITH CHECK ADD FOREIGN KEY([BookingID])
REFERENCES [dbo].[Booking] ([BookingID])
GO
ALTER TABLE [dbo].[User]  WITH CHECK ADD FOREIGN KEY([RoleID])
REFERENCES [dbo].[Role] ([RoleID])
GO
USE [master]
GO
ALTER DATABASE [BookingDb] SET  READ_WRITE 
GO

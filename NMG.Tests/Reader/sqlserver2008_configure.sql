﻿/* Generated by SQL Server 2008 */

USE [NMG_TEST]
GO

/****** Object:  Table [dbo].[CATEGORIES]    Script Date: 05/17/2013 12:20:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CATEGORIES](
	[ID] [int] NOT NULL,
	[NAME] [nvarchar](255) NULL,
 CONSTRAINT [PK_CATEGORIES] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[STORES]    Script Date: 05/17/2013 12:20:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[STORES](
	[ID] [int] NOT NULL,
	[NAME] [nvarchar](255) NULL,
	[DESCRIPTION] [text] NULL,
 CONSTRAINT [PK_STORES] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

/****** Object:  Table [dbo].[PRODUCTS]    Script Date: 05/17/2013 12:20:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PRODUCTS](
	[ID] [int] NOT NULL,
	[NAME] [nvarchar](255) NOT NULL,
	[CATEGORY_ID] [int] NULL,
 CONSTRAINT [PK_PRODUCTS] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[INVENTORIES]    Script Date: 05/17/2013 12:20:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[INVENTORIES](
	[ID] [bigint] NOT NULL,
	[STORE_ID] [int] NOT NULL,
	[PRODUCT_ID] [int] NOT NULL,
	[QUANTITY] [numeric](16, 3) NOT NULL,
	[ADDED_AT] [datetime] NULL,
	[MODIFIED_AT] [datetime] NULL,
	[PRICE] [money] NULL,
 CONSTRAINT [PK_INVENTORIES] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Object:  ForeignKey [FK_CATEGORIES_CATEGORIES]    Script Date: 05/17/2013 12:20:04 ******/
ALTER TABLE [dbo].[CATEGORIES]  WITH CHECK ADD  CONSTRAINT [FK_CATEGORIES_CATEGORIES] FOREIGN KEY([ID])
REFERENCES [dbo].[CATEGORIES] ([ID])
GO
ALTER TABLE [dbo].[CATEGORIES] CHECK CONSTRAINT [FK_CATEGORIES_CATEGORIES]
GO

/****** Object:  ForeignKey [FK_INVENTORIES_PRODUCTS]    Script Date: 05/17/2013 12:20:04 ******/
ALTER TABLE [dbo].[INVENTORIES]  WITH CHECK ADD  CONSTRAINT [FK_INVENTORIES_PRODUCTS] FOREIGN KEY([PRODUCT_ID])
REFERENCES [dbo].[PRODUCTS] ([ID])
GO
ALTER TABLE [dbo].[INVENTORIES] CHECK CONSTRAINT [FK_INVENTORIES_PRODUCTS]
GO

/****** Object:  ForeignKey [FK_INVENTORIES_STORES]    Script Date: 05/17/2013 12:20:04 ******/
ALTER TABLE [dbo].[INVENTORIES]  WITH CHECK ADD  CONSTRAINT [FK_INVENTORIES_STORES] FOREIGN KEY([STORE_ID])
REFERENCES [dbo].[STORES] ([ID])
GO
ALTER TABLE [dbo].[INVENTORIES] CHECK CONSTRAINT [FK_INVENTORIES_STORES]
GO

/****** Object:  ForeignKey [PROD_CATEG_FK]    Script Date: 05/17/2013 12:20:04 ******/
ALTER TABLE [dbo].[PRODUCTS]  WITH CHECK ADD  CONSTRAINT [PROD_CATEG_FK] FOREIGN KEY([CATEGORY_ID])
REFERENCES [dbo].[CATEGORIES] ([ID])
GO
ALTER TABLE [dbo].[PRODUCTS] CHECK CONSTRAINT [PROD_CATEG_FK]
GO
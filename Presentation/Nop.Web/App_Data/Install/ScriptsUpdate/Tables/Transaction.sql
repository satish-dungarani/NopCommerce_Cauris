USE [nop430_trans_hafid]
GO

/****** Object:  Table [dbo].[Transaction]    Script Date: 1/24/2022 9:47:47 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Transaction](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TransactionNumber] [uniqueidentifier] NOT NULL,
	[ModeratorIndentity] [nvarchar](max) NULL,
	[ContractSignatureDate] [datetime2](7) NOT NULL,
	[QuotationId] [int] NOT NULL,
	[CaurisModeratorId] [int] NULL,
	[Amount] [decimal](18, 2) NOT NULL,
	[Tax] [decimal](18, 2) NOT NULL,
	[PaymentTerm] [nvarchar](max) NULL,
	[Inspection] [bit] NOT NULL,
	[Insurance] [bit] NOT NULL,
	[PartialShipping] [bit] NOT NULL,
	[DeliveryTermId] [int] NOT NULL,
	[LoadingOrigin] [nvarchar](max) NULL,
	[Destination] [nvarchar](max) NULL,
	[LastDateOfShipment] [datetime2](7) NOT NULL,
	[DocumentsRequirement] [nvarchar](max) NULL,
	[TransactionStatusId] [int] NULL,
	[VendorDocumentId] [int] NULL,
	[CaurisDocumentId] [int] NULL,
	[CustomerDocumentId] [int] NULL,
	[PaymentProofDocumentId] [int] NULL,
	[CreatedOnUtc] [datetime2](7) NOT NULL,
	[UpdatedOnUtc] [datetime2](7) NULL,
 CONSTRAINT [PK_Transaction] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO



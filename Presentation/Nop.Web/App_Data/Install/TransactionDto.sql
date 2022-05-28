/****** Object:  Table [dbo].[TransactionDto]    Script Date: 1/2/2022 5:37:50 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [Transaction](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TransactionNumber] [uniqueidentifier] NOT NULL,
	ModeratorIndentity [nvarchar](max) NULL,
	ContractSignatureDate  [datetime2](7) NOT NULL,
	[QuotationId] [int] NOT NULL,
	[CaurisModeratorId] [int] NULL,
	[Amount] [decimal](18, 2) NOT NULL,
	Tax [decimal](18, 2) NOT NULL,
	PaymentTerm [nvarchar](max) NULL,
	Inspection [bit] NOT NULL,
	Insurance [bit] NOT NULL,
	PartialShipping [bit] NOT NULL,
	DeliveryTermId [int] NOT NULL,
	LoadingOrigin  [nvarchar](max) NULL,
	Destination  [nvarchar](max) NULL,
	LastDateOfShipment [datetime2](7) NOT NULL,
	DocumentsRequirement[nvarchar](max) NULL,
	TransactionStatusId [int] NULL,
	VendorDocumentId [int] NULL,
	[CaurisDocumentId] [int] NULL,
	[CustomerDocumentId] [int] NULL,
	[PaymentProofDocumentId] [int] NULL,
	[CreatedOnUtc] [datetime2](7) NOT NULL,
	[UpdatedOnUtc] [datetime2](7) NULL,
 CONSTRAINT [PK_Transaction] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].TransactionComment(
	[Id] [int] IDENTITY(1,1) NOT NULL,
	TransactionId [int] NOT NULL,
	ModeratorId [int] NOT NULL,
	ModeratorComment [nvarchar](max) NULL,
	CreatedDate  [datetime2](7) NOT NULL
 CONSTRAINT [PK_TransactionComment] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

CREATE TABLE [dbo].[Notification](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EntityId] [int] NULL,
	[EntityName] [nvarchar](50) NULL,
	[CreatedBy] [int] NULL,
	[CreatedFor] [int] NULL,
	[IsRead] [bit] NULL,
	[Description] [nvarchar](100) NULL,
	[CreatedDate] [datetime] NULL,
 CONSTRAINT [PK_Notification] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

INSERT INTO `cauris_twt`.`permissionrecord`(Name,SystemName,Category) VALUES ('Admin Area.Manage Cauris Transaction','ManageCaurisTransaction','Cauris');
go

----- my sql version ---

/* SET ANSI_NULLS ON */
 

/* SET QUOTED_IDENTIFIER ON */
 


CREATE TABLE TransactionComment(
	`Id` int AUTO_INCREMENT NOT NULL,
	`TransactionId` int NOT NULL,
	`ModeratorId` Longtext NOT NULL,
	`ModeratorComment` Longtext NULL,
	`CreatedDate` Datetime(6) NOT NULL,
 CONSTRAINT `PK_TransactionComment` PRIMARY KEY 
(
	`Id` ASC
) 
);

CREATE TABLE Notification(
	`Id` int AUTO_INCREMENT NOT NULL,
	`EntityId` int NULL,
	`EntityName` nvarchar(50) NULL,
	`CreatedBy` int NULL,
	`CreatedFor` int NULL,
	`IsRead` Tinyint NULL,
	`Description` nvarchar(100) NULL,
	`CreatedDate` datetime(3) NULL,
 CONSTRAINT `PK_Notification` PRIMARY KEY 
(
	`Id` ASC
) 
);

CREATE TABLE Transaction(
	`Id` int AUTO_INCREMENT NOT NULL,
	`TransactionNumber` Char(36) NOT NULL,
	`ModeratorIndentity` Longtext NULL,
	`ContractSignatureDate` Datetime(6) NOT NULL,
	`QuotationId` int NOT NULL,
	`CaurisModeratorId` int NULL,
	`Amount` decimal(18, 2) NOT NULL,
	`Tax` decimal(18, 2) NOT NULL,
	`PaymentTerm` Longtext NULL,
	`Inspection` Tinyint NOT NULL,
	`Insurance` Tinyint NOT NULL,
	`PartialShipping` Tinyint NOT NULL,
	`DeliveryTermId` int NOT NULL,
	`LoadingOrigin` Longtext NULL,
	`Destination` Longtext NULL,
	`LastDateOfShipment` Datetime(6) NOT NULL,
	`DocumentsRequirement` Longtext NULL,
	`TransactionStatusId` int NULL,
	`VendorDocumentId` int NULL,
	`CaurisDocumentId` int NULL,
	`CustomerDocumentId` int NULL,
	`PaymentProofDocumentId` int NULL,
	`CreatedOnUtc` Datetime(6) NOT NULL,
	`UpdatedOnUtc` Datetime(6) NULL,
 CONSTRAINT `PK_Transaction` PRIMARY KEY 
(
	`Id` ASC
) 
);
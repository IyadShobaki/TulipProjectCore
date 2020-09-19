CREATE TABLE [dbo].[Product]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [ProductName] NVARCHAR(50) NOT NULL, 
    [Description] NVARCHAR(4000) NOT NULL, 
    [ProductImage] NVARCHAR(250) NOT NULL, 
    [RetailPrice] MONEY NOT NULL, 
    [QuantityInStock] INT NOT NULL, 
    [CreatedDate] DATETIME2 NOT NULL DEFAULT getutcdate(), 
    [UpdatedDate] DATETIME2 NOT NULL DEFAULT getutcdate(), 
    [IsTaxable] BIT NOT NULL DEFAULT 1, 
    [Sex] BIT NOT NULL 
)

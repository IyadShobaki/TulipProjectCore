CREATE TABLE [dbo].[Order]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [UserId] NVARCHAR(128) NOT NULL, 
    [OrderDate] DATETIME2 NOT NULL DEFAULT getutcdate(), 
    [SubTotal] MONEY NOT NULL, 
    [Tax] MONEY NOT NULL, 
    [Total] MONEY NOT NULL, 
    CONSTRAINT [FK_Order_User] FOREIGN KEY (UserId) REFERENCES [User](Id)
)

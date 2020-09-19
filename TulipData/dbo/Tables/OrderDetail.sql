CREATE TABLE [dbo].[OrderDetail]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [OrderId] INT NOT NULL, 
    [ProductId] INT NOT NULL, 
    [Quantity] INT NOT NULL DEFAULT 1,
    [PurchasePrice] MONEY NOT NULL, 
    [Tax] MONEY NOT NULL DEFAULT 0, 
    CONSTRAINT [FK_OrderDetail_Order] FOREIGN KEY (OrderId) REFERENCES [Order](Id), 
    CONSTRAINT [FK_OrderDetail_Product] FOREIGN KEY (ProductId) REFERENCES Product(Id)
    
)

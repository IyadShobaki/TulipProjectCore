CREATE PROCEDURE [dbo].[spInventory_GetAll]
AS
begin 

	set nocount on;

	select i.[ProductId], i.[Quantity], i.[PurchasePrice], i.[PurchaseDate]
			, p.ProductName, p.QuantityInStock, p.RetailPrice
	from dbo.Inventory i
	inner join dbo.Product p on i.ProductId = p.Id;
end
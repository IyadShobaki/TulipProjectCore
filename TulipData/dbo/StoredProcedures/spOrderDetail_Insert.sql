CREATE PROCEDURE [dbo].[spOrderDetail_Insert]
	@OrderId int,
	@ProductId int,
	@Quantity int,
	@PurchasePrice money,
	@Tax money
AS
begin
	set nocount on;

	insert into [dbo].[OrderDetail] (OrderId, ProductId, Quantity, PurchasePrice, Tax)
	values (@OrderId, @ProductId, @Quantity, @PurchasePrice, @Tax);
	
end

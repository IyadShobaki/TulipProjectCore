CREATE PROCEDURE [dbo].[spProduct_UpdateQuantity]
	@Id int,
	@QuantityInStock int
AS
begin
	set nocount on;

	update dbo.Product
	set QuantityInStock = @QuantityInStock, UpdatedDate = getutcdate()
	where Id = @Id;

end

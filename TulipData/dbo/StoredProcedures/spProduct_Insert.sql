CREATE PROCEDURE [dbo].[spProduct_Insert]
	@id int output,
	@ProductName nvarchar(50),
	@Description nvarchar(4000),
	@ProductImage nvarchar(250),
	@RetailPrice money,
	@QuantityInStock int,
	@IsTaxable bit,
	@Sex bit
AS
begin
	set nocount on;

	insert into dbo.Product( ProductName, [Description], ProductImage, RetailPrice, QuantityInStock,IsTaxable, Sex)
	values (@ProductName, @Description, @ProductImage, @RetailPrice, @QuantityInStock,@IsTaxable, @Sex);

	select @id = SCOPE_IDENTITY();
end			
			
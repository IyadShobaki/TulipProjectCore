CREATE PROCEDURE [dbo].[spOrder_Insert]
	@id int output,
	@UserId nvarchar(128),
	@SubTotal money,
	@Tax money,
	@Total money

AS
begin
	set nocount on;

	insert into dbo.[Order] (UserId, SubTotal, Tax, Total)
	values (@UserId, @SubTotal, @Tax, @Total);

	
	select @id = SCOPE_IDENTITY();
end

CREATE PROCEDURE [dbo].[spDeleteOrderById]
	@Id int

as
begin
	set nocount on;

	delete from dbo.[Order]
	where Id = @Id;
end

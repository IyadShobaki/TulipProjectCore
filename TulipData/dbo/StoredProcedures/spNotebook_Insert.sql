CREATE PROCEDURE [dbo].[spNotebook_Insert]
	@Id int output,
	@UserId nvarchar(128),
	@Name nvarchar(50)
AS
begin
	set nocount on;
	
	insert into Notebook (UserId, [Name])
	values (@UserId, @Name);

	select @Id = SCOPE_IDENTITY();
end

CREATE PROCEDURE [dbo].[spNotebooks_GetAll]
	@UserId nvarchar(128)
AS
begin
	set nocount on;
	
	SELECT [Id], [UserId], [Name]
	from Notebook
	where UserId = @UserId;
end
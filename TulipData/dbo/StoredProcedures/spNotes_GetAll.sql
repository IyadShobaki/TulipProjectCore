CREATE PROCEDURE [dbo].[spNotes_GetAll]
AS
begin
	set nocount on;
	
	SELECT [Id], [NotebookId], [Title], [FileLocation], [CreatedDate], [UpdatedDate]
	from Note
end

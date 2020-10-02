CREATE PROCEDURE [dbo].[spNote_Insert]
	@Id int output,
	@NotebookId int,
	@Title nvarchar(100),
	@FileLocation nvarchar(500),
	@CreatedDate datetime2(7),
	@UpdatedDate datetime2(7)
As
begin
	set nocount on;

	insert into Note (NotebookId, Title, FileLocation, CreatedDate, UpdatedDate)
	values (@NotebookId, @Title, @FileLocation, @CreatedDate, @UpdatedDate)

	select @Id = SCOPE_IDENTITY();
end

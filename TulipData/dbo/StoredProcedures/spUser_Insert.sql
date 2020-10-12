CREATE PROCEDURE [dbo].[spUser_Insert]
	@Id nvarchar(128),
	@FirstName nvarchar(50),
	@LastName nvarchar(50),
	@EmailAddress nvarchar(256),
	@CreatedDate datetime2(7)
AS
begin
	
	set nocount on;
	insert into [dbo].[User] (Id, FirstName, LastName, EmailAddress, CreatedDate)
	values (@Id, @FirstName, @LastName, @EmailAddress, @CreatedDate)

end

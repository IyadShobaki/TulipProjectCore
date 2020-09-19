CREATE PROCEDURE [dbo].[spOrder_OrdersReport]

AS
begin
	set nocount on;

	select [o].[OrderDate], [o].[SubTotal], [o].[Tax], [o].[Total], u.FirstName, u.LastName, u.EmailAddress
	from dbo.[Order] o
	inner join dbo.[User] u on o.UserId = u.Id
end

﻿CREATE PROCEDURE [dbo].[spProduct_GetAll]
AS
begin
	set nocount on;

	select [Id], [ProductName], [Description], [ProductImage], [RetailPrice], [QuantityInStock], [IsTaxable], [Sex]
	from dbo.Product
	order by ProductName;

end

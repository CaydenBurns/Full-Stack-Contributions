


CREATE proc [dbo].[StripeCustomers_Insert]
								@UserId int
								,@CustomerId nvarchar(100)
								,@Id int OUTPUT





AS

/*
Declare @Id int = 0;
Declare @UserId int = 1232
       ,@CustomerId nvarchar(100) = 'cust_0000123'

EXEC  dbo.StripeCustomers_Insert
				@UserId
				,@CustomerId
				,@Id OUTPUT


				Select *
				FROM dbo.StripeCustomers


*/


BEGIN

INSERT INTO [dbo].[StripeCustomers]
           ([UserId]
           ,[CustomerId])
     VALUES
           (@UserId
           ,@CustomerId)

		   set @Id = SCOPE_IDENTITY()




END


GO

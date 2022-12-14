USE [MiVet]
GO
/****** Object:  StoredProcedure [dbo].[StripeSubscription_SelectAll]    Script Date: 10/10/2022 6:06:16 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author: Cayden Burns
-- Create date: 09/30/2022
-- Description: Select all rows from the StripSubscription table
-- Code Reviewer: Jared Dayoub

-- MODIFIED BY: author
-- MODIFIED DATE:12/1/2020
-- Code Reviewer:
-- Note:
-- =============================================
CREATE PROC [dbo].[StripeSubscription_SelectAll]



AS
/*
EXECUTE dbo.StripeSubscription_SelectAll
*/

BEGIN

SELECT  [Id],
		[productId],
	    [Name],
	    [priceId]

FROM dbo.StripeSubscription
END

GO

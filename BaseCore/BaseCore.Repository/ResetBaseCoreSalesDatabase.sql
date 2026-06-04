IF DB_ID(N'BaseCoreSales') IS NOT NULL
BEGIN
    ALTER DATABASE [BaseCoreSales] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE [BaseCoreSales];
END

CREATE DATABASE [BaseCoreSales];
GO

USE [BaseCoreSales];
GO

PRINT 'BaseCoreSales database dropped and recreated. Run the API again to apply EF migrations.';

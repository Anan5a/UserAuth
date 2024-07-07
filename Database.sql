-- First create the database using SQL Server Management Studio


USE [UserAuth];  

-- Create table PasswordReset
CREATE TABLE PasswordReset (
    Id bigint IDENTITY(1,1) NOT NULL ,
    UserId bigint NOT NULL,
    Token nvarchar(256) NOT NULL,
    ExpiresAt datetime2 NOT NULL,
    CONSTRAINT PK_PasswordReset PRIMARY KEY (Id),
);

-- Create table Users
CREATE TABLE Users (
    Id bigint IDENTITY(1,1) NOT NULL,
    Name nvarchar(50) NOT NULL,
    Email nvarchar(100) NOT NULL,
    Password nvarchar(256) NOT NULL,
    CreatedAt datetime2 NOT NULL,
    ModifiedAt datetime2 NULL,
    CONSTRAINT PK_Users PRIMARY KEY (Id)
);

CREATE TABLE [dbo].[Images] (
    [Id]          UNIQUEIDENTIFIER NOT NULL,
    [Name]        NVARCHAR (50)    NOT NULL,
    [Description] NVARCHAR (500)   NULL,
    [Picture]     NVARCHAR (MAX)   NOT NULL,
    [DateCreated] DATETIME         DEFAULT (getdate()) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);



CREATE TABLE [dbo].[Note]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [NotebookId] INT NOT NULL, 
    [Title] NVARCHAR(100) NOT NULL, 
    [FileLocation] NVARCHAR(500) NOT NULL, 
    [CreatedDate] DATETIME2 NOT NULL , 
    [UpdatedDate] DATETIME2 NOT NULL , 
    CONSTRAINT [FK_Note_Notebook] FOREIGN KEY (NotebookId) REFERENCES Notebook(Id)
)

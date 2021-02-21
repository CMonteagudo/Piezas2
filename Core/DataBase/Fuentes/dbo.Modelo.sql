CREATE TABLE [dbo].[Modelo] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [Nombre]      NVARCHAR (50)  NOT NULL,
    [Marca]       INT            NULL,
    [Descripcion] NVARCHAR (MAX) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_Modelo]
    ON [dbo].[Modelo]([Nombre] ASC);


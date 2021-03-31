CREATE TABLE [dbo].[Categoria] (
    [Id]          INT            NOT NULL,
    [Nombre]      NVARCHAR (50)  NOT NULL,
    [Logo]        NVARCHAR (50)  NULL,
    [Descripcion] NVARCHAR (MAX) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_Categoria]
    ON [dbo].[Categoria]([Nombre] ASC);


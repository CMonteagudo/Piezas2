CREATE TABLE [dbo].[Fabricante] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [Nombre]      NVARCHAR (50)  NOT NULL,
    [Logo]        NVARCHAR (50)  NULL,
    [Descripcion] NVARCHAR (MAX) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_Fabricante]
    ON [dbo].[Fabricante]([Nombre] ASC);


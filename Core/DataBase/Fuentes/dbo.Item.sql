CREATE TABLE [dbo].[Item] (
    [Id]          INT             IDENTITY (1, 1) NOT NULL,
    [Fabricante]  INT             NOT NULL,
    [Categoria]   INT             NOT NULL,
    [Nombre]      NVARCHAR (80)   NOT NULL,
    [Codigo]      NVARCHAR (50)   NOT NULL,
    [Foto]        NVARCHAR (50)   NULL,
    [Precio]      DECIMAL (18, 2) NULL,
    [Descripcion] NVARCHAR (MAX)  NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Fabricante] FOREIGN KEY ([Fabricante]) REFERENCES [dbo].[Fabricante] ([Id]) ON UPDATE CASCADE,
    CONSTRAINT [FK_Categoria] FOREIGN KEY ([Categoria]) REFERENCES [dbo].[Categoria] ([Id]) ON UPDATE CASCADE
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Identificador del Item', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Item', @level2type = N'COLUMN', @level2name = N'Id';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Codigo del fabricante', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Item', @level2type = N'COLUMN', @level2name = N'Fabricante';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Código del conjunto que pertence el Item', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Item', @level2type = N'COLUMN', @level2name = N'Categoria';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Nombre del Item', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Item', @level2type = N'COLUMN', @level2name = N'Nombre';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Código del Item', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Item', @level2type = N'COLUMN', @level2name = N'Codigo';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Nombre de la imagen con la foto', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Item', @level2type = N'COLUMN', @level2name = N'Foto';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Precio del Item en euros', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Item', @level2type = N'COLUMN', @level2name = N'Precio';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Descripción del Item', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Item', @level2type = N'COLUMN', @level2name = N'Descripcion';


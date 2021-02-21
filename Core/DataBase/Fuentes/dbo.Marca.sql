CREATE TABLE [dbo].[Marca] (
    [Id]          INT           IDENTITY (1, 1) NOT NULL,
    [Nombre]      NVARCHAR (50) NOT NULL,
    [Logo]        NVARCHAR (50) NULL,
    [Descripcion] TEXT          NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_Marca]
    ON [dbo].[Marca]([Nombre] ASC);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Codigo que identfica la marca', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Marca', @level2type = N'COLUMN', @level2name = N'Id';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Nombre de la marca', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Marca', @level2type = N'COLUMN', @level2name = N'Nombre';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Nombre de la imagen con el logotipo de la marca', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Marca', @level2type = N'COLUMN', @level2name = N'Logo';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Descripción sobre la marca', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Marca', @level2type = N'COLUMN', @level2name = N'Descripcion';


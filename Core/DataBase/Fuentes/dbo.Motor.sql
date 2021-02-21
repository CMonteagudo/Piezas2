CREATE TABLE [dbo].[Motor] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [Nombre]      NVARCHAR (50)  NOT NULL,
    [Descripcion] NVARCHAR (MAX) NULL,
    [Combustible] INT            DEFAULT ((0)) NOT NULL,
    [Capacidad]   INT            DEFAULT ((0)) NOT NULL,
    [Potencia]    INT            DEFAULT ((0)) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_Motor]
    ON [dbo].[Motor]([Nombre] ASC);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Tipo de combustible 0- Gasolina 1-Diesel', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Motor', @level2type = N'COLUMN', @level2name = N'Combustible';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Capacidad en Litros', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Motor', @level2type = N'COLUMN', @level2name = N'Capacidad';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Potencia en CV', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Motor', @level2type = N'COLUMN', @level2name = N'Potencia';


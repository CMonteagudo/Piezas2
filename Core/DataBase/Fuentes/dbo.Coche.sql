CREATE TABLE [dbo].[Coche] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [Marca]       INT            NOT NULL,
    [Modelo]      INT            NOT NULL,
    [Motor]       INT            NOT NULL,
    [Caja]        NVARCHAR (50)  NULL,
    [Carroceria]  NVARCHAR (50)  NULL,
    [Foto]        NVARCHAR (80)  NULL,
    [Descripcion] NVARCHAR (MAX) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Marca] FOREIGN KEY ([Marca]) REFERENCES [dbo].[Marca] ([Id]) ON UPDATE CASCADE,
    CONSTRAINT [FK_Modelo] FOREIGN KEY ([Modelo]) REFERENCES [dbo].[Modelo] ([Id]) ON UPDATE CASCADE,
    CONSTRAINT [FK_Motor] FOREIGN KEY ([Motor]) REFERENCES [dbo].[Motor] ([Id]) ON UPDATE CASCADE
);


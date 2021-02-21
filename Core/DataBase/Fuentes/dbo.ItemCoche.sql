CREATE TABLE [dbo].[ItemCoche] (
    [IdItem]  INT NOT NULL,
    [IdCoche] INT NOT NULL,
    CONSTRAINT [PK_ItemCoche] PRIMARY KEY CLUSTERED ([IdItem] ASC, [IdCoche] ASC),
    CONSTRAINT [FK_Coche] FOREIGN KEY ([IdCoche]) REFERENCES [dbo].[Coche] ([Id]),
    CONSTRAINT [FK_Item] FOREIGN KEY ([IdItem]) REFERENCES [dbo].[Item] ([Id])
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Identificador del Item', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ItemCoche', @level2type = N'COLUMN', @level2name = N'IdItem';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Identificador del coche donde se usa', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ItemCoche', @level2type = N'COLUMN', @level2name = N'IdCoche';


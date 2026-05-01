using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerce.API.Migrations
{
    /// <inheritdoc />
    public partial class RebuildCartItemsWithGuidKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                IF OBJECT_ID(N'[CartItems]', N'U') IS NOT NULL
                   AND EXISTS (
                       SELECT 1
                       FROM sys.columns c
                       INNER JOIN sys.types t ON c.user_type_id = t.user_type_id
                       WHERE c.[object_id] = OBJECT_ID(N'[CartItems]', N'U')
                           AND c.[name] IN (N'Id', N'CartId', N'ProductId')
                           AND t.[name] <> N'uniqueidentifier'
                   )
                BEGIN
                    DROP TABLE [CartItems];
                END
                """);

            migrationBuilder.Sql("""
                IF OBJECT_ID(N'[CartItems]', N'U') IS NULL
                BEGIN
                    CREATE TABLE [CartItems] (
                        [Id] uniqueidentifier NOT NULL,
                        [CartId] uniqueidentifier NOT NULL,
                        [ProductId] uniqueidentifier NOT NULL,
                        [Quantity] int NOT NULL,
                        [UnitPrice] decimal(18,2) NOT NULL,
                        CONSTRAINT [PK_CartItems] PRIMARY KEY ([Id]),
                        CONSTRAINT [FK_CartItems_Carts_CartId] FOREIGN KEY ([CartId])
                            REFERENCES [Carts] ([Id]) ON DELETE CASCADE,
                        CONSTRAINT [FK_CartItems_Products_ProductId] FOREIGN KEY ([ProductId])
                            REFERENCES [Products] ([Id]) ON DELETE NO ACTION
                    );
                END
                """);

            migrationBuilder.Sql("""
                IF NOT EXISTS (
                    SELECT 1
                    FROM sys.indexes
                    WHERE [name] = N'IX_CartItems_CartId'
                        AND [object_id] = OBJECT_ID(N'[CartItems]', N'U')
                )
                BEGIN
                    CREATE INDEX [IX_CartItems_CartId] ON [CartItems] ([CartId]);
                END
                """);

            migrationBuilder.Sql("""
                IF NOT EXISTS (
                    SELECT 1
                    FROM sys.indexes
                    WHERE [name] = N'IX_CartItems_CartId_ProductId'
                        AND [object_id] = OBJECT_ID(N'[CartItems]', N'U')
                )
                BEGIN
                    CREATE UNIQUE INDEX [IX_CartItems_CartId_ProductId] ON [CartItems] ([CartId], [ProductId]);
                END
                """);

            migrationBuilder.Sql("""
                IF NOT EXISTS (
                    SELECT 1
                    FROM sys.indexes
                    WHERE [name] = N'IX_CartItems_ProductId'
                        AND [object_id] = OBJECT_ID(N'[CartItems]', N'U')
                )
                BEGIN
                    CREATE INDEX [IX_CartItems_ProductId] ON [CartItems] ([ProductId]);
                END
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                IF OBJECT_ID(N'[CartItems]', N'U') IS NOT NULL
                BEGIN
                    DROP TABLE [CartItems];
                END
                """);
        }
    }
}

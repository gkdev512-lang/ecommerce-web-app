using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerce.API.Migrations
{
    /// <inheritdoc />
    public partial class AddCartTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                IF OBJECT_ID(N'[Carts]', N'U') IS NULL
                BEGIN
                    CREATE TABLE [Carts] (
                        [Id] uniqueidentifier NOT NULL,
                        [UserId] uniqueidentifier NOT NULL,
                        [CreatedAt] datetime2 NOT NULL,
                        CONSTRAINT [PK_Carts] PRIMARY KEY ([Id]),
                        CONSTRAINT [FK_Carts_Users_UserId] FOREIGN KEY ([UserId])
                            REFERENCES [Users] ([Id]) ON DELETE CASCADE
                    );
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
                    WHERE [name] = N'IX_Carts_UserId'
                        AND [object_id] = OBJECT_ID(N'[Carts]', N'U')
                )
                BEGIN
                    CREATE UNIQUE INDEX [IX_Carts_UserId] ON [Carts] ([UserId]);
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

            migrationBuilder.Sql("""
                IF OBJECT_ID(N'[Carts]', N'U') IS NOT NULL
                BEGIN
                    DROP TABLE [Carts];
                END
                """);
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerce.API.Migrations
{
    /// <inheritdoc />
    public partial class AddOrders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                IF OBJECT_ID(N'[OrderItems]', N'U') IS NOT NULL
                   AND EXISTS (
                       SELECT 1
                       FROM sys.columns c
                       INNER JOIN sys.types t ON c.user_type_id = t.user_type_id
                       WHERE c.[object_id] = OBJECT_ID(N'[OrderItems]', N'U')
                           AND c.[name] IN (N'Id', N'OrderId', N'ProductId')
                           AND t.[name] <> N'uniqueidentifier'
                   )
                BEGIN
                    DROP TABLE [OrderItems];
                END
                """);

            migrationBuilder.Sql("""
                IF OBJECT_ID(N'[Orders]', N'U') IS NOT NULL
                   AND EXISTS (
                       SELECT 1
                       FROM sys.columns c
                       INNER JOIN sys.types t ON c.user_type_id = t.user_type_id
                       WHERE c.[object_id] = OBJECT_ID(N'[Orders]', N'U')
                           AND c.[name] IN (N'Id', N'UserId')
                           AND t.[name] <> N'uniqueidentifier'
                   )
                BEGIN
                    IF OBJECT_ID(N'[OrderItems]', N'U') IS NOT NULL
                    BEGIN
                        DROP TABLE [OrderItems];
                    END

                    DROP TABLE [Orders];
                END
                """);

            migrationBuilder.Sql("""
                IF OBJECT_ID(N'[Orders]', N'U') IS NULL
                BEGIN
                    CREATE TABLE [Orders] (
                        [Id] uniqueidentifier NOT NULL,
                        [UserId] uniqueidentifier NOT NULL,
                        [OrderDate] datetime2 NOT NULL,
                        [TotalAmount] decimal(18,2) NOT NULL,
                        [Status] nvarchar(50) NOT NULL CONSTRAINT [DF_Orders_Status] DEFAULT N'Pending',
                        [ShippingAddress] nvarchar(1000) NOT NULL,
                        CONSTRAINT [PK_Orders] PRIMARY KEY ([Id]),
                        CONSTRAINT [FK_Orders_Users_UserId] FOREIGN KEY ([UserId])
                            REFERENCES [Users] ([Id]) ON DELETE CASCADE
                    );
                END
                """);

            migrationBuilder.Sql("""
                IF OBJECT_ID(N'[Orders]', N'U') IS NOT NULL
                   AND COL_LENGTH(N'[Orders]', N'OrderDate') IS NULL
                BEGIN
                    ALTER TABLE [Orders]
                    ADD [OrderDate] datetime2 NOT NULL CONSTRAINT [DF_Orders_OrderDate] DEFAULT SYSUTCDATETIME();

                    ALTER TABLE [Orders] DROP CONSTRAINT [DF_Orders_OrderDate];
                END
                """);

            migrationBuilder.Sql("""
                IF OBJECT_ID(N'[Orders]', N'U') IS NOT NULL
                   AND COL_LENGTH(N'[Orders]', N'TotalAmount') IS NULL
                BEGIN
                    ALTER TABLE [Orders]
                    ADD [TotalAmount] decimal(18,2) NOT NULL CONSTRAINT [DF_Orders_TotalAmount] DEFAULT 0;

                    ALTER TABLE [Orders] DROP CONSTRAINT [DF_Orders_TotalAmount];
                END
                """);

            migrationBuilder.Sql("""
                IF OBJECT_ID(N'[Orders]', N'U') IS NOT NULL
                   AND COL_LENGTH(N'[Orders]', N'Status') IS NULL
                BEGIN
                    ALTER TABLE [Orders]
                    ADD [Status] nvarchar(50) NOT NULL CONSTRAINT [DF_Orders_Status] DEFAULT N'Pending';
                END
                """);

            migrationBuilder.Sql("""
                IF OBJECT_ID(N'[Orders]', N'U') IS NOT NULL
                   AND COL_LENGTH(N'[Orders]', N'ShippingAddress') IS NULL
                BEGIN
                    ALTER TABLE [Orders]
                    ADD [ShippingAddress] nvarchar(1000) NOT NULL CONSTRAINT [DF_Orders_ShippingAddress] DEFAULT N'';

                    ALTER TABLE [Orders] DROP CONSTRAINT [DF_Orders_ShippingAddress];
                END
                """);

            migrationBuilder.Sql("""
                IF OBJECT_ID(N'[OrderItems]', N'U') IS NULL
                BEGIN
                    CREATE TABLE [OrderItems] (
                        [Id] uniqueidentifier NOT NULL,
                        [OrderId] uniqueidentifier NOT NULL,
                        [ProductId] uniqueidentifier NOT NULL,
                        [Quantity] int NOT NULL,
                        [UnitPrice] decimal(18,2) NOT NULL,
                        CONSTRAINT [PK_OrderItems] PRIMARY KEY ([Id]),
                        CONSTRAINT [FK_OrderItems_Orders_OrderId] FOREIGN KEY ([OrderId])
                            REFERENCES [Orders] ([Id]) ON DELETE CASCADE,
                        CONSTRAINT [FK_OrderItems_Products_ProductId] FOREIGN KEY ([ProductId])
                            REFERENCES [Products] ([Id]) ON DELETE NO ACTION
                    );
                END
                """);

            migrationBuilder.Sql("""
                IF OBJECT_ID(N'[OrderItems]', N'U') IS NOT NULL
                   AND COL_LENGTH(N'[OrderItems]', N'UnitPrice') IS NULL
                BEGIN
                    ALTER TABLE [OrderItems]
                    ADD [UnitPrice] decimal(18,2) NOT NULL CONSTRAINT [DF_OrderItems_UnitPrice] DEFAULT 0;

                    EXEC(N'
                        UPDATE oi
                        SET [UnitPrice] = p.[Price]
                        FROM [OrderItems] oi
                        INNER JOIN [Products] p ON p.[Id] = oi.[ProductId]
                        WHERE oi.[UnitPrice] = 0
                    ');

                    ALTER TABLE [OrderItems] DROP CONSTRAINT [DF_OrderItems_UnitPrice];
                END
                """);

            migrationBuilder.Sql("""
                IF NOT EXISTS (
                    SELECT 1
                    FROM sys.indexes
                    WHERE [name] = N'IX_OrderItems_OrderId'
                        AND [object_id] = OBJECT_ID(N'[OrderItems]', N'U')
                )
                BEGIN
                    CREATE INDEX [IX_OrderItems_OrderId] ON [OrderItems] ([OrderId]);
                END
                """);

            migrationBuilder.Sql("""
                IF NOT EXISTS (
                    SELECT 1
                    FROM sys.indexes
                    WHERE [name] = N'IX_OrderItems_ProductId'
                        AND [object_id] = OBJECT_ID(N'[OrderItems]', N'U')
                )
                BEGIN
                    CREATE INDEX [IX_OrderItems_ProductId] ON [OrderItems] ([ProductId]);
                END
                """);

            migrationBuilder.Sql("""
                IF NOT EXISTS (
                    SELECT 1
                    FROM sys.indexes
                    WHERE [name] = N'IX_Orders_OrderDate'
                        AND [object_id] = OBJECT_ID(N'[Orders]', N'U')
                )
                BEGIN
                    CREATE INDEX [IX_Orders_OrderDate] ON [Orders] ([OrderDate]);
                END
                """);

            migrationBuilder.Sql("""
                IF NOT EXISTS (
                    SELECT 1
                    FROM sys.indexes
                    WHERE [name] = N'IX_Orders_UserId'
                        AND [object_id] = OBJECT_ID(N'[Orders]', N'U')
                )
                BEGIN
                    CREATE INDEX [IX_Orders_UserId] ON [Orders] ([UserId]);
                END
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                IF OBJECT_ID(N'[OrderItems]', N'U') IS NOT NULL
                BEGIN
                    DROP TABLE [OrderItems];
                END
                """);

            migrationBuilder.Sql("""
                IF OBJECT_ID(N'[Orders]', N'U') IS NOT NULL
                BEGIN
                    DROP TABLE [Orders];
                END
                """);
        }
    }
}

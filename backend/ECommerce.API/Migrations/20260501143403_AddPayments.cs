using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerce.API.Migrations
{
    /// <inheritdoc />
    public partial class AddPayments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                IF OBJECT_ID(N'[Payments]', N'U') IS NOT NULL
                   AND EXISTS (
                       SELECT 1
                       FROM sys.columns c
                       INNER JOIN sys.types t ON c.user_type_id = t.user_type_id
                       WHERE c.[object_id] = OBJECT_ID(N'[Payments]', N'U')
                           AND c.[name] IN (N'Id', N'OrderId')
                           AND t.[name] <> N'uniqueidentifier'
                   )
                BEGIN
                    DROP TABLE [Payments];
                END
                """);

            migrationBuilder.Sql("""
                IF OBJECT_ID(N'[Payments]', N'U') IS NULL
                BEGIN
                    CREATE TABLE [Payments] (
                        [Id] uniqueidentifier NOT NULL,
                        [OrderId] uniqueidentifier NOT NULL,
                        [Amount] decimal(18,2) NOT NULL,
                        [PaymentMethod] nvarchar(100) NOT NULL,
                        [Status] nvarchar(50) NOT NULL CONSTRAINT [DF_Payments_Status] DEFAULT N'Pending',
                        [TransactionId] nvarchar(100) NOT NULL,
                        [PaidAt] datetime2 NULL,
                        CONSTRAINT [PK_Payments] PRIMARY KEY ([Id]),
                        CONSTRAINT [FK_Payments_Orders_OrderId] FOREIGN KEY ([OrderId])
                            REFERENCES [Orders] ([Id]) ON DELETE CASCADE
                    );
                END
                """);

            migrationBuilder.Sql("""
                IF OBJECT_ID(N'[Payments]', N'U') IS NOT NULL
                   AND COL_LENGTH(N'[Payments]', N'Amount') IS NULL
                BEGIN
                    ALTER TABLE [Payments]
                    ADD [Amount] decimal(18,2) NOT NULL CONSTRAINT [DF_Payments_Amount] DEFAULT 0;

                    ALTER TABLE [Payments] DROP CONSTRAINT [DF_Payments_Amount];
                END
                """);

            migrationBuilder.Sql("""
                IF OBJECT_ID(N'[Payments]', N'U') IS NOT NULL
                   AND COL_LENGTH(N'[Payments]', N'PaymentMethod') IS NULL
                BEGIN
                    ALTER TABLE [Payments]
                    ADD [PaymentMethod] nvarchar(100) NOT NULL CONSTRAINT [DF_Payments_PaymentMethod] DEFAULT N'';

                    ALTER TABLE [Payments] DROP CONSTRAINT [DF_Payments_PaymentMethod];
                END
                """);

            migrationBuilder.Sql("""
                IF OBJECT_ID(N'[Payments]', N'U') IS NOT NULL
                   AND COL_LENGTH(N'[Payments]', N'Status') IS NULL
                BEGIN
                    ALTER TABLE [Payments]
                    ADD [Status] nvarchar(50) NOT NULL CONSTRAINT [DF_Payments_Status] DEFAULT N'Pending';
                END
                """);

            migrationBuilder.Sql("""
                IF OBJECT_ID(N'[Payments]', N'U') IS NOT NULL
                   AND COL_LENGTH(N'[Payments]', N'TransactionId') IS NULL
                BEGIN
                    ALTER TABLE [Payments]
                    ADD [TransactionId] nvarchar(100) NOT NULL CONSTRAINT [DF_Payments_TransactionId] DEFAULT N'';

                    ALTER TABLE [Payments] DROP CONSTRAINT [DF_Payments_TransactionId];
                END
                """);

            migrationBuilder.Sql("""
                IF OBJECT_ID(N'[Payments]', N'U') IS NOT NULL
                   AND COL_LENGTH(N'[Payments]', N'PaidAt') IS NULL
                BEGIN
                    ALTER TABLE [Payments] ADD [PaidAt] datetime2 NULL;
                END
                """);

            migrationBuilder.Sql("""
                IF NOT EXISTS (
                    SELECT 1
                    FROM sys.indexes
                    WHERE [name] = N'IX_Payments_OrderId'
                        AND [object_id] = OBJECT_ID(N'[Payments]', N'U')
                )
                BEGIN
                    CREATE UNIQUE INDEX [IX_Payments_OrderId] ON [Payments] ([OrderId]);
                END
                """);

            migrationBuilder.Sql("""
                IF NOT EXISTS (
                    SELECT 1
                    FROM sys.indexes
                    WHERE [name] = N'IX_Payments_TransactionId'
                        AND [object_id] = OBJECT_ID(N'[Payments]', N'U')
                )
                BEGIN
                    CREATE UNIQUE INDEX [IX_Payments_TransactionId] ON [Payments] ([TransactionId]);
                END
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                IF OBJECT_ID(N'[Payments]', N'U') IS NOT NULL
                BEGIN
                    DROP TABLE [Payments];
                END
                """);
        }
    }
}

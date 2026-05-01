using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerce.API.Migrations
{
    /// <inheritdoc />
    public partial class AddProducts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                IF OBJECT_ID(N'[Products]', N'U') IS NULL
                BEGIN
                    CREATE TABLE [Products] (
                        [Id] uniqueidentifier NOT NULL,
                        [Name] nvarchar(150) NOT NULL,
                        [Description] nvarchar(1000) NOT NULL,
                        [Price] decimal(18,2) NOT NULL,
                        [StockQuantity] int NOT NULL,
                        [CategoryId] int NOT NULL,
                        [ImageUrl] nvarchar(1000) NOT NULL,
                        [CreatedAt] datetime2 NOT NULL,
                        CONSTRAINT [PK_Products] PRIMARY KEY ([Id]),
                        CONSTRAINT [FK_Products_Categories_CategoryId] FOREIGN KEY ([CategoryId])
                            REFERENCES [Categories] ([Id]) ON DELETE NO ACTION
                    );
                END
                """);

            migrationBuilder.Sql("""
                IF NOT EXISTS (
                    SELECT 1
                    FROM sys.indexes
                    WHERE [name] = N'IX_Products_CategoryId'
                        AND [object_id] = OBJECT_ID(N'[Products]', N'U')
                )
                BEGIN
                    CREATE INDEX [IX_Products_CategoryId] ON [Products] ([CategoryId]);
                END
                """);

            migrationBuilder.Sql("""
                IF NOT EXISTS (
                    SELECT 1
                    FROM sys.indexes
                    WHERE [name] = N'IX_Products_Name'
                        AND [object_id] = OBJECT_ID(N'[Products]', N'U')
                )
                BEGIN
                    CREATE INDEX [IX_Products_Name] ON [Products] ([Name]);
                END
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                IF OBJECT_ID(N'[Products]', N'U') IS NOT NULL
                BEGIN
                    DROP TABLE [Products];
                END
                """);
        }
    }
}

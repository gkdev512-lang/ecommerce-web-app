using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerce.API.Migrations
{
    /// <inheritdoc />
    public partial class AddMissingProductColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                IF COL_LENGTH(N'[Products]', N'StockQuantity') IS NULL
                BEGIN
                    ALTER TABLE [Products]
                    ADD [StockQuantity] int NOT NULL CONSTRAINT [DF_Products_StockQuantity] DEFAULT 0;

                    ALTER TABLE [Products]
                    DROP CONSTRAINT [DF_Products_StockQuantity];
                END
                """);

            migrationBuilder.Sql("""
                IF COL_LENGTH(N'[Products]', N'ImageUrl') IS NULL
                BEGIN
                    ALTER TABLE [Products]
                    ADD [ImageUrl] nvarchar(1000) NOT NULL CONSTRAINT [DF_Products_ImageUrl] DEFAULT N'';

                    ALTER TABLE [Products]
                    DROP CONSTRAINT [DF_Products_ImageUrl];
                END
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                IF COL_LENGTH(N'[Products]', N'ImageUrl') IS NOT NULL
                BEGIN
                    ALTER TABLE [Products] DROP COLUMN [ImageUrl];
                END
                """);

            migrationBuilder.Sql("""
                IF COL_LENGTH(N'[Products]', N'StockQuantity') IS NOT NULL
                BEGIN
                    ALTER TABLE [Products] DROP COLUMN [StockQuantity];
                END
                """);
        }
    }
}

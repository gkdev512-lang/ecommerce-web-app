using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerce.API.Migrations
{
    /// <inheritdoc />
    public partial class AddMissingCartItemColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                IF OBJECT_ID(N'[CartItems]', N'U') IS NOT NULL
                   AND COL_LENGTH(N'[CartItems]', N'UnitPrice') IS NULL
                BEGIN
                    ALTER TABLE [CartItems]
                    ADD [UnitPrice] decimal(18,2) NOT NULL CONSTRAINT [DF_CartItems_UnitPrice] DEFAULT 0;

                    EXEC(N'
                        UPDATE ci
                        SET [UnitPrice] = p.[Price]
                        FROM [CartItems] ci
                        INNER JOIN [Products] p ON p.[Id] = ci.[ProductId]
                        WHERE ci.[UnitPrice] = 0
                    ');

                    ALTER TABLE [CartItems]
                    DROP CONSTRAINT [DF_CartItems_UnitPrice];
                END
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                IF OBJECT_ID(N'[CartItems]', N'U') IS NOT NULL
                   AND COL_LENGTH(N'[CartItems]', N'UnitPrice') IS NOT NULL
                BEGIN
                    ALTER TABLE [CartItems] DROP COLUMN [UnitPrice];
                END
                """);
        }
    }
}

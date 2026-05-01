using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerce.API.Migrations
{
    /// <inheritdoc />
    public partial class AddMissingCategoryColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                IF COL_LENGTH(N'[Categories]', N'Description') IS NULL
                BEGIN
                    ALTER TABLE [Categories]
                    ADD [Description] nvarchar(500) NOT NULL CONSTRAINT [DF_Categories_Description] DEFAULT N'';

                    ALTER TABLE [Categories]
                    DROP CONSTRAINT [DF_Categories_Description];
                END
                """);

            migrationBuilder.Sql("""
                IF COL_LENGTH(N'[Categories]', N'CreatedAt') IS NULL
                BEGIN
                    ALTER TABLE [Categories]
                    ADD [CreatedAt] datetime2 NOT NULL CONSTRAINT [DF_Categories_CreatedAt] DEFAULT SYSUTCDATETIME();

                    ALTER TABLE [Categories]
                    DROP CONSTRAINT [DF_Categories_CreatedAt];
                END
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                IF COL_LENGTH(N'[Categories]', N'CreatedAt') IS NOT NULL
                BEGIN
                    ALTER TABLE [Categories] DROP COLUMN [CreatedAt];
                END
                """);

            migrationBuilder.Sql("""
                IF COL_LENGTH(N'[Categories]', N'Description') IS NOT NULL
                BEGIN
                    ALTER TABLE [Categories] DROP COLUMN [Description];
                END
                """);
        }
    }
}

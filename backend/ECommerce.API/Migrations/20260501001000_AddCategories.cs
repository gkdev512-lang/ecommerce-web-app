using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerce.API.Migrations
{
    /// <inheritdoc />
    public partial class AddCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                IF OBJECT_ID(N'[Categories]', N'U') IS NULL
                BEGIN
                    CREATE TABLE [Categories] (
                        [Id] int NOT NULL IDENTITY(1,1),
                        [Name] nvarchar(100) NOT NULL,
                        [Description] nvarchar(500) NOT NULL,
                        [CreatedAt] datetime2 NOT NULL,
                        CONSTRAINT [PK_Categories] PRIMARY KEY ([Id])
                    );
                END
                """);

            migrationBuilder.Sql("""
                IF NOT EXISTS (
                    SELECT 1
                    FROM sys.indexes
                    WHERE [name] = N'IX_Categories_Name'
                        AND [object_id] = OBJECT_ID(N'[Categories]', N'U')
                )
                BEGIN
                    CREATE UNIQUE INDEX [IX_Categories_Name] ON [Categories] ([Name]);
                END
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                IF OBJECT_ID(N'[Categories]', N'U') IS NOT NULL
                BEGIN
                    DROP TABLE [Categories];
                END
                """);
        }
    }
}

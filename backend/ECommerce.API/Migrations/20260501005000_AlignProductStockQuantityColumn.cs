using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerce.API.Migrations
{
    /// <inheritdoc />
    public partial class AlignProductStockQuantityColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                IF COL_LENGTH(N'[Products]', N'StockQuantity') IS NULL
                   AND COL_LENGTH(N'[Products]', N'Stock') IS NOT NULL
                BEGIN
                    EXEC sp_rename N'[Products].[Stock]', N'StockQuantity', N'COLUMN';
                END
                """);

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
                IF COL_LENGTH(N'[Products]', N'Stock') IS NOT NULL
                   AND COL_LENGTH(N'[Products]', N'StockQuantity') IS NOT NULL
                BEGIN
                    EXEC(N'UPDATE [Products] SET [StockQuantity] = [Stock] WHERE [StockQuantity] = 0');

                    DECLARE @dropConstraintsSql nvarchar(max) = N'';

                    SELECT @dropConstraintsSql = @dropConstraintsSql
                        + N'ALTER TABLE [Products] DROP CONSTRAINT [' + dc.[name] + N'];'
                    FROM sys.default_constraints dc
                    INNER JOIN sys.columns c
                        ON c.[object_id] = dc.parent_object_id
                        AND c.column_id = dc.parent_column_id
                    WHERE dc.parent_object_id = OBJECT_ID(N'[Products]', N'U')
                        AND c.[name] = N'Stock';

                    SELECT @dropConstraintsSql = @dropConstraintsSql
                        + N'ALTER TABLE [Products] DROP CONSTRAINT [' + cc.[name] + N'];'
                    FROM sys.check_constraints cc
                    WHERE cc.parent_object_id = OBJECT_ID(N'[Products]', N'U')
                        AND cc.[definition] LIKE N'%[[]Stock]%';

                    IF @dropConstraintsSql <> N''
                    BEGIN
                        EXEC sp_executesql @dropConstraintsSql;
                    END

                    ALTER TABLE [Products] DROP COLUMN [Stock];
                END
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                IF COL_LENGTH(N'[Products]', N'Stock') IS NULL
                   AND COL_LENGTH(N'[Products]', N'StockQuantity') IS NOT NULL
                BEGIN
                    EXEC sp_rename N'[Products].[StockQuantity]', N'Stock', N'COLUMN';
                END
                """);
        }
    }
}

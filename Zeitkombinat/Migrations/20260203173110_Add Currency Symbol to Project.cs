using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zeitkombinat.Migrations
{
    /// <inheritdoc />
    public partial class AddCurrencySymboltoProject : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CurrencySymbol",
                table: "Projects",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrencySymbol",
                table: "Projects");
        }
    }
}

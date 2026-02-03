using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zeitkombinat.Migrations
{
    /// <inheritdoc />
    public partial class IsDoneonTaskItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDone",
                table: "Tasks",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDone",
                table: "Tasks");
        }
    }
}

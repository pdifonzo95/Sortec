using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sortec.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLegajoInUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Legajo",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Legajo",
                table: "Users");
        }
    }
}

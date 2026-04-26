using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddStatMatch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PlayerOneHealth",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "PlayerTwoHealth",
                table: "Matches");

            migrationBuilder.AddColumn<string>(
                name: "PlayerOneStats",
                table: "Matches",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.AddColumn<string>(
                name: "PlayerTwoStats",
                table: "Matches",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PlayerOneStats",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "PlayerTwoStats",
                table: "Matches");

            migrationBuilder.AddColumn<int>(
                name: "PlayerOneHealth",
                table: "Matches",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PlayerTwoHealth",
                table: "Matches",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}

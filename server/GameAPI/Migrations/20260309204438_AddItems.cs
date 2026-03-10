using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Weapon");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WeaponType",
                table: "WeaponType");

            migrationBuilder.DropColumn(
                name: "DefaultDamage",
                table: "WeaponType");

            migrationBuilder.RenameTable(
                name: "WeaponType",
                newName: "ItemTypes");

            migrationBuilder.RenameColumn(
                name: "ImageRef",
                table: "ItemTypes",
                newName: "Category");

            migrationBuilder.AddColumn<int>(
                name: "Damage",
                table: "ItemTypes",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "ItemTypes",
                type: "TEXT",
                maxLength: 8,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ItemTypes",
                table: "ItemTypes",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Items",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    InventoryType = table.Column<string>(type: "TEXT", nullable: false),
                    Position = table.Column<int>(type: "INTEGER", nullable: true),
                    Level = table.Column<int>(type: "INTEGER", nullable: false),
                    Health = table.Column<int>(type: "INTEGER", nullable: false),
                    ItemTypeId = table.Column<Guid>(type: "TEXT", nullable: false),
                    GameUserId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Items_GameUsers_GameUserId",
                        column: x => x.GameUserId,
                        principalTable: "GameUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Items_ItemTypes_ItemTypeId",
                        column: x => x.ItemTypeId,
                        principalTable: "ItemTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Items_GameUserId",
                table: "Items",
                column: "GameUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_ItemTypeId",
                table: "Items",
                column: "ItemTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Items");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ItemTypes",
                table: "ItemTypes");

            migrationBuilder.DropColumn(
                name: "Damage",
                table: "ItemTypes");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "ItemTypes");

            migrationBuilder.RenameTable(
                name: "ItemTypes",
                newName: "WeaponType");

            migrationBuilder.RenameColumn(
                name: "Category",
                table: "WeaponType",
                newName: "ImageRef");

            migrationBuilder.AddColumn<int>(
                name: "DefaultDamage",
                table: "WeaponType",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_WeaponType",
                table: "WeaponType",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Weapon",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    WeaponTypeId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Health = table.Column<int>(type: "INTEGER", nullable: false),
                    Level = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Weapon", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Weapon_WeaponType_WeaponTypeId",
                        column: x => x.WeaponTypeId,
                        principalTable: "WeaponType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Weapon_WeaponTypeId",
                table: "Weapon",
                column: "WeaponTypeId");
        }
    }
}

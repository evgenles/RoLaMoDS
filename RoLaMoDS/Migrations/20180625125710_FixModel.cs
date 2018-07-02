using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RoLaMoDS.Migrations
{
    public partial class FixModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Images_ModelsNNDB_ModelNNId",
                table: "Images");

            migrationBuilder.DropForeignKey(
                name: "FK_ModelsNNDB_AspNetUsers_UserId",
                table: "ModelsNNDB");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ModelsNNDB",
                table: "ModelsNNDB");

            migrationBuilder.RenameTable(
                name: "ModelsNNDB",
                newName: "ModelsNN");

            migrationBuilder.RenameIndex(
                name: "IX_ModelsNNDB_UserId",
                table: "ModelsNN",
                newName: "IX_ModelsNN_UserId");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "ModelsNN",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "URL",
                table: "ModelsNN",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ModelsNN",
                table: "ModelsNN",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "ModelsNNClasses",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    NumberClass = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    ModelId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModelsNNClasses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ModelsNNClasses_ModelsNN_ModelId",
                        column: x => x.ModelId,
                        principalTable: "ModelsNN",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ModelsNNClasses_ModelId",
                table: "ModelsNNClasses",
                column: "ModelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Images_ModelsNN_ModelNNId",
                table: "Images",
                column: "ModelNNId",
                principalTable: "ModelsNN",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ModelsNN_AspNetUsers_UserId",
                table: "ModelsNN",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Images_ModelsNN_ModelNNId",
                table: "Images");

            migrationBuilder.DropForeignKey(
                name: "FK_ModelsNN_AspNetUsers_UserId",
                table: "ModelsNN");

            migrationBuilder.DropTable(
                name: "ModelsNNClasses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ModelsNN",
                table: "ModelsNN");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "ModelsNN");

            migrationBuilder.DropColumn(
                name: "URL",
                table: "ModelsNN");

            migrationBuilder.RenameTable(
                name: "ModelsNN",
                newName: "ModelsNNDB");

            migrationBuilder.RenameIndex(
                name: "IX_ModelsNN_UserId",
                table: "ModelsNNDB",
                newName: "IX_ModelsNNDB_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ModelsNNDB",
                table: "ModelsNNDB",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Images_ModelsNNDB_ModelNNId",
                table: "Images",
                column: "ModelNNId",
                principalTable: "ModelsNNDB",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ModelsNNDB_AspNetUsers_UserId",
                table: "ModelsNNDB",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

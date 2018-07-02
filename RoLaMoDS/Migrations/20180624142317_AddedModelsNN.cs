using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RoLaMoDS.Migrations
{
    public partial class AddedModelsNN : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ModelNNId",
                table: "Images",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ModelsNNDB",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    IsPublished = table.Column<bool>(nullable: false),
                    UserId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModelsNNDB", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ModelsNNDB_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Images_ModelNNId",
                table: "Images",
                column: "ModelNNId");

            migrationBuilder.CreateIndex(
                name: "IX_ModelsNNDB_UserId",
                table: "ModelsNNDB",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Images_ModelsNNDB_ModelNNId",
                table: "Images",
                column: "ModelNNId",
                principalTable: "ModelsNNDB",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Images_ModelsNNDB_ModelNNId",
                table: "Images");

            migrationBuilder.DropTable(
                name: "ModelsNNDB");

            migrationBuilder.DropIndex(
                name: "IX_Images_ModelNNId",
                table: "Images");

            migrationBuilder.DropColumn(
                name: "ModelNNId",
                table: "Images");
        }
    }
}

using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace TestUsers.Data.Migrations
{
    public partial class ClasseAventureAdd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AventureID",
                table: "Utilisateurs",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Aventures",
                columns: table => new
                {
                    AventureID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ImageUrl = table.Column<string>(nullable: true),
                    NomAventure = table.Column<string>(nullable: true),
                    Vote = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Aventures", x => x.AventureID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Utilisateurs_AventureID",
                table: "Utilisateurs",
                column: "AventureID");

            migrationBuilder.AddForeignKey(
                name: "FK_Utilisateurs_Aventures_AventureID",
                table: "Utilisateurs",
                column: "AventureID",
                principalTable: "Aventures",
                principalColumn: "AventureID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Utilisateurs_Aventures_AventureID",
                table: "Utilisateurs");

            migrationBuilder.DropTable(
                name: "Aventures");

            migrationBuilder.DropIndex(
                name: "IX_Utilisateurs_AventureID",
                table: "Utilisateurs");

            migrationBuilder.DropColumn(
                name: "AventureID",
                table: "Utilisateurs");
        }
    }
}

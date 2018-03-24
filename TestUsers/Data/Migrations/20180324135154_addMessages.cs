using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace TestUsers.Data.Migrations
{
    public partial class addMessages : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LesMessagesDesAventures",
                columns: table => new
                {
                    MessageAventureID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AventureID1 = table.Column<int>(nullable: true),
                    ChoixDirectionIdMessageNumero1 = table.Column<int>(nullable: true),
                    ChoixDirectionIdMessageNumero2 = table.Column<int>(nullable: true),
                    ChoixDirectionIdMessageNumero3 = table.Column<int>(nullable: true),
                    ContenuMessage = table.Column<string>(nullable: true),
                    ImageUrl = table.Column<string>(nullable: true),
                    TitreMessage = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LesMessagesDesAventures", x => x.MessageAventureID);
                    table.ForeignKey(
                        name: "FK_LesMessagesDesAventures_Aventures_AventureID1",
                        column: x => x.AventureID1,
                        principalTable: "Aventures",
                        principalColumn: "AventureID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LesMessagesDesAventures_AventureID1",
                table: "LesMessagesDesAventures",
                column: "AventureID1");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LesMessagesDesAventures");
        }
    }
}

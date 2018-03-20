using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace TestUsers.Data.Migrations
{
    public partial class changeAvatarLogic : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvatarImage",
                table: "Utilisateurs");

            migrationBuilder.AddColumn<string>(
                name: "UrlAvatarImage",
                table: "Utilisateurs",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UrlAvatarImage",
                table: "Utilisateurs");

            migrationBuilder.AddColumn<byte[]>(
                name: "AvatarImage",
                table: "Utilisateurs",
                nullable: true);
        }
    }
}

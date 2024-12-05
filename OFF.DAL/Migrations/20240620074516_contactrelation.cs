using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OFF.DAL.Migrations
{
    public partial class contactrelation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ContactEntries",
                table: "ContactEntries");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "ContactEntries");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "ContactEntries");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "ContactEntries");

            migrationBuilder.RenameTable(
                name: "ContactEntries",
                newName: "Contacts");

            migrationBuilder.RenameColumn(
                name: "Query",
                table: "Contacts",
                newName: "Message");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfMessage",
                table: "Contacts",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Contacts",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Contacts",
                table: "Contacts",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Contacts_UserId",
                table: "Contacts",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Contacts_AspNetUsers_UserId",
                table: "Contacts",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contacts_AspNetUsers_UserId",
                table: "Contacts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Contacts",
                table: "Contacts");

            migrationBuilder.DropIndex(
                name: "IX_Contacts_UserId",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "DateOfMessage",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Contacts");

            migrationBuilder.RenameTable(
                name: "Contacts",
                newName: "ContactEntries");

            migrationBuilder.RenameColumn(
                name: "Message",
                table: "ContactEntries",
                newName: "Query");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "ContactEntries",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "ContactEntries",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "ContactEntries",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ContactEntries",
                table: "ContactEntries",
                column: "Id");
        }
    }
}

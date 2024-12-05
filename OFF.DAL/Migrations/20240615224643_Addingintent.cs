using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OFF.DAL.Migrations
{
    public partial class Addingintent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TransactionId",
                table: "Payments",
                newName: "PaymentIntentId");

            migrationBuilder.AddColumn<string>(
                name: "ClientSecret",
                table: "Subscriptions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentIntentedId",
                table: "Subscriptions",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClientSecret",
                table: "Subscriptions");

            migrationBuilder.DropColumn(
                name: "PaymentIntentedId",
                table: "Subscriptions");

            migrationBuilder.RenameColumn(
                name: "PaymentIntentId",
                table: "Payments",
                newName: "TransactionId");
        }
    }
}

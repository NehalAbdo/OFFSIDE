using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OFF.DAL.Migrations
{
    public partial class dropmethoddd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PaymentIntentedId",
                table: "Subscriptions",
                newName: "PaymentIntentId");

            migrationBuilder.AddColumn<int>(
                name: "SubscribtionStatus",
                table: "Subscriptions",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SubscribtionStatus",
                table: "Subscriptions");

            migrationBuilder.RenameColumn(
                name: "PaymentIntentId",
                table: "Subscriptions",
                newName: "PaymentIntentedId");
        }
    }
}

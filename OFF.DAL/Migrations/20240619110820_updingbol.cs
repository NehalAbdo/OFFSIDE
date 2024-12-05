using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OFF.DAL.Migrations
{
    public partial class updingbol : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SubscribtionStatus",
                table: "Subscriptions");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SubscribtionStatus",
                table: "Subscriptions",
                type: "int",
                nullable: true);
        }
    }
}

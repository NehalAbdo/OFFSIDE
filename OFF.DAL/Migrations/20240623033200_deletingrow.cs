using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OFF.DAL.Migrations
{
    public partial class deletingrow : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContractsMade",
                table: "Agents");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContractsMade",
                table: "Agents",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}

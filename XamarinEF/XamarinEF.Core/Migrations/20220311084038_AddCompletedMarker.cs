using Microsoft.EntityFrameworkCore.Migrations;

namespace XamarinEF.Core.Migrations
{
    public partial class AddCompletedMarker : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Completed",
                table: "TodoItems",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Completed",
                table: "TodoItems");
        }
    }
}

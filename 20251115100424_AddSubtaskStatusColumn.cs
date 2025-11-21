using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Master.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddSubtaskStatusColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Subtasks",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Subtasks");
        }
    }
}

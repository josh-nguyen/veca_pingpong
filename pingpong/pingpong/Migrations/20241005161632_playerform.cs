using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace pingpong.Migrations
{
    /// <inheritdoc />
    public partial class playerform : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Form",
                table: "Players",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Form",
                table: "Players");
        }
    }
}

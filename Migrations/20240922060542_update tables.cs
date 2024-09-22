using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InstaHub.Migrations
{
    /// <inheritdoc />
    public partial class updatetables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "SentimentAnalysis",
                table: "Tickets",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.AddColumn<float>(
                name: "DegreeOfSentiment",
                table: "Tickets",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<int>(
                name: "CountOfTickets",
                table: "Admins",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DegreeOfSentiment",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "CountOfTickets",
                table: "Admins");

            migrationBuilder.AlterColumn<float>(
                name: "SentimentAnalysis",
                table: "Tickets",
                type: "real",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}

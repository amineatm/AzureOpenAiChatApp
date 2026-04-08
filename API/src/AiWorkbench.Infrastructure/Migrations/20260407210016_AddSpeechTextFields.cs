using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AiWorkbench.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSpeechTextFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "DisplayName",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GeneratedText",
                table: "SpeechSessions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TranscribedText",
                table: "SpeechSessions",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GeneratedText",
                table: "SpeechSessions");

            migrationBuilder.DropColumn(
                name: "TranscribedText",
                table: "SpeechSessions");

            migrationBuilder.AlterColumn<string>(
                name: "DisplayName",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AiWorkbench.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(320)", maxLength: 320, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GeneralChatMessages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GeneralChatMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GeneralChatMessages_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ImageGenerations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Prompt = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BlobUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Model = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImageGenerations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ImageGenerations_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SpeechSessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SessionType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SourceBlobUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResultBlobUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpeechSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SpeechSessions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UploadedDocuments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SizeBytes = table.Column<long>(type: "bigint", nullable: false),
                    BlobUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UploadedDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UploadedDocuments_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DocumentEmbeddings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DocumentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChunkIndex = table.Column<int>(type: "int", nullable: false),
                    EmbeddingJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TextSnippet = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentEmbeddings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentEmbeddings_UploadedDocuments_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "UploadedDocuments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RagChatMessages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SourceDocumentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RagChatMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RagChatMessages_UploadedDocuments_SourceDocumentId",
                        column: x => x.SourceDocumentId,
                        principalTable: "UploadedDocuments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RagChatMessages_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DocumentEmbeddings_DocumentId",
                table: "DocumentEmbeddings",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_GeneralChatMessages_UserId",
                table: "GeneralChatMessages",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ImageGenerations_UserId",
                table: "ImageGenerations",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RagChatMessages_SourceDocumentId",
                table: "RagChatMessages",
                column: "SourceDocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_RagChatMessages_UserId",
                table: "RagChatMessages",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SpeechSessions_UserId",
                table: "SpeechSessions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UploadedDocuments_UserId",
                table: "UploadedDocuments",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DocumentEmbeddings");

            migrationBuilder.DropTable(
                name: "GeneralChatMessages");

            migrationBuilder.DropTable(
                name: "ImageGenerations");

            migrationBuilder.DropTable(
                name: "RagChatMessages");

            migrationBuilder.DropTable(
                name: "SpeechSessions");

            migrationBuilder.DropTable(
                name: "UploadedDocuments");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}

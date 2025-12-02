using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LinguaNews.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ArticleSnapshots",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ArticleUrl = table.Column<string>(type: "TEXT", maxLength: 2048, nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    SourceName = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    ImageUrl = table.Column<string>(type: "TEXT", maxLength: 2048, nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    Content = table.Column<string>(type: "TEXT", nullable: false),
                    FetchedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticleSnapshots", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Translations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LanguageCode = table.Column<string>(type: "TEXT", maxLength: 5, nullable: false),
                    TranslatedText = table.Column<string>(type: "TEXT", nullable: false),
                    ArticleSnapshotId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Translations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Translations_ArticleSnapshots_ArticleSnapshotId",
                        column: x => x.ArticleSnapshotId,
                        principalTable: "ArticleSnapshots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Translations_ArticleSnapshotId",
                table: "Translations",
                column: "ArticleSnapshotId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Translations");

            migrationBuilder.DropTable(
                name: "ArticleSnapshots");
        }
    }
}

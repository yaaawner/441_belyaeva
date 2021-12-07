using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Server.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Results",
                columns: table => new
                {
                    ResultsId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Type = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Results", x => x.ResultsId);
                });

            migrationBuilder.CreateTable(
                name: "DetectedObject",
                columns: table => new
                {
                    DetectedObjectId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Path = table.Column<string>(type: "TEXT", nullable: true),
                    x1 = table.Column<float>(type: "REAL", nullable: false),
                    y1 = table.Column<float>(type: "REAL", nullable: false),
                    x2 = table.Column<float>(type: "REAL", nullable: false),
                    y2 = table.Column<float>(type: "REAL", nullable: false),
                    BitmapImage = table.Column<byte[]>(type: "BLOB", nullable: true),
                    OutputPath = table.Column<string>(type: "TEXT", nullable: true),
                    TypeResultsId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetectedObject", x => x.DetectedObjectId);
                    table.ForeignKey(
                        name: "FK_DetectedObject_Results_TypeResultsId",
                        column: x => x.TypeResultsId,
                        principalTable: "Results",
                        principalColumn: "ResultsId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DetectedObject_TypeResultsId",
                table: "DetectedObject",
                column: "TypeResultsId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DetectedObject");

            migrationBuilder.DropTable(
                name: "Results");
        }
    }
}

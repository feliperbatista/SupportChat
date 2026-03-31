using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCaegoryToConversation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CategoryId",
                table: "Conversations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ConversationCategorys",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DepartmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConversationCategorys", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConversationCategorys_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Conversations_CategoryId",
                table: "Conversations",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ConversationCategorys_DepartmentId",
                table: "ConversationCategorys",
                column: "DepartmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Conversations_ConversationCategorys_CategoryId",
                table: "Conversations",
                column: "CategoryId",
                principalTable: "ConversationCategorys",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Conversations_ConversationCategorys_CategoryId",
                table: "Conversations");

            migrationBuilder.DropTable(
                name: "ConversationCategorys");

            migrationBuilder.DropIndex(
                name: "IX_Conversations_CategoryId",
                table: "Conversations");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Conversations");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixCategoryTableName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ConversationCategorys_Departments_DepartmentId",
                table: "ConversationCategorys");

            migrationBuilder.DropForeignKey(
                name: "FK_Conversations_ConversationCategorys_CategoryId",
                table: "Conversations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ConversationCategorys",
                table: "ConversationCategorys");

            migrationBuilder.RenameTable(
                name: "ConversationCategorys",
                newName: "Categories");

            migrationBuilder.RenameIndex(
                name: "IX_ConversationCategorys_DepartmentId",
                table: "Categories",
                newName: "IX_Categories_DepartmentId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Categories",
                table: "Categories",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_Departments_DepartmentId",
                table: "Categories",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Conversations_Categories_CategoryId",
                table: "Conversations",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categories_Departments_DepartmentId",
                table: "Categories");

            migrationBuilder.DropForeignKey(
                name: "FK_Conversations_Categories_CategoryId",
                table: "Conversations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Categories",
                table: "Categories");

            migrationBuilder.RenameTable(
                name: "Categories",
                newName: "ConversationCategorys");

            migrationBuilder.RenameIndex(
                name: "IX_Categories_DepartmentId",
                table: "ConversationCategorys",
                newName: "IX_ConversationCategorys_DepartmentId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ConversationCategorys",
                table: "ConversationCategorys",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ConversationCategorys_Departments_DepartmentId",
                table: "ConversationCategorys",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Conversations_ConversationCategorys_CategoryId",
                table: "Conversations",
                column: "CategoryId",
                principalTable: "ConversationCategorys",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}

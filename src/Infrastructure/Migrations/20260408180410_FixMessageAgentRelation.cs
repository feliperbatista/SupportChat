using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixMessageAgentRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Messages_SentByAgentId",
                table: "Messages",
                column: "SentByAgentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Agents_SentByAgentId",
                table: "Messages",
                column: "SentByAgentId",
                principalTable: "Agents",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Agents_SentByAgentId",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Messages_SentByAgentId",
                table: "Messages");
        }
    }
}

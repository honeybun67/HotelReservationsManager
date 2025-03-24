using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelReservationsManagerManager.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddClientsHistory2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ClientHistoryId",
                table: "ClientHistories",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClientHistories_ClientHistoryId",
                table: "ClientHistories",
                column: "ClientHistoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClientHistories_ClientHistories_ClientHistoryId",
                table: "ClientHistories",
                column: "ClientHistoryId",
                principalTable: "ClientHistories",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClientHistories_ClientHistories_ClientHistoryId",
                table: "ClientHistories");

            migrationBuilder.DropIndex(
                name: "IX_ClientHistories_ClientHistoryId",
                table: "ClientHistories");

            migrationBuilder.DropColumn(
                name: "ClientHistoryId",
                table: "ClientHistories");
        }
    }
}

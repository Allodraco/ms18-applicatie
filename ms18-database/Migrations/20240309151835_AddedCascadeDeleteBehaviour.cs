using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Maasgroep.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddedCascadeDeleteBehaviour : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_albums_albums_ParentAlbumId",
                schema: "photoAlbum",
                table: "albums");

            migrationBuilder.CreateIndex(
                name: "IX_costCentre_Name",
                schema: "receipt",
                table: "costCentre",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_albums_albums_ParentAlbumId",
                schema: "photoAlbum",
                table: "albums",
                column: "ParentAlbumId",
                principalSchema: "photoAlbum",
                principalTable: "albums",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_albums_albums_ParentAlbumId",
                schema: "photoAlbum",
                table: "albums");

            migrationBuilder.DropIndex(
                name: "IX_costCentre_Name",
                schema: "receipt",
                table: "costCentre");

            migrationBuilder.AddForeignKey(
                name: "FK_albums_albums_ParentAlbumId",
                schema: "photoAlbum",
                table: "albums",
                column: "ParentAlbumId",
                principalSchema: "photoAlbum",
                principalTable: "albums",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

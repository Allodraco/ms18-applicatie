using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Maasgroep.Database.Migrations
{
    /// <inheritdoc />
    public partial class smallFixes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_photos_albums_AlbumLocationId",
                schema: "photoAlbum",
                table: "photos");

            migrationBuilder.AddForeignKey(
                name: "FK_photos_albums_AlbumLocationId",
                schema: "photoAlbum",
                table: "photos",
                column: "AlbumLocationId",
                principalSchema: "photoAlbum",
                principalTable: "albums",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_photos_albums_AlbumLocationId",
                schema: "photoAlbum",
                table: "photos");

            migrationBuilder.AddForeignKey(
                name: "FK_photos_albums_AlbumLocationId",
                schema: "photoAlbum",
                table: "photos",
                column: "AlbumLocationId",
                principalSchema: "photoAlbum",
                principalTable: "albums",
                principalColumn: "Id");
        }
    }
}

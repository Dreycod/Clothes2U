using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class PhotoCorrection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "pho_uri_photo",
                schema: "sae_clothes2u",
                table: "t_e_photo_pho");

            migrationBuilder.AddColumn<byte[]>(
                name: "pho_image",
                schema: "sae_clothes2u",
                table: "t_e_photo_pho",
                type: "bytea",
                nullable: false,
                defaultValue: new byte[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "pho_image",
                schema: "sae_clothes2u",
                table: "t_e_photo_pho");

            migrationBuilder.AddColumn<string>(
                name: "pho_uri_photo",
                schema: "sae_clothes2u",
                table: "t_e_photo_pho",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}

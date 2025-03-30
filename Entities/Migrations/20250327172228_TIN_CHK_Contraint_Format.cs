using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entities.Migrations
{
    /// <inheritdoc />
    public partial class TIN_CHK_Contraint_Format : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CHK_TIN",
                table: "Persons");

            migrationBuilder.CreateIndex(
                name: "IX_Persons_CountryID",
                table: "Persons",
                column: "CountryID");

            migrationBuilder.AddCheckConstraint(
                name: "CKK_TIN",
                table: "Persons",
                sql: "LEN([TaxIdentificationNumber]) = 8");

            migrationBuilder.AddForeignKey(
                name: "FK_Persons_Countries_CountryID",
                table: "Persons",
                column: "CountryID",
                principalTable: "Countries",
                principalColumn: "CountryID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Persons_Countries_CountryID",
                table: "Persons");

            migrationBuilder.DropIndex(
                name: "IX_Persons_CountryID",
                table: "Persons");

            migrationBuilder.DropCheckConstraint(
                name: "CKK_TIN",
                table: "Persons");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_TIN",
                table: "Persons",
                sql: "LEN([TaxIdentificationNumber]) = 8");
        }
    }
}

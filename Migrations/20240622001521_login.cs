using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace firstORM.Migrations
{
    /// <inheritdoc />
    public partial class login : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Fornecedore",
                table: "Fornecedore");

            migrationBuilder.RenameTable(
                name: "Fornecedore",
                newName: "Fornecedor");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Fornecedor",
                table: "Fornecedor",
                column: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Fornecedor",
                table: "Fornecedor");

            migrationBuilder.RenameTable(
                name: "Fornecedor",
                newName: "Fornecedore");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Fornecedore",
                table: "Fornecedore",
                column: "id");
        }
    }
}

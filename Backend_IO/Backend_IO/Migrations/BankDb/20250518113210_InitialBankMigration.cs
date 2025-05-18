using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend_IO.Migrations.BankDb
{
    /// <inheritdoc />
    public partial class InitialBankMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BankCards",
                columns: table => new
                {
                    CardNumber = table.Column<string>(type: "TEXT", nullable: false),
                    CVV2 = table.Column<string>(type: "TEXT", nullable: false),
                    ExpirationDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Balance = table.Column<decimal>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BankCards", x => x.CardNumber);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BankCards");
        }
    }
}

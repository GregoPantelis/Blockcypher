using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ICMarkets.Blockcypher.Infrastructure.Persistance.Migrations.Blockcypher
{
    /// <inheritdoc />
    public partial class InitialBlockcypher : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Blockchains",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Coin = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Chain = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    BlockchainData = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UtcCreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Blockchains", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Blockchains");
        }
    }
}

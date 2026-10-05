using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EkubCircle.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRounds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Rounds",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CircleId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoundNumber = table.Column<int>(type: "integer", nullable: false),
                    ReceiverMembershipId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    OpenedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PaidOutAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PayoutAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rounds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rounds_CircleMemberships_ReceiverMembershipId",
                        column: x => x.ReceiverMembershipId,
                        principalTable: "CircleMemberships",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Rounds_Circles_CircleId",
                        column: x => x.CircleId,
                        principalTable: "Circles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Rounds_CircleId_RoundNumber",
                table: "Rounds",
                columns: new[] { "CircleId", "RoundNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Rounds_ReceiverMembershipId",
                table: "Rounds",
                column: "ReceiverMembershipId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Rounds");
        }
    }
}

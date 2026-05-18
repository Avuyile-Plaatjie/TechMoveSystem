using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TechMoveSystem.Migrations
{
    /// <inheritdoc />
    public partial class UpdateServiceRequestFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Cost",
                table: "ServiceRequests",
                newName: "CostInZar");

            migrationBuilder.RenameColumn(
                name: "ServiceRequestId",
                table: "ServiceRequests",
                newName: "Id");

            migrationBuilder.AddColumn<decimal>(
                name: "CostInUsd",
                table: "ServiceRequests",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CostInUsd",
                table: "ServiceRequests");

            migrationBuilder.RenameColumn(
                name: "CostInZar",
                table: "ServiceRequests",
                newName: "Cost");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "ServiceRequests",
                newName: "ServiceRequestId");
        }
    }
}

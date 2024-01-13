using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MrPorker.Migrations
{
    /// <inheritdoc />
    public partial class AddAddymerMeasurementsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Measurements",
                table: "Measurements");

            migrationBuilder.RenameTable(
                name: "Measurements",
                newName: "MeasurementModel");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MeasurementModel",
                table: "MeasurementModel",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_MeasurementModel",
                table: "MeasurementModel");

            migrationBuilder.RenameTable(
                name: "MeasurementModel",
                newName: "Measurements");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Measurements",
                table: "Measurements",
                column: "Id");
        }
    }
}

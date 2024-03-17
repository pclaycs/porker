using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MrPorker.Migrations
{
    /// <inheritdoc />
    public partial class AddEloScoreFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Agility",
                table: "Measurements",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Endurance",
                table: "Measurements",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Overall",
                table: "Measurements",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Strength",
                table: "Measurements",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Agility",
                table: "EunoraMeasurements",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Endurance",
                table: "EunoraMeasurements",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Overall",
                table: "EunoraMeasurements",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Strength",
                table: "EunoraMeasurements",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Agility",
                table: "BluMeasurements",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Endurance",
                table: "BluMeasurements",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Overall",
                table: "BluMeasurements",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Strength",
                table: "BluMeasurements",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Agility",
                table: "AlexMeasurements",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Endurance",
                table: "AlexMeasurements",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Overall",
                table: "AlexMeasurements",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Strength",
                table: "AlexMeasurements",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Agility",
                table: "AddymerMeasurements",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Endurance",
                table: "AddymerMeasurements",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Overall",
                table: "AddymerMeasurements",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Strength",
                table: "AddymerMeasurements",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Agility",
                table: "Measurements");

            migrationBuilder.DropColumn(
                name: "Endurance",
                table: "Measurements");

            migrationBuilder.DropColumn(
                name: "Overall",
                table: "Measurements");

            migrationBuilder.DropColumn(
                name: "Strength",
                table: "Measurements");

            migrationBuilder.DropColumn(
                name: "Agility",
                table: "EunoraMeasurements");

            migrationBuilder.DropColumn(
                name: "Endurance",
                table: "EunoraMeasurements");

            migrationBuilder.DropColumn(
                name: "Overall",
                table: "EunoraMeasurements");

            migrationBuilder.DropColumn(
                name: "Strength",
                table: "EunoraMeasurements");

            migrationBuilder.DropColumn(
                name: "Agility",
                table: "BluMeasurements");

            migrationBuilder.DropColumn(
                name: "Endurance",
                table: "BluMeasurements");

            migrationBuilder.DropColumn(
                name: "Overall",
                table: "BluMeasurements");

            migrationBuilder.DropColumn(
                name: "Strength",
                table: "BluMeasurements");

            migrationBuilder.DropColumn(
                name: "Agility",
                table: "AlexMeasurements");

            migrationBuilder.DropColumn(
                name: "Endurance",
                table: "AlexMeasurements");

            migrationBuilder.DropColumn(
                name: "Overall",
                table: "AlexMeasurements");

            migrationBuilder.DropColumn(
                name: "Strength",
                table: "AlexMeasurements");

            migrationBuilder.DropColumn(
                name: "Agility",
                table: "AddymerMeasurements");

            migrationBuilder.DropColumn(
                name: "Endurance",
                table: "AddymerMeasurements");

            migrationBuilder.DropColumn(
                name: "Overall",
                table: "AddymerMeasurements");

            migrationBuilder.DropColumn(
                name: "Strength",
                table: "AddymerMeasurements");
        }
    }
}

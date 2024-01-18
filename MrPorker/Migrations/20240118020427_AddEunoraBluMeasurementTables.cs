using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MrPorker.Migrations
{
    /// <inheritdoc />
    public partial class AddEunoraBluMeasurementTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BluMeasurements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Timestamp = table.Column<long>(type: "INTEGER", nullable: false),
                    Weight = table.Column<float>(type: "REAL", nullable: false),
                    BodyMassIndex = table.Column<float>(type: "REAL", nullable: false),
                    BodyFat = table.Column<float>(type: "REAL", nullable: false),
                    FatFreeBodyWeight = table.Column<float>(type: "REAL", nullable: false),
                    SubcutaneousFat = table.Column<float>(type: "REAL", nullable: false),
                    VisceralFat = table.Column<float>(type: "REAL", nullable: false),
                    BodyWater = table.Column<float>(type: "REAL", nullable: false),
                    SkeletalMuscle = table.Column<float>(type: "REAL", nullable: false),
                    MuscleMass = table.Column<float>(type: "REAL", nullable: false),
                    BoneMass = table.Column<float>(type: "REAL", nullable: false),
                    Protein = table.Column<float>(type: "REAL", nullable: false),
                    BasalMetabolicRate = table.Column<float>(type: "REAL", nullable: false),
                    MetabolicAge = table.Column<float>(type: "REAL", nullable: false),
                    Remarks = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BluMeasurements", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EunoraMeasurements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Timestamp = table.Column<long>(type: "INTEGER", nullable: false),
                    Weight = table.Column<float>(type: "REAL", nullable: false),
                    BodyMassIndex = table.Column<float>(type: "REAL", nullable: false),
                    BodyFat = table.Column<float>(type: "REAL", nullable: false),
                    FatFreeBodyWeight = table.Column<float>(type: "REAL", nullable: false),
                    SubcutaneousFat = table.Column<float>(type: "REAL", nullable: false),
                    VisceralFat = table.Column<float>(type: "REAL", nullable: false),
                    BodyWater = table.Column<float>(type: "REAL", nullable: false),
                    SkeletalMuscle = table.Column<float>(type: "REAL", nullable: false),
                    MuscleMass = table.Column<float>(type: "REAL", nullable: false),
                    BoneMass = table.Column<float>(type: "REAL", nullable: false),
                    Protein = table.Column<float>(type: "REAL", nullable: false),
                    BasalMetabolicRate = table.Column<float>(type: "REAL", nullable: false),
                    MetabolicAge = table.Column<float>(type: "REAL", nullable: false),
                    Remarks = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EunoraMeasurements", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BluMeasurements");

            migrationBuilder.DropTable(
                name: "EunoraMeasurements");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MrPorker.Migrations
{
    /// <inheritdoc />
    public partial class FixMeasurementModelTableIssue : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MeasurementModel");

            migrationBuilder.CreateTable(
                name: "AddymerMeasurements",
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
                    table.PrimaryKey("PK_AddymerMeasurements", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Measurements",
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
                    table.PrimaryKey("PK_Measurements", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AddymerMeasurements");

            migrationBuilder.DropTable(
                name: "Measurements");

            migrationBuilder.CreateTable(
                name: "MeasurementModel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BasalMetabolicRate = table.Column<float>(type: "REAL", nullable: false),
                    BodyFat = table.Column<float>(type: "REAL", nullable: false),
                    BodyMassIndex = table.Column<float>(type: "REAL", nullable: false),
                    BodyWater = table.Column<float>(type: "REAL", nullable: false),
                    BoneMass = table.Column<float>(type: "REAL", nullable: false),
                    FatFreeBodyWeight = table.Column<float>(type: "REAL", nullable: false),
                    MetabolicAge = table.Column<float>(type: "REAL", nullable: false),
                    MuscleMass = table.Column<float>(type: "REAL", nullable: false),
                    Protein = table.Column<float>(type: "REAL", nullable: false),
                    Remarks = table.Column<string>(type: "TEXT", nullable: true),
                    SkeletalMuscle = table.Column<float>(type: "REAL", nullable: false),
                    SubcutaneousFat = table.Column<float>(type: "REAL", nullable: false),
                    Timestamp = table.Column<long>(type: "INTEGER", nullable: false),
                    VisceralFat = table.Column<float>(type: "REAL", nullable: false),
                    Weight = table.Column<float>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeasurementModel", x => x.Id);
                });
        }
    }
}

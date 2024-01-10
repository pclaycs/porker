using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MrPorker.Migrations
{
    /// <inheritdoc />
    public partial class CreateHoroscopesPhrasesAndMeasurementsTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Horoscopes",
                columns: table => new
                {
                    UserId = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Sign = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Horoscopes", x => x.UserId);
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

            migrationBuilder.CreateTable(
                name: "Phrases",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Content = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Phrases", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Horoscopes");

            migrationBuilder.DropTable(
                name: "Measurements");

            migrationBuilder.DropTable(
                name: "Phrases");
        }
    }
}

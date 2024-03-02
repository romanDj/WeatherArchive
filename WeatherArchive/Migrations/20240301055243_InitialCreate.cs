using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WeatherArchive.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Weathers",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Temp = table.Column<double>(type: "float", nullable: false),
                    AirHumidity = table.Column<double>(type: "float", nullable: false),
                    DewPoint = table.Column<double>(type: "float", nullable: false),
                    AtmPressure = table.Column<double>(type: "float", nullable: false),
                    DirWind = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SpeedWind = table.Column<double>(type: "float", nullable: true),
                    Cloudy = table.Column<double>(type: "float", nullable: true),
                    BottomCloudy = table.Column<double>(type: "float", nullable: true),
                    HorVisibility = table.Column<double>(type: "float", nullable: true),
                    WeatherCond = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Weathers", x => x.ID);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Weathers");
        }
    }
}

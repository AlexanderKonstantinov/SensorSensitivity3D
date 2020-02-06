using Microsoft.EntityFrameworkCore.Migrations;

namespace SensorSensitivity3D.DAL.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Configurations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: false),
                    DrawingIsVisible = table.Column<bool>(nullable: false),
                    SubstratePath = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Configurations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Geophones",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: false),
                    ConfigId = table.Column<int>(nullable: false),
                    HoleNumber = table.Column<int>(nullable: false),
                    X = table.Column<double>(nullable: false),
                    Y = table.Column<double>(nullable: false),
                    Z = table.Column<double>(nullable: false),
                    R = table.Column<int>(nullable: false),
                    Color = table.Column<string>(nullable: false),
                    GIsVisible = table.Column<bool>(nullable: false),
                    SIsVisible = table.Column<bool>(nullable: false),
                    IsGood = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Geophones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Geophones_Configurations_ConfigId",
                        column: x => x.ConfigId,
                        principalTable: "Configurations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Configurations",
                columns: new[] { "Id", "DrawingIsVisible", "Name", "SubstratePath" },
                values: new object[] { 1, false, "Конфигурация 1", "C:\\Users\\Александер\\Documents\\GitHub\\SensorSensitivity3D\\SensorSensitivity3D\\Pioner.dxf" });

            migrationBuilder.InsertData(
                table: "Configurations",
                columns: new[] { "Id", "DrawingIsVisible", "Name", "SubstratePath" },
                values: new object[] { 2, false, "Конфигурация 2", "C:\\Users\\Александер\\Documents\\GitHub\\SensorSensitivity3D\\SensorSensitivity3D\\Pioner.dxf" });

            migrationBuilder.InsertData(
                table: "Configurations",
                columns: new[] { "Id", "DrawingIsVisible", "Name", "SubstratePath" },
                values: new object[] { 3, false, "Конфигурация 3", "C:\\Users\\Александер\\Documents\\GitHub\\SensorSensitivity3D\\SensorSensitivity3D\\Pioner.dxf" });

            migrationBuilder.InsertData(
                table: "Geophones",
                columns: new[] { "Id", "Color", "ConfigId", "GIsVisible", "HoleNumber", "IsGood", "Name", "R", "SIsVisible", "X", "Y", "Z" },
                values: new object[] { 8, "#46b7207b", 1, false, 7, true, "Геофон 7", 50, false, -768.0, -1119.0, 109.0 });

            migrationBuilder.InsertData(
                table: "Geophones",
                columns: new[] { "Id", "Color", "ConfigId", "GIsVisible", "HoleNumber", "IsGood", "Name", "R", "SIsVisible", "X", "Y", "Z" },
                values: new object[] { 14, "#46ee550a", 3, true, 13, true, "Геофон 13", 50, true, -767.0, -1009.0, 191.0 });

            migrationBuilder.InsertData(
                table: "Geophones",
                columns: new[] { "Id", "Color", "ConfigId", "GIsVisible", "HoleNumber", "IsGood", "Name", "R", "SIsVisible", "X", "Y", "Z" },
                values: new object[] { 13, "#4675faff", 3, false, 12, true, "Геофон 12", 50, false, -771.0, -1117.0, 152.0 });

            migrationBuilder.InsertData(
                table: "Geophones",
                columns: new[] { "Id", "Color", "ConfigId", "GIsVisible", "HoleNumber", "IsGood", "Name", "R", "SIsVisible", "X", "Y", "Z" },
                values: new object[] { 11, "#46d492c2", 3, false, 10, true, "Геофон 10", 50, false, -541.0, -1216.0, 101.0 });

            migrationBuilder.InsertData(
                table: "Geophones",
                columns: new[] { "Id", "Color", "ConfigId", "GIsVisible", "HoleNumber", "IsGood", "Name", "R", "SIsVisible", "X", "Y", "Z" },
                values: new object[] { 9, "#463c2b7b", 3, true, 8, true, "Геофон 8", 50, false, -711.0, -1112.0, 139.0 });

            migrationBuilder.InsertData(
                table: "Geophones",
                columns: new[] { "Id", "Color", "ConfigId", "GIsVisible", "HoleNumber", "IsGood", "Name", "R", "SIsVisible", "X", "Y", "Z" },
                values: new object[] { 5, "#46002bae", 3, false, 4, true, "Геофон 4", 50, true, -529.0, -1129.0, 155.0 });

            migrationBuilder.InsertData(
                table: "Geophones",
                columns: new[] { "Id", "Color", "ConfigId", "GIsVisible", "HoleNumber", "IsGood", "Name", "R", "SIsVisible", "X", "Y", "Z" },
                values: new object[] { 3, "#46514743", 3, true, 2, true, "Геофон 2", 50, false, -616.0, -1117.0, 201.0 });

            migrationBuilder.InsertData(
                table: "Geophones",
                columns: new[] { "Id", "Color", "ConfigId", "GIsVisible", "HoleNumber", "IsGood", "Name", "R", "SIsVisible", "X", "Y", "Z" },
                values: new object[] { 2, "#46fbd91e", 3, true, 1, true, "Геофон 1", 50, false, -775.0, -1079.0, 165.0 });

            migrationBuilder.InsertData(
                table: "Geophones",
                columns: new[] { "Id", "Color", "ConfigId", "GIsVisible", "HoleNumber", "IsGood", "Name", "R", "SIsVisible", "X", "Y", "Z" },
                values: new object[] { 1, "#460c09fa", 3, false, 0, true, "Геофон 0", 50, true, -465.0, -1202.0, 197.0 });

            migrationBuilder.InsertData(
                table: "Geophones",
                columns: new[] { "Id", "Color", "ConfigId", "GIsVisible", "HoleNumber", "IsGood", "Name", "R", "SIsVisible", "X", "Y", "Z" },
                values: new object[] { 27, "#4695a42a", 2, false, 26, true, "Геофон 26", 50, false, -701.0, -1130.0, 167.0 });

            migrationBuilder.InsertData(
                table: "Geophones",
                columns: new[] { "Id", "Color", "ConfigId", "GIsVisible", "HoleNumber", "IsGood", "Name", "R", "SIsVisible", "X", "Y", "Z" },
                values: new object[] { 26, "#4686d14a", 2, false, 25, true, "Геофон 25", 50, true, -741.0, -1167.0, 163.0 });

            migrationBuilder.InsertData(
                table: "Geophones",
                columns: new[] { "Id", "Color", "ConfigId", "GIsVisible", "HoleNumber", "IsGood", "Name", "R", "SIsVisible", "X", "Y", "Z" },
                values: new object[] { 22, "#460a50c6", 2, true, 21, true, "Геофон 21", 50, false, -813.0, -1139.0, 204.0 });

            migrationBuilder.InsertData(
                table: "Geophones",
                columns: new[] { "Id", "Color", "ConfigId", "GIsVisible", "HoleNumber", "IsGood", "Name", "R", "SIsVisible", "X", "Y", "Z" },
                values: new object[] { 21, "#46bab0ab", 2, false, 20, true, "Геофон 20", 50, true, -526.0, -1068.0, 101.0 });

            migrationBuilder.InsertData(
                table: "Geophones",
                columns: new[] { "Id", "Color", "ConfigId", "GIsVisible", "HoleNumber", "IsGood", "Name", "R", "SIsVisible", "X", "Y", "Z" },
                values: new object[] { 17, "#46057340", 2, true, 16, true, "Геофон 16", 50, false, -727.0, -1180.0, 120.0 });

            migrationBuilder.InsertData(
                table: "Geophones",
                columns: new[] { "Id", "Color", "ConfigId", "GIsVisible", "HoleNumber", "IsGood", "Name", "R", "SIsVisible", "X", "Y", "Z" },
                values: new object[] { 7, "#469f78ac", 2, false, 6, true, "Геофон 6", 50, false, -535.0, -1087.0, 203.0 });

            migrationBuilder.InsertData(
                table: "Geophones",
                columns: new[] { "Id", "Color", "ConfigId", "GIsVisible", "HoleNumber", "IsGood", "Name", "R", "SIsVisible", "X", "Y", "Z" },
                values: new object[] { 6, "#46841155", 2, true, 5, true, "Геофон 5", 50, false, -780.0, -1242.0, 148.0 });

            migrationBuilder.InsertData(
                table: "Geophones",
                columns: new[] { "Id", "Color", "ConfigId", "GIsVisible", "HoleNumber", "IsGood", "Name", "R", "SIsVisible", "X", "Y", "Z" },
                values: new object[] { 4, "#469996e1", 2, true, 3, true, "Геофон 3", 50, false, -793.0, -1193.0, 160.0 });

            migrationBuilder.InsertData(
                table: "Geophones",
                columns: new[] { "Id", "Color", "ConfigId", "GIsVisible", "HoleNumber", "IsGood", "Name", "R", "SIsVisible", "X", "Y", "Z" },
                values: new object[] { 30, "#46a79dd1", 1, false, 29, true, "Геофон 29", 50, true, -545.0, -1195.0, 195.0 });

            migrationBuilder.InsertData(
                table: "Geophones",
                columns: new[] { "Id", "Color", "ConfigId", "GIsVisible", "HoleNumber", "IsGood", "Name", "R", "SIsVisible", "X", "Y", "Z" },
                values: new object[] { 29, "#4691f232", 1, true, 28, true, "Геофон 28", 50, true, -495.0, -1013.0, 178.0 });

            migrationBuilder.InsertData(
                table: "Geophones",
                columns: new[] { "Id", "Color", "ConfigId", "GIsVisible", "HoleNumber", "IsGood", "Name", "R", "SIsVisible", "X", "Y", "Z" },
                values: new object[] { 28, "#4640fc2a", 1, true, 27, true, "Геофон 27", 50, true, -614.0, -1249.0, 104.0 });

            migrationBuilder.InsertData(
                table: "Geophones",
                columns: new[] { "Id", "Color", "ConfigId", "GIsVisible", "HoleNumber", "IsGood", "Name", "R", "SIsVisible", "X", "Y", "Z" },
                values: new object[] { 25, "#46fb397a", 1, false, 24, true, "Геофон 24", 50, true, -772.0, -1213.0, 192.0 });

            migrationBuilder.InsertData(
                table: "Geophones",
                columns: new[] { "Id", "Color", "ConfigId", "GIsVisible", "HoleNumber", "IsGood", "Name", "R", "SIsVisible", "X", "Y", "Z" },
                values: new object[] { 24, "#46fa69b3", 1, false, 23, true, "Геофон 23", 50, false, -535.0, -1075.0, 204.0 });

            migrationBuilder.InsertData(
                table: "Geophones",
                columns: new[] { "Id", "Color", "ConfigId", "GIsVisible", "HoleNumber", "IsGood", "Name", "R", "SIsVisible", "X", "Y", "Z" },
                values: new object[] { 20, "#46c0a815", 1, false, 19, true, "Геофон 19", 50, true, -678.0, -1020.0, 133.0 });

            migrationBuilder.InsertData(
                table: "Geophones",
                columns: new[] { "Id", "Color", "ConfigId", "GIsVisible", "HoleNumber", "IsGood", "Name", "R", "SIsVisible", "X", "Y", "Z" },
                values: new object[] { 19, "#464f3fdf", 1, true, 18, true, "Геофон 18", 50, false, -486.0, -1117.0, 171.0 });

            migrationBuilder.InsertData(
                table: "Geophones",
                columns: new[] { "Id", "Color", "ConfigId", "GIsVisible", "HoleNumber", "IsGood", "Name", "R", "SIsVisible", "X", "Y", "Z" },
                values: new object[] { 18, "#46e6336c", 1, true, 17, true, "Геофон 17", 50, true, -658.0, -1097.0, 106.0 });

            migrationBuilder.InsertData(
                table: "Geophones",
                columns: new[] { "Id", "Color", "ConfigId", "GIsVisible", "HoleNumber", "IsGood", "Name", "R", "SIsVisible", "X", "Y", "Z" },
                values: new object[] { 16, "#4641a014", 1, false, 15, true, "Геофон 15", 50, false, -690.0, -1219.0, 151.0 });

            migrationBuilder.InsertData(
                table: "Geophones",
                columns: new[] { "Id", "Color", "ConfigId", "GIsVisible", "HoleNumber", "IsGood", "Name", "R", "SIsVisible", "X", "Y", "Z" },
                values: new object[] { 12, "#46f517ec", 1, true, 11, true, "Геофон 11", 50, true, -622.0, -1155.0, 203.0 });

            migrationBuilder.InsertData(
                table: "Geophones",
                columns: new[] { "Id", "Color", "ConfigId", "GIsVisible", "HoleNumber", "IsGood", "Name", "R", "SIsVisible", "X", "Y", "Z" },
                values: new object[] { 10, "#4636b127", 1, false, 9, true, "Геофон 9", 50, false, -779.0, -1110.0, 119.0 });

            migrationBuilder.InsertData(
                table: "Geophones",
                columns: new[] { "Id", "Color", "ConfigId", "GIsVisible", "HoleNumber", "IsGood", "Name", "R", "SIsVisible", "X", "Y", "Z" },
                values: new object[] { 15, "#469ae129", 3, false, 14, true, "Геофон 14", 50, false, -549.0, -1135.0, 207.0 });

            migrationBuilder.InsertData(
                table: "Geophones",
                columns: new[] { "Id", "Color", "ConfigId", "GIsVisible", "HoleNumber", "IsGood", "Name", "R", "SIsVisible", "X", "Y", "Z" },
                values: new object[] { 23, "#4650196d", 3, false, 22, true, "Геофон 22", 50, true, -488.0, -1007.0, 105.0 });

            migrationBuilder.CreateIndex(
                name: "IX_Geophones_ConfigId",
                table: "Geophones",
                column: "ConfigId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Geophones");

            migrationBuilder.DropTable(
                name: "Configurations");
        }
    }
}

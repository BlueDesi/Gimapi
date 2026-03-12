using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gimapi.Migrations
{
    /// <inheritdoc />
    public partial class FixAdminHash : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$2a$11$Xm7766/Wrc.0B0C6p7.SkeS.uM7Gz9.F8.7f88888888888888888");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$2a$11$h.B.f6mS8.5P9mH7H9z1Ou3Jp5X7E5v1B9mH7H9z1Ou3Jp5X7E5v1");
        }
    }
}

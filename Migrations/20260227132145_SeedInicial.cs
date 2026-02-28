using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Gimapi.Migrations
{
    /// <inheritdoc />
    public partial class SeedInicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "NombreRol" },
                values: new object[,]
                {
                    { 1, "Admin" },
                    { 2, "Empleado" },
                    { 3, "Socio" }
                });

            migrationBuilder.InsertData(
                table: "Usuarios",
                columns: new[] { "Id", "Activo", "Apellido", "DNI", "Email", "MembresiaId", "Nombre", "Password", "RolId" },
                values: new object[,]
                {
                    { 1, true, "Gimnasio", "12345678", "admin@gimapi.com", null, "Admin", "admin", 1 },
                    { 2, true, "Prueba", "99999999", "socio@gimapi.com", null, "Socio", "socio", 3 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}

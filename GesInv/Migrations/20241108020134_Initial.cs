using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GesInv.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categoria",
                columns: table => new
                {
                    CategoriaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Categori__F353C1E5979A939A", x => x.CategoriaId);
                });

            migrationBuilder.CreateTable(
                name: "Proveedor",
                columns: table => new
                {
                    ProveedorId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NombreContacto = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Telefono = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    CorreoElectronico = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Direccion = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Proveedo__61266A592B6E3FC7", x => x.ProveedorId);
                });

            migrationBuilder.CreateTable(
                name: "Producto",
                columns: table => new
                {
                    ProductoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Precio = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Stock = table.Column<int>(type: "int", nullable: false),
                    CategoriaId = table.Column<int>(type: "int", nullable: true),
                    ProveedorId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Producto__A430AEA3289804DB", x => x.ProductoId);
                    table.ForeignKey(
                        name: "FK__Producto__Catego__15502E78",
                        column: x => x.CategoriaId,
                        principalTable: "Categoria",
                        principalColumn: "CategoriaId");
                    table.ForeignKey(
                        name: "FK__Producto__Provee__164452B1",
                        column: x => x.ProveedorId,
                        principalTable: "Proveedor",
                        principalColumn: "ProveedorId");
                });

            migrationBuilder.CreateTable(
                name: "MovimientoInventario",
                columns: table => new
                {
                    MovimientoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductoId = table.Column<int>(type: "int", nullable: true),
                    Cantidad = table.Column<int>(type: "int", nullable: false),
                    FechaMovimiento = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    TipoMovimiento = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Notas = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Movimien__BF923C2C4D1F0734", x => x.MovimientoId);
                    table.ForeignKey(
                        name: "FK__Movimient__Produ__1920BF5C",
                        column: x => x.ProductoId,
                        principalTable: "Producto",
                        principalColumn: "ProductoId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_MovimientoInventario_ProductoId",
                table: "MovimientoInventario",
                column: "ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_Producto_CategoriaId",
                table: "Producto",
                column: "CategoriaId");

            migrationBuilder.CreateIndex(
                name: "IX_Producto_ProveedorId",
                table: "Producto",
                column: "ProveedorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MovimientoInventario");

            migrationBuilder.DropTable(
                name: "Producto");

            migrationBuilder.DropTable(
                name: "Categoria");

            migrationBuilder.DropTable(
                name: "Proveedor");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlaxtorGym.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tipos_cliente",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nombre = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Descripcion = table.Column<string>(type: "TEXT", maxLength: 250, nullable: false),
                    Activo = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tipos_cliente", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tipos_membresia",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nombre = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    DuracionDias = table.Column<int>(type: "INTEGER", nullable: false),
                    Descripcion = table.Column<string>(type: "TEXT", maxLength: 250, nullable: false),
                    Activo = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tipos_membresia", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NombreUsuario = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    ContrasenaHash = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    NombreCompleto = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Rol = table.Column<int>(type: "INTEGER", nullable: false),
                    Activo = table.Column<bool>(type: "INTEGER", nullable: false),
                    IntentosFallidos = table.Column<int>(type: "INTEGER", nullable: false),
                    BloqueadoHasta = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usuarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "clientes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Cedula = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    NombreCompleto = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                    Telefono = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    CorreoElectronico = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    FechaNacimiento = table.Column<DateTime>(type: "TEXT", nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Activo = table.Column<bool>(type: "INTEGER", nullable: false),
                    TipoClienteId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_clientes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_clientes_tipos_cliente_TipoClienteId",
                        column: x => x.TipoClienteId,
                        principalTable: "tipos_cliente",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "precios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Monto = table.Column<decimal>(type: "TEXT", precision: 12, scale: 2, nullable: false),
                    FechaInicioVigencia = table.Column<DateTime>(type: "TEXT", nullable: false),
                    FechaFinVigencia = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Activo = table.Column<bool>(type: "INTEGER", nullable: false),
                    TipoClienteId = table.Column<int>(type: "INTEGER", nullable: false),
                    TipoMembresiaId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_precios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_precios_tipos_cliente_TipoClienteId",
                        column: x => x.TipoClienteId,
                        principalTable: "tipos_cliente",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_precios_tipos_membresia_TipoMembresiaId",
                        column: x => x.TipoMembresiaId,
                        principalTable: "tipos_membresia",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "bitacora_cambios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FechaHora = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Accion = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Entidad = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    EntidadId = table.Column<int>(type: "INTEGER", nullable: true),
                    Descripcion = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    UsuarioId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_bitacora_cambios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_bitacora_cambios_usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "mediciones_fisicas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FechaMedicion = table.Column<DateTime>(type: "TEXT", nullable: false),
                    PesoKg = table.Column<decimal>(type: "TEXT", precision: 6, scale: 2, nullable: false),
                    EstaturaCm = table.Column<decimal>(type: "TEXT", precision: 6, scale: 2, nullable: true),
                    PorcentajeGrasa = table.Column<decimal>(type: "TEXT", precision: 5, scale: 2, nullable: true),
                    Observacion = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    ClienteId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mediciones_fisicas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_mediciones_fisicas_clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "membresias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FechaInicio = table.Column<DateTime>(type: "TEXT", nullable: false),
                    FechaVencimiento = table.Column<DateTime>(type: "TEXT", nullable: false),
                    PrecioAplicado = table.Column<decimal>(type: "TEXT", precision: 12, scale: 2, nullable: false),
                    Activa = table.Column<bool>(type: "INTEGER", nullable: false),
                    ClienteId = table.Column<int>(type: "INTEGER", nullable: false),
                    TipoMembresiaId = table.Column<int>(type: "INTEGER", nullable: false),
                    PrecioId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_membresias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_membresias_clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_membresias_precios_PrecioId",
                        column: x => x.PrecioId,
                        principalTable: "precios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_membresias_tipos_membresia_TipoMembresiaId",
                        column: x => x.TipoMembresiaId,
                        principalTable: "tipos_membresia",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "notificaciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Mensaje = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "TEXT", nullable: false),
                    FechaEnvio = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Enviada = table.Column<bool>(type: "INTEGER", nullable: false),
                    ClienteId = table.Column<int>(type: "INTEGER", nullable: false),
                    MembresiaId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notificaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_notificaciones_clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_notificaciones_membresias_MembresiaId",
                        column: x => x.MembresiaId,
                        principalTable: "membresias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "pagos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Monto = table.Column<decimal>(type: "TEXT", precision: 12, scale: 2, nullable: false),
                    FechaPago = table.Column<DateTime>(type: "TEXT", nullable: false),
                    MetodoPago = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Observacion = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    MembresiaId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pagos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_pagos_membresias_MembresiaId",
                        column: x => x.MembresiaId,
                        principalTable: "membresias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_bitacora_cambios_Entidad_EntidadId",
                table: "bitacora_cambios",
                columns: new[] { "Entidad", "EntidadId" });

            migrationBuilder.CreateIndex(
                name: "IX_bitacora_cambios_FechaHora",
                table: "bitacora_cambios",
                column: "FechaHora");

            migrationBuilder.CreateIndex(
                name: "IX_bitacora_cambios_UsuarioId",
                table: "bitacora_cambios",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_clientes_Cedula",
                table: "clientes",
                column: "Cedula",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_clientes_NombreCompleto",
                table: "clientes",
                column: "NombreCompleto");

            migrationBuilder.CreateIndex(
                name: "IX_clientes_TipoClienteId",
                table: "clientes",
                column: "TipoClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_mediciones_fisicas_ClienteId_FechaMedicion",
                table: "mediciones_fisicas",
                columns: new[] { "ClienteId", "FechaMedicion" });

            migrationBuilder.CreateIndex(
                name: "IX_membresias_Activa_FechaVencimiento",
                table: "membresias",
                columns: new[] { "Activa", "FechaVencimiento" });

            migrationBuilder.CreateIndex(
                name: "IX_membresias_ClienteId",
                table: "membresias",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_membresias_PrecioId",
                table: "membresias",
                column: "PrecioId");

            migrationBuilder.CreateIndex(
                name: "IX_membresias_TipoMembresiaId",
                table: "membresias",
                column: "TipoMembresiaId");

            migrationBuilder.CreateIndex(
                name: "IX_notificaciones_ClienteId",
                table: "notificaciones",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_notificaciones_Enviada_FechaCreacion",
                table: "notificaciones",
                columns: new[] { "Enviada", "FechaCreacion" });

            migrationBuilder.CreateIndex(
                name: "IX_notificaciones_MembresiaId",
                table: "notificaciones",
                column: "MembresiaId");

            migrationBuilder.CreateIndex(
                name: "IX_pagos_FechaPago",
                table: "pagos",
                column: "FechaPago");

            migrationBuilder.CreateIndex(
                name: "IX_pagos_MembresiaId",
                table: "pagos",
                column: "MembresiaId");

            migrationBuilder.CreateIndex(
                name: "IX_precios_TipoClienteId_TipoMembresiaId_Activo",
                table: "precios",
                columns: new[] { "TipoClienteId", "TipoMembresiaId", "Activo" });

            migrationBuilder.CreateIndex(
                name: "IX_precios_TipoMembresiaId",
                table: "precios",
                column: "TipoMembresiaId");

            migrationBuilder.CreateIndex(
                name: "IX_tipos_cliente_Nombre",
                table: "tipos_cliente",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tipos_membresia_Nombre",
                table: "tipos_membresia",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_NombreUsuario",
                table: "usuarios",
                column: "NombreUsuario",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "bitacora_cambios");

            migrationBuilder.DropTable(
                name: "mediciones_fisicas");

            migrationBuilder.DropTable(
                name: "notificaciones");

            migrationBuilder.DropTable(
                name: "pagos");

            migrationBuilder.DropTable(
                name: "usuarios");

            migrationBuilder.DropTable(
                name: "membresias");

            migrationBuilder.DropTable(
                name: "clientes");

            migrationBuilder.DropTable(
                name: "precios");

            migrationBuilder.DropTable(
                name: "tipos_cliente");

            migrationBuilder.DropTable(
                name: "tipos_membresia");
        }
    }
}

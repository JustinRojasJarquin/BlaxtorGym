using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlaxtorGym.Migrations;

public partial class AgregarPerimetrosMedicionFisica : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<decimal>("Hombros", "mediciones_fisicas", type: "TEXT", precision: 6, scale: 2, nullable: true);
        migrationBuilder.AddColumn<decimal>("Pecho", "mediciones_fisicas", type: "TEXT", precision: 6, scale: 2, nullable: true);
        migrationBuilder.AddColumn<decimal>("BicepsDerecho", "mediciones_fisicas", type: "TEXT", precision: 6, scale: 2, nullable: true);
        migrationBuilder.AddColumn<decimal>("BicepsIzquierdo", "mediciones_fisicas", type: "TEXT", precision: 6, scale: 2, nullable: true);
        migrationBuilder.AddColumn<decimal>("Cintura", "mediciones_fisicas", type: "TEXT", precision: 6, scale: 2, nullable: true);
        migrationBuilder.AddColumn<decimal>("MusloDerecho", "mediciones_fisicas", type: "TEXT", precision: 6, scale: 2, nullable: true);
        migrationBuilder.AddColumn<decimal>("MusloIzquierdo", "mediciones_fisicas", type: "TEXT", precision: 6, scale: 2, nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn("Hombros", "mediciones_fisicas");
        migrationBuilder.DropColumn("Pecho", "mediciones_fisicas");
        migrationBuilder.DropColumn("BicepsDerecho", "mediciones_fisicas");
        migrationBuilder.DropColumn("BicepsIzquierdo", "mediciones_fisicas");
        migrationBuilder.DropColumn("Cintura", "mediciones_fisicas");
        migrationBuilder.DropColumn("MusloDerecho", "mediciones_fisicas");
        migrationBuilder.DropColumn("MusloIzquierdo", "mediciones_fisicas");
    }
}

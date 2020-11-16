using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MVCBasico12D.Migrations
{
    public partial class MVCBasico12DContextSchoolDBContext : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Alumno",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(nullable: true),
                    Apellido = table.Column<string>(nullable: true),
                    Dni = table.Column<string>(nullable: true),
                    FechaNacimiento = table.Column<DateTime>(nullable: false),
                    Email = table.Column<string>(nullable: true),
                    Telefono = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alumno", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Curso",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Sigla = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Curso", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CursoAlumno",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AlumnoId = table.Column<int>(nullable: false),
                    CursoId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CursoAlumno", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CursoMateria",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CursoId = table.Column<int>(nullable: false),
                    MateriaId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CursoMateria", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Materia",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(nullable: true),
                    Anio = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Materia", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MateriaProfesor",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProfesorId = table.Column<int>(nullable: false),
                    MateriaId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MateriaProfesor", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Nota",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AlumnoId = table.Column<int>(nullable: false),
                    MateriaId = table.Column<int>(nullable: false),
                    Nota1 = table.Column<int>(nullable: false),
                    Cuatrimestre = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Nota", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Profesor",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(nullable: true),
                    Apellido = table.Column<string>(nullable: true),
                    Dni = table.Column<string>(nullable: true),
                    FechaNacimiento = table.Column<DateTime>(nullable: false),
                    Email = table.Column<string>(nullable: true),
                    Telefono = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Profesor", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Alumno");

            migrationBuilder.DropTable(
                name: "Curso");

            migrationBuilder.DropTable(
                name: "CursoAlumno");

            migrationBuilder.DropTable(
                name: "CursoMateria");

            migrationBuilder.DropTable(
                name: "Materia");

            migrationBuilder.DropTable(
                name: "MateriaProfesor");

            migrationBuilder.DropTable(
                name: "Nota");

            migrationBuilder.DropTable(
                name: "Profesor");
        }
    }
}

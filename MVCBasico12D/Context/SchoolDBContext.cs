using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MVCBasico12D.Models;

namespace MVCBasico12D.Context
{
    public class SchoolDBContext : DbContext
    {
        public SchoolDBContext()
        {
        }

        public SchoolDBContext(DbContextOptions<SchoolDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Alumno> Alumno { get; set; }
        public virtual DbSet<Curso> Curso { get; set; }
        public virtual DbSet<CursoAlumno> CursoAlumno { get; set; }
        public virtual DbSet<CursoMateria> CursoMateria { get; set; }
        public virtual DbSet<Materia> Materia { get; set; }
        public virtual DbSet<MateriaProfesor> MateriaProfesor { get; set; }
        public virtual DbSet<Nota> Nota { get; set; }
        public virtual DbSet<Profesor> Profesor { get; set; }
        public virtual DbSet<Usuario> Usuarios { get; set; }

    }
}

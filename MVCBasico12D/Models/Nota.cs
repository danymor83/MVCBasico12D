using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MVCBasico12D.Models
{
    public class Nota
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int AlumnoId { get; set; }
        public int MateriaId { get; set; }
        public int Nota1 { get; set; }
        public int Cuatrimestre { get; set; }
    }
}

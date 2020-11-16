using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MVCBasico12D.Models
{
    public class Profesor
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Dni { get; set; }

        [Display(Name = "Fecha de Nacimiento")]
        public DateTime FechaNacimiento { get; set; }
        public string Email { get; set; }
        public string Telefono { get; set; }
    }
}

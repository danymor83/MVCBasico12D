using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVCBasico12D.Models
{
    public class Usuario
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        //TIPOS: 0 = admin, 1 = alumno, 2 = profesor
        public int Tipo { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
    }
}

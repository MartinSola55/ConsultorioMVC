using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ConsultorioMVC.Models
{
    public class Persona
    {
        public int ID { set; get; }

        [Required]
        [StringLength(30, ErrorMessage = "Debes añadir un nombre válido")]
        public string Nombre { set; get; }

        [Required]
        [StringLength(30, ErrorMessage = "Debes añadir un apellido válido")]
        public string Apellido { set; get; }

        [Required]
        [Phone(ErrorMessage = "Debes añadir un teléfono válido")]
        public string Telefono { set; get; }

        [EmailAddress(ErrorMessage = "Debes ingresar un formato de email válido")]
        public string Correo { set; get; }

        [Required]
        [RegularExpression("[0-9]", ErrorMessage = "Debes añadir una obra social válida")]
        public int IDObraSocial { set; get; }
    }
}
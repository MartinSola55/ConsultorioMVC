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

        [Required(ErrorMessage = "Debes añadir un nombre")]
        [StringLength(30, ErrorMessage = "Debes añadir un nombre de menos de 30 caracteres")]
        [RegularExpression(@"^[a-zA-Z\u00C0-\u017F\s]+$", ErrorMessage = "Ingrese un nombre válido")]
        public string Nombre { set; get; }

        [Required(ErrorMessage = "Debes añadir un apellido")]
        [StringLength(30, ErrorMessage = "Debes añadir un apellido de menos de 30 caracteres")]
        [RegularExpression(@"^[a-zA-Z\u00C0-\u017F\s]+$", ErrorMessage = "Ingrese un apellido válido")]
        public string Apellido { set; get; }

        [Required(ErrorMessage = "Debes añadir un teléfono")]
        [Phone(ErrorMessage = "Debes añadir un teléfono válido")]
        public string Telefono { set; get; }

        [EmailAddress(ErrorMessage = "Debes ingresar un formato de email válido")]
        public string Correo { set; get; }

        [Required(ErrorMessage = "Debes seleccionar una obra social")]
        public Models.ObraSocial ObraSocial { set; get; }
    }
}
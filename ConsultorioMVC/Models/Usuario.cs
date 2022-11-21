using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ConsultorioMVC.Models
{
    public class Usuario
    {
        public int ID { set; get; }
        [Required(ErrorMessage = "Debes completar con un email")]
        [EmailAddress(ErrorMessage = "Ingrese un formato de email válido")]
        public string Email { set; get; }

        [Required(ErrorMessage = "Debes completar con una contraseña")]
        [DataType(DataType.Password, ErrorMessage = "Ingrese una contraseña válida")]
        public string Password { set; get; }
    }
}
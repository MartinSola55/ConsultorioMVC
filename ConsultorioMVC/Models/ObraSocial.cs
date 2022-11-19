using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ConsultorioMVC.Models
{
    public class ObraSocial
    {
        [Required(ErrorMessage = "Debes seleccionar una obra social")]
        public int ID { set; get; }
        
        [Required(ErrorMessage = "Debes añadir un nombre")]
        [StringLength(50, ErrorMessage = "Debes añadir un nombre de menos de 50 caracteres")]
        [RegularExpression(@"^[a-zA-Z\u00C0-\u017F\s]+$", ErrorMessage = "Debes añadir un nombre válido")]
        public string Nombre { set; get; }

        [Required(ErrorMessage = "Debes indicar si la obra social está habilitada o no")]
        public bool Habilitada { set; get; }

    }
}
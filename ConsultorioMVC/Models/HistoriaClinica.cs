using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ConsultorioMVC.Models
{
    public class HistoriaClinica
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Debes seleccionar un paciente")]
        [RegularExpression("^[0-9]+$", ErrorMessage = "Debes añadir un paciente válido")]
        public int IDPaciente { get; set; }

        [Required(ErrorMessage = "Debes ingresar una descripción")]
        [StringLength(1500, ErrorMessage = "Debes añadir una descripción de menos de 1500 caracteres")]
        [RegularExpression(@"^[a-zA-Z\u00C0-\u017F\s\n0-9.-]+$", ErrorMessage = "Ingrese una descripción válida")]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "Debes ingresar una fecha")]
        public DateTime Fecha { get; set; }
    }
}
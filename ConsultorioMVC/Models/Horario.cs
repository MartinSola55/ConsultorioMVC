using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ConsultorioMVC.Models
{
    public class Horario
    {
        [Required(ErrorMessage = "Debes seleccionar una hora")]
        [RegularExpression("^([1-9]|1[0-3])+$", ErrorMessage = "Debes seleccionar una hora válida")]
        public int ID { set; get; }
        public DateTime Hora { set; get; }

        public string HoraString { get; set; }

    }
}
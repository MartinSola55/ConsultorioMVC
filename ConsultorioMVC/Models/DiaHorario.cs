using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Web;

namespace ConsultorioMVC.Models
{
    public class DiaHorario
    {
        public int ID { set; get; }

        [Required(ErrorMessage = "Debes seleccionar un horario")]
        public Horario Horario { set; get; }

        [Required(ErrorMessage = "Debes seleccionar un día")]
        public DateTime Dia { set; get; }
        public bool Disponible { set; get; }

    }
}
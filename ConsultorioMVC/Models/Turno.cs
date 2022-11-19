using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ConsultorioMVC.Models
{
    public class Turno
    {
        public int ID { set; get; }
        public Persona Persona { set; get; }
        public DiaHorario DiaHorario { set; get; }

    }
}
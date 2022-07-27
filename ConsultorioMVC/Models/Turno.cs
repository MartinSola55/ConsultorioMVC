using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ConsultorioMVC.Models
{
    public class Turno
    {
        public int ID { set; get; }
        public int IDPaciente { set; get; }
        public int IDDiaHorario { set; get; }

    }
}
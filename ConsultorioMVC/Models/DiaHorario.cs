using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ConsultorioMVC.Models
{
    public class DiaHorario
    {
        public int ID { set; get; }
        public int IDHorario { set; get; }
        public DateTime Dia { set; get; }
        public bool Disponible { set; get; }

    }
}
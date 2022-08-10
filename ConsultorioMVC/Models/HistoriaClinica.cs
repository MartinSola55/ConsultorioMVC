using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ConsultorioMVC.Models
{
    public class HistoriaClinica
    {
        public int ID { set; get; }
        public int PacienteID { get; set; }
        public string Descripcion { set; get; }
        public DateTime Fecha { get; set; }
    }
}
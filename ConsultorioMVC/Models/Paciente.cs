using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ConsultorioMVC.Models
{
    public class Paciente
    {
        public int ID { set; get; }
        public string Nombre { set; get; }
        public string Apellido { set; get; }
        public string Telefono { set; get; }
        public string Correo { set; get; }
        public int IDObraSocial { set; get; }
    }
}
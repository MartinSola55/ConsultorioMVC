using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ConsultorioMVC.Models
{
    public class Usuario
    {
        public int ID { set; get; }
        public string Email { set; get; }
        public string Password { set; get; }
    }
}
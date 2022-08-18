using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace ConsultorioMVC.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Inicio()
        {
            return View();
        }
        public ActionResult CerrarSesion()
        {
            Session["idUsuario"] = null;
            return RedirectToAction("Inicio", "Main");
        }
        public int validar(string email, string pass)
        {
            int valido;
            try
            {
                DataClasesDataContext bd = new DataClasesDataContext();
                SHA256Managed sha = new SHA256Managed();
                byte[] passNoCifrada = Encoding.Default.GetBytes(pass);
                byte[] bytesCifrados = sha.ComputeHash(passNoCifrada);
                string passCifrada = BitConverter.ToString(bytesCifrados).Replace("-", string.Empty);

                valido = bd.Usuarios.Where(u => u.email.Equals(email) && u.password.Equals(passCifrada)).Count();
                
                if (valido == 1)
                {
                    Session["idUsuario"] = bd.Usuarios.Where(u => u.email.Equals(email) && u.password.Equals(passCifrada)).First().id;
                }
            } catch (Exception e)
            {
                valido = 0;
            }
            return valido;
        }
    }
}
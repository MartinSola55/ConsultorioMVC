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
        /*public ActionResult Insertar()
        {
            string password = "Consultorio123";
            string email = "fernandorsola@yahoo.com.ar";
            DataClasesDataContext bd = new DataClasesDataContext();
            SHA256Managed sha = new SHA256Managed();
            byte[] passNoCifrada = Encoding.Default.GetBytes(password);
            byte[] bytesCifrados = sha.ComputeHash(passNoCifrada);
            string passCifrada = BitConverter.ToString(bytesCifrados).Replace("-", string.Empty);

            Usuario user = new Usuario
            {
                email = email,
                password = passCifrada
            };

            bd.Usuarios.InsertOnSubmit(user);
            bd.SubmitChanges();

            return View();
        }*/
        public ActionResult Validar(string email, string password)
        {
            try
            {
                //Cifrar contraseña
                DataClasesDataContext bd = new DataClasesDataContext();
                SHA256Managed sha = new SHA256Managed();
                byte[] passNoCifrada = Encoding.Default.GetBytes(password);
                byte[] bytesCifrados = sha.ComputeHash(passNoCifrada);
                string passCifrada = BitConverter.ToString(bytesCifrados).Replace("-", string.Empty);

                Usuario user = bd.Usuarios.Where(u => u.email.Equals(email) && u.password.Equals(passCifrada)).FirstOrDefault();
                
                if (user != null)
                {
                    Session["idUsuario"] = user.id;
                    return RedirectToAction("Inicio", "Turnos");
                }
                ViewBag.Error = 1;
                ViewBag.Message = "Email y/o contraseña incorrectos";
                return View("Inicio");
            } catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                ViewBag.Error = 2;
                return View("Inicio");
            }
        }
    }
}
using ConsultorioMVC.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace ConsultorioMVC.Controllers
{
    [Seguridad]
    public class UsuariosController : Controller
    {
        // GET: Usuarios
        public ActionResult Inicio()
        {
            return View();
        }
        public JsonResult getAll(string email)
        {
            DataClasesDataContext bd = new DataClasesDataContext();
            var usuario = from u in bd.Usuarios
                          select new
                          {
                              u.id,
                              u.email
                          };
            return Json(usuario, JsonRequestBehavior.AllowGet);
        }
        public JsonResult getOne(int id)
        {
            DataClasesDataContext bd = new DataClasesDataContext();
            var usuario = from u in bd.Usuarios
                          where u.id == id
                          select new
                          {
                              u.id,
                              u.email
                          };
            return Json(usuario, JsonRequestBehavior.AllowGet);
        }
        public int save(Usuario usuario)
        {
            DataClasesDataContext bd = new DataClasesDataContext();
            int regAfectados = 0;
            SHA256Managed sha = new SHA256Managed();
            byte[] passNoCifrada = Encoding.Default.GetBytes(usuario.password);
            byte[] passCifrada = sha.ComputeHash(passNoCifrada);
            usuario.password = BitConverter.ToString(passCifrada).Replace("-", string.Empty);
            try
            {
                int repetido = bd.Usuarios.Where(u => u.email.Equals(usuario.email) && !u.id.Equals(usuario.id)).Count();
                if (repetido == 0)
                {
                    if (usuario.id == 0)
                    {
                        bd.Usuarios.InsertOnSubmit(usuario);
                        bd.SubmitChanges();
                        regAfectados = 1;
                    }
                    else
                    {
                        Usuario usuarioOld = bd.Usuarios.Where(u => u.id.Equals(usuario.id)).First();
                        usuarioOld.email = usuario.email;
                        usuarioOld.password = usuario.password;
                        bd.SubmitChanges();
                        regAfectados = 1;
                    }
                }
                else
                {
                    regAfectados = -1;
                }
            }
            catch (Exception e)
            {
                regAfectados = 0;
            }
            return regAfectados;
        }
        public JsonResult filtraUsuarios(string email)
        {
            DataClasesDataContext bd = new DataClasesDataContext();
            var usuario = bd.Usuarios.Where(u => u.email.Contains(email))
                .Select(u => new { u.id, u.email});
            return Json(usuario, JsonRequestBehavior.AllowGet);
        }

    }
}
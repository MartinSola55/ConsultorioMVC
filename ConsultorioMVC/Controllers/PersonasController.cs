using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Linq.SqlClient;

namespace ConsultorioMVC.Controllers
{
    public class PersonasController : Controller
    {
        // GET: Personas
        public ActionResult Inicio()
        {
            return View();
        }
        public JsonResult getAll()
        {
            DataClasesDataContext bd = new DataClasesDataContext();
            var personas = from p in bd.Personas
                            join os in bd.ObrasSociales on p.obra_social_id equals os.id
                            select new
                            {
                                id = p.id,
                                nombre = p.nombre,
                                apellido = p.apellido,
                                telefono = p.telefono,
                                correo = p.correo,
                                obra_social_id = p.obra_social_id,
                                nombreOS = os.nombre
                            };
            return Json(personas, JsonRequestBehavior.AllowGet);
        }
        public JsonResult getOne(int id)
        {
            DataClasesDataContext bd = new DataClasesDataContext();
            var persona = from p in bd.Personas
                           join os in bd.ObrasSociales on p.obra_social_id equals os.id
                           where p.id == id
                            select new
                            {
                                id = p.id,
                                nombre = p.nombre,
                                apellido = p.apellido,
                                telefono = p.telefono,
                                correo = p.correo,
                                obra_social_id = p.obra_social_id,
                                nombreOS = os.nombre
                            };
            return Json(persona, JsonRequestBehavior.AllowGet);
        }
        public int save(Persona persona)
        {
            DataClasesDataContext bd = new DataClasesDataContext();
            int regAfectados = 0;
            try
            {
                if (persona.id == 0)
                {
                    bd.Personas.InsertOnSubmit(persona);
                    bd.SubmitChanges();
                    regAfectados = 1;
                }
                else
                {
                    Persona personaOld = bd.Personas.Where(p => p.id.Equals(persona.id)).First();
                    personaOld.nombre = persona.nombre;
                    personaOld.apellido = persona.apellido;
                    personaOld.telefono = persona.telefono;
                    personaOld.correo = persona.correo;
                    personaOld.obra_social_id = persona.obra_social_id;
                    bd.SubmitChanges();
                    regAfectados = 1;
                }
            }
            catch (Exception)
            {
                regAfectados = 0;
            }
            return regAfectados;
        }
        public int delete(Persona persona)
        {
            DataClasesDataContext bd = new DataClasesDataContext();
            int regAfectados = 0;
            try
            {
                Persona personaOld = bd.Personas.Where(p => p.id.Equals(persona.id)).First();
                bd.Personas.DeleteOnSubmit(personaOld);
                bd.SubmitChanges();
                regAfectados = 1;
            }
            catch (Exception)
            {
                regAfectados = 0;
            }
            return regAfectados;
        }
        public JsonResult buscarPersonasNombreApellido(string nombre, string apellido)
        {
            DataClasesDataContext bd = new DataClasesDataContext();
            var personas = from p in bd.Personas
                           join os in bd.ObrasSociales on p.obra_social_id equals os.id
                           select new
                           {
                               id = p.id,
                               nombre = p.nombre,
                               apellido = p.apellido,
                               telefono = p.telefono,
                               correo = p.correo,
                               obra_social_id = p.obra_social_id,
                               nombreOS = os.nombre
                           };
            personas = personas.Where(p => p.nombre.Contains(nombre) && p.apellido.Contains(apellido));
            return Json(personas, JsonRequestBehavior.AllowGet);
        }
        public JsonResult buscarPersonasNombApOS(string nombre, string apellido, int os)
        {
            DataClasesDataContext bd = new DataClasesDataContext();
            var personas = from p in bd.Personas
                            join osoc in bd.ObrasSociales on p.obra_social_id equals osoc.id
                            select new
                            {
                                id = p.id,
                                nombre = p.nombre,
                                apellido = p.apellido,
                                telefono = p.telefono,
                                correo = p.correo,
                                obra_social_id = p.obra_social_id,
                                nombreOS = osoc.nombre
                            };
            personas = personas.Where(p => p.nombre.Contains(nombre) && p.apellido.Contains(apellido) && p.obra_social_id.Equals(os));
            return Json(personas, JsonRequestBehavior.AllowGet);
        }
    }
}
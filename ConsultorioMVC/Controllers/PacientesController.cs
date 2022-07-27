using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ConsultorioMVC.Controllers
{
    public class PacientesController : Controller
    {
        // GET: Pacientes
        public ActionResult Inicio()
        {
            return View();
        }
        public JsonResult getAll()
        {
            DataClasesDataContext bd = new DataClasesDataContext();
            var pacientes = bd.Pacientes.Select(p=> new { p.id, p.nombre, p.apellido, p.telefono, p.correo, p.obra_social_id, nombreOS = p.ObrasSociales.nombre });
            var obras_sociales = bd.ObrasSociales.Select(p=> new { p.id, p.nombre});
            var lista = pacientes.Join(
                obras_sociales,
                pac => pac.obra_social_id,
                os => os.id,
                (pac, os) => pac);
            //var lista = bd.Pacientes.Select(p => new { p.id, p.nombre, p.apellido, p.telefono, p.correo, p.obra_social_id });
            return Json(lista, JsonRequestBehavior.AllowGet);
        }
        public JsonResult getOne(int id)
        {
            DataClasesDataContext bd = new DataClasesDataContext();
            var lista = bd.Pacientes.Where(p => p.id.Equals(id))
                .Select(p => new { p.id, p.nombre, p.apellido, p.telefono, p.correo, p.obra_social_id });
            return Json(lista, JsonRequestBehavior.AllowGet);
        }
        public int save(Paciente paciente)
        {
            DataClasesDataContext bd = new DataClasesDataContext();
            int regAfectados = 0;
            try
            {
                if (paciente.id == 0)
                {
                    bd.Pacientes.InsertOnSubmit(paciente);
                    bd.SubmitChanges();
                    regAfectados = 1;
                }
                else
                {
                    Paciente pacienteOld = bd.Pacientes.Where(p => p.id.Equals(paciente.id)).First();
                    pacienteOld.nombre = paciente.nombre;
                    pacienteOld.apellido = paciente.apellido;
                    pacienteOld.telefono = paciente.telefono;
                    pacienteOld.correo = paciente.correo;
                    pacienteOld.obra_social_id = paciente.obra_social_id;
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
        public int delete(Paciente paciente)
        {
            DataClasesDataContext bd = new DataClasesDataContext();
            int regAfectados = 0;
            try
            {
                Paciente pacienteOld = bd.Pacientes.Where(p => p.id.Equals(paciente.id)).First();
                bd.Pacientes.DeleteOnSubmit(pacienteOld);
                bd.SubmitChanges();
                regAfectados = 1;
            }
            catch (Exception)
            {
                regAfectados = 0;
            }
            return regAfectados;
        }
        public JsonResult buscarPacientesNombreApellido(string nombre, string apellido)
        {
            DataClasesDataContext bd = new DataClasesDataContext();
            var paciente = bd.Pacientes.Where(p => p.nombre.Contains(nombre) && p.apellido.Contains(apellido))
                .Select(p => new { p.id, p.nombre, p.apellido, p.telefono, p.correo, p.obra_social_id });
            return Json(paciente, JsonRequestBehavior.AllowGet);
        }
        public JsonResult buscarPacientesNombApOS(string nombre, string apellido, int os)
        {
            DataClasesDataContext bd = new DataClasesDataContext();
            var paciente = bd.Pacientes.Where(p => p.nombre.Contains(nombre) && p.apellido.Contains(apellido) && p.obra_social_id.Equals(os) )
                .Select(p => new { p.id, p.nombre, p.apellido, p.telefono, p.correo, p.obra_social_id });
            return Json(paciente, JsonRequestBehavior.AllowGet);
        }

    }
}
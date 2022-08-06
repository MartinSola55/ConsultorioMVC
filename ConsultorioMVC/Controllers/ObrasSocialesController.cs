using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ConsultorioMVC.Controllers
{
    public class ObrasSocialesController : Controller
    {
        // GET: ObrasSociales
        public ActionResult Inicio()
        {
            return View();
        }
        public JsonResult getAll()
        {
            DataClasesDataContext bd = new DataClasesDataContext();
            var lista = bd.ObrasSociales.Select(p => new { p.id, p.nombre, p.habilitada });
            return Json(lista, JsonRequestBehavior.AllowGet);
        }
        public JsonResult getHabilitadas()
        {
            DataClasesDataContext bd = new DataClasesDataContext();
            var lista = bd.ObrasSociales.Where(p=> p.habilitada).Select(p => new { p.id, p.nombre, p.habilitada });
            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        public JsonResult getOne(int id)
        {
            DataClasesDataContext bd = new DataClasesDataContext();
            var lista = bd.ObrasSociales.Where(p => p.id.Equals(id)).Select(p => new { p.id, p.nombre, p.habilitada });
            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        public int save(ObrasSociales os)
        {
            DataClasesDataContext bd = new DataClasesDataContext();
            int regAfectados = 0;
            try
            {
                if (os.id == 0)
                {
                    bd.ObrasSociales.InsertOnSubmit(os);
                    bd.SubmitChanges();
                    regAfectados = 1;
                } else
                {
                    ObrasSociales osOld =  bd.ObrasSociales.Where(p => p.id.Equals(os.id)).First();
                    osOld.nombre = os.nombre;
                    osOld.habilitada = os.habilitada;
                    bd.SubmitChanges();
                    regAfectados = 1;
                }
            } catch (Exception)
            {
                regAfectados = 0;
            }
            return regAfectados;
        }
        public int delete(ObrasSociales os)
        {
            DataClasesDataContext bd = new DataClasesDataContext();
            int regAfectados = 0;
            try
            {
                ObrasSociales osOld = bd.ObrasSociales.Where(p => p.id.Equals(os.id)).First();
                bd.ObrasSociales.DeleteOnSubmit(osOld);
                bd.SubmitChanges();
                regAfectados = 1;
            }
            catch (Exception)
            {
                regAfectados = 0;
            }
            return regAfectados;
        }
        public JsonResult filtrarObrasSocialesHabilitadas(string nombre, int habilitada)
        {
            DataClasesDataContext bd = new DataClasesDataContext();
            var os = bd.ObrasSociales.Where(p => p.nombre.Contains(nombre) && p.habilitada.Equals(habilitada))
                .Select(p => new { p.id, p.nombre, p.habilitada });
            return Json(os, JsonRequestBehavior.AllowGet);
        }

        public JsonResult buscarObraSocialNombre(string nombre)
        {
            DataClasesDataContext bd = new DataClasesDataContext();
            var os = bd.ObrasSociales.Where(p => p.nombre.Contains(nombre)).Select(p => new { p.id, p.nombre, p.habilitada });
            return Json(os, JsonRequestBehavior.AllowGet);
        }
    }
}
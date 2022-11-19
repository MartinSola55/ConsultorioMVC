using ConsultorioMVC.Filter;
using ConsultorioMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ConsultorioMVC.Controllers
{
    [Seguridad]
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
            var lista = bd.ObrasSociales.Select(p => new { p.id, p.nombre, p.habilitada }).OrderBy(p => p.nombre);
            return Json(lista, JsonRequestBehavior.AllowGet);
        }
        public JsonResult getHabilitadas()
        {
            DataClasesDataContext bd = new DataClasesDataContext();
            var particular = bd.ObrasSociales.Where(p => p.nombre.Equals("PARTICULAR")).Select(p => new { p.id, p.nombre, p.habilitada }).FirstOrDefault();
            var lista = bd.ObrasSociales.Where(p=> p.habilitada && !p.nombre.Equals("PARTICULAR")).Select(p => new { p.id, p.nombre, p.habilitada }).OrderBy(p => p.nombre);
            
            LinkedList<ObraSocial> habilitadas = new LinkedList<ObraSocial>();
            ObraSocial os = new ObraSocial { ID = particular.id, Habilitada = particular.habilitada, Nombre = particular.nombre };
            habilitadas.AddFirst(os);
            foreach (var item in lista)
            {
                ObraSocial oS = new ObraSocial { ID = item.id, Habilitada = item.habilitada, Nombre = item.nombre };
                habilitadas.AddLast(oS);
            }
            return Json(habilitadas, JsonRequestBehavior.AllowGet);
        }
        public JsonResult getOne(int id)
        {
            DataClasesDataContext bd = new DataClasesDataContext();
            var lista = bd.ObrasSociales.Where(p => p.id.Equals(id)).Select(p => new { p.id, p.nombre, p.habilitada });
            return Json(lista, JsonRequestBehavior.AllowGet);
        }
        public JsonResult getParticular()
        {
            DataClasesDataContext bd = new DataClasesDataContext();
            var particular = bd.ObrasSociales.Where(p => p.nombre.Equals("PARTICULAR")).Select(p => new { p.id, p.nombre, p.habilitada });
            return Json(particular, JsonRequestBehavior.AllowGet);
        }
        public JsonResult getResto()
        {
            DataClasesDataContext bd = new DataClasesDataContext();
            var resto = bd.ObrasSociales.Where(p => !p.nombre.Equals("PARTICULAR")).Select(p => new { p.id, p.nombre, p.habilitada }).OrderBy(p=> p.nombre);
            return Json(resto, JsonRequestBehavior.AllowGet);
        }
        public int save(ObrasSociales os)
        {
            DataClasesDataContext bd = new DataClasesDataContext();
            int regAfectados = 0;
            try
            {
                int repetido = bd.ObrasSociales
                    .Where(p => p.nombre.Equals(os.nombre)).Count();
                if (repetido == 0)
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
                } else
                {
                    regAfectados = -1;
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
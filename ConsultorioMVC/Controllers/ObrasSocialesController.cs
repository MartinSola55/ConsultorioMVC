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
        DataClasesDataContext bd = new DataClasesDataContext();
        // GET: ObrasSociales
        public ActionResult Inicio()
        {
            return View();
        }
        public JsonResult getAll()
        {
            var lista = bd.ObrasSociales.Select(p => new { p.id, p.nombre, p.habilitada }).OrderBy(p => p.nombre);
            return Json(lista, JsonRequestBehavior.AllowGet);
        }
        public JsonResult getHabilitadas()
        {
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
            var lista = bd.ObrasSociales.Where(p => p.id.Equals(id)).Select(p => new { p.id, p.nombre, p.habilitada });
            return Json(lista, JsonRequestBehavior.AllowGet);
        }
        public JsonResult getParticular()
        {
            var particular = bd.ObrasSociales.Where(p => p.nombre.Equals("PARTICULAR")).Select(p => new { p.id, p.nombre, p.habilitada });
            return Json(particular, JsonRequestBehavior.AllowGet);
        }
        public JsonResult getResto()
        {
            var resto = bd.ObrasSociales.Where(p => !p.nombre.Equals("PARTICULAR")).Select(p => new { p.id, p.nombre, p.habilitada }).OrderBy(p=> p.nombre);
            return Json(resto, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult Save(ObrasSociales os)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var repetido = bd.ObrasSociales
                        .Where(p => p.nombre.Equals(os.nombre) && !p.id.Equals(os.id)).FirstOrDefault();
                    if (repetido == null)
                    {
                        if (os.id == 0)
                        {
                            bd.ObrasSociales.InsertOnSubmit(os);
                            bd.SubmitChanges();
                        } else
                        {
                            ObrasSociales osOld =  bd.ObrasSociales.Where(p => p.id.Equals(os.id)).First();
                            osOld.nombre = os.nombre;
                            osOld.habilitada = os.habilitada;
                            bd.SubmitChanges();
                        }
                        ViewBag.Message = "La obra social se guardó correctamente";
                    }
                    else
                    {
                        ViewBag.Message = "La obra social ingresada ya existe";
                        ViewBag.Error = 1;
                    }
                } catch (Exception)
                {
                    ViewBag.Message = "Hubo un error con la base de datos. No se ha podido guardar la obra social";
                    ViewBag.Error = 2;
                }
            }
            return View("Inicio");
        }
        public ActionResult Delete(ObrasSociales os)
        {
            try
            {
                ObrasSociales osOld = bd.ObrasSociales.Where(p => p.id.Equals(os.id)).First();
                bd.ObrasSociales.DeleteOnSubmit(osOld);
                bd.SubmitChanges();
                ViewBag.Message = "La obra social se eliminó correctamente";
            }
            catch (Exception)
            {
                ViewBag.Message = "Hubo un error con la base de datos. No se ha podido eliminar la obra social";
                ViewBag.Error = 2;
            }
            return View("Inicio");
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
            var os = bd.ObrasSociales.Where(p => p.nombre.Contains(nombre)).Select(p => new { p.id, p.nombre, p.habilitada });
            return Json(os, JsonRequestBehavior.AllowGet);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ConsultorioMVC.Controllers
{
    public class TurnosController : Controller
    {
        // GET: Turnos
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult getAll()
        {
            DataClasesDataContext bd = new DataClasesDataContext();
            var turnos = bd.Turnos.Select(p=> new { p.paciente_id, p.dia_horario_id });
            return Json(turnos, JsonRequestBehavior.AllowGet);
        }
    }
}
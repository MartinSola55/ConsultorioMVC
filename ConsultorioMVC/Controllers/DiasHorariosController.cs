using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ConsultorioMVC.Controllers
{
    public class DiasHorariosController : Controller
    {
        // GET: DiaHorario
        public ActionResult Inicio()
        {
            return View();
        }
        public JsonResult getHoras(string dia)
        {
            DataClasesDataContext bd = new DataClasesDataContext();
            var horas = from dh in bd.DiaHorarios
                        join h in bd.Horarios on dh.horario_id equals h.id
                        where dh.dia == Convert.ToDateTime(dia)
                        && dh.disponible == true
                        select new
                        {
                            idHora = h.id,
                            hora = h.hora.ToShortTimeString(),
                        };
            return Json(horas, JsonRequestBehavior.AllowGet);
        }
        public int insert(string[] dias, int[] horas)
        {
            DataClasesDataContext bd = new DataClasesDataContext();
            int regAfectados = 0;
            try
            {
                List<DiaHorario> diasHorarios = new List<DiaHorario>();
                foreach (string d in dias)
                {
                    foreach (int h in horas)
                    {
                        DiaHorario dh = new DiaHorario
                        {
                            dia = Convert.ToDateTime(d),
                            horario_id = h,
                            disponible = true
                        };
                        diasHorarios.Add(dh);
                    }
                };
                foreach (DiaHorario diaH in diasHorarios)
                {
                    bd.DiaHorarios.InsertOnSubmit(diaH);
                    regAfectados++;
                }
                bd.SubmitChanges();
            }
            catch (Exception)
            {
                regAfectados = 0;
            }
            return regAfectados;
        }
    }
}
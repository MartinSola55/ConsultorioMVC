using ConsultorioMVC.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ConsultorioMVC.Controllers
{
    [Seguridad]
    public class DiasHorariosController : Controller
    {
        // GET: DiaHorario
        public ActionResult Inicio()
        {
            return View();
        }
        public JsonResult getOne(int id)
        {
            DataClasesDataContext bd = new DataClasesDataContext();
            var diaHorario = from dh in bd.DiaHorarios
                        join h in bd.Horarios
                            on dh.horario_id equals h.id
                        where dh.id == id
                        select new
                        {
                            id = dh.id,
                            dia = dh.dia.ToShortDateString(),
                            hora = h.hora.ToShortTimeString(),
                        };
            return Json(diaHorario, JsonRequestBehavior.AllowGet);
        }
        public int delete(DiaHorario dia_horario)
        {
            DataClasesDataContext bd = new DataClasesDataContext();
            int regAfectados = 0;
            try
            {
                DiaHorario dh = bd.DiaHorarios.Where(d => d.id.Equals(dia_horario.id)).First();
                bd.DiaHorarios.DeleteOnSubmit(dh);
                bd.SubmitChanges();
                regAfectados = 1;
            }
            catch (Exception)
            {
                regAfectados = 0;
            }
            return regAfectados;
        }
        public JsonResult getHoras(string dia)
        {
            DataClasesDataContext bd = new DataClasesDataContext();
            var horas = from dh in bd.DiaHorarios
                        join h in bd.Horarios
                            on dh.horario_id equals h.id
                        where dh.dia == Convert.ToDateTime(dia)
                        && dh.disponible == true
                        select new
                        {
                            idHora = h.id,
                            hora = h.hora.ToShortTimeString(),
                        };
            return Json(horas, JsonRequestBehavior.AllowGet);
        }
        public JsonResult insert(string[] dias, int[] horas)
        {
            DataClasesDataContext bd = new DataClasesDataContext();
            int regAfectados = 0;
            int regRepetidos = 0;
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
                        int repetido = bd.DiaHorarios
                            .Where(diah => diah.dia.Equals(dh.dia)
                            && diah.horario_id.Equals(dh.horario_id))
                            .Count();
                        if (repetido == 0)
                        {
                            diasHorarios.Add(dh);
                        }
                        else
                        {
                            regRepetidos++;
                        }
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
                regRepetidos = 0;
            }
            int[] resultado = { regAfectados, regRepetidos };
            return Json(resultado, JsonRequestBehavior.AllowGet);
        }
    }
}
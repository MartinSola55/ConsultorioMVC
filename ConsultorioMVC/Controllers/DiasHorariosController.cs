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
    public class DiasHorariosController : Controller
    {
        DataClasesDataContext bd = new DataClasesDataContext();
        // GET: DiaHorario
        public ActionResult Inicio()
        {
            return View(listarHorarios());
        }
        public JsonResult getOne(int id)
        {
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
        [HttpPost]
        public ActionResult Save(DiaHorarioViewModel dh)
        {
            int regAfectados = 0;
            int regRepetidos = 0;
            try
            {
                //Listado de dias
                string[] dias = dh.DiasString.Trim().Split(',');

                //Listado de horas
                string[] horas = dh.IDHorarios.Trim().Split(',');

                List<DiaHorario> diasHorarios = new List<DiaHorario>();
                foreach (string d in dias)
                {
                    foreach (string h in horas)
                    {
                        DiaHorario diaHorario = new DiaHorario
                        {
                            dia = Convert.ToDateTime(d),
                            horario_id = int.Parse(h),
                            disponible = true
                        };
                        int repetido = bd.DiaHorarios
                            .Where(diaho => diaho.dia.Equals(diaHorario.dia)
                            && diaho.horario_id.Equals(diaHorario.horario_id))
                            .Count();
                        if (repetido == 0)
                        {
                            diasHorarios.Add(diaHorario);
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
                ViewBag.Error = 1;
                regAfectados = 0;
                regRepetidos = 0;
            } 
            finally
            {
                ViewBag.Afectados = regAfectados;
                ViewBag.Repetidos = regRepetidos;
            }
            return View("Inicio", listarHorarios());
        }
        public class DiaHorarioViewModel
        {
            public List<Models.Horario> Horarios { get; set; }
            public string IDHorarios { get; set; }
            public string DiasString { get; set; }
            public List<DateTime> Dias { get; set; }
        }
        public DiaHorarioViewModel listarHorarios()
        {
            Horario[] horarios = bd.Horarios.ToArray();
            DiaHorarioViewModel dh = new DiaHorarioViewModel();
            dh.Horarios = new List<Models.Horario>();
            foreach (Horario h in horarios)
            {
                Models.Horario horario = new Models.Horario { Hora = h.hora, ID = h.id };
                dh.Horarios.Add(horario);
            }
            return dh;
        }
    }
}
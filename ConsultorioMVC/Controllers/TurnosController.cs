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
        public ActionResult Inicio()
        {
            return View();
        }
        public JsonResult getAll(string dia)
        {
            DataClasesDataContext bd = new DataClasesDataContext();
            var turnos = from t in bd.Turnos
                         join p in bd.Personas on t.persona_id equals p.id
                         join os in bd.ObrasSociales on p.obra_social_id equals os.id
                         join dh in bd.DiaHorarios on t.dia_horario_id equals dh.id
                         join h in bd.Horarios on dh.horario_id equals h.id
                         where dh.dia == Convert.ToDateTime(dia)
                         select new
                         {
                             idTurno = t.id,
                             idPersona = p.id,
                             idOS = os.id,
                             idDiaHorario = os.id,
                             idHorario = h.id,
                             nombre = p.nombre,
                             apellido = p.apellido,
                             obraSocial = os.nombre,
                             telefono = p.telefono,
                             hora = h.hora.ToShortTimeString(),
                         };
            return Json(turnos, JsonRequestBehavior.AllowGet);
        }
        public JsonResult getOne(int id)
        {
            DataClasesDataContext bd = new DataClasesDataContext();
            var turno = from t in bd.Turnos
                         join p in bd.Personas on t.persona_id equals p.id
                         join os in bd.ObrasSociales on p.obra_social_id equals os.id
                         join dh in bd.DiaHorarios on t.dia_horario_id equals dh.id
                         join h in bd.Horarios on dh.horario_id equals h.id
                         where t.id == id
                         select new
                         {
                             idTurno = t.id,
                             idPersona = p.id,
                             idOS = os.id,
                             idDiaHorario = dh.id,
                             idHorario = h.id,
                             nombre = p.nombre,
                             apellido = p.apellido,
                             obraSocial = os.nombre,
                             telefono = p.telefono,
                             correo = p.correo,
                             hora = h.hora.ToShortTimeString(),
                         };
            return Json(turno, JsonRequestBehavior.AllowGet);
        }
        public JsonResult getHoras(string dia, int hora)
        {
            DataClasesDataContext bd = new DataClasesDataContext();
            var horas = from dh in bd.DiaHorarios
                        join h in bd.Horarios on dh.horario_id equals h.id
                        where dh.dia == Convert.ToDateTime(dia) && 
                        (dh.disponible == true || h.id == hora)
                        select new
                        {
                            idHora = h.id,
                            hora = h.hora.ToShortTimeString(),
                        };
            return Json(horas, JsonRequestBehavior.AllowGet);
        }
        public int save(Turno turno)
        {
            DataClasesDataContext bd = new DataClasesDataContext();
            int regAfectados = 0;
            try
            {
                var diaH = (from dh in bd.DiaHorarios
                            where dh.dia == turno.DiaHorario.dia
                            && dh.horario_id == turno.DiaHorario.horario_id
                            select dh).FirstOrDefault();
                diaH.disponible = false;
                if (turno.id == 0)
                {
                    var lastP = bd.Personas.ToArray().LastOrDefault();
                    var persona = new Persona
                    {
                        id = lastP.id + 1,
                        nombre = turno.Persona.nombre,
                        apellido = turno.Persona.apellido,
                        correo = turno.Persona.correo,
                        obra_social_id = turno.Persona.obra_social_id,
                        telefono = turno.Persona.telefono
                    };

                    var turnoNew = new Turno
                    {
                        persona_id = persona.id,
                        dia_horario_id = diaH.id
                    };

                    bd.Personas.InsertOnSubmit(persona);
                    bd.Turnos.InsertOnSubmit(turnoNew);
                    bd.SubmitChanges();
                    regAfectados = 1;
                }
                else
                {
                    var turnoActual = (from t in bd.Turnos
                                      where t.id == turno.id
                                      select t).FirstOrDefault();
                    var persona = (from p in bd.Personas
                                   where p.id == turno.Persona.id
                                   select p).FirstOrDefault();
                    var diaHOld = (from dh in bd.DiaHorarios
                                   where dh.id == turno.DiaHorario.id
                                   select dh).FirstOrDefault();

                    persona.nombre = turno.Persona.nombre;
                    persona.apellido = turno.Persona.apellido;
                    persona.telefono = turno.Persona.telefono;
                    persona.correo = turno.Persona.correo;
                    persona.obra_social_id = turno.Persona.obra_social_id;

                    turnoActual.persona_id = turno.persona_id;
                    turnoActual.dia_horario_id = diaH.id;

                    diaHOld.disponible = true;

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
    }
}
using ConsultorioMVC.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Transactions;
using ConsultorioMVC.Models;

namespace ConsultorioMVC.Controllers
{
    [Seguridad]
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
            var query = from dh in bd.DiaHorarios
                         join h in bd.Horarios
                            on dh.horario_id equals h.id
                         join tur in bd.Turnos
                            on dh.id equals tur.dia_horario_id
                            into turno
                            from t in turno.DefaultIfEmpty()
                         join per in bd.Personas
                            on t.persona_id equals per.id
                            into persona
                            from p in persona.DefaultIfEmpty()
                         join obraS in bd.ObrasSociales
                            on p.obra_social_id equals obraS.id
                            into obraSocial
                            from os in obraSocial.DefaultIfEmpty()
                         where dh.dia == Convert.ToDateTime(dia)
                         select new
                         {
                             idTurno = (t ?? new Turno { id = 0}).id,
                             idPersona = (p ?? new Persona { id = 0 }).id,
                             idOS = (os ?? new ObrasSociales { id = 0 }).id,
                             idDiaHorario = dh.id,
                             idHorario = h.id,
                             nombre = (p ?? new Persona { id = 0, nombre = "" }).nombre,
                             apellido = (p ?? new Persona { id = 0, apellido = "" }).apellido,
                             obraSocial = (os ?? new ObrasSociales { id = 0, nombre = "" }).nombre,
                             telefono = (p ?? new Persona { id = 0, telefono = "" }).telefono,
                             hora = h.hora,
                             disponible = dh.disponible
                         };
            LinkedList<Models.Turno> turnos = new LinkedList<Models.Turno>();
            foreach (var var in query)
            {
                Models.Turno turno = new Models.Turno
                {
                    ID = var.idTurno,
                    Persona = new Models.Persona { ID = var.idPersona, Nombre = var.nombre, Apellido = var.apellido, Telefono = var.telefono, ObraSocial = new ObraSocial { ID = var.idOS, Nombre = var.obraSocial } },
                    DiaHorario = new Models.DiaHorario { ID = var.idDiaHorario, Disponible = var.disponible, Horario = new Models.Horario { ID = var.idHorario, HoraString = var.hora.ToString("HH:mm") } }
                };
                turnos.AddLast(turno);
            }
            return Json(turnos, JsonRequestBehavior.AllowGet);
        }
        public JsonResult getOne(int id)
        {
            DataClasesDataContext bd = new DataClasesDataContext();
            var turno = from t in bd.Turnos
                         join p in bd.Personas
                            on t.persona_id equals p.id
                         join os in bd.ObrasSociales
                            on p.obra_social_id equals os.id
                         join dh in bd.DiaHorarios
                            on t.dia_horario_id equals dh.id
                         join h in bd.Horarios
                            on dh.horario_id equals h.id
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
                             disponible = dh.disponible
                         };
            return Json(turno, JsonRequestBehavior.AllowGet);
        }
        public JsonResult getHoras(string dia, int hora)
        {
            DataClasesDataContext bd = new DataClasesDataContext();
            var horas = from dh in bd.DiaHorarios
                        join h in bd.Horarios
                            on dh.horario_id equals h.id
                        where dh.dia == Convert.ToDateTime(dia)
                        && (dh.disponible == true || h.id == hora)
                        orderby h.hora ascending
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
                    var persona = new Persona
                    {
                        nombre = turno.Persona.nombre,
                        apellido = turno.Persona.apellido,
                        correo = turno.Persona.correo,
                        obra_social_id = turno.Persona.obra_social_id,
                        telefono = turno.Persona.telefono
                    };

                    var turnoNew = new Turno
                    {
                        dia_horario_id = diaH.id
                    };

                    using(var transaccion = new TransactionScope())
                    {
                        bd.Personas.InsertOnSubmit(persona);
                        bd.SubmitChanges();

                        turnoNew.persona_id = persona.id;
                        bd.Turnos.InsertOnSubmit(turnoNew);
                        bd.SubmitChanges();
                        transaccion.Complete();
                    }
                    regAfectados = 1;
                }
                else
                {
                    var turnoActual = (from t in bd.Turnos
                                      where t.id == turno.id
                                      select t).First();
                    var persona = (from p in bd.Personas
                                   where p.id == turno.Persona.id
                                   select p).First();
                    var diaHOld = (from dh in bd.DiaHorarios
                                   where dh.id == turno.DiaHorario.id
                                   select dh).First();

                    persona.nombre = turno.Persona.nombre;
                    persona.apellido = turno.Persona.apellido;
                    persona.telefono = turno.Persona.telefono;
                    persona.correo = turno.Persona.correo;
                    persona.obra_social_id = turno.Persona.obra_social_id;

                    turnoActual.persona_id = turno.persona_id;
                    turnoActual.dia_horario_id = diaH.id;

                    if (diaHOld.id != diaH.id)
                    {
                        diaHOld.disponible =  true;
                    }

                    bd.SubmitChanges();
                    regAfectados = 1;
                }
            }
            catch (Exception e)
            {
                regAfectados = 0;
            }
            return regAfectados;
        }
        public int delete(Turno turno)
        {
            DataClasesDataContext bd = new DataClasesDataContext();
            int regAfectados = 0;
            try
            {
                using (var transaccion = new TransactionScope())
                {
                    Turno tur = bd.Turnos.Where(t => t.id.Equals(turno.id)).First();
                    bd.Turnos.DeleteOnSubmit(tur);
                    bd.SubmitChanges();

                    Persona per = bd.Personas.Where(p => p.id.Equals(turno.persona_id)).First();
                    bd.Personas.DeleteOnSubmit(per);
                    DiaHorario diaH = bd.DiaHorarios.Where(dh => dh.id.Equals(turno.dia_horario_id)).First();
                    diaH.disponible = true;
                    bd.SubmitChanges();

                    transaccion.Complete();
                }

                regAfectados = 1;
            }
            catch (Exception)
            {
                regAfectados = 0;
            }
            return regAfectados;
        }
    }
}
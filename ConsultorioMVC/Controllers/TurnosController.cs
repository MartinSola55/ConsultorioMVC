using ConsultorioMVC.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Transactions;
using ConsultorioMVC.Models;
using System.Globalization;
using System.Text.RegularExpressions;

namespace ConsultorioMVC.Controllers
{
    [Seguridad]
    public class TurnosController : Controller
    {
        DataClasesDataContext bd = new DataClasesDataContext();
        // GET: Turnos
        public ActionResult Inicio()
        {
            ViewBag.listadoObrasSociales = listadoObrasSociales();
            ViewBag.listadoHorarios = listadoHorarios();
            return View();
        }
        public JsonResult getAll(string dia)
        {
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
        [HttpPost]
        public void Save(Models.Turno turno)
        {
            if (!ModelState.IsValid)
            {
                try
                {
                    var diaH = (from dh in bd.DiaHorarios
                                where dh.dia == turno.DiaHorario.Dia
                                && dh.horario_id == turno.DiaHorario.Horario.ID
                                select dh).FirstOrDefault();
                    diaH.disponible = false;

                    var repetido = bd.Turnos.Where(t => t.DiaHorario.dia.Equals(turno.DiaHorario.Dia) && t.DiaHorario.horario_id.Equals(turno.DiaHorario.Horario.ID) && !t.id.Equals(turno.ID)).FirstOrDefault();

                    if (repetido == null) {
                        if (turno.ID == 0)
                        {
                            Persona persona = new Persona
                            {
                                nombre = toUpperFirst(turno.Persona.Nombre),
                                apellido = toUpperFirst(turno.Persona.Apellido),
                                telefono = toNumber(turno.Persona.Telefono),
                                obra_social_id = turno.Persona.ObraSocial.ID,
                            };

                            if (turno.Persona.Correo != null)
                            {
                                persona.correo = turno.Persona.Correo.ToLower();
                            }

                            var turnoNew = new Turno
                            {
                                dia_horario_id = diaH.id
                            };

                            using (var transaccion = new TransactionScope())
                            {
                                bd.Personas.InsertOnSubmit(persona);
                                bd.SubmitChanges();

                                turnoNew.persona_id = persona.id;
                                bd.Turnos.InsertOnSubmit(turnoNew);
                                bd.SubmitChanges();
                                transaccion.Complete();
                            }
                        }
                        else
                        {
                            var turnoActual = (from t in bd.Turnos
                                              where t.id == turno.ID
                                              select t).First();
                            var persona = (from p in bd.Personas
                                           where p.id == turno.Persona.ID
                                           select p).First();
                            var diaHOld = (from dh in bd.DiaHorarios
                                           where dh.id == turno.DiaHorario.ID
                                           select dh).First();

                            persona.nombre = turno.Persona.Nombre;
                            persona.apellido = turno.Persona.Apellido;
                            persona.telefono = turno.Persona.Telefono;
                            persona.correo = turno.Persona.Correo;
                            persona.obra_social_id = turno.Persona.ObraSocial.ID;

                            turnoActual.persona_id = turno.Persona.ID;
                            turnoActual.dia_horario_id = diaH.id;

                            if (diaHOld.id != diaH.id)
                            {
                                diaHOld.disponible =  true;
                            }

                            bd.SubmitChanges();
                        }
                    }
                }
                catch (Exception)
                {

                }
                ViewBag.listadoObrasSociales = listadoObrasSociales();
                ViewBag.listadoHorarios = listadoHorarios();
            }
        }
        [HttpPost]
        public ActionResult Delete(Models.Turno turno)
        {
            try
            {
                using (var transaccion = new TransactionScope())
                {
                    Turno tur = bd.Turnos.Where(t => t.id.Equals(turno.ID)).First();
                    bd.Turnos.DeleteOnSubmit(tur);
                    bd.SubmitChanges();

                    Persona per = bd.Personas.Where(p => p.id.Equals(turno.Persona.ID)).First();
                    bd.Personas.DeleteOnSubmit(per);
                    DiaHorario diaH = bd.DiaHorarios.Where(dh => dh.id.Equals(turno.DiaHorario.ID)).First();
                    diaH.disponible = true;
                    bd.SubmitChanges();

                    transaccion.Complete();
                }

                ViewBag.Message = "El turno se eliminó correctamente";
            }
            catch (Exception)
            {
                ViewBag.Message = "Hubo un error al eliminar el turno";
                ViewBag.Error = 1;
            }
            ViewBag.listadoObrasSociales = listadoObrasSociales();
            ViewBag.listadoHorarios = listadoHorarios();
            return View("Inicio");
        }
        public string toUpperFirst(string titulo)
        {
            titulo = Regex.Replace(titulo, @"[^A-Za-zñáéíóúÁÉÍÓÚÑÜü' ]", string.Empty);
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(titulo.ToLower());
        }
        public string toNumber(string numero)
        {
            numero = Regex.Replace(numero, @"[^0-9]", string.Empty);
            return numero;
        }
        public IEnumerable<SelectListItem> listadoObrasSociales()
        {
            IEnumerable<SelectListItem> lista = null;
            try
            {
                IEnumerable<ObrasSociales> particular = bd.ObrasSociales.Where(o => o.nombre.Equals("PARTICULAR"));
                IEnumerable<ObrasSociales> obrasSociales = bd.ObrasSociales.ToList().OrderBy(o => o.nombre).Where(o => o.habilitada.Equals(true));

                LinkedList<ObrasSociales> listado = new LinkedList<ObrasSociales>();
                listado.AddFirst(new ObrasSociales { id = particular.First().id, nombre = particular.First().nombre });
                foreach (var item in obrasSociales)
                {
                    listado.AddLast(new ObrasSociales { id = item.id, nombre = item.nombre });
                }

                lista = listado.Select(o => new SelectListItem { Text = o.nombre, Value = o.id.ToString() });
            }
            catch (Exception)
            {

            }
            return lista;
        }
        public IEnumerable<SelectListItem> listadoHorarios()
        {
            IEnumerable<SelectListItem> lista = null;
            try
            {
                IEnumerable<Horario> horarios = bd.Horarios.ToList();
                lista = horarios.Select(ho => new SelectListItem { Text = ho.hora.ToShortTimeString(), Value = ho.id.ToString() });
            }
            catch (Exception)
            {

            }
            return lista;
        }
    }
}
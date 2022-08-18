using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Globalization;
using System.Text.RegularExpressions;

namespace ConsultorioMVC.Controllers
{
    public class MainController : Controller
    {
        // GET: Main
        public ActionResult Inicio()
        {
            return View();
        }
        public ActionResult Contacto()
        {
            return View();
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

        public JsonResult getHoras(string dia)
        {
            DataClasesDataContext bd = new DataClasesDataContext();
            var horas = from dh in bd.DiaHorarios
                        join h in bd.Horarios
                            on dh.horario_id equals h.id
                        where dh.dia == Convert.ToDateTime(dia)
                        && dh.disponible == true
                        orderby h.hora ascending
                        select new
                        {
                            idHora = h.id,
                            hora = h.hora.ToShortTimeString(),
                        };
            return Json(horas, JsonRequestBehavior.AllowGet);
        }
        public JsonResult getOSParticular()
        {
            DataClasesDataContext bd = new DataClasesDataContext();
            var particular = bd.ObrasSociales.Where(p => p.nombre.Equals("PARTICULAR")).Select(p => new { p.id, p.nombre, p.habilitada });
            return Json(particular, JsonRequestBehavior.AllowGet);
        }
        public JsonResult getOSHabilitadas()
        {
            DataClasesDataContext bd = new DataClasesDataContext();
            var lista = bd.ObrasSociales.Where(p => p.habilitada && !p.nombre.Equals("PARTICULAR")).Select(p => new { p.id, p.nombre, p.habilitada }).OrderBy(p => p.nombre);
            return Json(lista, JsonRequestBehavior.AllowGet);
        }
        public int saveTurno(Turno turno)
        {
            DataClasesDataContext bd = new DataClasesDataContext();
            int regAfectados = 0;
            try
            {
                Persona persona = new Persona
                {
                    nombre = toUpperFirst(turno.Persona.nombre),
                    apellido = toUpperFirst(turno.Persona.apellido),
                    telefono = toNumber(turno.Persona.telefono),
                    obra_social_id = turno.Persona.obra_social_id,
                    correo = turno.Persona.correo
                };

                var diaH = (from dh in bd.DiaHorarios
                            where dh.dia == turno.DiaHorario.dia
                            && dh.horario_id == turno.DiaHorario.horario_id
                            select dh).FirstOrDefault();
                diaH.disponible = false;

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
                regAfectados = 1;
            }
            catch (Exception e)
            {
                regAfectados = 0;
            }
            return regAfectados;
        }
    }
}
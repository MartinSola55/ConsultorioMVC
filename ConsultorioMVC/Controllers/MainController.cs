using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Globalization;
using System.Text.RegularExpressions;
using ConsultorioMVC.Models;
using System.Net.Mail;
using System.Net;
using System.IO;
using System.Runtime.InteropServices.ComTypes;


namespace ConsultorioMVC.Controllers
{
    public class MainController : Controller
    {
        DataClasesDataContext bd = new DataClasesDataContext();

        // GET: Main
        public ActionResult Inicio()
        {
            ViewBag.listadoObrasSociales = listadoObrasSociales();
            ViewBag.listadoHorarios = listadoHorarios();
            return View();
        }
        [HttpPost]
        public ActionResult Inicio(Models.Turno turno)
        {
            ViewBag.listadoObrasSociales = listadoObrasSociales();
            ViewBag.listadoHorarios = listadoHorarios();
            if (ModelState.IsValid)
            {
                return View(turno);
            }
            return RedirectToAction(nameof(Inicio));
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
            var particular = bd.ObrasSociales.Where(p => p.nombre.Equals("PARTICULAR")).Select(p => new { p.id, p.nombre, p.habilitada });
            return Json(particular, JsonRequestBehavior.AllowGet);
        }
        public JsonResult getOSHabilitadas()
        {
            var lista = bd.ObrasSociales.Where(p => p.habilitada && !p.nombre.Equals("PARTICULAR")).Select(p => new { p.id, p.nombre, p.habilitada }).OrderBy(p => p.nombre);
            return Json(lista, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult Save(Models.Turno turno)
        {
            if (!ModelState.IsValid)
            {
                try
                {
                    Persona persona = new Persona
                    {
                        nombre = toUpperFirst(turno.Persona.Nombre),
                        apellido = toUpperFirst(turno.Persona.Apellido),
                        telefono = toNumber(turno.Persona.Telefono),
                        obra_social_id = turno.Persona.ObraSocial.ID,
                        correo = turno.Persona.Correo
                    };

                    var diaH = (from dh in bd.DiaHorarios
                                where dh.dia == turno.DiaHorario.Dia
                                && dh.horario_id == turno.DiaHorario.Horario.ID
                                select dh).FirstOrDefault();
                    diaH.disponible = false;

                    var turnoNew = new Turno
                    {
                        dia_horario_id = diaH.id
                    };

                    var repetido = bd.Turnos.Where(t => t.DiaHorario.dia.Equals(turno.DiaHorario.Dia) && t.DiaHorario.horario_id.Equals(turno.DiaHorario.Horario.ID)).FirstOrDefault();
                
                    if (repetido == null)
                    {
                        using (var transaccion = new TransactionScope())
                        {
                            bd.Personas.InsertOnSubmit(persona);
                            bd.SubmitChanges();

                            turnoNew.persona_id = persona.id;
                            bd.Turnos.InsertOnSubmit(turnoNew);
                            bd.SubmitChanges();
                            transaccion.Complete();
                        }
                        ViewBag.Message = "Tu turno se otorgó correctamente";

                        //Enviar email
                        try
                        {
                            if (turno.Persona.Correo != null)
                            {
                                SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);

                                string email = Environment.GetEnvironmentVariable("ENV_EMAIL");
                                string password = Environment.GetEnvironmentVariable("ENV_PASSWORD");

                                smtp.Credentials = new NetworkCredential(email, password);
                                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                                smtp.EnableSsl = true;

                                MailMessage mail = new MailMessage();
                                mail.From = new MailAddress("turnos@fernandosolatraumatologia.com.ar", "Dr. Fernando Sola");
                                mail.To.Add(new MailAddress(turno.Persona.Correo));
                                mail.Subject = "Tu turno ha sido confirmado";
                                mail.IsBodyHtml = true;

                                string path = "~/Views/Main/Email.html";
                                string body = System.IO.File.ReadAllText(System.Web.HttpContext.Current.Server.MapPath(path));
                                body = body.Replace("$NOMBRE", turno.Persona.Apellido + ", " + turno.Persona.Nombre);
                                body = body.Replace("$DIA", turno.DiaHorario.Dia.ToShortDateString());
                                body = body.Replace("$HORA", diaH.Horario.hora.ToShortTimeString());
                                body = body.Replace("$ANIO", DateTime.Now.Year.ToString());
                                mail.Body = body;

                                smtp.Send(mail);
                                smtp.Dispose();
                                smtp = null;

                                ViewBag.EmailMessage = "Por favor, revise su bandeja de entrada o spam";
                            }
                        }
                        catch (Exception)
                        {
                                ViewBag.EmailMessage = "No ha sido posible enviar el recordatorio por correo";
                        }
                        turno.DiaHorario.Horario.Hora = diaH.Horario.hora;
                    } else
                    {
                        ViewBag.Message = "El turno ya ha sido otorgado";
                        ViewBag.Error = 1;
                    }
                }
                catch (Exception)
                {
                    ViewBag.Message = "Hubo un error con la base de datos. No se ha podido otorgar tu turno";
                    ViewBag.Error = 2;
                }
                ViewBag.listadoObrasSociales = listadoObrasSociales();
                ViewBag.listadoHorarios = listadoHorarios();
                return View("Inicio", turno);
            }
            ViewBag.listadoObrasSociales = listadoObrasSociales();
            ViewBag.listadoHorarios = listadoHorarios();
            return RedirectToAction(nameof(Inicio));
        }
        public IEnumerable<SelectListItem> listadoObrasSociales()
        {
            IEnumerable<SelectListItem> lista = null;
            try
            {
                IEnumerable<ObrasSociales> particular = bd.ObrasSociales.Where(o => o.nombre.Equals("PARTICULAR"));                
                IEnumerable<ObrasSociales> obrasSociales = bd.ObrasSociales.ToList().OrderBy(o => o.nombre).Where(o => o.habilitada.Equals(true));

                LinkedList<ObrasSociales> listado = new LinkedList<ObrasSociales>();
                listado.AddFirst(new ObrasSociales { id = particular.First().id, nombre = particular.First().nombre});
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
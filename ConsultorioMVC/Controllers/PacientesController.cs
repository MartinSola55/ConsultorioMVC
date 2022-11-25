using ConsultorioMVC.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Linq.SqlClient;
using ConsultorioMVC.Models;
using System.Globalization;
using System.Text.RegularExpressions;

namespace ConsultorioMVC.Controllers
{
    [Seguridad]
    public class PacientesController : Controller
    {
        DataClasesDataContext bd = new DataClasesDataContext();
        // GET: Pacientes
        public ActionResult Inicio()
        {
            ViewBag.listadoObrasSociales = listadoObrasSociales();
            return View();
        }
        public ActionResult DatosPaciente()
        {
            if (TempData.Count == 1)
            {
                ViewBag.Message = TempData["Message"].ToString();
            }
            else if (TempData.Count == 2)
            {
                ViewBag.Message = TempData["Message"].ToString();
                ViewBag.Error = TempData["Error"];
            }
            return View();
        }
        public JsonResult getOne(int id)
        {
            var paciente = from p in bd.Pacientes
                            join osoc in bd.ObrasSociales
                                on p.obra_social_id equals osoc.id
                                into pac
                                from obraSoc in pac.DefaultIfEmpty()
                            where p.id == id
                            select new
                            {
                                id = p.id,
                                nombre = p.nombre,
                                apellido = p.apellido,
                                telefono = p.telefono,
                                direccion = p.direccion,
                                localidad = p.localidad,
                                fecha_nac = p.fecha_nacimiento.ToString(),
                                obra_social_id = p.obra_social_id,
                                nombreOS = obraSoc.nombre
                            };
            return Json(paciente, JsonRequestBehavior.AllowGet);
        }
        public string toUpperFirst(string titulo)
        {
            titulo = Regex.Replace(titulo, @"[^A-Za-z\u00C0-\u017F\s']", string.Empty);
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(titulo.ToLower());
        }
        public string toNumber(string numero)
        {
            numero = Regex.Replace(numero, @"[^0-9]", string.Empty);
            return numero;
        }
        [HttpPost]
        public ActionResult Save(Models.Paciente pac)
        {
                try
                {
                    Paciente paciente = new Paciente
                    {
                        id = pac.ID,
                        nombre = this.toUpperFirst(pac.Nombre),
                        apellido = this.toUpperFirst(pac.Apellido),
                        direccion = pac.Direccion,
                        fecha_nacimiento = pac.FechaNacimiento,
                        localidad = pac.Localidad,
                        telefono = this.toNumber(pac.Telefono),
                        obra_social_id = pac.ObraSocial.ID
                    };
                    var repetido = bd.Pacientes
                            .Where(p => p.nombre.Contains(paciente.nombre)
                            && p.apellido.Contains(paciente.apellido)
                            && p.fecha_nacimiento.Equals(paciente.fecha_nacimiento)
                            && !p.id.Equals(paciente.id))
                            .FirstOrDefault();
                    if (repetido == null)
                    {
                        if (paciente.id == 0)
                        {
                            bd.Pacientes.InsertOnSubmit(paciente);
                            bd.SubmitChanges();
                            ViewBag.Message = "El paciente se guardó correctamente";
                        }
                        else
                        {
                            Paciente pacienteOld = bd.Pacientes.Where(p => p.id.Equals(paciente.id)).First();
                            pacienteOld.nombre = paciente.nombre;
                            pacienteOld.apellido = paciente.apellido;
                            pacienteOld.telefono = paciente.telefono;
                            pacienteOld.direccion = paciente.direccion;
                            pacienteOld.localidad = paciente.localidad;
                            pacienteOld.fecha_nacimiento = paciente.fecha_nacimiento;
                            pacienteOld.obra_social_id = paciente.obra_social_id;
                            bd.SubmitChanges();
                            ViewBag.Message = "El paciente se guardó correctamente";
                        }
                    } else
                    {
                        ViewBag.Message = "El paciente ingresado ya existe";
                        ViewBag.Error = 1;
                    }
                }
                catch (Exception)
                {
                    ViewBag.Message = "Hubo un error con la base de datos. No se ha podido guardar el paciente";
                    ViewBag.Error = 2;
                }
            ViewBag.listadoObrasSociales = listadoObrasSociales();
            return View("Inicio");
        }
        [HttpPost]
        public ActionResult Delete(Models.Paciente paciente)
        {
            try
            {
                Paciente pacienteOld = bd.Pacientes.Where(p => p.id.Equals(paciente.ID)).First();
                bd.Pacientes.DeleteOnSubmit(pacienteOld);
                bd.SubmitChanges();
                ViewBag.Message = "El paciente se eliminó correctamente";
            }
            catch (Exception)
            {
                ViewBag.Message = "Hubo un error con la base de datos. No se ha podido eliminar el paciente";
                ViewBag.Error = 2;
            }
            ViewBag.listadoObrasSociales = listadoObrasSociales();
            return View("Inicio");
        }
        public JsonResult filtrarPacientes(string nombre, string apellido, string nacimiento, int os)
        {
            var pacientes = from p in bd.Pacientes
                            join osoc in bd.ObrasSociales
                                on p.obra_social_id equals osoc.id
                                into paciente
                                from obraSoc in paciente.DefaultIfEmpty()
                            select new
                            {
                                id = p.id,
                                nombre = p.nombre,
                                apellido = p.apellido,
                                telefono = p.telefono,
                                direccion = p.direccion,
                                localidad = p.localidad,
                                fecha_nac = p.fecha_nacimiento.ToString(),
                                obra_social_id = p.obra_social_id,
                                nombreOS = obraSoc.nombre
                            };
            string fechaFormated = "";
            if (nacimiento != "")
            {
                DateTime fecha = Convert.ToDateTime(nacimiento);
                string dia = fecha.Day.ToString();
                dia = dia.Length == 1 ? "0" + dia : dia;
                string mes = fecha.Month.ToString();
                mes = mes.Length == 1 ? "0" + mes : mes;
                string anio = fecha.Year.ToString();
                fechaFormated = anio + "-" + mes + "-" + dia;
            }
            if (os != 0 || nacimiento != "" || nombre != "" || apellido != "")
            {
                if (os == 0 && nacimiento == "")
                {
                    pacientes = pacientes.Where(p => p.nombre.Contains(nombre) && p.apellido.Contains(apellido));
                }
                else if (os == 0)
                {
                    pacientes = from p in pacientes
                                where p.nombre.Contains(nombre) && p.apellido.Contains(apellido) && p.fecha_nac.Equals(fechaFormated)
                                select p;
                }
                else if (nacimiento == "")
                {
                    pacientes = pacientes.Where(p => p.nombre.Contains(nombre) && p.apellido.Contains(apellido) && p.obra_social_id.Equals(os));
                }
                else
                {
                    pacientes = pacientes.Where(p => p.nombre.Contains(nombre) && p.apellido.Contains(apellido) && p.obra_social_id.Equals(os) && p.fecha_nac.Equals(fechaFormated));
                }
            }
            return Json(pacientes, JsonRequestBehavior.AllowGet);
        }
        public JsonResult getHistoriasClinicas(int id)
        {
            var hist_clinicas = from hc in bd.HistoriasClinicas
                            where hc.paciente_id == id
                            orderby hc.fecha descending
                            select new
                            {
                                idHC = hc.id,
                                descripcion = hc.descripcion,
                                fecha = hc.fecha.ToString()
                            };
            return Json(hist_clinicas, JsonRequestBehavior.AllowGet);
        }
        public JsonResult getHC(int id)
        {
            DataClasesDataContext bd = new DataClasesDataContext();
            var historiaClinica = from hc in bd.HistoriasClinicas
                                  where hc.id == id
                                  select new
                                  {
                                      idHC = hc.id,
                                      descripcion = hc.descripcion,
                                      fecha = hc.fecha.ToString()
                                  };
            return Json(historiaClinica, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult SaveHC(Models.HistoriaClinica histc)
        {
            if (ModelState.IsValid)
            {
                HistoriasClinica historiaC = new HistoriasClinica
                {
                    id = histc.ID,
                    descripcion = histc.Descripcion,
                    fecha = histc.Fecha,
                    paciente_id = histc.IDPaciente
                };
                try
                {
                    if (historiaC.id == 0)
                    {
                        bd.HistoriasClinicas.InsertOnSubmit(historiaC);
                        bd.SubmitChanges();
                        TempData["Message"] = "La historia clínica se guardó correctamente";
                    }
                    else
                    {
                        HistoriasClinica historiaOld = bd.HistoriasClinicas.Where(hc => hc.id.Equals(historiaC.id)).First();
                        historiaOld.fecha = historiaC.fecha;
                        historiaOld.descripcion = historiaC.descripcion;
                        bd.SubmitChanges();
                        TempData["Message"] = "La historia clínica se actualizó correctamente";
                    }
                }
                catch (Exception)
                {
                    TempData["Message"] = "Hubo un error con la base de datos. No se ha podido guardar la historia clínica";
                    TempData["Error"] = 2;
                }
            }
            return RedirectToAction("DatosPaciente", new { id = histc.IDPaciente });
        }
        [HttpPost]
        public ActionResult DeleteHC(Models.HistoriaClinica historiaC)
        {
            try
            {
                HistoriasClinica historiaOld = bd.HistoriasClinicas.Where(hc => hc.id.Equals(historiaC.ID)).First();
                bd.HistoriasClinicas.DeleteOnSubmit(historiaOld);
                bd.SubmitChanges();
                TempData["Message"] = "La historia clínica se eliminó correctamente";
            }
            catch (Exception)
            {
                TempData["Message"] = "Hubo un error con la base de datos. No se ha podido eliminar la historia clínica";
                TempData["Error"] = 2;
            }
            return RedirectToAction("DatosPaciente", new { id = historiaC.IDPaciente });
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
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Linq.SqlClient;

namespace ConsultorioMVC.Controllers
{
    public class PacientesController : Controller
    {
        // GET: Pacientes
        public ActionResult Inicio()
        {
            return View();
        }
        public ActionResult DatosPaciente(int id)
        {
            Paciente paciente = this.getPaciente(id);
            return View();
        }
        private Paciente getPaciente(int id)
        {
            DataClasesDataContext bd = new DataClasesDataContext();
            Paciente paciente = new Paciente();
            var dbPac = from p in bd.Pacientes
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
            return paciente;
        }
        public JsonResult getOne(int id)
        {
            DataClasesDataContext bd = new DataClasesDataContext();
            var paciente = from p in bd.Pacientes
                            join osoc in bd.ObrasSociales on p.obra_social_id equals osoc.id
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
        public int save(Paciente paciente)
        {
            DataClasesDataContext bd = new DataClasesDataContext();
            int regAfectados = 0;
            try
            {
                if (paciente.id == 0)
                {
                    bd.Pacientes.InsertOnSubmit(paciente);
                    bd.SubmitChanges();
                    regAfectados = 1;
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
                    regAfectados = 1;
                }
            }
            catch (Exception)
            {
                regAfectados = 0;
            }
            return regAfectados;
        }
        public int delete(Paciente paciente)
        {
            DataClasesDataContext bd = new DataClasesDataContext();
            int regAfectados = 0;
            try
            {
                Paciente pacienteOld = bd.Pacientes.Where(p => p.id.Equals(paciente.id)).First();
                bd.Pacientes.DeleteOnSubmit(pacienteOld);
                bd.SubmitChanges();
                regAfectados = 1;
            }
            catch (Exception)
            {
                regAfectados = 0;
            }
            return regAfectados;
        }
        public JsonResult filtrarPacientes(string nombre, string apellido, string nacimiento, int os)
        {
            DataClasesDataContext bd = new DataClasesDataContext();
            var pacientes = from p in bd.Pacientes
                            join osoc in bd.ObrasSociales on p.obra_social_id equals osoc.id
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

            if (os == 0 && nacimiento == "")
            {
                pacientes = pacientes.Where(p => p.nombre.Contains(nombre) && p.apellido.Contains(apellido));
            }
            else if (os == 0)
            {
                DateTime fecha = Convert.ToDateTime(nacimiento);
                string dia = fecha.Day.ToString();
                dia = dia.Length == 1 ? "0" + dia : dia;
                string mes = fecha.Month.ToString();
                mes = mes.Length == 1 ? "0" + mes : mes;
                string anio = fecha.Year.ToString();
                string fechaOK = anio + "-" + mes + "-" + dia;

                pacientes = from p in pacientes
                            where p.nombre.Contains(nombre) && p.apellido.Contains(apellido) && p.fecha_nac == fechaOK
                            select p;
            }
            else if (nacimiento == "")
            {
                pacientes = pacientes.Where(p => p.nombre.Contains(nombre) && p.apellido.Contains(apellido) && p.obra_social_id.Equals(os));
            }
            else
            {
                pacientes = pacientes.Where(p => p.nombre.Contains(nombre) && p.apellido.Contains(apellido) && p.obra_social_id.Equals(os) && p.fecha_nac.Equals(nacimiento));
            }
            return Json(pacientes, JsonRequestBehavior.AllowGet);
        }
        public JsonResult getHistoriasClinicas(int id)
        {
            DataClasesDataContext bd = new DataClasesDataContext();
            var hist_clinicas = from hc in bd.HistoriasClinicas
                            where hc.paciente_id == id
                            orderby hc.fecha
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
        public int saveHC(HistoriasClinica historiaC)
        {
            DataClasesDataContext bd = new DataClasesDataContext();
            int regAfectados = 0;
            try
            {
                if (historiaC.id == 0)
                {
                    bd.HistoriasClinicas.InsertOnSubmit(historiaC);
                    bd.SubmitChanges();
                    regAfectados = 1;
                }
                else
                {
                    HistoriasClinica historiaOld = bd.HistoriasClinicas.Where(hc => hc.id.Equals(historiaC.id)).First();
                    historiaOld.fecha = historiaC.fecha;
                    historiaOld.descripcion = historiaC.descripcion;
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
        public int deleteHC(HistoriasClinica historiaC)
        {
            DataClasesDataContext bd = new DataClasesDataContext();
            int regAfectados = 0;
            try
            {
                HistoriasClinica historiaOld = bd.HistoriasClinicas.Where(p => p.id.Equals(historiaC.id)).First();
                bd.HistoriasClinicas.DeleteOnSubmit(historiaOld);
                bd.SubmitChanges();
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
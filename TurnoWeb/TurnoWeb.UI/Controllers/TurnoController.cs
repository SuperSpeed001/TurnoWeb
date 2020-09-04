using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TurnosWeb.UI;
using TurnoWeb.UI.Models;

namespace TurnoWeb.UI.Controllers
{
    public class TurnoController : Controller
    {

        // GET: Turno
        public ActionResult Index()
        {
           
            SolicitarTurnoAuxiliar auxiliar = new SolicitarTurnoAuxiliar();
            var result = auxiliar.GetTurnos().Select
                 (r => new SelectListItem
                 {
                     Value = $"{r.tur_id}",
                     Text = r.tur_nombre
                 })
                 .OrderBy(o => o.Text)
                 .ToList();

            TurnoViewModel turnoViewModel = new TurnoViewModel
            {
                ListadoTurnos = new List<SelectListItem>(),
                TablaTurnosViewModels = new List<TablaTurnosViewModel>(),
                TurId = 0
            };
            turnoViewModel.ListadoTurnos = result;
            turnoViewModel.PerFalta = DateTime.Now;
            turnoViewModel.TablaTurnosViewModels.Add(new TablaTurnosViewModel { hor_dia = "00:00", box_nombre = string.Empty, HoraSeleccionada = 00, hor_fecha = DateTime.Now.ToShortDateString(), hor_hora = "00:00" });
            return View(turnoViewModel);
        }

        [Route("/Turno/GetTurnos")]
        [HttpPost]
        public ActionResult GetTurnos(int turId, string perFalta)
        {
            //string path = Path.Combine(_env.ContentRootPath, "\\wwwroot\\images\\Local");
            SolicitarTurnoAuxiliar auxiliar = new SolicitarTurnoAuxiliar();
            var PerFalta = DateTime.Parse(perFalta);
            var turnoId = turId;
            
            var dia = (PerFalta.Day > 0 && PerFalta.Day < 10) ? "0" + PerFalta.Day.ToString() : PerFalta.Day.ToString();
            var mes = (PerFalta.Month > 0 && PerFalta.Month < 10) ? "0" + PerFalta.Month.ToString() : PerFalta.Month.ToString();
            var anio = PerFalta.Year;

             //  Obtener hora minuto y segundo para control
            var hora = DateTime.Now.Hour;
            var minutos = DateTime.Now.Minute;
            var segundos = DateTime.Now.Second;

            var fecha = anio + mes + dia;
            var horaMinutosSegundo = (
                    (hora >0 && hora <10) ? "0" + hora.ToString() : hora.ToString()) + ":" + 
                    ((minutos >0 && minutos <10) ? "0" + minutos.ToString(): minutos.ToString()) + ":" + 
                    ((segundos>0 && segundos<10)? "0" + segundos.ToString() : segundos.ToString());
            var result = new List<TablaTurnosViewModel>();

            if (PerFalta.ToShortDateString() == DateTime.Now.ToShortDateString())
            {
                //result = _context.ListarHorariosPorFecha(turnoId, fecha, horaMinutosSegundo).Result.ToList();
                result = auxiliar.ListarHorarioPorFecha(turnoId, fecha, horaMinutosSegundo);
            }
            else
            {
                result = auxiliar.ListarHorarioPorFecha(turnoId, fecha, string.Empty);
            }

            ViewData["Titulo"] = result.Count > 0 ? result[1].tur_nombre + " para el día " + result[1].hor_dia : "";
            return PartialView("_TablaTurnosPartial", result);
        }

        [Route("/Turno/CreateAjax")]
        [HttpPost]
        public ActionResult CreateAjax(string dni, string nombre, string telefono, string email, string idHoraSel, string idTurnoSel, string fecha, string observacion)
        {
            if (string.IsNullOrEmpty(dni)) return Json("Debe ingresar los datos");
            string[] datosId = idHoraSel.Split('/');
            var nombreBox = datosId[2];
            var horaSel = datosId[1];
            var diaSel = datosId[3];
            var nombreTurno = datosId[4];

            var exito1 = int.TryParse(dni, out int dniValido);
            var exito2 = int.TryParse(datosId[0], out int idHorario);
            var exito3 = int.TryParse(idTurnoSel, out int idTurno);

            if (!exito1 && !exito2 && !exito3 && !string.IsNullOrEmpty(fecha)) return Json("Los Valores ingresados no son Correctos");


            SolicitarTurnoAuxiliar auxiliar = new SolicitarTurnoAuxiliar();
            List<ControlTurnoViewModel> result = auxiliar.ControlTurno(dni, idTurno).ToList();

            if (result.Count == 0)
            {                                                                                           //add 2 merge
                var personaId = auxiliar.IncertarPersona(dniValido, nombre, email, telefono, idHorario, observacion);
                if (personaId > 0)
                {
                    PrintViewModel model = new PrintViewModel
                    {
                        Dia = diaSel,  //nombreDia
                        Hora = horaSel,  //hora
                        NombreBox = nombreBox,  //nombre box
                        NombreTurno = nombreTurno,
                        HoraId = idHorario,
                        Dni = dni,
                        Nombre = nombre,
                        Email = email,
                        Fecha = fecha,
                        Observacion = observacion
                    };

                    byte[] data = auxiliar.GenerarImagen(model);

                    return Json(new { base64imgage = Convert.ToBase64String(data) });                   

                }
                else
                {
                    return Json("Error, no se pudo guardar los datos, revise los datos ingresados");
                }
            }
            else
            {
                var cadenaVacia = string.Empty;
                foreach (var item in result)
                {
                    cadenaVacia = "Usted ya solicitó un Turno con Fecha " +item.FechaTurno;
                }
               
                return Json(cadenaVacia);   //ya tiene turno asignado
            }
        }




        // GET: Turno/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Turno/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Turno/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Turno/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Turno/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Turno/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Turno/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}

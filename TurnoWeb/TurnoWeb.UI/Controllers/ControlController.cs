namespace TurnoWeb.UI.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Web.Mvc;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using TurnosWeb.UI;
    using TurnoWeb.UI.Models;
    using TurnoWeb.UI.Service;

    public class ControlController : Controller
    {
        // GET: Control
        public ActionResult _GetControl()
        {
            var aux = new SolicitarTurnoAuxiliar();

            var result = aux.GetTurnos().Select
                 (r => new SelectListItem
                 {
                     Value = $"{r.Idturno}",
                     Text = r.NombreTurno
                 })
                 .OrderBy(o => o.Text)
                 .ToList();
            TurnoViewModel turnoViewModel = new TurnoViewModel
            {
                ListadoTurnos = new List<SelectListItem>(),
                TablaTurnosViewModels = new List<TablaTurnosViewModel>(),
                ListadoTurnoReservados = new List<ListadoTurnoReservadosViewModel>(),
                TurId = 0
            };
            turnoViewModel.ListadoTurnos = result;
            turnoViewModel.PerFalta = DateTime.Now;

            return View("Index", turnoViewModel);
        }

        // GET: Control/Details/5
        public ActionResult SendAs400()
        {
            As400 as400 = new As400();
            as400.ConnectionAs400();
            return View();
        }

        [Route("/Control/GetTurnosFecha")]
        [HttpPost]
        public ActionResult GetTurnosFecha(string fecha, string turnoId)
        {
            var PerFalta = DateTime.Parse(fecha);

            var dia = (PerFalta.Day > 0 && PerFalta.Day < 10) ? "0" + PerFalta.Day.ToString() : PerFalta.Day.ToString();
            var mes = (PerFalta.Month > 0 && PerFalta.Month < 10) ? "0" + PerFalta.Month.ToString() : PerFalta.Month.ToString();
            var anio = PerFalta.Year.ToString();

            var aux = new SolicitarTurnoAuxiliar();
            var result = aux.GetTurnosParaCO(int.Parse(turnoId), dia, mes, anio);

            return PartialView("_TablaTurnosParaCO", result);

        }

        public ActionResult ExportToExcel(string fecha, string turnoId)
        {
            var PerFalta = DateTime.Parse(fecha);

            var dia = (PerFalta.Day > 0 && PerFalta.Day < 10) ? "0" + PerFalta.Day.ToString() : PerFalta.Day.ToString();
            var mes = (PerFalta.Month > 0 && PerFalta.Month < 10) ? "0" + PerFalta.Month.ToString() : PerFalta.Month.ToString();
            var anio = PerFalta.Year.ToString();

            var aux = new SolicitarTurnoAuxiliar();
            var result = aux.GetTurnosParaCO(int.Parse(turnoId), dia, mes, anio);


            if (result.Count > 0)
            {
                //var context = new ExportExcelServices();
                // string[] columnas = { "Fecha", "Día", "Hora", "D.N.I.","Nombre de Persona", "Observaciones", "Box", "Email", "Teléfono"};

                //byte[] filecontent = ExportExcelServices.ExportExcel(result, "Turnos", true, columnas);
                //return File(filecontent, ExportExcelServices.ExcelContentType, "Turnos.xls");

                var vg = new GridView
                {
                    DataSource = result
                };
                vg.DataBind();
                Response.ClearContent();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment; filename=DemoExcel.xlsx");
                Response.ContentType = "application/ms-excel";
                Response.Charset = "";
                StringWriter objStringWriter = new StringWriter();
                HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);
                vg.RenderControl(objHtmlTextWriter);
                Response.Output.Write(objStringWriter.ToString());
                Response.Flush();
                Response.End();
                return View();
            }
            else
            {
                return Json("No tiene Datos", JsonRequestBehavior.AllowGet);
            }
        }

    }
}

namespace TurnoWeb.UI.Service
{
    using System.Collections.Generic;
    using System.Web.Mvc;
    using TurnoWeb.UI.Models;

    public class Auxiliar
    {
        private Client ApiServices;
        public Auxiliar()
        {
            ApiServices = new Client();
        }

        public List<ListadoTurnoViewModel> CargarTurnos()
        {
            ///turnoApi/api   http://199.5.80.8/TurnoApi/api/turno/getturnos
            var response = ApiServices.GetList<ListadoTurnoViewModel>("/TurnoApi/api/turno", "/GetTurnos/").Result;

            if (!response.IsSuccess)
            {

                return new List<ListadoTurnoViewModel>();
            }

            var list = (List<ListadoTurnoViewModel>)response.Result;

            return list;
        }
    }

}
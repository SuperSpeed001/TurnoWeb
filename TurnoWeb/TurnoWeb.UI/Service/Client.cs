namespace TurnoWeb.UI.Service
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Configuration;
    using TurnoWeb.UI.Models;

    public class Client
    {
        // GET: Ivuj
        //var response = await ApiServices.GetList<TurnoSeleccionadoViewModel>("http://190.52.32.117", "/turnoApi/api", "/GetTurnosReservados/");
        public string UrlBase
        {
            get
            {
                return WebConfigurationManager.AppSettings["Url"];
            }

        }

        public async Task<Response> GetList<T>(string prefix, string controller)
        {
            try
            {
                var client = new HttpClient
                {
                    //BaseAddress = new Uri(urlBase)
                };

                var url = $"{UrlBase}{prefix}{controller}";
                var response = await client.GetAsync(url);
                var answer = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return new Response
                    {
                        IsSuccess = false,
                        Message = answer
                    };
                }

                var list = JsonConvert.DeserializeObject<List<T>>(answer);

                return new Response
                {
                    IsSuccess = true,
                    Result = list
                };

            }
            catch (Exception ex)
            {

                return new Response
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }
    }
}
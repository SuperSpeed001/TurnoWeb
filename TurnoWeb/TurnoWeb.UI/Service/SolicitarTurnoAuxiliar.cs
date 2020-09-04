namespace TurnosWeb.UI
{
    using Microsoft.Ajax.Utilities;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Data.SqlClient;
    using System.Drawing;
    using System.IO;
    using Turnos.Common.Models.DB;    
    using TurnoWeb.UI.Models;

    public class SolicitarTurnoAuxiliar
    {
        
        
        public SqlConnection _connectionString
        {
            get
            {
                return new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
            }
        }

        public SolicitarTurnoAuxiliar()
        {
            
        }

        private SqlConnection con;
        //To Handle connection related activities    
        private void connection()
        {
            string constr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            con = new SqlConnection(constr);

        }

        public List<sp_ListarTurnos> GetTurnos()
        {
            connection();
            using (SqlCommand cmd = new SqlCommand("sp_listarTurnos", con))
            {                
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();

                var response = new List<sp_ListarTurnos>();
                con.Open();
                da.Fill(dt);

                foreach (DataRow item in dt.Rows)
                {                   
                    response.Add(MapToValue(item));
                }
                return response;
            }
        }

        public List<TablaTurnosViewModel> ListarHorarioPorFecha(int turnoId, string fecha, string horaMinutosSegundo)
        {
            connection();
            var query = horaMinutosSegundo == string.Empty ? "sp_ListarHorarioPorFecha" : "sp_ListarHorarioPorFechaHora";
           
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@tur_id", turnoId));
                cmd.Parameters.Add(new SqlParameter("@hor_fecha", fecha));

                if (horaMinutosSegundo != string.Empty)
                    cmd.Parameters.Add(new SqlParameter("@hor_hora", horaMinutosSegundo));

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();

                var response = new List<TablaTurnosViewModel>();
                con.Open();
                da.Fill(dt);

                foreach (DataRow item in dt.Rows)
                {
                    response.Add(MapToValueFecha(item));
                }
                return response;
            }

        }

        public List<ControlTurnoViewModel> ControlTurno(string dni, int turnoId)
        {
            connection();
            using (SqlCommand cmd = new SqlCommand("sp_ControlTurno", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@per_dni", dni));
                cmd.Parameters.Add(new SqlParameter("@tur_id", turnoId));

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();

                var response = new List<ControlTurnoViewModel>();
                con.Open();
                da.Fill(dt);
                //TODO: Controlar sale exepcion hor_id                   

                foreach (DataRow item in dt.Rows)
                {
                    response.Add(MapToVallueControl(item));
                }
                return response;

            }
        }
                                                                                                        //add 2 merge
        public int IncertarPersona(int dni, string nombre, string email, string telefono, int horaId, string observacion)
        {

            connection();
            using (SqlCommand cmd = new SqlCommand("sp_insertPersona", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@per_dni", dni.ToString()));
                cmd.Parameters.Add(new SqlParameter("@per_nombre", nombre));
                cmd.Parameters.Add(new SqlParameter("@per_email", email));
                cmd.Parameters.Add(new SqlParameter("@per_telef", telefono));
                cmd.Parameters.Add("@per_id", SqlDbType.Int, 8);
                cmd.Parameters["@per_id"].Direction = ParameterDirection.Output;

                con.Open();
                var response = 0;

                try
                {
                    cmd.ExecuteNonQuery();
                    string personaId = cmd.Parameters["@per_id"].Value.ToString();

                    using (SqlCommand cmd2 = new SqlCommand("sp_updateHorario", con))
                    {
                        cmd2.CommandType = CommandType.StoredProcedure;
                        cmd2.Parameters.Add(new SqlParameter("@hor_id", horaId));
                        cmd2.Parameters.Add(new SqlParameter("@per_id", int.Parse(personaId))); 
                        cmd2.Parameters.Add(new SqlParameter("@hor_obser", (observacion == string.Empty ? " " : observacion) ));       //add 2 merge
                        cmd2.ExecuteNonQuery();
                    }

                    int.TryParse(personaId, out response);
                    return response;
                }
                catch (Exception ex)
                {
                    response = 0;
                    throw ex;
                }
                finally
                {
                    con.Close();
                }
            }
        }

        internal byte[] GenerarImagen(PrintViewModel model)
        {
            Bitmap bitimage;

            bitimage = (Bitmap)Image.FromFile(Path.Combine(System.Web.Hosting.HostingEnvironment.MapPath("~/wwwroot/image/"), "turno.png"));
            Font myFontLabels = new Font("Calibri", 14);
            _ = new SolidBrush(Color.White);
            Bitmap newimage = new Bitmap(bitimage.Width, bitimage.Height + 100);
            Graphics gr = Graphics.FromImage(newimage);
            gr.DrawImageUnscaled(bitimage, 0, 0);
            string uniqueFileName;
            try
            {
                gr.DrawString("Turno: " + model.NombreTurno, myFontLabels, Brushes.Brown, new RectangleF(5, bitimage.Height, bitimage.Width, 50));

                gr.DrawString("Nombre: " + model.Nombre, myFontLabels, Brushes.Brown, new RectangleF(5, bitimage.Height + 20, bitimage.Width, 50));
                gr.DrawString("D.N.I. : " + model.Dni, myFontLabels, Brushes.Brown, new RectangleF(5, bitimage.Height + 40, bitimage.Width, 50));
                gr.DrawString("Fecha: " + model.Fecha + " " + "Hora: " + model.Hora, myFontLabels, Brushes.Brown, new RectangleF(5, bitimage.Height + 60, bitimage.Width, 50));
                if (model.Observacion.IsNullOrWhiteSpace())
                {
                    //gr.DrawString("El presente Turno es válido para el día solicitado.", myFontLabels, Brushes.Brown, new RectangleF(5, bitimage.Height + 80, bitimage.Width, 50));
                }
                else
                {
                    gr.DrawString("Observación: " + model.Observacion, myFontLabels, Brushes.Brown, new RectangleF(5, bitimage.Height + 80, bitimage.Width, 50));
                }
                //gr.DrawString("El presente Turno es válido para el día solicitado.", myFontLabels, Brushes.Brown, new RectangleF(5, bitimage.Height + 80, bitimage.Width, 50));
                uniqueFileName = Guid.NewGuid().ToString() + "_" + "Turno";
                newimage.Save(Path.Combine(System.Web.Hosting.HostingEnvironment.MapPath("~/wwwroot/imageTurno/") + uniqueFileName + ".png"));
            }
            catch (Exception ex)
            {

                throw ex;
            }
            gr.Dispose();
            bitimage.Dispose();

            FileStream fileStream = new FileStream(Path.Combine(System.Web.Hosting.HostingEnvironment.MapPath("~/wwwroot/imageTurno/") + uniqueFileName + ".png"), FileMode.Open, FileAccess.Read);
            byte[] data = new byte[(int)fileStream.Length];
            fileStream.Read(data, 0, data.Length);

            return data;
        }


        #region Metodos Privados

        private sp_ListarTurnos MapToValue(DataRow reader)
        {
            return new sp_ListarTurnos()
            {
                tur_id = (int)reader["tur_id"],
                tur_nombre = reader["tur_nombre"].ToString()
            };
        }

        private TablaTurnosViewModel MapToValueFecha(DataRow reader)
        {
            return new TablaTurnosViewModel()
            {
                hor_id = (int)reader["hor_id"],
                hor_hora = reader["hor_hora"].ToString(),
                hor_dia = reader["hor_dia"].ToString(),
                box_nombre = reader["box_nombre"].ToString(),
                tur_nombre = reader["tur_nombre"].ToString(),
                hor_reserv = (bool)reader["hor_reserv"]
            };            
        }

        private ControlTurnoViewModel MapToVallueControl(DataRow reader)
        {
            if (!reader.IsNull(0))
            {
                return new ControlTurnoViewModel()
                {
                    DiaTurno = reader["Dia"].ToString(),
                    FechaTurno = DateTime.Parse(reader["Fecha"].ToString())//,
                                                                           //HoraTurno = reader["hor_dia"].ToString()
                };
            }
            else
            {
                return new ControlTurnoViewModel();
            }
           
        }
        
        #endregion

    }
}

namespace TurnosWeb.UI
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Data.SqlClient;   
    using System.Linq;    
    using System.Threading.Tasks;  
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
            List<TablaTurnosViewModel> lista = new List<TablaTurnosViewModel>();

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

        public int IncertarPersona(int dni, string nombre, string email, string telefono, int horaId)
        {
            string personaId = string.Empty;
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
                    personaId = cmd.Parameters["@per_id"].Value.ToString();

                    using (SqlCommand cmd2 = new SqlCommand("sp_updateHorario", con))
                    {
                        cmd2.CommandType = CommandType.StoredProcedure;
                        cmd2.Parameters.Add(new SqlParameter("@hor_id", horaId));
                        cmd2.Parameters.Add(new SqlParameter("@per_id", int.Parse(personaId)));

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
    }
}

namespace TurnoWeb.UI.Service
{
    using System;    
    using IBM.Data.DB2.iSeries;
    using System.Data;
    using TurnoWeb.UI.Models.Data;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data.SqlClient;

    public class As400
    {
        private iDB2Connection conAS400;
        private SqlConnection conSql;
        private SqlConnection conSql01;
        public void GetConnection()
        {
            string _sqlcon = "Data Source=199.5.83.158;User Id=PGMVTA01;Password=PGMVTA01;Default Collection=vivilib;";

            string constr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();     //PRODUCCION
            string constrSql01 = ConfigurationManager.ConnectionStrings["TestConnection"].ToString();       //TEST
            //Driver={Client Access ODBC Driver (32-bit)};System=my_system_name;Uid=myUsername;Pwd=myPassword;
            //string _sqlcon = "Driver={Client Access ODBC Driver (32-bit)};System=my_system_name;Uid=myUsername;Pwd=myPassword;";
            conAS400 = new iDB2Connection(_sqlcon);

            conSql = new SqlConnection(constr);
            conSql01 = new SqlConnection(constrSql01);
        }

        private void connection()
        {
            string constr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();     //PRODUCCION
            string constrSql01 = ConfigurationManager.ConnectionStrings["TestConnection"].ToString();       //TEST

            conSql = new SqlConnection(constr);
            conSql01 = new SqlConnection(constrSql01);
        }

        public void ConnectionAs400()
        {
            try
            {
                GetConnection();
                var query = "select nrocta, ayn, ndosol, cotit, ndocso, estad from vivilib.maevivi";    
               
                using (iDB2Command cmd = new iDB2Command(query,conAS400))
                {
                    cmd.CommandType = CommandType.Text;

                    iDB2DataAdapter da = new iDB2DataAdapter(cmd);
                    DataTable dt = new DataTable();

                    var response = new List<TitularViewModel>();
                    conAS400.Open();
                    da.Fill(dt);

                    foreach (DataRow item in dt.Rows)
                    {
                        response.Add(MapToValueAs400(item));
                    }
                    conSql01.Open();

                    /*Delete all data */
                    SqlCommand cmd1 = conSql01.CreateCommand(); ;
                            cmd1.CommandType = CommandType.Text;
                            cmd1.CommandText = "DELETE FROM Titular";
                            cmd1.ExecuteNonQuery();
                    /*Fin delete*/

                    using (SqlTransaction oTransaction = conSql01.BeginTransaction())
                    {
                        using (SqlCommand oCommand = conSql01.CreateCommand())
                        {
                            oCommand.Transaction = oTransaction;
                            oCommand.CommandType = CommandType.Text;
                            oCommand.CommandText = "INSERT INTO [Titular] ([AyNTitular] ,[DniTitular] ,[AyNCoTitular] ,[DniCoTitular] ,[NumeroCuenta]) VALUES (@AyNTitular, @DniTitular, @AyNCoTitular, @DniCoTitular, @NumeroCuenta );";
                            oCommand.Parameters.Add(new SqlParameter("@AyNTitular", SqlDbType.NChar));
                            oCommand.Parameters.Add(new SqlParameter("@DniTitular", SqlDbType.Decimal));
                            oCommand.Parameters.Add(new SqlParameter("@AyNCoTitular", SqlDbType.NChar));
                            oCommand.Parameters.Add(new SqlParameter("@DniCoTitular", SqlDbType.Decimal));
                            oCommand.Parameters.Add(new SqlParameter("@NumeroCuenta", SqlDbType.Decimal));
                            try
                            {
                                foreach (var oSetting in response)
                                {
                                    oCommand.Parameters[0].Value = oSetting.ApellidoNombreTitular;
                                    oCommand.Parameters[1].Value = oSetting.DniTitular;
                                    oCommand.Parameters[2].Value = oSetting.ApellidoNombreCoTitular;
                                    oCommand.Parameters[3].Value = oSetting.DniCoTitular;
                                    oCommand.Parameters[4].Value = oSetting.NumeroCuenta;
                                    if (oCommand.ExecuteNonQuery() != 1)
                                    {
                                        //'handled as needed, 
                                        //' but this snippet will throw an exception to force a rollback
                                        throw new InvalidProgramException();
                                    }
                                }
                                oTransaction.Commit();
                            }
                            catch (Exception)
                            {
                                oTransaction.Rollback();
                                throw;
                            }
                        }
                    }

                }               
            }
            catch (Exception ex)
            {

                throw ex;
            }
           

        }

        /* private ListadoTurnoReservadosViewModel MapToValueGeneric(DataRow reader)
        {

            return new ListadoTurnoReservadosViewModel
            {
                NombreTurno = (string)reader["tur_nombre"],
                Fecha = DateTime.Parse(reader["hor_fecha"].ToString()),
                Dia = (string)reader["hor_dia"],
                Hora = (string)reader["hor_hora"],
                Dni = (string)reader["per_dni"],
                NombrePersona = (string)reader["per_nombre"],
                Observacion = (string)reader["hor_obser"],
                NombreBox = (string)reader["box_nombre"],
                Estado = (bool)reader["hor_reserv"]
                
            };
        }*/
        private TitularViewModel MapToValueAs400(DataRow reader)
        {
            return new TitularViewModel
            {
                NumeroCuenta = (decimal)reader["nrocta"], 
                ApellidoNombreTitular = (string)reader["AYN"],                
                DniTitular = (decimal)reader["NDOSOL"],
                ApellidoNombreCoTitular = (string)reader["COTIT"],
                DniCoTitular = (decimal)reader["NDOCSO"]
            };
        }

        public As400() { }
    }
}
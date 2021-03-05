using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
namespace WebAPI_VND.DAL
{
    public class GravaLog
    {

        public void GravaReturn(int Clicodigo, string Log, string Metodo)
        {
            SqlConnection conexao = new SqlConnection(ConfigurationManager.AppSettings.Get("Conn17").ToString());
            SqlCommand comando = new SqlCommand();
            SqlDataAdapter da;
            DataSet ds = new DataSet();
            
                comando.Connection = conexao;
                comando.CommandType = CommandType.StoredProcedure;
                comando.CommandText = "STP_GRAVA_LOG_API_DOTZ";
                comando.CommandTimeout = 3000;
                comando.Parameters.Add("@CLICODIGO", SqlDbType.Int).Value = Clicodigo;
                comando.Parameters.Add("@LOG", SqlDbType.VarChar).Value = Log.ToString();
                comando.Parameters.Add("@METODO", SqlDbType.VarChar).Value = Metodo;

                da = new SqlDataAdapter(comando);
                da.Fill(ds, "Dados");
                conexao.Close();
            
            
           
        } 

    }
}
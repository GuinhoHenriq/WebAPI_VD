using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace WebAPI_VND.DAL
{
    public class CarregaInfoCliente
    {

        public DataSet ConsultaCliente(int Clicodigo)
        {
            System.Data.SqlClient.SqlConnection conexao = new SqlConnection(ConfigurationManager.AppSettings.Get("Conn17").ToString());
            SqlCommand comando = new SqlCommand();
            DataSet ds = new DataSet();
            SqlDataAdapter da;

            comando.Connection = conexao;
            comando.CommandType = CommandType.StoredProcedure;
            comando.CommandText = "STP_CONSULTA_CLIENTE_VINDI";
            comando.CommandTimeout = 3000;
            comando.Parameters.Add("@CLICODIGO", SqlDbType.Int).Value = Clicodigo;

            da = new SqlDataAdapter(comando);
            da.Fill(ds, "Dados");
            conexao.Close();
            return ds;
        }

    }
}
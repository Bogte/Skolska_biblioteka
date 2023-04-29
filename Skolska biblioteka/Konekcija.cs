using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace Skolska_biblioteka
{
    internal class Konekcija
    {
        
        static public SqlConnection Connect()
        {
            string CS;
            CS = ConfigurationManager.ConnectionStrings["home"].ConnectionString;
            SqlConnection conn = new SqlConnection(CS);
            return conn;
        }

        static public string Veza()
        {
            return ConfigurationManager.ConnectionStrings["home"].ConnectionString;
        }

        static public DataTable Unos(string Komanda)
        {
            DataTable Tabela = new DataTable();
            SqlDataAdapter adapter = new SqlDataAdapter(Komanda, Konekcija.Connect());
            adapter.Fill(Tabela);
            return Tabela;
        }

    }
}

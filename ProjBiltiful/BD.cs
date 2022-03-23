using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using CadastrosBasicos;

namespace ProjBiltiful
{
    public class BD
    {
        public string DataSource { get; }
        public string DataBase { get; }
        public string UserName { get; }
        public string Password { get; }
        public string ConnString { get; }

        public BD()
        {
            DataSource = "DESKTOP - 6VFRPCQ";
            DataBase = "Biltiful";
            UserName = "sa";
            Password = "";
            ConnString = @"Data Source=" + DataSource + ";Initial Catalog="
                                + DataBase + ";Persist Security Info=True;User ID=" + UserName + ";Password=" + Password;
        }

        public void GravarCliente(Cliente cliente)
        {
            SqlConnection connection = new(ConnString);

            string cpf = cliente.CPF;
            string nome = cliente.Nome;
            DateTime dataNasc = cliente.DataNascimento;
            char sexo = cliente.Sexo;
            DateTime ultimaCompra = cliente.UltimaVenda;
            DateTime dataCadastro = cliente.DataCadastro;
            char situacao = cliente.Situacao;
            int risco = 0;

            try
            {
                using (connection)
                {
                    string sql = $"INSERT INTO Cliente VALUES ({cpf}, {nome}, {dataNasc}, {sexo}, {ultimaCompra}, {dataCadastro}, {situacao}, {risco});";
                    connection.Open();
                    SqlCommand sqlCommand = new(sql, connection);
                    sqlCommand.ExecuteNonQuery();
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("EX -> " + ex.Message);
            }
        }

    }
}

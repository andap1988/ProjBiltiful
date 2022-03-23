using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;

namespace ProducaoCosmeticos
{
    public class BDProducao
    {
        public string DataSource { get; }
        public string DataBase { get; }
        public string UserName { get; }
        public string Password { get; }
        public string ConnString { get; }

        public BDProducao()
        {
            DataSource = "DESKTOP-6VFRPCQ";
            DataBase = "Biltiful";
            UserName = "sa";
            Password = "&A1T2";
            ConnString = @"Data Source=" + DataSource + ";Initial Catalog="
                                + DataBase + ";Persist Security Info=True;User ID=" + UserName + ";Password=" + Password;
        }

        public Producao GravarProducao(int idPassado, string produto, decimal qt, DateTime data)
        {
            SqlConnection connection = new(ConnString);

            string sql;
            int id;
            string dataProducao = data.ToString("yyyy/MM/dd").Replace("/", "-");


            if (idPassado == -1)
                id = UltimoID();
            else
                id = idPassado;

            if (id == 0)
                id = 1;
            else
                id++;

            try
            {
                using (connection)
                {
                    sql = $"INSERT INTO Producao VALUES ({id}, CONVERT(DATE, '{dataProducao}', 111), '{produto}', '{qt.ToString(new CultureInfo("en-US"))}' );";
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

            return LocalizarProducao(id);
        }

        public Producao LocalizarProducao(int id)
        {
            SqlConnection connection = new(ConnString);

            Producao producao = null;

            int codigo = 0;
            string dataProducao = "", produto = "";
            decimal qt = 0;

            string sql = $"SELECT * from Producao WHERE ID = {id};";

            try
            {
                connection.Open();
                using (SqlCommand command = new(sql, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            codigo = int.Parse(reader.GetValue(0).ToString());
                            dataProducao = reader.GetDateTime(1).ToString("dd/MM/yyyy");
                            produto = reader.GetValue(2).ToString();
                            qt = reader.GetDecimal(3);
                        }
                    }
                }
                connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("EX -> " + ex.Message);
            }

            if (string.IsNullOrEmpty(produto))
                return producao;
            else
            {
                producao = new();
                producao.Id = codigo.ToString().PadLeft(5, '0');
                producao.DataProducao = dataProducao;
                producao.Produto = produto;
                producao.Quantidade = qt;

                return producao;
            }
        }

        public void GravarItemProducao(List<ItemProducao> itens)
        {
            try
            {
                itens.ForEach(item =>
                {
                    SqlConnection connection = new(ConnString);

                    int id = int.Parse(item.Id);
                    DateTime data = DateTime.Parse(item.DataProducao);
                    string mprima = item.MateriaPrima;
                    decimal qt = item.QuantidadeMateriaPrima;

                    using (connection)
                    {
                        string sql = $"INSERT INTO Item_Producao VALUES ({id}, CONVERT(DATE, '{data.ToString("yyyy/MM/dd").Replace("/", "-")}', 111), {int.Parse(mprima.Substring(2, 4))}, '{item.QuantidadeMateriaPrima.ToString(new CultureInfo("en-US"))}');";
                        connection.Open();
                        SqlCommand sqlCommand = new(sql, connection);
                        sqlCommand.ExecuteNonQuery();
                        connection.Close();
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine("EX -> " + ex.Message);
            }
        }

        public List<ItemProducao> ImprimirItens(int codigo)
        {
            SqlConnection connection = new(ConnString);

            int id, mp;
            string dataProducao;
            decimal qtMP = 0;

            string sql = $"SELECT * from Item_Producao WHERE ID = {codigo};";

            List<ItemProducao> itens = new();
            ItemProducao item;

            try
            {
                connection.Open();
                using (SqlCommand command = new(sql, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            id = int.Parse(reader.GetValue(0).ToString());
                            dataProducao = reader.GetDateTime(1).ToString("dd/MM/yyyy");
                            mp = int.Parse(reader.GetValue(2).ToString());
                            qtMP = reader.GetDecimal(3);

                            item = new();
                            item.Id = id.ToString("0000");
                            item.DataProducao = dataProducao;
                            item.MateriaPrima = "MP" + mp.ToString("0000");
                            item.QuantidadeMateriaPrima = qtMP;
                            itens.Add(item);
                        }
                    }
                }
                connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("EX -> " + ex.Message);
            }

            return itens;
        }

        public int UltimoID()
        {
            string sql = $"SELECT * from Producao WHERE ID = (SELECT MAX(ID) from Producao);";
            int id = 0;

            SqlConnection connection = new(ConnString);

            try
            {
                connection.Open();
                using (SqlCommand command = new(sql, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            id = reader.GetInt32(0);
                        }
                    }
                }
                connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("EX -> " + ex.Message);
            }

            return id;
        }

        public bool TemProducao()
        {
            SqlConnection connection = new(ConnString);

            string sql = $"SELECT * from Producao;";
            bool flag = true;

            try
            {
                connection.Open();
                using (SqlCommand command = new(sql, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                            flag = true;
                        else
                            flag = false;
                    }
                }
                connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("EX -> " + ex.Message);
            }

            return flag;
        }

        public List<Producao> ListarProducoes()
        {
            SqlConnection connection = new(ConnString);

            List<Producao> producoes = new();
            Producao producao;

            int codigo = 0;
            string dataProducao = "", produto = "";
            decimal qt = 0;

            string sql = $"SELECT * from Producao;";

            try
            {
                connection.Open();
                using (SqlCommand command = new(sql, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            codigo = int.Parse(reader.GetValue(0).ToString());
                            dataProducao = reader.GetDateTime(1).ToString("dd/MM/yyyy");
                            produto = reader.GetValue(2).ToString();
                            qt = reader.GetDecimal(3);

                            producao = new();
                            producao.Id = codigo.ToString().PadLeft(5, '0');
                            producao.DataProducao = dataProducao;
                            producao.Produto = produto;
                            producao.Quantidade = qt;
                            producoes.Add(producao);
                        }
                    }
                }
                connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("EX -> " + ex.Message);
            }

            return producoes;
        }

        public void RemoverProducao(int id)
        {
            SqlConnection connection = new(ConnString);

            string sql = $"DELETE from Producao WHERE ID = {id};";

            try
            {
                using (connection)
                {
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

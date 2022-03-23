using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;

namespace VendasProdutos
{
    public class BDVenda
    {
        public string DataSource { get; }
        public string DataBase { get; }
        public string UserName { get; }
        public string Password { get; }
        public string ConnString { get; }

        public BDVenda()
        {
            DataSource = "DESKTOP-6VFRPCQ";
            DataBase = "Biltiful";
            UserName = "sa";
            Password = "&A1T2";
            ConnString = @"Data Source=" + DataSource + ";Initial Catalog="
                                + DataBase + ";Persist Security Info=True;User ID=" + UserName + ";Password=" + Password;
        }

        public Venda GravarVenda(string cpf, DateTime data, bool update = false, int idPassado = 0, decimal valorTotal = 0)
        {
            SqlConnection connection = new(ConnString);

            string sql;

            if (update)
            {
                try
                {
                    using (connection)
                    {
                        sql = $"UPDATE Venda SET Valor_Total = '{valorTotal.ToString(new CultureInfo("en-US"))}' WHERE ID = {idPassado};";
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

                return null;
            }
            else
            {
                string cpfFormatado = cpf.Insert(3, ".").Insert(7, ".").Insert(11, "-");
                string dataVenda = data.ToString("yyyy/MM/dd").Replace("/", "-");

                int id = UltimoID();

                if (id == 0)
                    id = 1;
                else
                    id++;

                try
                {
                    using (connection)
                    {
                        sql = $"INSERT INTO Venda (ID, Data_Venda, CPF_Cliente) VALUES ({id}, CONVERT(DATE, '{dataVenda}', 111), '{cpfFormatado}');";
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

                return LocalizarVenda(id);
            }
        }

        public int UltimoID()
        {
            string sql = $"SELECT * from Venda WHERE ID = (SELECT MAX(ID) from Venda);";
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

        public Venda LocalizarVenda(int id)
        {
            SqlConnection connection = new(ConnString);

            Venda venda = null;
            int codigo = 0;
            string dataVenda = "", cpf = "";
            decimal valorTotal = 0;

            string sql = $"SELECT * from Venda WHERE ID = {id};";

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
                            dataVenda = reader.GetDateTime(1).ToString("dd/MM/yyyy");
                            cpf = reader.GetValue(2).ToString();
                            valorTotal = reader.GetDecimal(3);
                        }
                    }
                }
                connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("EX -> " + ex.Message);
            }

            if (string.IsNullOrEmpty(cpf))
                return venda;
            else
            {
                venda = new();
                venda.Id = codigo;
                venda.DataVenda = Convert.ToDateTime(dataVenda);
                venda.Cliente = cpf;
                venda.ValorTotal = valorTotal;

                return venda;
            }
        }

        public void GravarItemVenda(List<ItemVenda> itens)
        {
            try
            {
                itens.ForEach(item =>
                {
                    SqlConnection connection = new(ConnString);

                    int id = item.Id;
                    string produto = item.Produto;
                    int qt = item.Quantidade;

                    using (connection)
                    {
                        string sql = $"INSERT INTO Item_Venda VALUES ({id}, '{produto}', {qt}, '{item.ValorUnitario.ToString(new CultureInfo("en-US"))}', '{item.TotalItem.ToString(new CultureInfo("en-US"))}');";
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

        public List<ItemVenda> ImprimirItens(int codigo)
        {
            SqlConnection connection = new(ConnString);

            string produto;
            int id, qt = 0;
            decimal valorUnitario = 0, totalItem = 0;

            string sql = $"SELECT * from Item_Venda WHERE ID = {codigo};";

            List<ItemVenda> itens = new();
            ItemVenda item;

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
                            produto = reader.GetValue(1).ToString();
                            qt = int.Parse(reader.GetValue(2).ToString());
                            valorUnitario = reader.GetDecimal(3);
                            totalItem = reader.GetDecimal(4);

                            item = new();
                            item.Id = id;
                            item.Produto = produto;
                            item.Quantidade = qt;
                            item.ValorUnitario = valorUnitario;
                            item.TotalItem = totalItem;
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

        public bool TemVenda()
        {
            SqlConnection connection = new(ConnString);

            string sql = $"SELECT * from Venda;";
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

        public List<Venda> ListarVendas()
        {
            SqlConnection connection = new(ConnString);

            List<Venda> vendas = new();
            Venda venda;

            int codigo = 0;
            string dataVenda = "", cpf = "";
            decimal valorTotal = 0;

            string sql = $"SELECT * from Venda;";

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
                            dataVenda = reader.GetDateTime(1).ToString("dd/MM/yyyy");
                            cpf = reader.GetValue(2).ToString();
                            valorTotal = reader.GetDecimal(3);

                            venda = new();
                            venda.Id = codigo;
                            venda.DataVenda = Convert.ToDateTime(dataVenda);
                            venda.Cliente = cpf;
                            venda.ValorTotal = valorTotal;
                            vendas.Add(venda);
                        }
                    }
                }
                connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("EX -> " + ex.Message);
            }

            return vendas;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace ComprasMateriasPrimas
{
    public class BDCompra
    {
        public string DataSource { get; }
        public string DataBase { get; }
        public string UserName { get; }
        public string Password { get; }
        public string ConnString { get; }

        public BDCompra()
        {
            DataSource = "DESKTOP-6VFRPCQ";
            DataBase = "Biltiful";
            UserName = "sa";
            Password = "&A1T2";
            ConnString = @"Data Source=" + DataSource + ";Initial Catalog="
                                + DataBase + ";Persist Security Info=True;User ID=" + UserName + ";Password=" + Password;
        }

        public Compra GravarCompra(string cnpj, DateTime data, bool update = false, int idPassado = 0, decimal valorTotal = 0)
        {
            SqlConnection connection = new(ConnString);

            string sql;

            if (update)
            {
                try
                {
                    using (connection)
                    {
                        sql = $"UPDATE Compra SET Valor_Total = {valorTotal} WHERE ID = {idPassado};";
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
                string cnpjFormatado = cnpj.Insert(2, ".").Insert(6, ".").Insert(10, "/").Insert(15, "-");
                string dataCompra = data.ToString("yyyy/MM/dd").Replace("/", "-");

                int id = UltimoID();

                if (id == 0)
                    id = 1;
                else
                    id++;

                try
                {
                    using (connection)
                    {
                        sql = $"INSERT INTO Compra (ID, Data_Compra, Fornecedor) VALUES ({id}, CONVERT(DATE, '{dataCompra}', 111), '{cnpjFormatado}');";
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

                return LocalizarCompra(id);
            }
        }

        public int UltimoID()
        {
            string sql = $"SELECT * from Compra WHERE ID = (SELECT MAX(ID) from Compra);";
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

        public Compra LocalizarCompra(int id)
        {
            SqlConnection connection = new(ConnString);

            Compra compra = null;
            int codigo = 0;
            string dataCompra = "", cnpj = "";
            decimal valorTotal = 0;

            string sql = $"SELECT * from Compra WHERE ID = {id};";

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
                            dataCompra = reader.GetDateTime(1).ToString("dd/MM/yyyy");
                            cnpj = reader.GetValue(2).ToString();
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

            if (string.IsNullOrEmpty(cnpj))
                return compra;
            else
            {
                compra = new();
                compra.Id = codigo;
                compra.DataCompra = Convert.ToDateTime(dataCompra);
                compra.Fornecedor = cnpj;
                compra.ValorTotal = valorTotal;

                return compra;
            }
        }

        public List<Compra> ListarCompras()
        {
            SqlConnection connection = new(ConnString);

            List<Compra> compras = new();
            Compra compra;

            int codigo = 0;
            string dataCompra = "", cnpj = "";
            decimal valorTotal = 0;

            string sql = $"SELECT * from Compra;";

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
                            dataCompra = reader.GetDateTime(1).ToString("dd/MM/yyyy");
                            cnpj = reader.GetValue(2).ToString();
                            valorTotal = reader.GetDecimal(3);

                            compra = new();
                            compra.Id = codigo;
                            compra.DataCompra = Convert.ToDateTime(dataCompra);
                            compra.Fornecedor = cnpj;
                            compra.ValorTotal = valorTotal;
                            compras.Add(compra);
                        }
                    }
                }
                connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("EX -> " + ex.Message);
            }

            return compras;
        }

        public void GravarItemCompra(List<ItemCompra> itens)
        {
            int id = 0, mprima;
            string dataCompra;
            decimal qt, valorUnitario, totalItem;

            try
            {
                itens.ForEach(item =>
                {
                    SqlConnection connection = new(ConnString);

                    id = item.Id;
                    dataCompra = item.DataCompra.Date.ToString("yyyy/MM/dd").Replace("/", "-");
                    mprima = int.Parse(item.MateriaPrima);
                    qt = item.Quantidade;
                    valorUnitario = item.ValorUnitario;
                    totalItem = item.TotalItem;

                    using (connection)
                    {
                        string sql = $"INSERT INTO Item_Compra VALUES ({id}, CONVERT(DATE, '{dataCompra}', 111), {mprima}, {qt}, {valorUnitario}, {totalItem});";
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

        public List<ItemCompra> ImprimirItens(int codigo)
        {
            SqlConnection connection = new(ConnString);

            string dataCompra = "";
            int id, mprima;
            decimal qt = 0, valorUnitario = 0, totalItem = 0;

            string sql = $"SELECT * from Item_Compra WHERE ID = {codigo};";

            List<ItemCompra> itens = new();
            ItemCompra item;

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
                            dataCompra = reader.GetDateTime(1).ToString("dd/MM/yyyy");
                            mprima = int.Parse(reader.GetValue(2).ToString());
                            qt = reader.GetDecimal(3);
                            valorUnitario = reader.GetDecimal(4);
                            totalItem = reader.GetDecimal(5);

                            item = new();
                            item.Id = id;
                            item.DataCompra = Convert.ToDateTime(dataCompra);
                            item.MateriaPrima = "MP" + mprima.ToString("0000");
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
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace CadastrosBasicos
{
    public class BDCadastro
    {
        public string DataSource { get; }
        public string DataBase { get; }
        public string UserName { get; }
        public string Password { get; }
        public string ConnString { get; }

        public BDCadastro()
        {
            DataSource = "DESKTOP-6VFRPCQ";
            DataBase = "Biltiful";
            UserName = "sa";
            Password = "&A1T2";
            ConnString = @"Data Source=" + DataSource + ";Initial Catalog="
                                + DataBase + ";Persist Security Info=True;User ID=" + UserName + ";Password=" + Password;
        }

        // Cliente
        public void GravarCliente(Cliente cliente)
        {
            SqlConnection connection = new(ConnString);

            string cpf = cliente.CPF.Insert(3, ".").Insert(7, ".").Insert(11, "-");
            string nome = cliente.Nome.Trim();
            string dataNasc = cliente.DataNascimento.Date.ToString("yyyy/MM/dd").Replace("/", "-");
            char sexo = char.Parse(cliente.Sexo.ToString().ToUpper());
            string ultimaCompra = cliente.UltimaVenda.Date.ToString("yyyy/MM/dd").Replace("/", "-");
            string dataCadastro = cliente.DataCadastro.Date.ToString("yyyy/MM/dd").Replace("/", "-");
            char situacao = char.Parse(cliente.Situacao.ToString().ToUpper());
            int risco = 0;

            try
            {
                using (connection)
                {
                    string sql = $"INSERT INTO Cliente VALUES ('{cpf}', '{nome}', CONVERT(DATE, '{dataNasc}', 111), '{sexo}', CONVERT(DATE, '{ultimaCompra}', 111), CONVERT(DATE, '{dataCadastro}', 111), '{situacao}', {risco});";
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

        public void BloquearCliente(string cpf)
        {
            string cpfFormatado = cpf.Insert(3, ".").Insert(7, ".").Insert(11, "-");
            int risco = 1;

            SqlConnection connection = new(ConnString);

            try
            {
                using (connection)
                {
                    string sql = $"UPDATE Cliente SET Risco = {risco} WHERE CPF = '{cpfFormatado}';";
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

        public void DesbloquearCliente(string cpf)
        {
            string cpfFormatado = cpf.Insert(3, ".").Insert(7, ".").Insert(11, "-");
            int risco = 0;

            SqlConnection connection = new(ConnString);

            try
            {
                using (connection)
                {
                    string sql = $"UPDATE Cliente SET Risco = {risco} WHERE CPF = '{cpfFormatado}';";
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

        public void EditarCliente(Cliente cliente, bool updateVenda = false, string dataUltimaVenda = "", string cpfPassado = "")
        {
            if (updateVenda)
            {
                SqlConnection connection = new(ConnString);

                string cpfFormatado = cliente.CPF.Insert(3, ".").Insert(7, ".").Insert(11, "-");

                try
                {
                    using (connection)
                    {
                        string sql = $"UPDATE Cliente SET UltimaCompra = CONVERT(DATE, '{dataUltimaVenda}', 111) WHERE CPF = '{cpfFormatado}';";
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
            else
            {
                string cpf = cliente.CPF.Insert(3, ".").Insert(7, ".").Insert(11, "-");
                string nome = cliente.Nome.Trim();
                string dataNasc = cliente.DataNascimento.Date.ToString("yyyy/MM/dd").Replace("/", "-");
                char situacao = char.Parse(cliente.Situacao.ToString().ToUpper());

                SqlConnection connection = new(ConnString);

                try
                {
                    using (connection)
                    {
                        string sql = $"UPDATE Cliente SET Nome = '{nome}', DataNasc = CONVERT(DATE, '{dataNasc}', 111), Situacao = '{situacao}'  WHERE CPF = '{cpf}';";
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

        public Cliente LocalizarCliente(string cpf)
        {
            SqlConnection connection = new(ConnString);

            string cpfFormatado = cpf.Insert(3, ".").Insert(7, ".").Insert(11, "-");

            string nome = "", dataNasc = "", ultimaCompra = "", dataCadastro = "";
            char sexo = 'n', situacao = 'n';
            bool risco = true;

            string sql = $"SELECT * from Cliente WHERE CPF = '{cpfFormatado}';";

            Cliente cliente = null;

            try
            {
                connection.Open();
                using (SqlCommand command = new(sql, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            cpfFormatado = reader.GetValue(0).ToString();
                            nome = reader.GetValue(1).ToString();
                            dataNasc = reader.GetDateTime(2).ToString("dd/MM/yyyy");
                            sexo = char.Parse(reader.GetValue(3).ToString());
                            ultimaCompra = reader.GetDateTime(4).ToString("dd/MM/yyyy");
                            dataCadastro = reader.GetDateTime(5).ToString("dd/MM/yyyy");
                            situacao = char.Parse(reader.GetValue(6).ToString());
                            risco = Boolean.Parse(reader.GetValue(7).ToString());
                        }
                    }
                }
                connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("EX -> " + ex.Message);
            }

            if (string.IsNullOrEmpty(nome))
                return cliente;
            else
            {
                cliente = new();
                cliente.CPF = cpf;
                cliente.Nome = nome;
                cliente.DataNascimento = Convert.ToDateTime(dataNasc);
                cliente.Sexo = sexo;
                cliente.UltimaVenda = Convert.ToDateTime(ultimaCompra);
                cliente.DataCadastro = Convert.ToDateTime(dataCadastro);
                cliente.Situacao = situacao;
                cliente.Condicao = risco;

                return cliente;
            }
        }

        public List<Cliente> ListarClientes()
        {
            SqlConnection connection = new(ConnString);

            string cpf = "", nome = "", dataNasc = "", ultimaCompra = "", dataCadastro = "";
            char sexo = 'n', situacao = 'n';
            bool risco = true;

            string sql = $"SELECT * from Cliente;";

            List<Cliente> clientes = new();
            Cliente cliente;

            try
            {
                connection.Open();
                using (SqlCommand command = new(sql, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            cpf = reader.GetValue(0).ToString();
                            nome = reader.GetValue(1).ToString();
                            dataNasc = reader.GetDateTime(2).ToString("dd/MM/yyyy");
                            sexo = char.Parse(reader.GetValue(3).ToString());
                            ultimaCompra = reader.GetDateTime(4).ToString("dd/MM/yyyy");
                            dataCadastro = reader.GetDateTime(5).ToString("dd/MM/yyyy");
                            situacao = char.Parse(reader.GetValue(6).ToString());
                            risco = Boolean.Parse(reader.GetValue(7).ToString());

                            cliente = new();
                            cliente.CPF = cpf;
                            cliente.Nome = nome;
                            cliente.DataNascimento = Convert.ToDateTime(dataNasc);
                            cliente.Sexo = sexo;
                            cliente.UltimaVenda = Convert.ToDateTime(ultimaCompra);
                            cliente.DataCadastro = Convert.ToDateTime(dataCadastro);
                            cliente.Situacao = situacao;
                            clientes.Add(cliente);
                        }
                    }
                }
                connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("EX -> " + ex.Message);
            }

            return clientes;
        }

        public bool TemCliente()
        {
            SqlConnection connection = new(ConnString);

            string sql = $"SELECT * from Cliente;";
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

        // Fornecedor
        public void GravarFornecedor(Fornecedor fornecedor)
        {
            SqlConnection connection = new(ConnString);

            string cnpj = fornecedor.CNPJ.Insert(2, ".").Insert(6, ".").Insert(10, "/").Insert(15, "-");
            string rsocial = fornecedor.RazaoSocial.Trim();
            string dataAbertura = fornecedor.DataAbertura.Date.ToString("yyyy/MM/dd").Replace("/", "-");
            string ultimaCompra = fornecedor.UltimaCompra.Date.ToString("yyyy/MM/dd").Replace("/", "-");
            string dataCadastro = fornecedor.DataCadastro.Date.ToString("yyyy/MM/dd").Replace("/", "-");
            char situacao = char.Parse(fornecedor.Situacao.ToString().ToUpper());
            int bloqueado = 0;

            try
            {
                using (connection)
                {
                    string sql = $"INSERT INTO Fornecedor VALUES ('{cnpj}', '{rsocial}', CONVERT(DATE, '{dataAbertura}', 111), CONVERT(DATE, '{ultimaCompra}', 111), CONVERT(DATE, '{dataCadastro}', 111), '{situacao}', {bloqueado});";
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

        public void EditarFornecedor(Fornecedor fornecedor)
        {
            SqlConnection connection = new(ConnString);

            string cnpj = fornecedor.CNPJ.Insert(2, ".").Insert(6, ".").Insert(10, "/").Insert(15, "-");
            string rsocial = fornecedor.RazaoSocial.Trim();
            char situacao = char.Parse(fornecedor.Situacao.ToString().ToUpper());

            try
            {
                using (connection)
                {
                    string sql = $"UPDATE Fornecedor SET Razao_Social = '{rsocial}', Situacao = '{situacao}'  WHERE CNPJ = '{cnpj}';";
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

        public void BloquearFornecedor(string cnpj)
        {

            SqlConnection connection = new(ConnString);

            string cnpjFormatado = cnpj.Insert(2, ".").Insert(6, ".").Insert(10, "/").Insert(15, "-");
            int bloqueado = 1;

            try
            {
                using (connection)
                {
                    string sql = $"UPDATE Fornecedor SET Bloqueado = {bloqueado}  WHERE CNPJ = '{cnpjFormatado}';";
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

        public void DesbloquearFornecedor(string cnpj)
        {

            SqlConnection connection = new(ConnString);

            string cnpjFormatado = cnpj.Insert(2, ".").Insert(6, ".").Insert(10, "/").Insert(15, "-");
            int bloqueado = 0;

            try
            {
                using (connection)
                {
                    string sql = $"UPDATE Fornecedor SET Bloqueado = {bloqueado}  WHERE CNPJ = '{cnpjFormatado}';";
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

        public Fornecedor LocalizarFornecedor(string cnpj)
        {
            SqlConnection connection = new(ConnString);

            string cnpjFormatado = cnpj.Insert(2, ".").Insert(6, ".").Insert(10, "/").Insert(15, "-");

            string rsocial = "", dataAbertura = "", ultimaCompra = "", dataCadastro = "";
            char situacao = 'n';
            bool bloqueado = true;

            string sql = $"SELECT * from Fornecedor WHERE CNPJ = '{cnpjFormatado}';";

            Fornecedor fornecedor = null;

            try
            {
                connection.Open();
                using (SqlCommand command = new(sql, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            cnpjFormatado = reader.GetValue(0).ToString();
                            rsocial = reader.GetValue(1).ToString();
                            dataAbertura = reader.GetDateTime(2).ToString("dd/MM/yyyy");
                            ultimaCompra = reader.GetDateTime(3).ToString("dd/MM/yyyy");
                            dataCadastro = reader.GetDateTime(4).ToString("dd/MM/yyyy");
                            situacao = char.Parse(reader.GetValue(5).ToString());
                            bloqueado = Boolean.Parse(reader.GetValue(6).ToString());
                        }
                    }
                }
                connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("EX -> " + ex.Message);
            }

            if (string.IsNullOrEmpty(rsocial))
                return fornecedor;
            else
            {
                fornecedor = new();
                fornecedor.CNPJ = cnpj;
                fornecedor.RazaoSocial = rsocial;
                fornecedor.DataAbertura = Convert.ToDateTime(dataAbertura);
                fornecedor.UltimaCompra = Convert.ToDateTime(ultimaCompra);
                fornecedor.DataCadastro = Convert.ToDateTime(dataCadastro);
                fornecedor.Situacao = situacao;
                fornecedor.Condicao = bloqueado;

                return fornecedor;
            }
        }

        public List<Fornecedor> ListarFornecedores()
        {
            SqlConnection connection = new(ConnString);

            string cnpj = "", rsocial = "", dataAbertura = "", ultimaCompra = "", dataCadastro = "";
            char situacao = 'n';
            bool bloqueado = true;

            string sql = $"SELECT * from Fornecedor;";

            List<Fornecedor> fornecedores = new();
            Fornecedor fornecedor;

            try
            {
                connection.Open();
                using (SqlCommand command = new(sql, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            cnpj = reader.GetValue(0).ToString();
                            rsocial = reader.GetValue(1).ToString();
                            dataAbertura = reader.GetDateTime(2).ToString("dd/MM/yyyy");
                            ultimaCompra = reader.GetDateTime(3).ToString("dd/MM/yyyy");
                            dataCadastro = reader.GetDateTime(4).ToString("dd/MM/yyyy");
                            situacao = char.Parse(reader.GetValue(5).ToString());
                            bloqueado = Boolean.Parse(reader.GetValue(6).ToString());

                            fornecedor = new();
                            fornecedor.CNPJ = cnpj;
                            fornecedor.RazaoSocial = rsocial;
                            fornecedor.DataAbertura = Convert.ToDateTime(dataAbertura);
                            fornecedor.UltimaCompra = Convert.ToDateTime(ultimaCompra);
                            fornecedor.DataCadastro = Convert.ToDateTime(dataCadastro);
                            fornecedor.Situacao = situacao;
                            fornecedores.Add(fornecedor);
                        }
                    }
                }
                connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("EX -> " + ex.Message);
            }

            return fornecedores;
        }

        public bool TemFornecedor()
        {
            SqlConnection connection = new(ConnString);

            string sql = $"SELECT * from Fornecedor;";
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

        // Produto
        public void GravarProduto(Produto produto)
        {
            SqlConnection connection = new(ConnString);

            string codigo = produto.CodigoBarras;
            string nome = produto.Nome.Trim();
            decimal valor = produto.ValorVenda;
            string ultimaVenda = produto.UltimaVenda.Date.ToString("yyyy/MM/dd").Replace("/", "-");
            string dataCadastro = produto.DataCadastro.Date.ToString("yyyy/MM/dd").Replace("/", "-");
            char situacao = char.Parse(produto.Situacao.ToString().ToUpper());

            try
            {
                using (connection)
                {
                    string sql = $"INSERT INTO Produto VALUES ('{codigo}', '{nome}', {valor}, CONVERT(DATE, '{ultimaVenda}', 111), CONVERT(DATE, '{dataCadastro}', 111), '{situacao}');";
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

        public void EditarProduto(string cod, string situacao, string dataUltimaVenda = null)
        {
            SqlConnection connection = new(ConnString);

            if (string.IsNullOrEmpty(dataUltimaVenda))
            {
                try
                {
                    using (connection)
                    {
                        string sql = $"UPDATE Produto SET Situacao = '{situacao}'  WHERE Codigo_Barras = '{cod}';";
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
            else
            {
                try
                {
                    using (connection)
                    {
                        string sql = $"UPDATE Produto SET Ultima_Venda = CONVERT(DATE, '{dataUltimaVenda}', 111)  WHERE Codigo_Barras = '{cod}';";
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

        public Produto LocalizarProduto(string codigo)
        {
            SqlConnection connection = new(ConnString);

            Produto produto = null;

            string nome = "", ultimaVenda = "", dataCadastro = "";
            char situacao = 'n';
            decimal valor = 0;

            string sql = $"SELECT * from Produto WHERE Codigo_Barras = '{codigo}';";

            try
            {
                connection.Open();
                using (SqlCommand command = new(sql, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            codigo = reader.GetValue(0).ToString();
                            nome = reader.GetValue(1).ToString();
                            valor = decimal.Parse(reader.GetValue(2).ToString());
                            ultimaVenda = reader.GetDateTime(3).ToString("dd/MM/yyyy");
                            dataCadastro = reader.GetDateTime(4).ToString("dd/MM/yyyy");
                            situacao = char.Parse(reader.GetValue(5).ToString());
                        }
                    }
                }
                connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("EX -> " + ex.Message);
            }

            if (string.IsNullOrEmpty(nome))
                return produto;
            else
            {
                produto = new();
                produto.CodigoBarras = codigo;
                produto.Nome = nome;
                produto.ValorVenda = valor;
                produto.UltimaVenda = Convert.ToDateTime(ultimaVenda);
                produto.DataCadastro = Convert.ToDateTime(dataCadastro);
                produto.Situacao = situacao;

                return produto;
            }
        }

        public List<Produto> ListarProdutos()
        {
            SqlConnection connection = new(ConnString);

            string nome = "", ultimaVenda = "", dataCadastro = "", codigo = "";
            char situacao = 'n';
            decimal valor = 0;

            string sql = $"SELECT * from Produto;";

            List<Produto> produtos = new();
            Produto produto;

            try
            {
                connection.Open();
                using (SqlCommand command = new(sql, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            codigo = reader.GetValue(0).ToString();
                            nome = reader.GetValue(1).ToString();
                            valor = decimal.Parse(reader.GetValue(2).ToString());
                            ultimaVenda = reader.GetDateTime(3).ToString("dd/MM/yyyy");
                            dataCadastro = reader.GetDateTime(4).ToString("dd/MM/yyyy");
                            situacao = char.Parse(reader.GetValue(5).ToString());

                            produto = new();
                            produto.CodigoBarras = codigo;
                            produto.Nome = nome;
                            produto.ValorVenda = valor;
                            produto.UltimaVenda = Convert.ToDateTime(ultimaVenda);
                            produto.DataCadastro = Convert.ToDateTime(dataCadastro);
                            produto.Situacao = situacao;
                            produtos.Add(produto);
                        }
                    }
                }
                connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("EX -> " + ex.Message);
            }

            return produtos;
        }

        public Produto RetornarProduto(string codigo)
        {
            SqlConnection connection = new(ConnString);

            string nome = "", ultimaVenda = "", dataCadastro = "";
            char situacao = 'n';
            decimal valor = 0;

            string sql = $"SELECT * from Produto WHERE Codigo_Barras = '{codigo}';";

            Produto produto = null;

            try
            {
                connection.Open();
                using (SqlCommand command = new(sql, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            codigo = reader.GetValue(0).ToString();
                            nome = reader.GetValue(1).ToString();
                            valor = decimal.Parse(reader.GetValue(2).ToString());
                            ultimaVenda = reader.GetDateTime(3).ToString("dd/MM/yyyy");
                            dataCadastro = reader.GetDateTime(4).ToString("dd/MM/yyyy");
                            situacao = char.Parse(reader.GetValue(5).ToString());

                            produto = new();
                            produto.CodigoBarras = codigo;
                            produto.Nome = nome;
                            produto.ValorVenda = valor;
                            produto.UltimaVenda = Convert.ToDateTime(ultimaVenda);
                            produto.DataCadastro = Convert.ToDateTime(dataCadastro);
                            produto.Situacao = situacao;
                        }
                    }
                }
                connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("EX -> " + ex.Message);
            }

            return produto;
        }

        public bool TemProduto()
        {
            SqlConnection connection = new(ConnString);

            string sql = $"SELECT * from Produto;";
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

        // Materia-Prima
        public void GravarMateriaPrima(MPrima mprima)
        {
            SqlConnection connection = new(ConnString);

            string nome = mprima.Nome.Trim();
            string ultimaCompra = mprima.UltimaCompra.Date.ToString("yyyy/MM/dd").Replace("/", "-");
            string dataCadastro = mprima.DataCadastro.Date.ToString("yyyy/MM/dd").Replace("/", "-");
            char situacao = char.Parse(mprima.Situacao.ToString().ToUpper());

            try
            {
                using (connection)
                {
                    string sql = $"INSERT INTO Materia_Prima VALUES ('{nome}', CONVERT(DATE, '{ultimaCompra}', 111), CONVERT(DATE, '{dataCadastro}', 111), '{situacao}');";
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

        public void EditarMateriaPrima(int cod, string situacao, string dataUltimaCompra = null)
        {
            SqlConnection connection = new(ConnString);

            if (string.IsNullOrEmpty(dataUltimaCompra))
            {
                try
                {
                    using (connection)
                    {
                        string sql = $"UPDATE Materia_Prima SET Situacao = '{situacao}'  WHERE ID = {cod};";
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
            else
            {
                try
                {
                    using (connection)
                    {
                        string sql = $"UPDATE Materia_Prima SET Ultima_Compra = CONVERT(DATE, '{dataUltimaCompra}', 111)  WHERE ID = {cod};";
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

        public MPrima LocalizarMateriaPrima(int codigo)
        {
            SqlConnection connection = new(ConnString);

            MPrima mprima = null;

            string nome = "", ultimaCompra = "", dataCadastro = "";
            char situacao = 'n';
            int id = 0;

            string sql = $"SELECT * from Materia_Prima WHERE ID = {codigo};";

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
                            nome = reader.GetValue(1).ToString();
                            ultimaCompra = reader.GetDateTime(2).ToString("dd/MM/yyyy");
                            dataCadastro = reader.GetDateTime(3).ToString("dd/MM/yyyy");
                            situacao = char.Parse(reader.GetValue(4).ToString());
                        }
                    }
                }
                connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("EX -> " + ex.Message);
            }

            if (string.IsNullOrEmpty(nome))
                return mprima;
            else
            {
                mprima = new();
                mprima.Id = "MP" + id.ToString("0000");
                mprima.Nome = nome;
                mprima.UltimaCompra = Convert.ToDateTime(ultimaCompra);
                mprima.DataCadastro = Convert.ToDateTime(dataCadastro);
                mprima.Situacao = situacao;

                return mprima;
            }
        }

        public List<MPrima> ListarMateriaPrimas()
        {
            SqlConnection connection = new(ConnString);

            string nome = "", ultimaCompra = "", dataCadastro = "";
            char situacao = 'n';
            int id = 0;

            string sql = $"SELECT * from Materia_Prima;";

            List<MPrima> mprimas = new();
            MPrima mprima;

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
                            nome = reader.GetValue(1).ToString();
                            ultimaCompra = reader.GetDateTime(2).ToString("dd/MM/yyyy");
                            dataCadastro = reader.GetDateTime(3).ToString("dd/MM/yyyy");
                            situacao = char.Parse(reader.GetValue(4).ToString());

                            mprima = new();
                            mprima.Id = "MP" + id.ToString("0000");
                            mprima.Nome = nome;
                            mprima.UltimaCompra = Convert.ToDateTime(ultimaCompra);
                            mprima.DataCadastro = Convert.ToDateTime(dataCadastro);
                            mprima.Situacao = situacao;
                        }
                    }
                }
                connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("EX -> " + ex.Message);
            }

            return mprimas;
        }

        public bool TemMPrima()
        {
            SqlConnection connection = new(ConnString);

            string sql = $"SELECT * from Materia_Prima;";
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
    }
}


using CadastrosBasicos.ManipulaArquivos;
using System;
using System.Collections.Generic;

namespace CadastrosBasicos
{
    public class Cliente
    {
        public Read read = new Read();
        public string CPF { get; set; }
        public string Nome { get; set; }
        public DateTime DataNascimento { get; set; }
        public char Sexo { get; set; }
        public DateTime UltimaVenda { get; set; }
        public DateTime DataCadastro { get; set; }
        public char Situacao { get; set; }
        public bool Condicao { get; set; }

        public Cliente()
        {

        }

        public Cliente(string cpf, string name, DateTime dataNascimento, char sexo, char situacao)
        {
            CPF = cpf;
            Nome = name;
            DataNascimento = dataNascimento;
            Sexo = sexo;
            UltimaVenda = DateTime.Now;
            DataCadastro = DateTime.Now;
            Situacao = situacao;
        }

        public Cliente(string cpf, string name, DateTime dataNascimento, char sexo, DateTime UltimaCompra, DateTime dataCadastro, char situacao)
        {
            CPF = cpf;
            Nome = name;
            DataNascimento = dataNascimento;
            Sexo = sexo;
            UltimaVenda = UltimaCompra;
            DataCadastro = dataCadastro;
            Situacao = situacao;
        }

        public void BloquearCadastro()
        {
            BDCadastro bd = new();
            Cliente cliente;

            Console.WriteLine("Insira o CPF para bloqueio: ");
            string cpf = Console.ReadLine();
            cpf = cpf.Replace(".", "").Replace("-", "");

            cliente = bd.LocalizarCliente(cpf);

            if (cliente == null)
            {
                Console.WriteLine(" CPF nao encontrado.");
                Console.WriteLine(" Pressione ENTER para voltar...");
                Console.ReadKey();
            }
            else
            {
                bd.BloquearCliente(cpf);

                Console.WriteLine(" Cliente bloqueado.");
                Console.WriteLine(" Pressione ENTER para voltar...");
                Console.ReadKey();
            }
        }

        public void DesbloquearCadastro()
        {
            BDCadastro bd = new();
            Cliente cliente;

            Console.WriteLine("Insira o CPF para desbloqueio: ");
            string cpf = Console.ReadLine();
            cpf = cpf.Replace(".", "").Replace("-", "");

            cliente = bd.LocalizarCliente(cpf);

            if (cliente == null)
            {
                Console.WriteLine(" CPF nao encontrado.");
                Console.WriteLine(" Pressione ENTER para voltar...");
                Console.ReadKey();
            }
            else
            {
                bd.DesbloquearCliente(cpf);

                Console.WriteLine(" Cliente desbloqueado.");
                Console.WriteLine(" Pressione ENTER para voltar...");
                Console.ReadKey();
            }
        }

        public void Editar()
        {
            BDCadastro bd = new();
            Cliente cliente;

            Console.WriteLine("Somente algumas informacoes podem ser alterada como (Nome/Data de Nascimento/sexo/Situacao), caso nao queira alterar alguma informacao pressione enter!");
            Console.Write("CPF: ");
            string cpf = Console.ReadLine();
            cpf = cpf.Replace(".", "").Replace("-", "");

            cliente = bd.LocalizarCliente(cpf);

            if (cliente != null)
            {
                Console.WriteLine("Nome: ");
                string nome = Console.ReadLine().Trim();
                Console.WriteLine("Data de nascimento: ");
                bool flag = DateTime.TryParse(Console.ReadLine(), out DateTime dNascimento);
                Console.WriteLine("Situacao [A - Ativo/I - Inativo]: ");
                bool flagSituacao = char.TryParse(Console.ReadLine().Trim().ToUpper(), out char situacao);

                cliente.Nome = nome == "" ? cliente.Nome : nome.PadLeft(50, ' ');
                cliente.DataNascimento = flag == false ? cliente.DataNascimento : dNascimento;
                cliente.Situacao = flagSituacao == false ? cliente.Situacao : situacao;

                bd.EditarCliente(cliente);

                Console.WriteLine(" Cliente editado.");
                Console.WriteLine(" Pressione ENTER para voltar...");
                Console.ReadKey();
            }
            else
            {
                Console.WriteLine(" CPF nao encontrado.");
                Console.WriteLine(" Pressione ENTER para voltar...");
                Console.ReadKey();
            }
        }

        public void Navegar()
        {
            Console.WriteLine("============== Cliente ==============");

            BDCadastro bd = new();
            List<Cliente> lista = bd.ListarClientes();

            if (lista.Count > 0)
            {
                int opcao = 0, posicao = 0;
                bool flag = false;
                do
                {
                    Console.Clear();
                    Console.WriteLine("============== Cliente ==============");

                    if (opcao == 0)
                    {
                        Console.WriteLine(lista[posicao].ToString());
                    }
                    else if (opcao == 1)
                    {
                        if (posicao == lista.Count - 1)
                            posicao = lista.Count - 1;
                        else
                            posicao++;
                        Console.WriteLine(lista[posicao].ToString());
                    }
                    else if (opcao == 2)
                    {
                        if (posicao == 0)
                            posicao = 0;
                        else
                            posicao--;
                        Console.WriteLine(lista[posicao].ToString());
                    }
                    else if (opcao == 3)
                    {
                        posicao = 0;
                        Console.WriteLine(lista[posicao].ToString());
                    }
                    else if (opcao == 4)
                    {
                        posicao = lista.Count - 1;
                        Console.WriteLine(lista[posicao].ToString());
                    }


                    Console.WriteLine(@"
1. Proximo 
2. Anterior
3. Primeiro
4. Ultimo
0. Voltar para menu anterior.
");
                    do
                    {
                        flag = int.TryParse(Console.ReadLine(), out opcao);
                    } while (flag != true);

                } while (opcao != 0);

            }
            else
            {
                Console.Clear();
                Console.WriteLine("Ainda nao tem nenhum cliente cadastrado");
                Console.WriteLine("Pressione enter para continuar...");
                Console.ReadKey();
            }
        }

        public void Localizar()
        {
            BDCadastro bd = new();

            Console.WriteLine("Insira o cpf para localizar: ");
            string cpf = Console.ReadLine();
            cpf = cpf.Replace(".", "").Replace("-", "");

            Cliente cliente = bd.LocalizarCliente(cpf);

            if (cliente != null)
            {
                string situacao = cliente.Situacao.ToString();
                if (situacao == "A")
                    situacao = "Ativo";
                else if (situacao == "I")
                    situacao = "Inativo";

                Console.WriteLine("\n---------------------------------------\n");
                Console.WriteLine($" CPF:           {cliente.CPF}");
                Console.WriteLine($" Nome:          {cliente.Nome}");
                Console.WriteLine($" Data Nasc.:    {cliente.DataNascimento:dd/MM/yyyy}");
                Console.WriteLine($" Sexo:          {(cliente.Sexo == 'M' ? "Masculino" : "Feminino")}");
                Console.WriteLine($" Ultima Compra: {cliente.UltimaVenda:dd/MM/yyyy}");
                Console.WriteLine($" Data Cadastro: {cliente.DataCadastro:dd/MM/yyyy}");
                Console.WriteLine($" Situacao:      {situacao}");
                Console.WriteLine($" Condicao:      {(cliente.Condicao ? "Bloqueado" : "Em dia")}");
                Console.WriteLine("\n---------------------------------------\n");
            }
            else
                Console.WriteLine("Nenhum cadastrado foi encontrado!");

            Console.WriteLine("Pressione ENTER para voltar...");
            Console.ReadKey();
        }

        public override string ToString()
        {
            return $"CPF: {CPF}\nNome: {Nome.Trim()}\nData de nascimento: {DataNascimento.ToString("dd/MM/yyyy")}\nSexo: {Sexo}\nUltima Compra: {UltimaVenda.ToString("dd/MM/yyyy")}\nDia de Cadastro: {DataCadastro.ToString("dd/MM/yyyy")}\nSituacao: {Situacao}";
        }

    }
}

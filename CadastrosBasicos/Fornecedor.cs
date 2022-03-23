using CadastrosBasicos.ManipulaArquivos;
using System;
using System.Collections.Generic;
using System.IO;

namespace CadastrosBasicos
{
    public class Fornecedor
    {
        public Read read = new Read();
        public string CNPJ { get; set; }
        public string RazaoSocial { get; set; }
        public DateTime DataAbertura { get; set; }
        public DateTime UltimaCompra { get; set; }
        public DateTime DataCadastro { get; set; }
        public char Situacao { get; set; }
        public bool Condicao { get; set; }

        public Fornecedor()
        {

        }
        public Fornecedor(string cnpj, string rSocial, DateTime dAbertura, char situacao)
        {
            CNPJ = cnpj;
            RazaoSocial = rSocial;
            DataAbertura = dAbertura;
            UltimaCompra = DateTime.Now;
            DataCadastro = DateTime.Now;
            Situacao = situacao;
        }
        public Fornecedor(string cnpj, string rSocial, DateTime dAbertura, DateTime uCompra, DateTime dCadastro, char situacao)
        {
            CNPJ = cnpj;
            RazaoSocial = rSocial;
            DataAbertura = dAbertura;
            UltimaCompra = DateTime.Now;
            DataCadastro = DateTime.Now;
            Situacao = situacao;
        }
        public void Navegar()
        {
            Console.WriteLine("============== Fornecedores ==============");

            BDCadastro bd = new();
            List<Fornecedor> lista = bd.ListarFornecedores();

            if (lista.Count > 0)
            {
                
                int opcao = 0, posicao = 0;
                bool flag = false;
                do
                {
                    Console.Clear();
                    Console.WriteLine("============== Fornecedores ==============");

                    if (opcao == 0)
                    {
                        Console.WriteLine(lista[posicao].ToString());
                    }
                    else if (opcao == 1)
                    {
                        if (posicao != lista.Count - 1)
                            posicao++;

                        Console.WriteLine(lista[posicao].ToString());
                    }
                    else if (opcao == 2)
                    {
                        if (posicao != 0)
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
                Console.WriteLine("Ainda nao tem nenhum fornecedor cadastrado");
                Console.WriteLine("Pressione enter para continuar");
                Console.ReadKey();
            }
        }

        public void Localizar()
        {
            BDCadastro bd = new();

            Console.WriteLine("Insira o CNPJ para localizar: ");
            string cnpj = Console.ReadLine();
            cnpj = cnpj.Replace(".", "").Replace("-", "").Replace("/", "");

            Fornecedor fornecedor = bd.LocalizarFornecedor(cnpj);

            if (fornecedor != null)
            {
                string situacao = fornecedor.Situacao.ToString();
                if (situacao == "A")
                    situacao = "Ativo";
                else if (situacao == "I")
                    situacao = "Inativo";

                Console.WriteLine("\n---------------------------------------\n");
                Console.WriteLine($" CNPJ:          {fornecedor.CNPJ}");
                Console.WriteLine($" Razao Social:  {fornecedor.RazaoSocial}");
                Console.WriteLine($" Data Abertura: {fornecedor.DataAbertura}");
                Console.WriteLine($" Ultima Compra: {fornecedor.UltimaCompra}");
                Console.WriteLine($" Data Cadastro: {fornecedor.DataCadastro}");
                Console.WriteLine($" Situacao:      {situacao}");
                Console.WriteLine($" Condicao:      {(fornecedor.Condicao ? "Bloqueado" : "Em dia")}");
                Console.WriteLine("\n---------------------------------------\n");
            }
            else
                Console.WriteLine("Nenhum cadastrado foi encontrado!");

            Console.WriteLine("Pressione ENTER para voltar...");
            Console.ReadKey();
        }

        public void BloquearFornecedor()
        {
            BDCadastro bd = new();
            Fornecedor fornecedor;

            Console.WriteLine("Insira o CNPJ para bloqueio: ");
            string cnpj = Console.ReadLine();
            cnpj = cnpj.Replace(".", "").Replace("-", "").Replace("/", "");

            fornecedor = bd.LocalizarFornecedor(cnpj);

            if (fornecedor == null)
            {
                Console.WriteLine(" CNPJ nao encontrado.");
                Console.WriteLine(" Pressione ENTER para voltar...");
                Console.ReadKey();
            }
            else
            {
                bd.BloquearFornecedor(cnpj);

                Console.WriteLine(" Fornecedor bloqueado.");
                Console.WriteLine(" Pressione ENTER para voltar...");
                Console.ReadKey();
            }
        }

        public void DesbloquearFornecedor()
        {
            BDCadastro bd = new();
            Fornecedor fornecedor;

            Console.WriteLine("Insira o CNPJ para desbloqueio: ");
            string cnpj = Console.ReadLine();
            cnpj = cnpj.Replace(".", "").Replace("-", "").Replace("/", "");

            fornecedor = bd.LocalizarFornecedor(cnpj);

            if (fornecedor == null)
            {
                Console.WriteLine(" CNPJ nao encontrado.");
                Console.WriteLine(" Pressione ENTER para voltar...");
                Console.ReadKey();
            }
            else
            {
                bd.DesbloquearFornecedor(cnpj);

                Console.WriteLine(" Fornecedor desbloqueado.");
                Console.WriteLine(" Pressione ENTER para voltar...");
                Console.ReadKey();
            }
        }

        public string RetornaArquivo()
        {
            return CNPJ + RazaoSocial + DataAbertura.ToString("dd/MM/yyyy") + UltimaCompra.ToString("dd/MM/yyyy") + DataCadastro.ToString("dd/MM/yyyy") + Situacao;
        }

        public void Editar()
        {
            BDCadastro bd = new();
            Fornecedor fornecedor;

            Console.WriteLine("Somente algumas informacoes podem ser alterada como (Razao social/situacao), caso nao queira alterar alguma informacao pressione enter!");
            Console.Write("CNPJ: ");
            string cnpj = Console.ReadLine();
            cnpj = cnpj.Replace(".", "").Replace("-", "").Replace("/", "");

            fornecedor = bd.LocalizarFornecedor(cnpj);

            if (fornecedor != null)
            {
                Console.WriteLine("Razao social: ");
                string nome = Console.ReadLine().Trim().PadLeft(50, ' ');
                Console.WriteLine("Situacao [A - Ativo/ I - inativo]: ");
                bool flagSituacao = char.TryParse(Console.ReadLine().ToString().ToUpper(), out char situacao);

                fornecedor.RazaoSocial = nome == "" ? fornecedor.RazaoSocial : nome;
                fornecedor.Situacao = flagSituacao == false ? fornecedor.Situacao : situacao;

                bd.EditarFornecedor(fornecedor);

                Console.WriteLine(" Fornecedor editado.");
                Console.WriteLine(" Pressione ENTER para voltar...");
                Console.ReadKey();
            }
            else
            {
                Console.WriteLine(" Nenhum CNPJ encontrado.");
                Console.WriteLine(" Pressione ENTER para voltar...");
                Console.ReadKey();
            }
        }

        public override string ToString()
        {
            return $"CNPJ: {CNPJ}\nRSocial: {RazaoSocial.Trim()}\nData de Abertura da empresa: {DataAbertura.ToString("dd/MM/yyyy")}\nUltima Compra: {UltimaCompra.ToString("dd/MM/yyyy")}\nData de Cadastro: {DataCadastro.ToString("dd/MM/yyyy")}\nSituacao: {Situacao}";
        }
    }
}

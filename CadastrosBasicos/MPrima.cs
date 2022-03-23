using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadastrosBasicos
{
    public class MPrima
    {
        public string Id { get; set; }
        public string Nome { get; set; }
        public DateTime UltimaCompra { get; set; }
        public DateTime DataCadastro { get; set; }
        public char Situacao { get; set; }

        public MPrima()
        {

        }

        public MPrima(string id, string nome, DateTime uCompra, DateTime dCadastro, char situacao)
        {
            Id = id;
            Nome = nome;
            UltimaCompra = uCompra;
            DataCadastro = dCadastro;
            Situacao = situacao;
        }

        public override string ToString()
        {
            return Id
                + Nome.PadLeft(20, ' ')
                + UltimaCompra.ToString("dd/MM/yyyy").Replace("/", "")
                + DataCadastro.ToString("dd/MM/yyyy").Replace("/", "")
                + Situacao;
        }

        public void Menu()
        {
            string escolha;

            do
            {
                Console.Clear();
                Console.WriteLine("\n=============== MATÉRIA-PRIMA ===============");
                Console.WriteLine("1. Cadastrar Matéria-Prima");
                Console.WriteLine("2. Localizar Matéria-Prima");
                Console.WriteLine("3. Imprimir Matérias-Primas");
                Console.WriteLine("4. Alterar Situação da Matéria-Prima");
                Console.WriteLine("---------------------------------------------");
                Console.WriteLine("0. Voltar ao menu anterior");
                Console.Write("\nEscolha: ");

                switch (escolha = Console.ReadLine())
                {
                    case "0":
                        break;
                    case "1":
                        Cadastrar();
                        break;
                    case "2":
                        Localizar();
                        break;
                    case "3":
                        ImprimirMPrimas();
                        break;
                    case "4":
                        AlterarSituacao();
                        break;

                    default:
                        Console.WriteLine("\n Opção inválida.");
                        Console.WriteLine("\n Pressione ENTER para voltar ao menu.");
                        Console.ReadKey();
                        break;
                }

            } while (escolha != "0");
        }

        public void Cadastrar()
        {
            MPrima MPrima = new MPrima();

            char sit = 'A';
            string nomeTemp;
            bool flag = true;

            do
            {
                Console.Clear();
                Console.WriteLine("\n Cadastro de Materia-prima\n");
                Console.Write(" Nome: ");
                nomeTemp = Console.ReadLine();
                Console.Write(" Situacao (A / I): ");
                sit = char.Parse(Console.ReadLine().ToUpper());

                if (nomeTemp == null)
                {
                    Console.WriteLine(" Nenhum campo podera ser vazio.");
                    Console.WriteLine(" Pressione ENTER para voltar ao cadastro.");
                    Console.ReadKey();
                }
                else
                {
                    if (nomeTemp.Length > 20)
                    {
                        Console.WriteLine(" Nome invalido. Digite apenas 20 caracteres.");
                        Console.WriteLine(" Pressione ENTER para voltar ao cadastro.");
                        Console.ReadKey();
                    }
                    else if ((sit != 'A') && (sit != 'I'))
                    {
                        Console.WriteLine(" Situacao invalida.");
                        Console.WriteLine(" Pressione ENTER para voltar ao cadastro.");
                        Console.ReadKey();
                    }
                    else
                    {
                        flag = false;

                        MPrima.Nome = nomeTemp;
                        MPrima.UltimaCompra = DateTime.Now.Date;
                        MPrima.DataCadastro = DateTime.Now.Date;
                        MPrima.Situacao = sit;

                        BDCadastro bd = new();
                        bd.GravarMateriaPrima(MPrima);

                        Console.WriteLine("\n Cadastro de Materia-prima concluido com sucesso!\n");
                        Console.WriteLine("\n Pressione ENTER para voltar ao menu");
                        Console.ReadKey();
                    }
                }

            } while (flag);
        }

        public void Localizar()
        {
            string cod;

            BDCadastro bd = new();

            Console.Clear();
            Console.WriteLine("\n Localizar Materia-prima");
            Console.Write("\n Digite o codigo da materia-prima (somente numeros): ");
            cod = Console.ReadLine();

            int codigo = int.Parse(cod);
            MPrima mprima = bd.LocalizarMateriaPrima(codigo);

            if (mprima == null)
            {
                Console.WriteLine("\n A materia-prima nao existe.");
                Console.WriteLine("\n Pressione ENTER para voltar ao menu");
                Console.ReadKey();
            }
            else
            {
                string situacao = mprima.Situacao.ToString();
                if (situacao == "A")
                    situacao = "Ativo";
                else if (situacao == "I")
                    situacao = "Inativo";

                Console.WriteLine("\n---------------------------------------\n");
                Console.WriteLine($" Codigo:        {mprima.Id}");
                Console.WriteLine($" Nome:          {mprima.Nome}");
                Console.WriteLine($" Ultima Compra: {mprima.UltimaCompra:dd/MM/yyyy}");
                Console.WriteLine($" Data Cadastro: {mprima.DataCadastro:dd/MM/yyyy}");
                Console.WriteLine($" Situacao:      {situacao}");
                Console.WriteLine("\n---------------------------------------\n");

                Console.WriteLine("\n Pressione ENTER para voltar ao menu");
                Console.ReadKey();
            }
        }

        public void AlterarSituacao()
        {
            string cod, situacao;
            bool flag = true;

            BDCadastro bd = new();

            Console.Clear();
            Console.WriteLine("\n Alterar Materia-prima");
            Console.Write("\n Digite o codigo da materia-prima (somente numeros): ");
            cod = Console.ReadLine();

            int codigo = int.Parse(cod);
            MPrima mprima = bd.LocalizarMateriaPrima(codigo);

            if (mprima == null)
            {
                Console.WriteLine("\n A materia-prima nao existe.");
                Console.WriteLine("\n Pressione ENTER para voltar ao menu");
                Console.ReadKey();
            }
            else
            {
                situacao = mprima.Situacao.ToString();
                if (situacao == "A")
                    situacao = "Ativo";
                else if (situacao == "I")
                    situacao = "Inativo";

                Console.WriteLine("\n---------------------------------------\n");
                Console.WriteLine($" Codigo:        {mprima.Id}");
                Console.WriteLine($" Nome:          {mprima.Nome}");
                Console.WriteLine($" Ultima Compra: {mprima.UltimaCompra:dd/MM/yyyy}");
                Console.WriteLine($" Data Cadastro: {mprima.DataCadastro:dd/MM/yyyy}");
                Console.WriteLine($" Situacao:      {situacao}");
                Console.WriteLine("\n---------------------------------------\n");

                do
                {
                    Console.Write("\n Qual a nova situacao da materia-prima (A / I): ");
                    situacao = Console.ReadLine().ToUpper();

                    if ((situacao != "A") && (situacao != "I"))
                    {
                        Console.WriteLine("\n Situacao invalida.");
                        Console.WriteLine("\n Pressione ENTER para voltar ao cadastro.");
                        Console.ReadKey();
                    }
                    else
                    {
                        flag = false;
                    }

                } while (flag);

                Atualizar(cod, null, situacao);
            }
        }

        public void Atualizar(string cod, string dataUltimaCompra = null, string situacaoAtualizada = null)
        {
            BDCadastro bd = new();

            int codigo = int.Parse(cod);
            MPrima mprima = bd.LocalizarMateriaPrima(codigo);

            if (mprima == null)
            {
                Console.WriteLine("\n A materia-prima nao existe.");
                Console.WriteLine("\n Pressione ENTER para voltar");
                Console.ReadKey();
            }
            else
            {
                bd.EditarMateriaPrima(codigo, situacaoAtualizada, dataUltimaCompra);

                Console.WriteLine("\n Materia-prima alterada.");
                Console.WriteLine("\n Pressione ENTER para voltar ao menu");
                Console.ReadKey();
            }
        }

        public string Impressao(MPrima mPrima)
        {
            string situacao = "";
            if (mPrima.Situacao == 'A')
                situacao = "Ativo";
            else if (mPrima.Situacao == 'I')
                situacao = "Inativo";

            return "\n"
                + "\n Codigo: \t" + mPrima.Id
                + "\n Nome: \t\t" + mPrima.Nome
                + "\n Ultima Venda: \t" + mPrima.UltimaCompra.ToString("dd/MM/yyyy")
                + "\n Data Cadastro: " + mPrima.DataCadastro.ToString("dd/MM/yyyy")
                + "\n Situacao: \t" + situacao
                + "\n";
        }

        public void ImprimirMPrimas()
        {
            BDCadastro bd = new();

            List<MPrima> MPrimas = bd.ListarMateriaPrimas();

            if (MPrimas.Count > 0)
            {
                string escolha;
                int opcao = 1, posicao = 0;
                bool flag = true;

                do
                {
                    if ((opcao < 1) || (opcao > 5))
                    {
                        Console.WriteLine("\n Opcao invalida.");
                        Console.WriteLine("\n Pressione ENTER para voltar.");
                        Console.ReadKey();
                        opcao = 1;
                    }
                    else
                    {
                        if (opcao == 5)
                        {
                            flag = false;
                            return;
                        }
                        else if (opcao == 1)
                        {
                            Console.Clear();
                            Console.WriteLine("\n Impressao de Materias-primas");
                            Console.WriteLine(" --------------------------- ");
                            posicao = MPrimas.IndexOf(MPrimas.First());
                            Console.WriteLine($"\n Materia-prima {posicao + 1}");
                            Console.WriteLine(Impressao(MPrimas.First()));
                        }
                        else if (opcao == 4)
                        {
                            Console.Clear();
                            Console.WriteLine("\n Impressao de Materias-primas");
                            Console.WriteLine(" --------------------------- ");
                            posicao = MPrimas.IndexOf(MPrimas.Last());
                            Console.WriteLine($"\n Materia-prima {posicao + 1}");
                            Console.WriteLine(Impressao(MPrimas.Last()));
                        }
                        else if (opcao == 2)
                        {
                            if (posicao == 0)
                            {
                                Console.Clear();
                                Console.WriteLine("\n Impressao de Materias-primas");
                                Console.WriteLine(" --------------------------- ");
                                Console.WriteLine("\n Nao ha materia-prima anterior.\n");
                                Console.WriteLine(" --------------------------- ");
                                posicao = MPrimas.IndexOf(MPrimas.First());
                                Console.WriteLine($"\n Materia-prima {posicao + 1}");
                                Console.WriteLine(Impressao(MPrimas.First()));
                            }
                            else
                            {
                                Console.Clear();
                                Console.WriteLine("\n Impressao de Materias-primas");
                                Console.WriteLine(" --------------------------- ");
                                posicao--;
                                Console.WriteLine($"\n Materia-prima {posicao + 1}");
                                Console.WriteLine(Impressao(MPrimas[posicao]));
                                posicao = MPrimas.IndexOf(MPrimas[posicao]);
                            }
                        }
                        else if (opcao == 3)
                        {
                            if (posicao == MPrimas.IndexOf(MPrimas.Last()))
                            {
                                Console.Clear();
                                Console.WriteLine("\n Impressao de Materias-primas");
                                Console.WriteLine(" --------------------------- ");
                                Console.WriteLine("\n Nao ha proxima materia-prima.\n");
                                Console.WriteLine(" --------------------------- ");
                                Console.WriteLine($"\n Materia-prima {posicao + 1}");
                                Console.WriteLine(Impressao(MPrimas.Last()));
                                posicao = MPrimas.IndexOf(MPrimas.Last());
                            }
                            else
                            {
                                Console.Clear();
                                Console.WriteLine("\n Impressao de Materias-primas");
                                Console.WriteLine(" --------------------------- ");
                                posicao++;
                                Console.WriteLine($"\n Materia-prima {posicao + 1}");
                                Console.WriteLine(Impressao(MPrimas[posicao]));
                                posicao = MPrimas.IndexOf(MPrimas[posicao]);
                            }
                        }

                        Console.WriteLine(" ------------------------------------------------------------------ ");
                        Console.WriteLine("\n Navegacao\n");
                        Console.WriteLine(" 1 - Primeira / 2 - Anterior / 3 - Proxima / 4 - Ultima / 5 - Sair");
                        Console.Write("\n Escolha: ");
                        escolha = Console.ReadLine();
                        int.TryParse(escolha, out opcao);
                    }

                } while (flag);
            }
            else
            {
                Console.WriteLine("\n Nao ha materias-primas cadastradas\n");
                Console.WriteLine("\n Pressione ENTER para voltar");
                Console.ReadKey();
            }
        }

        public MPrima RetornaMateriaPrima(string cod)
        {
            BDCadastro bd = new();

            int codigo = int.Parse(cod);
            MPrima mprima = bd.LocalizarMateriaPrima(codigo);

            return mprima;
        }
    }
}


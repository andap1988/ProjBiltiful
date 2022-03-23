using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using CadastrosBasicos;

namespace ProducaoCosmeticos
{
    public class Producao
    {
        #region Propriedades da Produção
        public string Id { get; set; }
        public string DataProducao { get; set; }
        public string Produto { get; set; }
        public decimal Quantidade { get; set; }
        public int Contador { get; set; }
        #endregion
        #region Construtor
        public Producao(string id, string dataProducao, string produto, decimal quantidade)
        {
            Id = id;
            DataProducao = dataProducao;
            Produto = produto;
            Quantidade = quantidade;
            Contador = 1;
        }

        public Producao() { }

        #endregion

        public void SubMenu()
        {
            BDCadastro bdCadastro = new();
            BDProducao bdProducao = new();
            string escolha;

            do
            {
                Console.Clear();
                Console.WriteLine("\n=============== PRODUÇÃO ===============");
                Console.WriteLine("1. Cadastrar uma produção");
                Console.WriteLine("2. Localizar um registro");
                Console.WriteLine("3. Imprimir por registro");
                Console.WriteLine("---------------------------------------");
                Console.WriteLine("0. Voltar ao menu anterior");
                Console.Write("\nEscolha: ");

                switch (escolha = Console.ReadLine())
                {
                    case "0":
                        break;
                    case "1":
                        if (bdCadastro.TemMPrima() && bdCadastro.TemProduto())
                        {
                            Console.Clear();
                            Cadastrar();
                        }
                        else
                        {
                            Console.WriteLine("Não ha produtos ou materias primas cadastradas. Favor verificar!");
                            Console.WriteLine("\n Pressione ENTER para voltar ao menu.");
                            Console.ReadKey();
                        }
                        break;
                    case "2":
                        if (bdProducao.TemProducao())
                        {
                            Console.Clear();
                            Localizar();
                        }
                        else
                        {
                            Console.WriteLine(" Não ha producao para exibir");
                            Console.WriteLine("\n Pressione ENTER para voltar ao menu.");
                            Console.ReadKey();
                        }
                        break;
                    case "3":
                        if (bdProducao.TemProducao())
                        {
                            Console.Clear();
                            ImprimirProducao();
                        }
                        else
                        {
                            Console.WriteLine(" Não ha producao para exibir");
                            Console.WriteLine("\n Pressione ENTER para voltar ao menu.");
                            Console.ReadKey();
                        }
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
            List<ItemProducao> itens = new();
            BDCadastro bdCadastro = new();
            BDProducao bdProducao = new();
            Produto pproduto;
            MPrima materiaPrima;
            Producao producao = null;

            string dataProducao = DateTime.Now.ToString("dd/MM/yyyy");
            string produto = null, auxiliarProduto, id, codigoMateriaPrima;
            decimal quantidade = 0, auxiliarQuantidade, quantidadeMateriaPrima;
            int escolha, opcao = 0;
            bool control = false;

            id = Contador.ToString().PadLeft(5, '0');

            Console.Write("\nData de produção: " + dataProducao + "\n");

            do
            {
                Console.Write("Digite o código do produto: ");
                auxiliarProduto = Console.ReadLine();

                if (auxiliarProduto.Length != 13)
                {
                    Console.WriteLine(" Codigo do produto invalido.");
                    Console.WriteLine(" Pressione ENTER para voltar...");
                    Console.ReadKey();
                }
                else
                {
                    pproduto = bdCadastro.LocalizarProduto(auxiliarProduto);

                    if (pproduto == null)
                    {

                        Console.WriteLine(" Código de produto inválido!");
                        Console.WriteLine("\n\n\t Pressione ENTER para continuar...");
                        Console.ReadKey();
                        Console.Clear();
                    }
                    else if (pproduto.Situacao.Equals('I'))
                    {
                        Console.WriteLine("Este produto esta inativo");
                        Console.ReadKey();
                        return;
                    }
                    else
                    {
                        produto = pproduto.CodigoBarras;
                        control = true;
                    }
                }

            } while (control != true);

            control = false;

            do
            {
                Console.Write("Quantidade: ");
                auxiliarQuantidade = decimal.Parse(Console.ReadLine());

                if (auxiliarQuantidade > 0 && auxiliarQuantidade < 1000)
                {
                    quantidade = auxiliarQuantidade;
                    control = true;
                }
                else
                {
                    Console.WriteLine("\nNão é possivel adicionar a quantidade digitada!");
                    Console.WriteLine("\nDigite a quantidade novamente");
                }

            }
            while (control != true);

            do
            {
                control = false;

                do
                {
                    Console.Write("Digite o código da matéria prima (somente numeros): ");
                    codigoMateriaPrima = Console.ReadLine();
                    int codigo = int.Parse(codigoMateriaPrima);

                    materiaPrima = bdCadastro.LocalizarMateriaPrima(codigo);

                    if (materiaPrima == null)
                    {

                        Console.WriteLine("Código de matéria prima inválida!");
                        Console.WriteLine("\n\n\t Pressione ENTER para continuar...");
                        Console.ReadKey();
                        Console.Clear();
                    }
                    else if (materiaPrima.Situacao.Equals('I'))
                    {
                        Console.WriteLine("O item escolhido esta inativo.");
                        Console.ReadKey();
                    }
                    else
                        control = true;
                } while (control != true);

                Console.Write("Digite a quantidade de matéria prima que será utilizada: ");
                quantidadeMateriaPrima = decimal.Parse(Console.ReadLine());

                if (quantidadeMateriaPrima < 0 || quantidadeMateriaPrima >= 1000)
                {
                    Console.WriteLine("Não é possivel adicionar essa quantidade de matéria prima");
                    continue;
                }

                DateTime data = DateTime.Parse(dataProducao);
                if (producao == null)
                    producao = bdProducao.GravarProducao(-1, produto, quantidade, data);

                ItemProducao item = new(producao.Id, dataProducao, materiaPrima.Id, quantidadeMateriaPrima);

                itens.Add(item);

                Console.WriteLine("Gostaria de adicionar mais uma materia prima?\n(1) Sim\n(2) Não");
                Console.Write("Resposta: "); opcao = int.Parse(Console.ReadLine());

            } while (opcao == 1);

            Console.WriteLine("Gostaria de finalizar o registro ou deseja excluí-lo agora mesmo?");
            Console.WriteLine("1. Finalizar\n2. Cancelar registro");

            Console.Write("Resposta: "); escolha = int.Parse(Console.ReadLine());

            if (escolha == 1)
            {
                bdProducao.GravarItemProducao(itens);

                Console.WriteLine("\n\tRegistro efetuado com sucesso!");
                Console.ReadKey();
                Console.WriteLine("\n\n\t Pressione ENTER para continuar...");
                Console.Clear();
            }
            else
            {
                bdProducao.RemoverProducao(int.Parse(producao.Id));

                Console.WriteLine("O registro foi cancelado com sucesso!");
                Console.ReadKey();
                Console.WriteLine("\n\n\t Pressione ENTER para continuar...");
                Console.Clear();
            }
        }

        public void Localizar()
        {
            string cod;

            BDProducao bdProducao = new();

            Console.Clear();
            Console.WriteLine("\n Localizar producao");
            Console.Write("\n Digite o codigo da producao: ");
            cod = Console.ReadLine();

            int codigo = int.Parse(cod);
            Producao producao = bdProducao.LocalizarProducao(codigo);

            List<ItemProducao> itensProducao = null;

            if (producao == null)
            {
                Console.WriteLine("\n A producao nao existe.");
                Console.WriteLine("\n Pressione ENTER para voltar ao menu");
                Console.ReadKey();
            }
            else
            {
                Console.WriteLine("\n---------------------------------------\n");
                Console.WriteLine($" Codigo:          {producao.Id:0000}");
                Console.WriteLine($" Data Producao:   {producao.DataProducao:dd/MM/yyyy}");
                Console.WriteLine($" Produto:         {producao.Produto}");
                Console.WriteLine($" Quantidade:      {producao.Quantidade}");
                Console.WriteLine("\n---------------------------------------\n");

                itensProducao = bdProducao.ImprimirItens(int.Parse(producao.Id));

                Console.WriteLine("\n Cod.       Materia-prima            Qt.   ");
                Console.WriteLine(" ----------------------------------------------\n");
                itensProducao.ForEach(item =>
                {
                    Console.WriteLine($" {item.Id:0000}          {item.MateriaPrima}           {item.QuantidadeMateriaPrima,8}");
                });

                Console.WriteLine("\n Pressione ENTER para voltar ao menu");
                Console.ReadKey();
            }
        }

        public static void ImprimirProducao()
        {
            BDProducao bdProducao = new();

            Console.Clear();

            List<Producao> Producao = bdProducao.ListarProducoes();

            int posicao = 0, max = Producao.Count;
            string escolha = "0", msgInicial, msgSaida;

            msgInicial = $"\n ...:: Lista de Producao ::...\n";
            msgSaida = " Caso queira voltar ao menu anterior, basta digitar 9 e pressionar ENTER\n";

            Console.Clear();
            Console.WriteLine(msgInicial);
            Console.WriteLine(msgSaida);
            Console.WriteLine(" -------------------------------------------------------------------------\n");
            Console.WriteLine($" 1º Registro\n");
            DesenharDados(Producao.First());

            do
            {
                Console.WriteLine("\n 1 - Primeiro / 2 - Anterior / 3 - Proximo / 4 - Ultimo\n");
                Console.Write(" Escolha: ");
                escolha = Console.ReadLine();

                if (escolha != "1" && escolha != "2" && escolha != "3" && escolha != "4" && escolha != "9")
                {
                    Console.WriteLine("\n xxxx Opcao invalida.");
                    Console.WriteLine("\n Pressione ENTER para voltar...\n");
                    Console.ReadKey();
                }
                else if (escolha == "9")
                    return;
                else
                {
                    if (escolha == "1")
                    {
                        if (posicao == 0)
                            Console.WriteLine("\n xxxx Ja estamos no primeiro registro.");
                        else
                        {
                            Console.Clear();
                            Console.WriteLine(msgInicial);
                            Console.WriteLine(msgSaida);
                            Console.WriteLine(" -------------------------------------------------------------------------\n");
                            Console.WriteLine($" 1º Registro\n");
                            DesenharDados(Producao.First());
                            posicao = 0;
                        }
                    }
                    else if (escolha == "2")
                    {
                        if (posicao == 0)
                            Console.WriteLine("\n xxxx Nao ha registro anterior.");
                        else
                        {
                            posicao--;
                            Console.Clear();
                            Console.WriteLine(msgInicial);
                            Console.WriteLine(msgSaida);
                            Console.WriteLine(" -------------------------------------------------------------------------\n");
                            Console.WriteLine($" {posicao + 1}º Registro\n");
                            DesenharDados(Producao[posicao]);
                        }
                    }
                    else if (escolha == "3")
                    {
                        if (posicao == Producao.Count - 1)
                            Console.WriteLine("\n xxxx Nao ha proximo registro.");
                        else
                        {
                            posicao++;
                            Console.Clear();
                            Console.WriteLine(msgInicial);
                            Console.WriteLine(msgSaida);
                            Console.WriteLine(" -------------------------------------------------------------------------\n");
                            Console.WriteLine($" {posicao + 1}º Registro\n");
                            DesenharDados(Producao[posicao]);
                        }
                    }
                    else if (escolha == "4")
                    {
                        if (posicao == Producao.Count - 1)
                            Console.WriteLine("\n xxxx Ja estamos no ultimo registro.");
                        else
                        {
                            Console.Clear();
                            Console.WriteLine(msgInicial);
                            Console.WriteLine(msgSaida);
                            Console.WriteLine(" -------------------------------------------------------------------------\n");
                            Console.WriteLine($" {Producao.Count}º Registro (ultimo registro)\n");
                            DesenharDados(Producao.Last());
                            posicao = Producao.Count - 1;
                        }
                    }
                }
            } while (escolha != "9");
        }

        public static void DesenharDados(Producao producao)
        {
            List<ItemProducao> itensProducao = null;
            BDProducao bdProducao = new();

            Console.WriteLine("\n---------------------------------------\n");
            Console.WriteLine($" Codigo:          {producao.Id:0000}");
            Console.WriteLine($" Data Producao:   {producao.DataProducao:dd/MM/yyyy}");
            Console.WriteLine($" Produto:         {producao.Produto}");
            Console.WriteLine($" Quantidade:      {producao.Quantidade}");
            Console.WriteLine("\n---------------------------------------\n");

            itensProducao = bdProducao.ImprimirItens(int.Parse(producao.Id));

            Console.WriteLine("\n Cod.       Materia-prima            Qt.   ");
            Console.WriteLine(" ----------------------------------------------\n");
            itensProducao.ForEach(item =>
            {
                Console.WriteLine($" {item.Id:0000}          {item.MateriaPrima}           {item.QuantidadeMateriaPrima,8}");
            });
            Console.WriteLine("\n\n                               --/-------/--\n");
        }

        public override string ToString()
        {
            return
                "\n************ Registro de Produção ************\n\n"
                + "ID: " + Id.ToString().PadLeft(5, '0')
                + "\nData de produção: " + DataProducao
                + "\nProduto: " + Produto
                + "\nQuantidade: " + Quantidade.ToString("000.#0").TrimStart('0');

        }
    }
}

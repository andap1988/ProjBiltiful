using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CadastrosBasicos;

namespace VendasProdutos
{
    public class MenuVendas
    {
        public static void SubMenu()
        {
            BDCadastro bdCadastro = new();
            BDVenda bdVenda = new();

            string opcao;

            do
            {
                Console.Clear();

                Console.WriteLine("=============== VENDAS ===============");
                Console.WriteLine("1. Nova Venda");
                Console.WriteLine("2. Consultar Venda");
                Console.WriteLine("3. Imprimir Registros de Venda");
                Console.WriteLine("--------------------------------------");
                Console.WriteLine("0. Voltar");
                Console.Write("\nEscolha: ");

                switch (opcao = Console.ReadLine())
                {
                    case "1":
                        if (bdCadastro.TemCliente())
                            NovaVenda();
                        else
                        {
                            Console.WriteLine(" Para realizar uma venda sera necessario cadastrar um cliente");
                            Console.WriteLine("\nPressione ENTER para voltar ao menu");
                            Console.ReadKey();
                        }
                        break;

                    case "2":
                        if (bdVenda.TemVenda())
                            LocalizarVenda();
                        else
                        {
                            Console.WriteLine(" Nao ha venda para localizar.");
                            Console.WriteLine("\nPressione ENTER para voltar ao menu");
                            Console.ReadKey();
                        }
                        break;
                    case "3":
                        if (bdVenda.TemVenda())
                            ImprimirVendas();
                        else
                        {
                            Console.WriteLine(" Nao ha venda para localizar.");
                            Console.WriteLine("\nPressione ENTER para voltar ao menu");
                            Console.ReadKey();
                        }
                        break;
                    case "0":
                        break;

                    default:
                        Console.Clear();
                        Console.WriteLine("Opção inválida");
                        Console.WriteLine("\nPressione ENTER para voltar ao menu");
                        break;
                }
            } while (opcao != "0");
        }

        public static void NovaVenda()
        {
            Console.Clear();
            BDCadastro bdCadastro = new();
            BDVenda bdVenda = new();
            Cliente cliente;
            Venda venda;
            decimal valorTotal = 0;

            Console.WriteLine("Informe o CPF do cliente:");
            string cpf = Console.ReadLine();
            cpf = cpf.Replace(".", "").Replace("-", "");

            if (cpf.Length < 11)
            {
                Console.Clear();
                Console.WriteLine("\n CPF invalido;");
                Console.WriteLine("\n Pressione ENTER para voltar ao menu");
                Console.ReadKey();
                return;
            }

            cliente = bdCadastro.LocalizarCliente(cpf);

            if (cliente == null)
            {
                Console.Clear();
                Console.WriteLine("\n Cliente não encontrado");
                Console.WriteLine("\n Pressione ENTER para voltar ao menu");
                Console.ReadKey();
                return;
            }
            else if (cliente.Condicao)
            {
                Console.Clear();
                Console.WriteLine("\n Falha ao iniciar a venda. Procure pelo gerente do local.");
                Console.WriteLine("\n Pressione ENTER para voltar ao menu");
                Console.ReadKey();
                return;
            }
            else
            {
                Console.Clear();

                venda = bdVenda.GravarVenda(cpf, DateTime.Now.Date);

                Console.Write($"Venda Nº {venda.Id.ToString().PadLeft(5, '0')}\tData: {venda.DataVenda:dd/MM/yyyy}");
                Console.WriteLine();

                List<ItemVenda> itensVenda = new List<ItemVenda>();

                int itens = 1;
                string escolha;

                do
                {
                    Produto produto;
                    int qtd = 0;
                    decimal totalItens = 0;

                    do
                    {
                        produto = new Produto();

                        Console.WriteLine("\nDigite o Código do Produto:");
                        string codProduto = Console.ReadLine();

                        produto = bdCadastro.LocalizarProduto(codProduto);

                        if (produto == null)
                        {
                            Console.WriteLine("\nProduto não encontrado ou código inválido.");
                            Console.ReadKey();
                            Console.Clear();
                            continue;
                        }
                        else if (produto.Situacao.Equals('I'))
                        {
                            Console.WriteLine("\nProduto inativo.");
                            Console.ReadKey();
                            Console.Clear();
                            continue;
                        }

                        Console.WriteLine("\nInforme a quantidade:");
                        qtd = int.Parse(Console.ReadLine());


                        if (qtd <= 0 || qtd > 999)
                        {
                            Console.WriteLine("Informe uma quantidade entre 1 e 999");
                            Console.ReadKey();
                            Console.Clear();
                            continue;
                        }

                        totalItens = qtd * produto.ValorVenda;

                        if (totalItens > (decimal)9999.99)
                        {
                            Console.WriteLine("Valor total dos item passou o limite permitido de $ 9.999,99");
                            Console.ReadKey();
                            Console.Clear();
                            continue;
                        }
                    } while ((qtd <= 0 || qtd > 999) || totalItens > (decimal)9999.99 || produto == null);

                    Console.Clear();

                    decimal valor = produto.ValorVenda;
                    itensVenda.Add(new ItemVenda(venda.Id, produto.CodigoBarras, qtd, valor));

                    Console.WriteLine("Id\tProduto\t\tQtd\tV.Unitário\tT.Item");
                    Console.WriteLine("------------------------------------------------------");

                    

                    itensVenda.ForEach(item =>
                    {
                        Console.WriteLine(item.ToString());
                        valorTotal += item.TotalItem;
                        venda.ValorTotal = valorTotal;
                    });

                    Console.WriteLine("------------------------------------------------------");
                    Console.WriteLine($"\t\t\t\t\t\t{venda.ValorTotal.ToString("#.00")}");


                    do
                    {
                        Console.WriteLine("\nAdicionar novo produto?");
                        Console.WriteLine("[ S ] Sim\t[ N ] Não");
                        escolha = Console.ReadLine().ToUpper();

                        Console.Clear();
                    } while (escolha != "S" && escolha != "N");


                    if (escolha == "S")
                        itens++;
                    else
                        break;

                    if (itens == 4)
                    {
                        Console.Clear();
                        Console.WriteLine("Seu carrinho está cheio!");
                        Console.ReadKey();
                        break;
                    }

                } while (itens != 4);


                do
                {
                    Console.Clear();
                    Console.WriteLine("----------------------------------------------------------");
                    Console.WriteLine("                           CLIENTE                        ");
                    Console.WriteLine("----------------------------------------------------------");
                    Console.WriteLine($"Nome:\t\t{cliente.Nome.TrimStart(' ')}");
                    Console.WriteLine($"CPF:\t\t{cliente.CPF.Insert(3, ".").Insert(7, ".").Insert(11, "-")}");
                    Console.WriteLine($"Data Nasc.:\t{cliente.DataNascimento.ToString("dd/MM/yyyy")}");
                    Console.WriteLine($"Ultima Compra:\t{cliente.UltimaVenda.ToString("dd/MM/yyyy")}");
                    Console.WriteLine("\n\n----------------------------------------------------------");
                    Console.WriteLine($"Venda Nº {venda.Id.ToString().PadLeft(5, '0')}\t\t\tData: {venda.DataVenda.ToString("dd/MM/yyyy")}");
                    Console.WriteLine("----------------------------------------------------------");
                    Console.WriteLine("\n\nId\tProduto\t\tQtd\tV.Unitário\tT.Item");
                    Console.WriteLine("----------------------------------------------------------");
                    itensVenda.ForEach(item => Console.WriteLine(item.ToString()));
                    Console.WriteLine("----------------------------------------------------------");
                    Console.WriteLine($"\t\t\t\t\t\t{venda.ValorTotal.ToString("#.00")}");

                    Console.WriteLine("\n\n");

                    Console.WriteLine("[ F ] Finalizar venda\t[ C ] Cancelar venda");
                    escolha = Console.ReadLine().ToUpper();

                } while (escolha != "F" && escolha != "C");

                if (escolha == "F")
                {
                    bdVenda.GravarItemVenda(itensVenda);
                    bdVenda.GravarVenda(cliente.CPF, venda.DataVenda, true, venda.Id, valorTotal);

                    itensVenda.ForEach(item =>
                        new Produto().Atualizar(item.Produto, venda.DataVenda.Date.ToString("yyyy/MM/dd").Replace("/", "-")));

                    bdCadastro.EditarCliente(cliente, true, venda.DataVenda.ToString("yyyy/MM/dd").Replace("/", "-"), cliente.CPF);

                    Console.WriteLine("\n\nVenda cadastrada com sucesso!\nPressione ENTER para voltar ao Menu Vendas...");
                    Console.ReadKey();
                }
                else if (escolha == "C")
                {
                    bdVenda.RemoverVenda(venda.Id);

                    Console.WriteLine("O registro foi cancelado com sucesso!");
                    Console.ReadKey();
                    Console.WriteLine("\n\n\t Pressione ENTER para continuar...");
                    Console.Clear();
                }
            }
        }

        public static void LocalizarVenda()
        {
            Console.Clear();

            BDVenda bdVenda = new();
            Venda venda = new Venda();
            ItemVenda itemVenda = new ItemVenda();
            List<ItemVenda> itensVenda = null;

            Console.WriteLine("Informe a venda que deseja buscar: ");
            int.TryParse(Console.ReadLine(), out int id);

            venda = bdVenda.LocalizarVenda(id);

            if (venda != null)
            {
                Console.WriteLine("\n---------------------------------------\n");
                Console.WriteLine($" Codigo:          {venda.Id:0000}");
                Console.WriteLine($" Data Venda:      {venda.DataVenda:dd/MM/yyyy}");
                Console.WriteLine($" CPF Cliente:     {venda.Cliente}");
                Console.WriteLine($" Valor Total:     {venda.ValorTotal}");
                Console.WriteLine("\n---------------------------------------\n");

                itensVenda = bdVenda.ImprimirItens(venda.Id);

                Console.WriteLine("\n Cod.             Produto             V. Unitario         Qt.         Total");
                Console.WriteLine(" ---------------------------------------------------------------------------\n");
                itensVenda.ForEach(item =>
                {
                    Console.WriteLine($" {item.Id:0000}          {item.Produto}         {item.ValorUnitario,8}       {item.Quantidade,8}       {item.TotalItem,8}");
                });

                Console.WriteLine("\n\nPressione ENTER para voltar ao menu...\n");
                Console.ReadLine();
            }
            else
            {
                Console.WriteLine(" Venda não registrada!\nPressione ENTER para voltar ao menu...");
                Console.ReadKey();
            }
        }

        public static void ImprimirVendas()
        {
            BDVenda bdVenda = new();

            Console.Clear();

            List<Venda> Vendas = bdVenda.ListarVendas();

            int posicao = 0, max = Vendas.Count;
            string escolha = "0", msgInicial, msgSaida;

            msgInicial = $"\n ...:: Lista de Vendas ::...\n";
            msgSaida = " Caso queira voltar ao menu anterior, basta digitar 9 e pressionar ENTER\n";

            Console.Clear();
            Console.WriteLine(msgInicial);
            Console.WriteLine(msgSaida);
            Console.WriteLine(" -------------------------------------------------------------------------\n");
            Console.WriteLine($" 1º Registro\n");
            DesenharDados(Vendas.First());

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
                            DesenharDados(Vendas.First());
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
                            DesenharDados(Vendas[posicao]);
                        }
                    }
                    else if (escolha == "3")
                    {
                        if (posicao == Vendas.Count - 1)
                            Console.WriteLine("\n xxxx Nao ha proximo registro.");
                        else
                        {
                            posicao++;
                            Console.Clear();
                            Console.WriteLine(msgInicial);
                            Console.WriteLine(msgSaida);
                            Console.WriteLine(" -------------------------------------------------------------------------\n");
                            Console.WriteLine($" {posicao + 1}º Registro\n");
                            DesenharDados(Vendas[posicao]);
                        }
                    }
                    else if (escolha == "4")
                    {
                        if (posicao == Vendas.Count - 1)
                            Console.WriteLine("\n xxxx Ja estamos no ultimo registro.");
                        else
                        {
                            Console.Clear();
                            Console.WriteLine(msgInicial);
                            Console.WriteLine(msgSaida);
                            Console.WriteLine(" -------------------------------------------------------------------------\n");
                            Console.WriteLine($" {Vendas.Count}º Registro (ultimo registro)\n");
                            DesenharDados(Vendas.Last());
                            posicao = Vendas.Count - 1;
                        }
                    }
                }
            } while (escolha != "9");
        }

        public static void DesenharDados(Venda venda)
        {
            List<ItemVenda> itensVenda = null;
            BDVenda bdVenda = new();

            Console.WriteLine("\n---------------------------------------\n");
            Console.WriteLine($" Codigo:          {venda.Id:0000}");
            Console.WriteLine($" Data Venda:      {venda.DataVenda:dd/MM/yyyy}");
            Console.WriteLine($" CPF Cliente:     {venda.Cliente}");
            Console.WriteLine($" Valor Total:     {venda.ValorTotal}");
            Console.WriteLine("\n---------------------------------------\n");

            itensVenda = bdVenda.ImprimirItens(venda.Id);

            Console.WriteLine("\n Cod.             Produto             V. Unitario         Qt.         Total");
            Console.WriteLine(" ---------------------------------------------------------------------------\n");
            itensVenda.ForEach(item =>
            {
                Console.WriteLine($" {item.Id:0000}          {item.Produto}         {item.ValorUnitario,8}       {item.Quantidade,8}       {item.TotalItem,8}");
            });
            Console.WriteLine("\n\n                               --/-------/--\n");
        }

    }
}
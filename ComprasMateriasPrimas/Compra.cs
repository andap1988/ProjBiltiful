using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CadastrosBasicos;
using CadastrosBasicos.ManipulaArquivos;

namespace ComprasMateriasPrimas
{
    public class Compra
    {
        public int Id { get; set; } // 5 00000
        public DateTime DataCompra { get; set; } //8 00/00/0000
        public string Fornecedor { get; set; } //12 00.000.000/0001-00
        public decimal ValorTotal { get; set; } // 7 00.000,00
        public List<ItemCompra> ListaDeItens { get; set; }

        public Compra()
        {
            DataCompra = DateTime.Now;
        }

        public Compra(int id, DateTime dCompra, string cnpjFornecedor, decimal vTotal)
        {
            Id = id;
            DataCompra = dCompra;
            Fornecedor = cnpjFornecedor;
            ValorTotal = vTotal;
        }

        public static void SubMenu()
        {
            BDCompra bdCompra = new();
            BDCadastro bdCadastro = new();

            int option = -1;
            while (option != 0)
            {
                Console.Clear();

                Console.WriteLine("=============== COMPRAS ===============");
                Console.WriteLine("1. Nova Compra");
                Console.WriteLine("2. Consultar Compra");
                Console.WriteLine("3. Imprimir Registros de Compra");
                Console.WriteLine("--------------------------------------");
                Console.WriteLine("0. Voltar");
                Console.Write("\nEscolha: ");

                option = int.Parse(Console.ReadLine());
                switch (option)
                {
                    case 1:
                        if (bdCadastro.TemFornecedor())
                            CadastraNovaCompra();
                        else
                        {
                            Console.WriteLine("Para realizar uma compra de materias-primas devera ter o registro de ao menos um fornecedor.");
                            Console.ReadKey();
                        }
                        break;
                    case 2:
                        Localizar();
                        break;
                    case 3:
                        ImprimirCompras();
                        break;
                }
                Console.Clear();
            }
        }

        private static void CadastraNovaCompra()
        {
            BDCompra bdCompra = new();
            BDCadastro bdCadastro = new();
            Fornecedor fornecedor = new();
            Compra compra = new();
            MPrima mprima = new();
            bool flag = true;
            string cnpjFornecedor;

            do
            {
                Console.Clear();
                Console.Write("CNPJ do Fornecedor: ");
                cnpjFornecedor = Console.ReadLine();
                cnpjFornecedor = cnpjFornecedor.Replace(".", "").Replace("/", "").Replace("-", "");

                if (cnpjFornecedor.Length < 14)
                {
                    Console.WriteLine(" CNPJ invalido");
                    Console.WriteLine(" Pressione ENTER para voltar...");
                    Console.ReadKey();
                }
                else
                {
                    fornecedor = bdCadastro.LocalizarFornecedor(cnpjFornecedor);

                    if (fornecedor == null)
                    {
                        Console.WriteLine(" CNPJ nao encontrado");
                        Console.WriteLine(" Pressione ENTER para voltar...");
                        Console.ReadKey();
                        return;
                    }
                    else if (fornecedor.Condicao)
                    {
                        Console.WriteLine(" CNPJ bloqueado para novas compras");
                        Console.WriteLine(" Pressione ENTER para voltar...");
                        Console.ReadKey();
                        return;
                    }
                    else
                        flag = false;
                }

            } while (flag);

            cnpjFornecedor = fornecedor.CNPJ;

            int count = 1;
            List<ItemCompra> itens = new();
            Console.WriteLine("\nItens da Compra\n");
            Console.Write("Quantidade de itens a comprar (limite de 3 itens por compra): ");
            int qtdd;
            do
            {
                Console.WriteLine("Você só pode comprar 3 itens por compra!");
                qtdd = int.Parse(Console.ReadLine());
            } while (qtdd > 3 || qtdd < 0);
            do
            {
                string idMP;
                Console.WriteLine($"Item {count}");
                do
                {
                    Console.Write("- Id da Matéria Prima (somente numeros): ");
                    idMP = Console.ReadLine();

                    int codigo = int.Parse(idMP);
                    mprima = bdCadastro.LocalizarMateriaPrima(codigo);
                    if (mprima.Situacao == 'I')
                    {
                        Console.WriteLine(" Materia-prima esta inativa. Escolha outra...");
                        Console.WriteLine(" Pressione ENTER para digitar novamente...");
                        Console.ReadKey();
                        mprima = null;
                    }

                } while (mprima == null);

                decimal valorUnitario;
                decimal quantidade;

                do
                {
                    Console.Write("- Valor unitário do item: ");
                    valorUnitario = decimal.Parse(Console.ReadLine());
                    Console.Write("- Quantidade do item que deseja comprar: ");
                    quantidade = decimal.Parse(Console.ReadLine());
                    if ((valorUnitario * quantidade) > 9999999) Console.WriteLine("O valor total do Item ultrapassou o limite de 99.999,99");
                } while ((valorUnitario * quantidade) > 9999999);

                if (count == 1)
                    compra = bdCompra.GravarCompra(fornecedor.CNPJ, DateTime.Now.Date);

                ItemCompra item = new(compra.Id,
                                        compra.DataCompra,
                                        idMP,
                                        quantidade,
                                        valorUnitario);
                itens.Add(item);
                count++;
            } while (count <= qtdd);

            decimal valorTotal = 0;
            compra.Fornecedor = cnpjFornecedor;
            itens.ForEach(item => valorTotal += item.TotalItem);

            bdCompra.GravarItemCompra(itens);
            bdCompra.GravarCompra(fornecedor.CNPJ, compra.DataCompra, true, compra.Id, valorTotal);
            itens.ForEach(item =>
                bdCadastro.EditarMateriaPrima(int.Parse(item.MateriaPrima), null, compra.DataCompra.ToString("yyyy/MM/dd").Replace("/", "-")));

            Console.WriteLine(" Compra concluida.");
            Console.WriteLine(" Pressione ENTER para voltar...");
            Console.ReadKey();
        }

        public static void Localizar()
        {
            string cod;

            BDCompra bdCompra = new();

            Console.Clear();
            Console.WriteLine("\n Localizar Compra");
            Console.Write("\n Digite o codigo da compra: ");
            cod = Console.ReadLine();

            int codigo = int.Parse(cod);
            Compra compra = bdCompra.LocalizarCompra(codigo);
            List<ItemCompra> itensCompra = null;

            if (compra == null)
            {
                Console.WriteLine("\n A compra nao existe.");
                Console.WriteLine("\n Pressione ENTER para voltar ao menu");
                Console.ReadKey();
            }
            else
            {
                Console.WriteLine("\n---------------------------------------\n");
                Console.WriteLine($" Codigo:          {compra.Id:0000}");
                Console.WriteLine($" Data Compra:     {compra.DataCompra:dd/MM/yyyy}");
                Console.WriteLine($" CNPJ Fornecedor: {compra.Fornecedor}");
                Console.WriteLine($" Valor Total:     {compra.ValorTotal}");
                Console.WriteLine("\n---------------------------------------\n");

                itensCompra = bdCompra.ImprimirItens(compra.Id);

                Console.WriteLine("\n Cod.       Materia-prima           V. Unitario         Qt.         Total");
                Console.WriteLine(" ---------------------------------------------------------------------------\n");
                itensCompra.ForEach(item =>
                {
                    Console.WriteLine($" {item.Id:0000}          {item.MateriaPrima}               {item.ValorUnitario,8}       {item.Quantidade,8}       {item.TotalItem,8}");
                });

                Console.WriteLine("\n Pressione ENTER para voltar ao menu");
                Console.ReadKey();
            }
        }

        public static void ImprimirCompras()
        {
            BDCompra bdCompra = new();

            Console.Clear();

            List<Compra> Compras = bdCompra.ListarCompras();

            int posicao = 0, max = Compras.Count;
            string escolha = "0", msgInicial, msgSaida;

            msgInicial = $"\n ...:: Lista de Compras ::...\n";
            msgSaida = " Caso queira voltar ao menu anterior, basta digitar 9 e pressionar ENTER\n";

            Console.Clear();
            Console.WriteLine(msgInicial);
            Console.WriteLine(msgSaida);
            Console.WriteLine(" -------------------------------------------------------------------------\n");
            Console.WriteLine($" 1º Registro\n");
            DesenharDados(Compras.First());

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
                            DesenharDados(Compras.First());
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
                            DesenharDados(Compras[posicao]);
                        }
                    }
                    else if (escolha == "3")
                    {
                        if (posicao == Compras.Count - 1)
                            Console.WriteLine("\n xxxx Nao ha proximo registro.");
                        else
                        {
                            posicao++;
                            Console.Clear();
                            Console.WriteLine(msgInicial);
                            Console.WriteLine(msgSaida);
                            Console.WriteLine(" -------------------------------------------------------------------------\n");
                            Console.WriteLine($" {posicao + 1}º Registro\n");
                            DesenharDados(Compras[posicao]);
                        }
                    }
                    else if (escolha == "4")
                    {
                        if (posicao == Compras.Count - 1)
                            Console.WriteLine("\n xxxx Ja estamos no ultimo registro.");
                        else
                        {
                            Console.Clear();
                            Console.WriteLine(msgInicial);
                            Console.WriteLine(msgSaida);
                            Console.WriteLine(" -------------------------------------------------------------------------\n");
                            Console.WriteLine($" {Compras.Count}º Registro (ultimo registro)\n");
                            DesenharDados(Compras.Last());
                            posicao = Compras.Count - 1;
                        }
                    }
                }
            } while (escolha != "9");
        }

        public static void DesenharDados(Compra compra)
        {
            List<ItemCompra> itensCompra = null;
            BDCompra bdCompra = new();

            Console.WriteLine("\n---------------------------------------\n");
            Console.WriteLine($" Codigo:          {compra.Id:0000}");
            Console.WriteLine($" Data Compra:     {compra.DataCompra:dd/MM/yyyy}");
            Console.WriteLine($" CNPJ Fornecedor: {compra.Fornecedor}");
            Console.WriteLine($" Valor Total:     {compra.ValorTotal}");
            Console.WriteLine("\n---------------------------------------\n");

            itensCompra = bdCompra.ImprimirItens(compra.Id);

            Console.WriteLine("\n Cod.       Materia-prima           V. Unitario         Qt.         Total");
            Console.WriteLine(" ---------------------------------------------------------------------------\n");
            itensCompra.ForEach(item =>
            {
                Console.WriteLine($" {item.Id:0000}          {item.MateriaPrima}               {item.ValorUnitario,8}       {item.Quantidade,8}       {item.TotalItem,8}");
            });
            Console.WriteLine("\n\n                               --/-------/--\n");
        }
    }
}
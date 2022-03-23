using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadastrosBasicos
{

    public class MenuCadastros
    {
        public static void SubMenu()
        {
            string escolha;

            do
            {
                Console.Clear();

                Console.WriteLine("=============== CADASTROS ===============");
                Console.WriteLine("1. Clientes / Fornecedores");
                Console.WriteLine("2. Produtos");
                Console.WriteLine("3. Matérias-Primas");
                Console.WriteLine("-----------------------------------------");
                Console.WriteLine("0. Voltar ao menu anterior");
                Console.Write("\nEscolha: ");

                switch(escolha = Console.ReadLine())
                {
                    case "0":
                        break;

                    case "1":
                        SubMenuClientesFornecedores();
                        break;

                    case "2":
                        new Produto().Menu();
                        break;

                    case "3":
                        new MPrima().Menu();
                        break;

                    default:
                        Console.Clear();
                        Console.WriteLine("Opção inválida");
                        Console.WriteLine("\nPressione ENTER para voltar ao menu");
                        break;
                }

            }while(escolha != "0");

        }

        public static void SubMenuClientesFornecedores()
        {
            string escolha;

            do
            {
                Console.Clear();

                Console.WriteLine("=============== CLIENTES / FORNECEDORES ===============");
                Console.WriteLine("1. Cadastar cliente");
                Console.WriteLine("2. Listar clientes");
                Console.WriteLine("3. Editar registro de cliente");
                Console.WriteLine("4. Bloquear cliente");
                Console.WriteLine("5. Desbloquear cliente");
                Console.WriteLine("6. Localizar cliente");
                Console.WriteLine("-------------------------------------------------------");
                Console.WriteLine("7. Cadastar fornecedor");
                Console.WriteLine("8. Listar fornecedores");
                Console.WriteLine("9. Editar registro de fornecedor");
                Console.WriteLine("10. Bloquear fornecedor");
                Console.WriteLine("11. Desbloquear fornecedor");
                Console.WriteLine("12. Localizar fornecedor");
                Console.WriteLine("-------------------------------------------------------");
                Console.WriteLine("0. Voltar ao menu anterior");
                Console.Write("\nEscolha: ");

                switch (escolha = Console.ReadLine())
                {
                    case "0":
                        break;
                    case "1":
                        NovoCliente();
                        break;
                    case "2":
                        new Cliente().Navegar();
                        break;
                    case "3":
                        new Cliente().Editar();
                        break;
                    case "4":
                        new Cliente().BloquearCadastro();                        
                        break;
                    case "5":
                        new Cliente().DesbloquearCadastro();
                        break;

                    case "6":
                        new Cliente().Localizar();
                        break;
                    case "7":
                        NovoFornecedor();
                        break;
                    case "8":
                        new Fornecedor().Navegar();
                        break;
                    case "9":
                        new Fornecedor().Editar();
                        break;
                    case "10":
                        new Fornecedor().BloquearFornecedor();
                        break;
                    case "11":
                        new Fornecedor().DesbloquearFornecedor();
                        break;
                    case "12":
                        new Fornecedor().Localizar();
                        break;
                    default:
                        Console.Clear();
                        Console.WriteLine("Opção inválida");
                        Console.WriteLine("\n Pressione ENTER para voltar ao menu");
                        break;
                }

            } while (escolha != "0");
        }

        public static void NovoCliente()
        {
            Console.Clear();

            bool flag;

            DateTime dNascimento;

            do
            {
                Console.Write("Data de nascimento: ");
                flag = DateTime.TryParse(Console.ReadLine(), out dNascimento);
            } while (flag != true);
            if (Validacoes.CalculaData(dNascimento))
            {
                RegistrarCliente(dNascimento);
            }
            else
            {
                Console.WriteLine("Menor de 18 anos nao pode ser cadastrado");
                Console.ReadKey();
            }
                
        }

        public static void NovoFornecedor()
        {
            Console.Clear();

            bool flag;

            DateTime dCriacao;

            do
            {
                Console.Write("Data de criacao da empresa:");
                flag = DateTime.TryParse(Console.ReadLine(), out dCriacao);
            } while (flag != true);

            if (Validacoes.CalculaCriacao(dCriacao))
            {
                RegistrarFornecedor(dCriacao);
            }
            else
            {
                Console.WriteLine("Empresa com menos de 6 meses nao deve ser cadastrada");
                Console.WriteLine("Pressione enter para continuar");
                Console.ReadKey();
            }
        }

        public static void RegistrarFornecedor(DateTime dFundacao)
        {
            string rSocial = "", cnpj = "";
            char situacao;

            BDCadastro bd = new();

            do
            {
                Console.Write("CNPJ: ");
                cnpj = Console.ReadLine();
                cnpj = cnpj.Trim();
                cnpj = cnpj.Replace(".", "").Replace("-", "").Replace("/", "");
            } while (Validacoes.ValidarCnpj(cnpj) == false);

            Fornecedor f = bd.LocalizarFornecedor(cnpj);

            if (f == null)
            {
                Console.Write("Razao social: ");
                rSocial = Console.ReadLine().Trim().PadLeft(50, ' ');
                Console.Write("Situacao (A - Ativo/ I - Inativo): ");
                situacao = char.Parse(Console.ReadLine());

                bd.GravarFornecedor(new Fornecedor(cnpj, rSocial, dFundacao, situacao));

                Console.WriteLine("Fornecedor cadastrado com sucesso!!");
                Console.ReadKey();
            }
            else
            {
                Console.WriteLine("Fornecedor ja cadastrado");
                Console.WriteLine("Pressione enter para continuar");
                Console.ReadKey();
            }      
        }

        public static void RegistrarCliente(DateTime dNascimento)
        {
            string cpf = "", nome = "";
            char situacao, sexo;

            BDCadastro bd = new();

            do
            {
                Console.Write("CPF: ");
                cpf = Console.ReadLine();
                cpf = cpf.Trim();
                cpf = cpf.Replace(".", "").Replace("-", "");

            } while (Validacoes.ValidarCpf(cpf) == false);

            Cliente c = bd.LocalizarCliente(cpf);

            if (c == null)
            {
                Console.Write("Nome: ");
                nome = Console.ReadLine().Trim().PadLeft(50, ' ');
                Console.Write("Genero (M - Masculino/ F - Feminino): ");
                sexo = char.Parse(Console.ReadLine());
                Console.Write("Situacao (A - Ativo/ I - Inativo): ");
                situacao = char.Parse(Console.ReadLine());

                bd.GravarCliente(new Cliente(cpf, nome, dNascimento, sexo, situacao));

                Console.WriteLine("Cliente cadastrado com sucesso!!");
                Console.ReadKey();
            }
            else
            {
                Console.WriteLine("Cliente ja cadastrado!!");
                Console.ReadKey();
            }
        }
    }
}
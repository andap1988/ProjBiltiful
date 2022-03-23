using System;
using System.Collections.Generic;
using System.IO;
using CadastrosBasicos;
using CadastrosBasicos.ManipulaArquivos;

namespace VendasProdutos
{
    public class Venda
    {
        public static Arquivos caminho = new Arquivos();

        public int Id { get; set; }
        public string Cliente { get; set; }
        public DateTime DataVenda { get; set; }
        public decimal ValorTotal { get; set; }

        public Venda()
        {
            //Id = NovoIdVenda();
            ValorTotal = 0;
        }

        public Venda(int id, string cliente, DateTime dataVenda, decimal vTotal)
        {
            Id = id;
            Cliente = cliente;
            DataVenda = dataVenda;
            ValorTotal = vTotal;
        }

        public override string ToString()
        {
            return $"Venda Nº {Id.ToString().PadLeft(5, '0')}\tData: {DataVenda.ToString("dd/MM/yyyy")}\nCliente: {Cliente}\nTotal da Venda: {ValorTotal.ToString("00000.00").TrimStart('0')}";
        }
    }
}

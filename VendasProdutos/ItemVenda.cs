using System;
using System.Collections.Generic;
using System.IO;

namespace VendasProdutos
{
    public class ItemVenda
    {
        private static Arquivos caminho = new Arquivos();

        public int Id { get; set; }
        public string Produto { get; set; }
        public int Quantidade { get; set; }
        public decimal ValorUnitario { get; set; }
        public decimal TotalItem { get; set; }

        public ItemVenda() { }

        public ItemVenda(int id, string produto, int quantidade, decimal valorUnitario)
        {
            Id = id;
            Produto = produto;
            Quantidade = quantidade;
            ValorUnitario = valorUnitario;
            TotalItem = quantidade * valorUnitario;
        }

        public override string ToString()
        {
            return $"{Id.ToString().PadLeft(5, '0')}\t{Produto}\t{Quantidade.ToString().PadLeft(3, '0')}\t{ValorUnitario.ToString("000.00").TrimStart('0')}\t\t{TotalItem.ToString("0000.00").TrimStart('0')}";
        }    
    }
}

using System;
using System.Collections.Generic;
using System.IO;

namespace ComprasMateriasPrimas
{
    public class ItemCompra
    {
        public ItemCompra(int id, DateTime dataCompra ,string materiaPrima, decimal quantidade, decimal valorUnitario)
        {
            Id = id;
            DataCompra = dataCompra;
            MateriaPrima = materiaPrima;
            Quantidade = quantidade;
            ValorUnitario = valorUnitario;
            TotalItem = quantidade * valorUnitario;
        }

        public ItemCompra() { }

        public int Id { get; set; } //5 campos
        public DateTime DataCompra { get; set; } //8 campos
        public string MateriaPrima { get; set; } //6 campos
        public decimal Quantidade { get; set; } //5 campos
        public decimal ValorUnitario { get; set; } //5 campos
        public decimal TotalItem { get; set; } //6 campos

        public override string ToString() =>$"{Id.ToString().PadLeft(5, '0')}" +
                                            $"{DataCompra.ToString("dd/MM/yyyy").Replace("/", "")}" +
                                            $"{MateriaPrima}" +
                                            $"{Quantidade.ToString().Replace(".", "").PadLeft(5, '0')}" +
                                            $"{ValorUnitario.ToString().Replace(".", "").PadLeft(5, '0')}" +
                                            $"{TotalItem.ToString().Replace(".", "").PadLeft(6, '0')}";

        public bool TotalMaximo() => TotalItem > 9999999;
    }
}
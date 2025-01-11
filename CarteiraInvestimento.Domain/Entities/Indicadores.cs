
namespace CarteiraInvestimento.Domain.Entities
{
    public class Indicadores
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public int Total { get; set; }
        public decimal ValorInvestido { get; set; }
        public DateTime DataAtualizacao { get; set; }
        public decimal DividendoTotal { get; set; }
        public decimal DY { get; set; }
        public decimal ValorUnitarioHoje { get; set; }
        public decimal ValorHoje { get; set; }
        public decimal DYHoje { get; set; }
        public decimal Lucro { get; set; }
        public decimal PorcentagemLucro { get; set; }
        public decimal PrecoValorPatrimonial { get; set; }
        public bool HouveLucro { get; set; }
        public decimal LucroMaisDividendos { get; set; }
        public bool HouveLucroComDividendos { get; set; }
        public decimal ValorMedioInvestidoUnitario { get; set; }
        public decimal Peso { get; set; }
        public decimal PesoPorSetor { get; set; }
        public string? Pais { get; set; }
        public string? Moeda { get; set; }
        public string? Setor { get; set; }
        public string? QuoteType { get; set; }
        public string? Industria { get; set; }
    }
}

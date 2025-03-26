
namespace AtividadeBimestral.DTO
{
    public class PagamentoDRO
    {
        public int transacaoId { get; set; }
        public int situacao {  get; set; }
        public decimal Valor { get; set; }
        public string Cartao { get; set; }
        public string CVV { get; set; }
        public int Parcelas { get; set; }
    }
}

namespace AtividadeBimestral.DTO
{
    public class PagamentoDTO
    {
        public decimal Valor { get; set; }
        public string Cartao { get; set; }
        public string CVV { get; set; }
        public int Parcelas { get; set; }
    }
}

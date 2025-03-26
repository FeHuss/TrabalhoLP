namespace AtividadeBimestral.Models
{
    public class Transacao
    {
        public enum TpSituacao
        {
            PENDENTE=1,
            CONFIRMADO = 2,
            CANCELADO = 3
        }


        public int   TransacaoId  { get; set; }
        public decimal   Valor { get; set; }
        public string Cartao  { get; set; }
        public string CVV { get; set; }
        public int   Parcelas  { get; set; }
        public TpSituacao Situacao  { get; set; }

    }
}

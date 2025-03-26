
using AtividadeBimestral.Repository;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AtividadeBimestral.Service
{
    public class CartaoService
    {
        private readonly CartaoRepositorio _cartaoRepositorio;
        private readonly AppSettings _appSettings;

        public CartaoService(CartaoRepositorio cartaoRepositorio, AppSettings appSettings)
        {
            _cartaoRepositorio = cartaoRepositorio;
            _appSettings = appSettings;
        }

        public string ObterBandeira(string cartao)
        {
            // Verifica se o número do cartão é válido
            
            if (string.IsNullOrEmpty(cartao) || cartao.Length > 16)
            {
                return null; // Número de cartão inválido
            }

            
            string primeiroQuatro = cartao.Substring(0, 4);
            char oitavoDigito = cartao[8];

            // Aplica a regra de negócio fictícia para identificar a bandeira
            if (primeiroQuatro == "1111" && oitavoDigito == '1')
            {
                return "VISA";
            }
            else if (primeiroQuatro == "2222" && oitavoDigito == '2')
            {
                return "MASTERCARD";
            }
            else if (primeiroQuatro == "3333" && oitavoDigito == '3')
            {
                return "ELO";
            }

            return null; // Bandeira desconhecida
        }

        public bool Validar(string numeroCartao)
        {
            var cartao = _cartaoRepositorio.ObterPorId(numeroCartao);

            if (cartao == null || cartao.Validade < DateTime.Now)
            {
                return false;
            }
            return true;
        }

    }
}

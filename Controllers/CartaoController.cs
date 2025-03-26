using AtividadeBimestral.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace AtividadeBimestral.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartaoController : ControllerBase
    {
        private readonly ILogger<CartaoController> _logger;

        private readonly CartaoService _cartaoService;
        public CartaoController(IHttpContextAccessor httpContextAccessor, ILogger<CartaoController> logger, CartaoService cartaoService)
        {
            _logger = logger;
            _cartaoService = cartaoService;
        }

        /// <summary>
        /// Obtem os numeros do cartao de credito e retorna sua bandeira se conhecida.
        /// </summary>
        /// <param name="cartao">Número do cartão de crédito (16 dígitos)</param>
        /// <returns>Retorna a bandeira caso conhecida</returns>
        /// <response code="200">Retorna a bandeira do cartão</response>
        /// <response code="404">Bandeira Não cadastrada ou número do cartão inválido</response>
        [HttpGet("{cartao}/obter-bandeira")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public IActionResult ObterBandeira(string cartao)
        {
            _logger.LogInformation("Recebida o cartao tentando obter sua bandeira: {Cartao}", cartao);

            var bandeira = _cartaoService.ObterBandeira(cartao);
            _logger.LogInformation("{cartao.Length()}");
            if (bandeira == null)
            {
                _logger.LogWarning("Bandeira não cadastrada ou número do cartão inválido: {Cartao}", cartao);
                return NotFound("Bandeira não cadastrada ou número do cartão inválido.");
            }

            _logger.LogInformation("Bandeira identificada: {Bandeira} para o cartão {Cartao}", bandeira, cartao);
            return Ok(new { bandeira });
        }

        /// <summary>
        /// Valida Um cartão com base no banco de dados
        /// </summary>
        /// <param name="cartao">Número do cartão de crédito (16 dígitos)</param>
        /// <returns>Retorna Veradeiro ou falso ou um erro 404 se desconhecida.</returns>
        [HttpGet("{cartao}/valido")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public IActionResult CartaoValido(string cartao)
        {
            _logger.LogInformation("Recebido o cartao, sera validado: {Cartao}", cartao);

            var valido = _cartaoService.Validar(cartao);

            if (!valido)
            {
                _logger.LogWarning("Cartão inválido: {Cartao}", cartao);
                return NotFound("Cartão inválido.");
            }

            _logger.LogInformation("Cartão válido: {Cartao}", cartao);
            return Ok(new { valido });
        }
    }
}

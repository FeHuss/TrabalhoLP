using AtividadeBimestral.DTO;
using AtividadeBimestral.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AtividadeBimestral.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PagamentosController : ControllerBase
    {
        private readonly ILogger<PagamentosController> _logger;
        private readonly TransacaoService _transacaoService;
        private readonly CartaoService _cartaoService;
        public PagamentosController(ILogger<PagamentosController> logger, TransacaoService transacaoService, CartaoService cartaoService)
        {
            _transacaoService = transacaoService;
            _cartaoService = cartaoService;
            _logger = logger;
        }

        /// <summary>
        /// Calcula e grava quantas parcelas e o quanto tem que pagar com juros
        /// </summary>
        /// <param name="parcelas">iforme dos valores e quantidade de parcelas</param>
        /// <returns>Lista das Parcelas calculadas</returns>
        [HttpPost("calcular-parcelas")]
        public ActionResult<ParcelasDRO> CalcularParcelas([FromBody] ParcelasDTO parcelas)
        {
            try
            {
                var parcelasCalculadas = _transacaoService.CalcularParcelas(parcelas);

                return Ok(parcelasCalculadas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao calcular parcelas.");
                return BadRequest($"Erro ao calcular parcelas: {ex.Message}");
            }
        }
        /// <summary>
        /// Inicia o Processo de pagamento de uma parcela
        /// </summary>
        /// <param name="criarPagamentoDTO">Detalhes do pagamento</param>
        /// <returns>retorna se o pagamento foi aprovado</returns>
        [HttpPost("pagamentos")]
        public ActionResult<PagamentoDRO> EfetuarPagamento([FromBody] PagamentoDTO pagamentoDTO)
        {
            try
            {
                if (!_cartaoService.Validar(pagamentoDTO.Cartao))
                {
                    return BadRequest("Cartão inválido.");
                }

                var pagamentoDRO = _transacaoService.EfetuarPagamento(pagamentoDTO);

                return Ok(pagamentoDRO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao efetuar pagamento.");
                return BadRequest($"Erro ao efetuar pagamento: {ex.Message}");
            }
        }
        /// <summary>
        /// Consulta como esta a situação de uma parcela
        /// </summary>
        /// <param name="id">numero da parcela</param>
        /// <returns>retorna a situação da parcela</returns>
        [HttpGet("{id}/situacao")]
        public ActionResult<int> ConsultarSituacaoPagamento( int id)
        {
            try
            {
                var situacao = _transacaoService.getStatusTransacao(id);

                return Ok(situacao);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao consultar situação do pagamento.");
                return BadRequest($"Erro ao consultar situação do pagamento: {ex.Message}");
            }
        }
        /// <summary>
        ///  Corfirma o pagamento de determinada parcela
        /// </summary>
        /// <param name="id">numero da parcela</param>
        /// <returns>Retorna se foi processada o pagamento</returns>
        [HttpPut("{id}/confirmar")]
        public ActionResult ConfirmarPagamento( int id)
        {
            try
            {
                return Ok(_transacaoService.ConfirmarPagamento(id));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao confirmar pagamento.");
                return BadRequest($"Erro ao confirmar pagamento: {ex.Message}");
            }
        }
        /// <summary>
        /// Cancela uma determinada parcela
        /// </summary>
        /// <param name="id">numero da parcela a ser cancelada</param>
        /// <returns>Retorna se parcela foi cancelada</returns>
        [HttpPut("{id}/cancelar")]
        public ActionResult CancelarPagamento(int id)
        {
            try
            {
                return Ok(_transacaoService.CancelarPagamento(id));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao cancelar pagamento.");
                return BadRequest($"Erro ao cancelar pagamento: {ex.Message}");
            }
        }

    }
}

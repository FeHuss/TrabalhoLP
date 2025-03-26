using AtividadeBimestral.DTO;
using AtividadeBimestral.Models;
using AtividadeBimestral.Repository;

namespace AtividadeBimestral.Service
{
    public class TransacaoService
    {
        private readonly TransacaoRepositorio _transacaoRepository;
        private readonly AppSettings _appSettings;

        public TransacaoService(TransacaoRepositorio transacaoRepository, AppSettings appSettings)
        {
            _transacaoRepository = transacaoRepository;
            _appSettings = appSettings;
        }

        public List<ParcelasDRO> CalcularParcelas(ParcelasDTO pagamentoRequest)
        {
            try
            {
                var parcelas = new List<ParcelasDRO>();

                var valorParcela = pagamentoRequest.ValorTotal * pagamentoRequest.TaxaJuros / pagamentoRequest.QuantidadeParcelas;

                for (int i = 0; i < pagamentoRequest.QuantidadeParcelas; i++)
                {
                    parcelas.Add(new ParcelasDRO
                    {
                        Parcela = i + 1,
                        Valor = valorParcela,
                    });
                }

                return parcelas;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public PagamentoDRO EfetuarPagamento(PagamentoDTO criarPagamentoDTO)
        {
            try
            {
                if (criarPagamentoDTO.Valor <= 0)
                {
                    throw new Exception("Valor total deve ser maior que zero");
                }

                Transacao novaTransacao = new Transacao
                {
                    Valor = criarPagamentoDTO.Valor,
                    Parcelas = criarPagamentoDTO.Parcelas,
                    Situacao = (Transacao.TpSituacao) 1,
                    CVV = criarPagamentoDTO.CVV,
                    Cartao = criarPagamentoDTO.Cartao,
                };

                if (_transacaoRepository.Adicionar(novaTransacao))
                {
                    return new PagamentoDRO
                    {
                        transacaoId = (int)novaTransacao.TransacaoId,
                        Valor = (decimal)novaTransacao.Valor,
                        Parcelas = (int)novaTransacao.Parcelas,
                        situacao = (int)novaTransacao.Situacao,
                        CVV = novaTransacao.CVV,
                        Cartao = novaTransacao.Cartao
                    };
                }
                else
                {
                    throw new Exception("Erro ao criar transação");
                }
            }
            catch (Exception ex)
            {
                // Lançando a exceção novamente com a mensagem detalhada
                throw new Exception($"Erro ao efetuar pagamento: {ex.Message}");
            }
        }

        public int getStatusTransacao(int id)
        {
            try
            {
                var transacao = _transacaoRepository.ObterPorId(id);

                if (transacao == null)
                {
                    throw new Exception("Transação não encontrada");
                }

                return (int) transacao.Situacao;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Transacao ConfirmarPagamento(int id)
        {
            try
            {
                var transacao = _transacaoRepository.ObterPorId(id);

                if (transacao == null)
                {
                    throw new Exception("Transação não encontrada");
                }
                if (transacao.Situacao == (Transacao.TpSituacao)2)
                {
                    throw new Exception("Transação já confirmada");
                }
                if (transacao.Situacao == (Transacao.TpSituacao)3)
                {
                    throw new Exception("Transação cancelada");
                }
                transacao.Situacao = (Transacao.TpSituacao)1;

                _transacaoRepository.Atualizar(transacao);

                return transacao;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Transacao CancelarPagamento(int id)
        {
            try
            {
                var transacao = _transacaoRepository.ObterPorId(id);

                if (transacao == null)
                {
                    throw new Exception("Transação não encontrada");
                }
                if (transacao.Situacao == (Transacao.TpSituacao)3)
                {
                    throw new Exception("Transação já cancelada");
                }
                if (transacao.Situacao == (Transacao.TpSituacao)2)
                {
                    throw new Exception("Transação já confirmada");
                }

                transacao.Situacao = (Transacao.TpSituacao)3;

                _transacaoRepository.Atualizar(transacao);

                return transacao;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}

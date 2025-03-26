using AtividadeBimestral.Models;
using MySql.Data.MySqlClient;
using Serilog;

namespace AtividadeBimestral.Repository
{
    public class TransacaoRepositorio:IRepository<Transacao>
    {
        private readonly MySqlDbContext _dbContext;
        private readonly ILogger<TransacaoRepositorio> _logger;

        public TransacaoRepositorio(MySqlDbContext dbContext, ILogger<TransacaoRepositorio> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public bool Adicionar(Transacao entity)
        {
            bool sucesso = false;

            try
            {
                using (MySqlCommand cmd = _dbContext.GetConnection().CreateCommand())
                {
                    

                    cmd.CommandText=@$" insert into Transacao ( Valor, Cartao, CVV, Parcelas, Situacao) VALUES ( @Valor, @Cartao, @CVV, @Parcelas, @Situacao);";
                        
                          
                            cmd.Parameters.AddWithValue("@Valor", entity.Valor);
                            cmd.Parameters.AddWithValue("@Cartao", entity.Cartao);
                            cmd.Parameters.AddWithValue("@CVV", entity.CVV);
                            cmd.Parameters.AddWithValue("@Parcelas", entity.Parcelas);
                            cmd.Parameters.AddWithValue("@Situacao", entity.Situacao);
                            int linhaAfetadas = cmd.ExecuteNonQuery();
                            entity.TransacaoId = (int)cmd.LastInsertedId;// Obtém o ID inserido
                            _logger.LogInformation("Transação {TransacaoId} adicionada com sucesso!", entity.TransacaoId);

                            sucesso = true;
                }
            }
            catch (MySqlException ex)
            {
                _logger.LogError(ex, "Erro ao adicionar transação {TransacaoId}", entity.TransacaoId);
                throw;
            }

            return sucesso;
        }

        public bool Atualizar(Transacao entity)
        {
            bool sucesso = false;
            try
            {
                using (MySqlCommand cmd = _dbContext.GetConnection().CreateCommand())
                {
                    cmd.CommandText = @$"update Transacao
                                    set Valor = @Valor,
                                        Cartao = @Cartao,
                                        CVV = @CVV,
                                        Parcelas = @Parcelas,
                                        Situacao = @Situacao
                                    where TransacaoId = @Id";
                    cmd.Parameters.AddWithValue("@Valor", entity.Valor);
                    cmd.Parameters.AddWithValue("@Cartao", entity.Cartao);
                    cmd.Parameters.AddWithValue("@CVV", entity.CVV);
                    cmd.Parameters.AddWithValue("@Parcelas", entity.Parcelas);
                    cmd.Parameters.AddWithValue("@Situacao", entity.Situacao);
                    cmd.Parameters.AddWithValue("@Id", entity.TransacaoId);

                    int linhaAfetadas = cmd.ExecuteNonQuery();
                    sucesso = true;
                }
            }
            catch (MySqlException ex)
            {
                _logger.LogError(ex, "Erro ao atualizar transação {TransacaoId}", entity.TransacaoId);
                throw;
            }

            return sucesso;
        }

        public Transacao ObterPorId(int id)
        {
            Transacao transacao = new Transacao();
            try
            {
                try
                {
                    using (MySqlCommand cmd = _dbContext.GetConnection().CreateCommand())
                    {
                      cmd.CommandText = "select * from Transacao where TransacaoId = @Id";
                      cmd.Parameters.AddWithValue("@Id", id);
                      using(MySqlDataReader dr = cmd.ExecuteReader())
                      {
                         if (dr.Read())
                         {
                            transacao.TransacaoId = Convert.ToInt32(dr["TransacaoId"]);
                            transacao.Valor = Convert.ToDecimal(dr["Valor"]);
                            transacao.Cartao = dr["Cartao"].ToString();
                            transacao.CVV = dr["CVV"].ToString();
                            transacao.Parcelas = Convert.ToInt32(dr["Parcelas"]);
                            transacao.Situacao = (Transacao.TpSituacao)(Convert.ToInt32(dr["Situacao"]));
                          }
                       }
                  }
                }catch(MySqlException ex)
                {
                    _logger.LogError(ex, "Erro ao tentar obter transacao {id}", id);
                    throw;
                }
            }
            catch (MySqlException ex)
            {
                _logger.LogError(ex, "Erro ao tentar obter transacao {id}",id);
                throw;
            }

            return transacao;
        }
    }
}

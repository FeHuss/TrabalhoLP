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

                   cmd.CommandText=@$" insert into Transacao (TransacaoId, Valor, Cartao, CVV, Parcelas, Situacao) VALUES (@TransacaoId, @Valor, @Cartao, @CVV, @Parcelas, @Situacao);SELECT LAST_INSERT_ID();";
                        
                            cmd.Parameters.AddWithValue("@TransacaoId", entity.TransacaoId);
                            cmd.Parameters.AddWithValue("@Valor", entity.Valor);
                            cmd.Parameters.AddWithValue("@Cartao", entity.Cartao);
                            cmd.Parameters.AddWithValue("@CVV", entity.CVV);
                            cmd.Parameters.AddWithValue("@Parcelas", entity.Parcelas);
                            cmd.Parameters.AddWithValue("@Situacao", entity.Situacao);
                            entity.TransacaoId = Convert.ToInt32(cmd.ExecuteScalar()); // Obtém o ID inserido
                            Log.Information("Transação {TransacaoId} adicionada com sucesso!", entity.TransacaoId);

                            sucesso = true;
                }
            }
            catch (MySqlException ex)
            {
                Log.Error(ex, "Erro ao adicionar transação {TransacaoId}", entity.TransacaoId);
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
                                    where Id = @Id";

                    int linhaAfetadas = cmd.ExecuteNonQuery();
                    sucesso = true;
                }
            }
            catch (MySqlException ex)
            {
                throw;
            }

            return sucesso;
        }

        public Transacao ObterPorId(int id)
        {
            Transacao transacao = new Transacao();
            try
            {
                using (MySqlCommand cmd = _dbContext.GetConnection().CreateCommand())
                {
                    cmd.CommandText = "select * from Transacao where Id = @Id";
                    cmd.Parameters.AddWithValue("@Id", id);
                    MySqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        transacao.TransacaoId = Convert.ToInt32(dr["TransacaoId"]);
                        transacao.Valor = Convert.ToDecimal(dr["Valor"]);
                        transacao.Cartao = dr["Cartao"].ToString();
                        transacao.CVV = dr["CVV"].ToString();
                        transacao.Parcelas = Convert.ToInt32(dr["Parcelas"]);
                    }
                 
                }
            }
            catch (MySqlException ex)
            {
                throw;
            }

            return transacao;
        }
    }
}

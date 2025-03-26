using AtividadeBimestral.Models;
using MySql.Data.MySqlClient;
using Serilog;

namespace AtividadeBimestral.Repository
{
    public class CartaoRepositorio
    {
        private readonly MySqlDbContext _dbContext;
        private readonly ILogger<CartaoRepositorio> _logger;

        public CartaoRepositorio(MySqlDbContext dbContext, ILogger<CartaoRepositorio> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public Cartao ObterPorId(string Numero)
        {
            Cartao cartao = null;

            try
            {
                using (MySqlCommand cmd = _dbContext.GetConnection().CreateCommand())
                {
                    cmd.CommandText = "select * from Cartao where Numero = @Numero";
                    cmd.Parameters.AddWithValue("@Numero", Numero);
                    using (MySqlDataReader dr = cmd.ExecuteReader()) // Usando 'using' para fechar corretamente
                    {
                        if (dr.Read())
                        {
                            cartao = new Cartao
                            {
                                numero = dr["Numero"].ToString(),
                                Validade = Convert.ToDateTime(dr["Validade"])
                            };
                        }
                    } // O 'using' fecha automaticamente o DataReader aqui
                }
            }
            catch (MySqlException ex)
            {
                throw;
            }

            return cartao;
        }
    }
}

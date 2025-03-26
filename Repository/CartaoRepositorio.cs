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
                    MySqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        cartao = new Cartao();
                        cartao.numero = dr["Numero"].ToString();
                        cartao.Validade = Convert.ToDateTime(dr["Validade"]);
                    }
                }
            }
            catch (MySqlException ex)
            {
                throw;
            }

            return cartao;
        }

        public IEnumerable<Cartao> ObterTodos()
        {
            List<Cartao> cartoes = new List<Cartao>();

            try
            {
                using (MySqlCommand cmd = _dbContext.GetConnection().CreateCommand())
                {
                    cmd.CommandText = "select * from Cartao";
                    MySqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        Cartao cartao = new Cartao();
                        cartao.numero = dr["Numero"].ToString();
                        cartao.Validade = Convert.ToDateTime(dr["Validade"]);
                        cartoes.Add(cartao);
                    }
                }
            }
            catch (MySqlException ex)
            {
                throw;
            }

            return cartoes;
        }
    }
}

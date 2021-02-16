using DapperMapper.Repositories;

namespace DapperMapper.Console
{
    public class ProdutosRepository : DapperRepository<Produtos>
    {
        public ProdutosRepository(string connectionString) : base(connectionString)
        {
        }
    }
}

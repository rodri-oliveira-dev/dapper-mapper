using DapperMapper.Repositories;

namespace DapperMapper.Console
{
    public class ContratoRepository : DapperRepository<Contrato>
    {
        public ContratoRepository(string connectionString) : base(connectionString)
        {
        }
    }
}

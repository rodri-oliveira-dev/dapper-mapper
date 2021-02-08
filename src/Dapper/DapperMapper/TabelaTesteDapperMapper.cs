using DapperMapper.Repositories;

namespace DapperMapper.Console
{
    public class TabelaTesteDapperMapper : DapperRepository<TabelaTeste>
    {
        public TabelaTesteDapperMapper(string connectionString) : base(connectionString)
        {
        }
    }
}

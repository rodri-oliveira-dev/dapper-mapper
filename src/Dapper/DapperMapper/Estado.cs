using Dapper.Contrib.Extensions;

namespace DapperMapper
{
    public class Estado
    {
        public string SiglaEstado { get; set; }
        public string NomeEstado { get; set; }
        public string NomeCapital { get; set; }
        public int IdRegiao { get; set; }
    }
}
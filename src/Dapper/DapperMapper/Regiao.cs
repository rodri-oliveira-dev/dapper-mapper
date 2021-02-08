using DapperMapper.Attributes;

namespace DapperMapper.Console
{
    [DapperTable("Regioes")]
    public class Regioes
    {
        [DapperColumn(primaryKey: true)]
        public int IdRegiao { get; set; }

        [DapperColumn]
        public string NomeRegiao { get; set; }
    }
}
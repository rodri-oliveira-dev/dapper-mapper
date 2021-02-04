using Dapper.FluentMap.Dommel.Mapping;

namespace DapperMapper
{
    public class RegioesMap : DommelEntityMap<Regioes>
    {
        public RegioesMap()
        {
            ToTable("dbo.Regioes");
            Map(p => p.NomeRegiao).ToColumn("NomeRegiao");
            Map(p => p.IdRegiao).IsKey().ToColumn("IdRegiao", false);
        }
    }
}

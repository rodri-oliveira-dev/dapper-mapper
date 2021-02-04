using Dapper.FluentMap.Dommel.Mapping;

namespace DapperMapper
{
    public class EstadoMap : DommelEntityMap<Estado>
    {
        public EstadoMap()
        {
            ToTable("dbo.Estados");
            Map(p => p.SiglaEstado).ToColumn("SiglaEstado").IsKey();
            Map(p => p.NomeCapital).ToColumn("CapitalNome");
            Map(p => p.NomeEstado).ToColumn("NomeEstado");
            Map(p => p.IdRegiao).ToColumn("IdRegiao");
        }
    }
}

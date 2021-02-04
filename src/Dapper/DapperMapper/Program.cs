namespace DapperMapper
{
    class Program
    {
        private const string StringConnection = "Data Source=localhost,1433;Initial Catalog=ExemplosDapper;User Id=sa;Password=Hc#2020@3011;MultipleActiveResultSets=True;";

        static void Main(string[] args)
        {

            var repo = new RegiaoRepository(StringConnection);

            var regioes = repo.Select<Regioes>(new Regioes { IdRegiao = 5 });

            //FluentMapper.EntityMaps.Clear();
            //FluentMapper.TypeConventions.Clear();
            //FluentMapper.Initialize(config =>
            //{
            //    config.AddMap(new EstadoMap());
            //    config.AddMap(new RegioesMap());
            //    config.ForDommel();
            //});


            //using (SqlConnection conexao = new SqlConnection(StringConnection))
            //{
            //    //var estado = conexao.Get<Estado>("SP");
            //    //var estados = conexao.GetAll<Estado>();

            //    var novoEstado = new Regioes { IdRegiao = 7, NomeRegiao="Teste Insert" };

            //    var teste =conexao.Insert(novoEstado);

            //    conexao.Update(new Regioes { IdRegiao = 7, NomeRegiao="Teste Update" });

            //    var estadoTeste = conexao.Get<Regioes>(7);

            //    conexao.Delete(new Regioes { IdRegiao = 7 });
            //}
        }
    }
}

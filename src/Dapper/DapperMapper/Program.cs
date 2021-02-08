using BenchmarkDotNet.Running;

namespace DapperMapper.Console
{
    class Program
    {

        private const string StringConnection = "Data Source=localhost,1433;Initial Catalog=ExemplosDapper;User Id=sa;Password=Hc#2020@3011;MultipleActiveResultSets=True;";

        static void Main(string[] args)
        {

            //var repoNovo = new TabelaTesteDapperMapper(StringConnection);

            //var repoAntigo = new TabelaTesteRepository(StringConnection);

            ////var regioes = repoNovo.GetAll();

            //repoNovo.Insert(new TabelaTeste { Campo1 = "Campo 1" });
            //repoAntigo.Insert(new TabelaTeste { Campo1 = "Campo 2" });

            //var regiao = repoNovo.GetById(9);

            //regiao.NomeRegiao = "Teste 2";

            //var teste2 = repoNovo.Update(regiao);

            //var regiao2 = repo.repoNovo(regiao);

            //repoNovo.Delete(regiao2);

            var summary = BenchmarkRunner.Run<MemoryBenchmarkerDemo>();

        }
    }
}

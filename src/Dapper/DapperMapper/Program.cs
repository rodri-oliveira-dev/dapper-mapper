using DapperMapper.Repositories;

namespace DapperMapper.Console
{
    class Program
    {
        private const string StringConnection = "Data Source=localhost,1433;Initial Catalog=ExemplosDapper;User Id=sa;Password=Hc#2020@3011;MultipleActiveResultSets=True;";

        static void Main(string[] args)
        {

            var repo = new DapperRepository<Regioes>(StringConnection);

            var regioes = repo.GetAll();

            var teste = repo.Insert(new Regioes { IdRegiao = 9, NomeRegiao = "Teste" });

            var regiao = repo.GetById(9);

            regiao.NomeRegiao = "Teste 2";

            var teste2 = repo.Update(regiao);

            var regiao2 = repo.GetById(regiao);

            repo.Delete(regiao2);

        }
    }
}

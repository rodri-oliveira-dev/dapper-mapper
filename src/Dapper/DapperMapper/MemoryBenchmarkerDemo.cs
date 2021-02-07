using BenchmarkDotNet.Attributes;
using Dapper;
using DapperMapper.Repositories;
using System.Data;
using System.Data.SqlClient;

namespace DapperMapper.Console
{
    [MemoryDiagnoser]
    public class MemoryBenchmarkerDemo
    {
        int NumberOfItems = 100;
        private const string StringConnection = "Data Source=localhost,1433;Initial Catalog=ExemplosDapper;User Id=sa;Password=Hc#2020@3011;MultipleActiveResultSets=True;";

        [Benchmark]
        public string Dapper()
        {
            for (int i = 0; i < NumberOfItems; i++)
            {
                DynamicParameters parms = new DynamicParameters();
                parms.Add("IdRegiao", 1000 + i);
                parms.Add("NomeRegiao", "Teste");

                var insertCommand = "INSERT INTO dbo.Regioes (IdRegiao, NomeRegiao) VALUES (@IdRegiao, @NomeRegiao);SELECT @@ROWCOUNT;";

                using IDbConnection db = new SqlConnection(StringConnection);
                db.ExecuteScalar<int>(insertCommand, parms, commandType: CommandType.Text);
            }
            return "Total Dapper";
        }
        [Benchmark]
        public string DapperMapper()
        {
            var repo = new DapperRepository<Regioes>(StringConnection);

            for (int i = 0; i < NumberOfItems; i++)
            {
                repo.Insert(new Regioes { IdRegiao = 2000 + i, NomeRegiao = "Teste" });
            }
            return "Total DapperMapper";
        }
    }
}
